using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class Enemy_BattleScene : Characters_Battle, DeckUse
{
	#region SIGNALS ———————————————————————————————————————————————————————————————————————————
	[Signal] public delegate void EndTurnSignalEventHandler(); //Segnale per far finire il turno
	//connesso da Enemy_BattleScene -> Codice Battlescene -> BattleScene
	[Signal] public delegate void IsBeenSelectedSignalEventHandler(Enemy_BattleScene enemy_BattleScene); //Segnale per passare il nemico selezionato al player
	//connesso da Enemy_BattleScene -> Codice Battlescene -> Player_BattleScene
	[Signal] public delegate void AnimateCardOnEnemyEventHandler(Card card, Enemy_BattleScene enemy_BattleScene); //Segnale per animare la carta su un nemico
	//connesso da Enemy_BattleScene -> Codice Battlescene -> BattleScene
	[Signal] public delegate void AnimateCardOnPlayerEventHandler(Card card); //Segnale per animare la carta sul player
	//connesso da Enemy_BattleScene -> Codice Battlescene -> BattleScene
	#endregion

	#region ATTRIBUTI ———————————————————————————————————————————————————————————————————————————
	private CollisionShape2D collisionShape2D;
	private Player_BattleScene player;
	private CardAnimation cardAnimation_toawait;
	#endregion
	
	#region NATURE ———————————————————————————————————————————————————————————————————————————
	[Export] private float reaction; //reazione del nemico
	[Flags] public enum EnemyNatureEnum{ //NATURA DEL NEMICO
		Docile = 1 << 1,
		Netural = 1 << 2,
		Evil = 1 << 3,
	}
	[ExportGroup("Nature")] //Export Flags
		[Export(PropertyHint.Flags, "Docile,Netural,Evil")] public int EnemyNature { get; set; } = 0;
	#endregion
	
	#region READY ———————————————————————————————————————————————————————————————————————————
	public override void _Ready(){
		//inizializzo i nodi
		BattleDeck = GetNode<BattleDeck>("BattleDeck");
		AnimationPlayer_char = GetNode<AnimationPlayer>("AnimationPlayer");
		collisionShape2D = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
		//emetto il segnale per preparare il battle deck
		EmitSignal("PrepareBattleDeckSignal"); //emette il segnale per preparare le risorse deck e poi il battledeck
		ToBeUnselected(); //disattiva la collisione cosi da non poter ricevere input quando non desiderato
	}
	#endregion
	
	#region FUNZIONI ———————————————————————————————————————————————————————————————————————————
	public void EndTurn(){ //termina il turno
		IsTurn = false;
		EmitSignal("EndTurnSignal"); //invia il segnale per terminare il turno e passare al successivo
	}

	public void ToBeSelected(){ //funzione per attivare la collisione cosi da poter ricevere input
		collisionShape2D.SetDeferred("disabled", false);
	}
	public void ToBeUnselected(){ //funzione per disattivare la collisione cosi da non poter ricevere input
		collisionShape2D.SetDeferred("disabled", true);
	}
	#endregion

	#region ENEMY AI ———————————————————————————————————————————————————————————————————————————
	public void PlayerStatusAnalysis(Player_BattleScene player){ //Controlliamo lo status del player: VITA - NUMERO CARTE - STATUS
		//CONTROLLO VITA PLAYER
		var playerLifePercent = player.Life / player.Max_Life * 100; //calcoliamo la percentuale di vita
		switch (playerLifePercent){
			case int life when (life >= 70): //se la vita è maggiore del 70%
				switch (EnemyNature){
					case 1: //docile
						reaction += 1.5F;
						break;
					case 2: //neutrale
						reaction += 2F;
						break;
					case 3: //evil
						reaction += 3F;
						break;
				}
				break;
			case int life when (life > 25 && life < 70): //se la vita è compresa tra il 26% e il 69%
				switch (EnemyNature){
					case 1: //docile
						reaction += 1F;
						break;
					case 2: //neutrale
						reaction += 2F;
						break;
					case 3: //evil
						reaction += 3.5F;
						break;
				}
				break;
			case int life when (life <= 25): //se la vita è minore o uguale al 25%
				switch (EnemyNature){
					case 1: //docile
						reaction += 0.5F;
						break;
					case 2: //neutrale
						reaction += 2F;
						break;
					case 3: //evil
						reaction += 4F;
						break;
				}
				break;
		}
		//CONTROLLO CARTE IN MANO PLAYER
		var cardsPlayer = player.BattleDeck.HandsCard.CountCardInHands(); //contiamo le carte in mano al player
		switch (cardsPlayer){
			case int cards when (cards == 4): //se il player ha 4
				switch (EnemyNature){
					case 1: //docile
						reaction += 2F;
						break;
					case 2: //neutrale
						reaction += 2F;
						break;
					case 3: //evil
						reaction += 3F;
						break;
				}
				break;
			case int cards when (cards == 3): //se il player ha 3
				switch (EnemyNature){
					case 1: //docile
						reaction += 1.5F;
						break;
					case 2: //neutrale
						reaction += 2F;
						break;
					case 3: //evil
						reaction += 3.5F;
						break;
				}
				break;
			case int cards when (cards == 2): //se il player ha 2 carte
				switch (EnemyNature){
					case 1: //docile
						reaction += 1F;
						break;
					case 2: //neutrale
						reaction += 2F;
						break;
					case 3: //evil
						reaction += 3.5F;
						break;
				}
				break;
			case int cards when (cards == 1): //se il player ha 1 carta
				switch (EnemyNature){
					case 1: //docile
						reaction += 0.5F;
						break;
					case 2: //neutrale
						reaction += 2F;
						break;
					case 3: //evil
						reaction += 4F;
						break;
				}
				break;
			case int cards when (cards == 0): //se il player ha 0 carta
				switch (EnemyNature){
					case 1: //docile
						reaction -= 0.5F;
						break;
					case 2: //neutrale
						reaction += 0F;
						break;
					case 3: //evil
						reaction -= 1F;
						break;
				}
				break;
		}
		//CONTROLLO STATUS PLAYER
		var playerStatus = player.CheckIfOnStatus();
		switch (playerStatus){
			case false:
				switch (EnemyNature){
					case 1: //docile
						reaction += 0.5F;
						break;
					case 2: //neutrale
						reaction += 1F;
						break;
					case 3: //evil
						reaction -= 1F;
						break;
				}
				break;
			case true:
				switch (EnemyNature){
					case 1: //docile
						reaction -= 0.5F;
						break;
					case 2: //neutrale
						reaction += 1F;
						break;
					case 3: //evil
						reaction += 1F;
						break;
				}
				break;
		}
	}
	public void EnemyStatusAnalysis(){ //Controlliamo lo status dell'enemy: VITA - NUMERO CARTE - STATUS
		//CONTROLLO VITA ENEMY
		var enemyLifePercent = Life / Max_Life * 100; //calcoliamo la percentuale di vita
		switch (enemyLifePercent){
			case int life when (life >= 70): //se la vita è maggiore del 70%
				switch (EnemyNature){
					case 1: //docile
						reaction += 1.5F;
						break;
					case 2: //neutrale
						reaction += 2F;
						break;
					case 3: //evil
						reaction += 3F;
						break;
				}
				break;
			case int life when (life > 25 && life < 70): //se la vita è compresa tra il 26% e il 69%
				switch (EnemyNature){
					case 1: //docile
						reaction += 0.5F;
						break;
					case 2: //neutrale
						reaction += 1F;
						break;
					case 3: //evil
						reaction += 1.5F;
						break;
				}
				break;
			case int life when (life <= 25): //se la vita è minore o uguale al 25%
				switch (EnemyNature){
					case 1: //docile
						reaction -= 0.5F;
						break;
					case 2: //neutrale
						reaction -= 2F;
						break;
					case 3: //evil
						reaction -= 3F;
						break;
				}
				break;
		}
		//CONTROLLO STATUS ENEMY
		var enemyStatus = CheckIfOnStatus();
		switch (enemyStatus){
			case false:
				switch (EnemyNature){
					case 1: //docile
						reaction += 0.5F;
						break;
					case 2: //neutrale
						reaction += 1F;
						break;
					case 3: //evil
						reaction += 2F;
						break;
				}
				break;
			case true:
				switch (EnemyNature){
					case 1: //docile
						reaction -= 0.5F;
						break;
					case 2: //neutrale
						reaction -= 1F;
						break;
					case 3: //evil
						reaction -= 2F;
						break;
				}
				break;
		}
	}
	public Queue<Card> CardsInHandAnalysis(){ //Analizziamo le carte in mano dell'enemy, si riordinano per mana e si scelgono in base alla reazione -> ChoosenCards
		//CONTROLLO E SUDDIVISIONE CARTE IN MANO
		var cards = BattleDeck.HandsCard.CardsInHand;
			//CARTE ATTIVE
		Card[] activeCards = new Card[4]; //creiamo un array di 4 carte
		for(int i = 0; i < BattleDeck.HandsCard.CountCardInHands(); i++){ //scorriamo le carte in mano
			if (cards[i].CardTarget == 2 /*player = 2*/){
				activeCards[i] = cards[i]; //se la carta ha come target il player la aggiungiamo all'array
			}
		}	
			//CARTE PASSIVE
		Card[] passiveCards = new Card[4]; //creiamo un array di 4 carte
		for(int i = 0; i < BattleDeck.HandsCard.CountCardInHands(); i++){ //scorriamo le carte in mano
			if (cards[i].CardTarget == 1 /*Self = 1*/){
				passiveCards[i] = cards[i]; //se la carta ha come target il nemico la aggiungiamo all'array
			}
		}
		//ORDINIAMO LE CARTE PER MANA E CREIAMO LE CODE
		var activeCards_queue = InsertionSortAndQueue(activeCards); //ordiniamo le carte attive
		var passiveCards_queue = InsertionSortAndQueue(passiveCards); //ordiniamo le carte passive
		//SCELTA CARTE
		//CALCOLO SCELTA PERCENTUALE
		var scelta_percentuale = ((reaction + 1) / (14 + 1)) *100; // la reaction va da -1 a 14, quindi + 1 per rendere tutto positivo
		Queue<Card> choosenCards = new Queue<Card>(); //creiamo una coda di carte
		Random random = new Random();
		for(int i = 0; i < BattleDeck.HandsCard.CountCardInHands(); i++){//scorriamo le carte
			var random_number = random.Next(0, 100); //generiamo un numero random da 0 a 100
			switch (random_number){
				case int n when n < scelta_percentuale: //carte attive
					AddChoosenCardFromActive(choosenCards, activeCards_queue, passiveCards_queue); //aggiungiamo la carta alla coda choosenCards
					break;
				case int n when n >= scelta_percentuale: //carte passive
					AddChoosenCardFromPassive(choosenCards, passiveCards_queue, activeCards_queue) ; //aggiungiamo la carta alla coda choosenCards
					break;
			}
		} 
		return choosenCards; //resituiamo la coda di carte pronta per essere eseguita
	}
	public async void ExecutionCard(Queue<Card> cards){ //funzione per eseguire le carte
		/*TESTING*/ GD.Print("Turno nemico iniziato");
		Is_attacking = true; //l'enemy sta attaccando
		var cards_count = cards.Count; //contiamo le carte, bisogna farlo prima perchè la coda cambia
		for(int i = 0; i < cards_count; i++){
			if (cards.Peek().ManaValue <= Mana){ //se la carta ha un costo minore o uguale al mana
				/*TESTING*/ GD.Print("Carta eseguita: " + cards.Peek().CardName + " Mana: " + cards.Peek().ManaValue);
				var selectedCard = cards.Dequeue(); //selezioniamo la carta
				UseMana(selectedCard.ManaValue); //usiamo il mana
				if (selectedCard.CardTarget == 2 /*Opponent*/){ // la carta è rivolta contro un avversario (player)
					Animate("Attack"); //Animiamo il nemico che attacca
					selectedCard.ExecuteCard(player);
					BattleDeck.HandsCard.RemoveCard(selectedCard); //rimuoviamo la carta dalla mano
					EmitSignal("AnimateCardOnPlayer", selectedCard); //Animiamo la carta sul player
				} else { //la carta è rivolta contro se stesso (nemico)
					Animate("Attack"); //Animiamo il nemico che attacca
					selectedCard.ExecuteCard(this);
					BattleDeck.HandsCard.RemoveCard(selectedCard); //rimuoviamo la carta dalla mano
					EmitSignal("AnimateCardOnEnemy", selectedCard, this); //animiamo la carta sul nemico
				}
				await ToSignal(cardAnimation_toawait, "EnemyCardAnimationFinished"); //aspettiamo che l'animazione finisca
			} else {
				/*TESTING*/ GD.Print("Carta non eseguita: " + cards.Peek().Name);
				continue;
			}
		}
		/*TESTING*/ GD.Print("Turno nemico finito");
		//una volta ciclate tutte le carte, finiamo il turno
		Is_attacking = false;
		EndTurn();
	}
	#endregion

	#region FUNCTIONALITY ———————————————————————————————————————————————————————————————————————————
	public Queue<Card> InsertionSortAndQueue(Card[] cards){ //decrescente, serve a ordinare le carte per mana
		//scritto interamente da Copilot
   		int n = cards.Length;
    	for (int i = 1; i < n; ++i){
        	Card key = cards[i];
        	int j = i - 1;
        	while (j >= 0 && cards[j] != null && key != null && cards[j].ManaValue < key.ManaValue){ //se cambia il < in > allora diventa crescente
            	cards[j + 1] = cards[j];
            	j = j - 1;
        	}
        	cards[j + 1] = key;
    	}
		Queue<Card> cards_queue = new Queue<Card>();
		foreach(Card card in cards){
			if(card != null){
				cards_queue.Enqueue(card);
			}
		}
		return cards_queue;
	}
	public void AddChoosenCardFromActive(Queue<Card> choosenCards, Queue<Card> activeCards_queue, Queue<Card> passiveCards_queue){
		//serve ad aggiungere carte attive alla coda delle carte scelte e se non ce ne sono prende le passive
		if(activeCards_queue.Count != 0){ //se ci sono carte attive allora metti in coda la prima
			choosenCards.Enqueue(activeCards_queue.Dequeue());
		} else { //se non ci sono passa a quelle passive
			choosenCards.Enqueue(passiveCards_queue.Dequeue());
		}
	}
	public void AddChoosenCardFromPassive(Queue<Card> choosenCards, Queue<Card> passiveCards_queue, Queue<Card> activeCards_queue){ 
		//serve ad aggiungere carte passive alla coda delle carte scelte e se non ce ne sono prende le attive
		if(passiveCards_queue.Count != 0){ //se ci sono carte passive allora metti in coda la prima
			choosenCards.Enqueue(passiveCards_queue.Dequeue());
		} else { //se non ci sono passa a quelle attive
			choosenCards.Enqueue(activeCards_queue.Dequeue());
		}
	}
	#endregion

	#region SIGNALS ———————————————————————————————————————————————————————————————————————————
	public void _on_BattleStart_Signal(){ //funzione del segnale BattleStart, emesso da BattleScene (collegato Godot)
		BattleDeck._on_BattleStart();//Prepara il battle deck
	}

	public void _on_battle_scene_is_turn_signal(){ //funzione del segnale IsTurnSignal per eseguire la mossa quando è il turno del nemico
        if (IsTurn){
		    ResetMana(); //resettiamo il mana
            DrawCard(); //peschiamo una carta
			
			//ENEMY AI
			PlayerStatusAnalysis(player); //analizziamo lo stato del player
			EnemyStatusAnalysis(); //analizziamo lo stato del nemico
			var choosenCards = CardsInHandAnalysis(); //analizziamo le carte in mano
			ExecutionCard(choosenCards); //eseguiamo le carte
        }
    }

	public void _on_area_2d_input_event(Node viewport, InputEvent @event, long shapeIdx){ //funzione del segnale input_event, collegato con Godot
		if (@event.IsActionPressed("ui_select")){ //se selezionato col tasto sinistro del mouse inva il segnale IsBeenSelectedSignal al player passando il nemico
			EmitSignal("IsBeenSelectedSignal", this); //serve per passare il nemico selezionato al player cosi da poter interagire con esso
		}
	}
	public void _on_battle_scene_pass_player_to_select(Player_BattleScene player){ //serve a passare il player al nemico
	//collegato con BattleScene -> BattleScene Code -> Enemy_BattleScene
		this.player = player;
	}
	public void _on_enemy_pass_card_animation_to_enemy(CardAnimation cardAnimation){ //serve a passare l'animazione della carta al nemico
		//collegato con BattleScene -> BattleScene Code -> Enemy_BattleScene
		if(IsTurn){ //controlla se è il suo turno cosi da far aggiornare la variabile solo al nemico effettivamente in turno e non tutti i nemici in campo
			cardAnimation_toawait = cardAnimation;
		}
	}
	#endregion

	#region GETTER/SETTER ———————————————————————————————————————————————————————————————————————————

	#endregion
}
