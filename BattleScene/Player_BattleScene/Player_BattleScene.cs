using Godot;
using System;

public partial class Player_BattleScene : Characters_Battle, DeckUse
{
    #region SIGNALS ———————————————————————————————————————————————————————————————————————————
    [Signal] public delegate void CardsOnGUIEventHandler(HandsCard handsCard); //Segnale per aggiornare la GUI delle carte in mano
    [Signal] public delegate void AnimateCardOnEnemyEventHandler(Card card, Enemy_BattleScene enemy_BattleScene); //Segnale per animare la carta su un nemico
    //Collegato Player -> Godot -> BattleScne
    [Signal] public delegate void AbleCardsCollisionEventHandler(Boolean value); //Segnale per disabilitare le collisioni delle carte
    //Collegato Player -> Godot -> HandsCard_Gui
    [Signal] public delegate void PartialAbleCardsCollisionEventHandler(Card card); //Segnale per disabilitare le collisioni di tutte le carte tranne una
    //Collegato Player -> Godot -> HandsCard_Gui
    #endregion

    #region ATTRIBUTI ———————————————————————————————————————————————————————————————————————————
    private HandCards_GUI handCards_GUI;
    private Enemy_BattleScene[] enemys; //array di nemici
    private Enemy_BattleScene selectedEnemy; //nemico selezionato
    private Card selectedCard; //carta selezionata
    #endregion

    #region INTERFACE ———————————————————————————————————————————————————————————————————————————
    public void _on_BattleStart_Signal(){ //funzione del segnale BattleStart, emesso da BattleScene (collegato Godot)
        BattleDeck._on_BattleStart();//Prepara il battle deck
        EmitSignal("CardsOnGUI", BattleDeck.HandsCard); //aggiorniamo lo stato delle carte in mano
    }

    public void SelectCard(Node viewport, InputEvent @event, long shapeIdx){ //segnale delle carte in mano, collegato con Codice Player(handscard_gui) -> Codice -> Card
        if (Input.IsActionJustPressed("ui_select")){ //se premo il tasto sinistro della carta la seleziono
            //eseguiamo la mossa della carta
            selectedCard = handCards_GUI.GetFocusedCard();
            /*PER TESTING*/ GD.Print(selectedCard.CardName);
            //disabilitiamo le collisioni delle altre carte
            EmitSignal("PartialAbleCardsCollision", selectedCard);
            //disconettiamo il segnale del mouse uscito per far si che l'animazione Glow sia permanente
            if (selectedCard.GetNode<Area2D>("Area2D").IsConnected("mouse_exited", new Callable (selectedCard, "_on_area_2d_mouse_exited")) == true){
                selectedCard.GetNode<Area2D>("Area2D").Disconnect("mouse_exited", new Callable (selectedCard, "_on_area_2d_mouse_exited"));
            }
            /*PER TESTING*/ GD.Print(selectedCard.CardName);
            /*PER TESTING*/ GD.Print(selectedCard.CardHandPosition);
            //eseguiamo la carta sel il mana è abbastanza
            if (Mana >= selectedCard.ManaValue){
                if (selectedCard.HasEnemy){ //Se la carta ha nemici da selezionare passiamo dalla selezione del nemico
                    SelectTarget();
                    //UseCard eseguito alla fine del segnale di selezione del nemico
                }else{ //se la carta non ha nemici viene eseguita immediatamente
                    UseCard();
                }
            } else { //non c'è abbastanza mana
                    //DA PROGRAMMARE
                /*PER TESTING*/ GD.Print("Mana insufficiente");
            }
        }

        if(Input.IsActionJustPressed("ui_deselect")){ //se premo il tasto destro del mouse deseleziono la carta
            selectedCard = handCards_GUI.GetFocusedCard();
            //colleghiamo prima di nuovo il segnale precedentemente disconnesso
            if (selectedCard.GetNode<Area2D>("Area2D").IsConnected("mouse_exited", new Callable (selectedCard, "_on_area_2d_mouse_exited")) == false){
                selectedCard.GetNode<Area2D>("Area2D").Connect("mouse_exited", new Callable (selectedCard, "_on_area_2d_mouse_exited"));
            }
            //annulliamo la carta selezionata
            selectedCard = null;
            /*PER TESTING*/ GD.Print("Carta deselezionata");
            //non serve più poter selezionare i nemici
            foreach(Enemy_BattleScene enemy in enemys){
                enemy.ToBeUnselected();
            }
            //riabilitiamo le collisioni delle carte cosi da poterle scegliere tutte
            EmitSignal("AbleCardsCollision", true);
        }
    }

    public void UseCard(){ //funzione chiamata per usare la carta
        UseMana(selectedCard.ManaValue); //usiamo il mana
        /*PER TESTING*/ GD.Print("Mana: " + Mana);
        
            //MOSSA DA ESEGUIRE

        //animiamo la carta passando carta e nemico selezionati se la carta ha nemici
        if (selectedCard.HasEnemy){
            EmitSignal("AnimateCardOnEnemy", selectedCard, selectedEnemy);
        } else { //altrimenti animiamo la carta senza nemici
            //ANIMAZIONE CARTE SENZA NEMICI
        }
        //Rimuoviamo la carta
        BattleDeck.HandsCard.RemoveCard(selectedCard);
        AwaitCardExpired(selectedCard); //aspettiamo che prima l'animazione della carta finisca
        //riprisitinamo a null le scelte
        selectedCard = null;
        selectedEnemy = null;
    }
    #region ASYNC FOR USECARD ———————————————————————————————————————————————————————————————————————————
    public async void AwaitCardExpired(Card card){
        await ToSignal(card, "CardExpired");
        EmitSignal("CardsOnGUI", BattleDeck.HandsCard);
    }
    #endregion

	public void SelectTarget(){ //serve per abilitare le collisioni del nemico, chiamata quando la carta ha nemici da selezionare
        //abilitiamo le collisioni dei nemici per poter selezionarlo
        foreach(Enemy_BattleScene enemy in enemys){
            enemy.ToBeSelected();
        }
	}
    #endregion
    
    #region SIGNALS ———————————————————————————————————————————————————————————————————————————
    public void _on_hand_cards_gui_connect_input_to_cards(HandCards_GUI handCards_GUI){ //segnale per connettere l'input sulle carte alla GUI, collegato con HandsCard_GUI -> Godot -> Player
        this.handCards_GUI = handCards_GUI;
        //effettuaimao un controllo sul segnale, se è gia connesso non lo connettiamo per evitare di ricevere errore
        foreach(Card card in handCards_GUI.HBoxContainer.GetChildren()){
            if (card.Is_input_connected == false){ //se non è già connesso al segnale, allora connetti
                card.GetNode<Area2D>("Area2D").Connect("input_event", new Callable (this, "SelectCard"));
                card.Is_input_connected = true;
            } else { //se è già connesso, disconettiamo e connettiamo per un nuovo callable
            //serve ad evitare sovrapposizioni, altrimenti il callable viene chiamato più volte
                card.GetNode<Area2D>("Area2D").Disconnect("input_event", new Callable (this, "SelectCard"));
                card.GetNode<Area2D>("Area2D").Connect("input_event", new Callable (this, "SelectCard"));
            }
        }
    }
    
    public void _on_battle_scene_is_turn_signal(){ //segnale per iniziare il turno, collegato con BattleScene -> Godot -> Player
        if (IsTurn){
            ResetMana(); //resettiamo il mana
            DrawCard(); //peschiamo una carta
            EmitSignal("CardsOnGUI", BattleDeck.HandsCard); //aggiornaimo la GUI delle carte in mano
            handCards_GUI.HBoxContainer.GetChild<Card>(handCards_GUI.HBoxContainer.GetChildCount() -1).Animate("Draw"); //Animiamo l'ultima carta pescata
            EmitSignal("AbleCardsCollision", true); //abilita le collisioni delle carte
        }
    }

    public void _on_battle_scene_pass_enemys_to_selected(Enemy_BattleScene[] enemys){ //segnale per passare i nemici al player, collegato con BattleScene -> Godot -> Player
        this.enemys = enemys;
    }

    public void _on_Enemy_Is_Been_Selected_Signal(Enemy_BattleScene enemy_BattleScene){ //segnale per dire che il nemico è stato selezionato, collegato con Enemy_BattleScene -> Codice BattleScene -> Player
        selectedEnemy = enemy_BattleScene;
        //Una volta selezionato il nemico disabilitiamo le collisioni degli altri
        foreach(Enemy_BattleScene enemy in enemys){
            enemy.ToBeUnselected();
        }
        //eseguiamo la carta poichè il nemico è stato selezionato
        UseCard();
    }

    #endregion
}
