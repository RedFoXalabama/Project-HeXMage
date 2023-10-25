using Godot;
using System;

public partial class Player_BattleScene : Characters_Battle, DeckUse
{
    #region SIGNALS ———————————————————————————————————————————————————————————————————————————
    [Signal] public delegate void CardsOnGUIEventHandler(HandsCard handsCard); //Segnale per aggiornare la GUI delle carte in mano
    //Collegato Player -> Godot -> HandsCard_Gui
    #endregion

    #region ATTRIBUTI ———————————————————————————————————————————————————————————————————————————
    private HandCards_GUI handCards_GUI;
    #endregion

    #region INTERFACE ———————————————————————————————————————————————————————————————————————————
    public void _on_BattleStart_Signal(){ //funzione del segnale BattleStart, emesso da BattleScene (collegato Godot)
        BattleDeck._on_BattleStart();//Prepara il battle deck        
        EmitSignal("CardsOnGUI", BattleDeck.HandsCard);
    }

    public void SelectCard(Node viewport, InputEvent @event, long shapeIdx){
        //var selectedCard = (Card)card.GetParent();
        if (@event.IsActionPressed("ui_select")){ //se  da problemi guardare documentazione godot per IsEcho()
            //eseguiamo la mossa della carta
            GD.Print("Mossa eseguita");
            Card selectedCard = handCards_GUI.GetFocusedCard();
            GD.Print(selectedCard.CardName);
            GD.Print(selectedCard.CardHandPosition);
            if (Mana >= selectedCard.ManaValue){
                UseMana(selectedCard.ManaValue);
                //MOSSA DA ESEGUIRE
                //Rimuoviamo la carta
                BattleDeck.HandsCard.RemoveCard(selectedCard);
                EmitSignal("CardsOnGUI", BattleDeck.HandsCard);
            }     
        }
    }

    public void UseCard(Card card){
        //EmitSignal("CardsOnGUI", BattleDeck.HandsCard); //aggiorna la GUI delle carte in mano
    }
	public void SelectTarget(){

	}
    #endregion
    
    #region SIGNALS ———————————————————————————————————————————————————————————————————————————
    public void _on_hand_cards_gui_connect_input_to_cards(HandCards_GUI handCards_GUI){
        this.handCards_GUI = handCards_GUI;
        //effettuaimao un controllo sul segnale, se è gia connesso non lo connettiamo per evitare di ricevere errore
        foreach(Card card in handCards_GUI.HBoxContainer.GetChildren()){
            if (card.Is_input_connected == false){ //se non è già connesso al segnale, allora connetti
                card.GetNode<Area2D>("Area2D").Connect("input_event", new Callable (this, "SelectCard"));
                card.Is_input_connected = true;
            }
        }
    }
    
    public void _on_battle_scene_is_turn_signal(){
        if (IsTurn){
            //le carte sono riabilitate tramite segnale dal battlescene all'HandCards_GUI
            ResetMana();
            DrawCard();
            EmitSignal("CardsOnGUI", BattleDeck.HandsCard);
            handCards_GUI.HBoxContainer.GetChild<Card>(handCards_GUI.HBoxContainer.GetChildCount() -1).Animate("Draw");
        }
    }
    #endregion
}
