using Godot;
using System;
using System.Linq;
using System.Threading;

public partial class HandCards_GUI : CanvasLayer
{
	#region SIGNAL ———————————————————————————————————————————————————————————————————————————
	[Signal] public delegate void ConnectInputToCardsEventHandler(HandCards_GUI handCards_GUI); //Segnale per collegare l'input alle carte in mano
	//Connesso al HandCards_GUI -> Godot -> Player
	[Signal] public delegate void CardsOnGUIUpdatedEventHandler(); //Segnale per indicare che le carte sono state aggiorante
	//utilizzato con solo emissione per AbleCardCollsion che aspetterà nel caso le carte siano state aggiornate (se non aspetta attiverà le collisioni di carte che stanno in queuefree)
	#endregion

	#region NODI ———————————————————————————————————————————————————————————————————————————
	HBoxContainer hBoxContainer; //contenitore di carte che si hanno in mano
	#endregion

	#region READY - PROCESS ———————————————————————————————————————————————————————————————————————————
	public override void _Ready(){
		hBoxContainer = GetNode<HBoxContainer>("HBoxContainer");
	}
	#endregion

	#region FUNZIONI ———————————————————————————————————————————————————————————————————————————
	public void ReSizeCardCollsion(int num_card){ //funzione da chiamare ogni qual volta si aggiornano le carti presenti in mano
		var size = new Vector2(
			//x: hBoxContainer.Size.X / hBoxContainer.GetChildCount(), //versione precedente
			x: hBoxContainer.Size.X / num_card,
			y: hBoxContainer.Size.Y
		);
		var pos = new Vector2(
			x: size.X / 2 ,
			y: size.Y / 2
		);
		foreach(Card card in hBoxContainer.GetChildren()){
			card.ReSizeCollsion(size, pos);
		}
	}

	public Card GetFocusedCard(){ //funzione che prende la carta che è in focus (quella puntata dal mouse)
		foreach(Card card in hBoxContainer.GetChildren()){
			if (card.IsFocused){
				return card;
			}
		}
		return null;
	}
	#endregion

	#region SEGNALI ———————————————————————————————————————————————————————————————————————————
	public void _on_player_cards_on_gui(HandsCard handsCard){ //segnale richiamato dal player per mostrare le carte a schermo, collegato con Godot
		//prima svuotiamo la mano per non avere problemi di doppioni, salvando l'ultima carta
		Card lastcard = null;
		if (hBoxContainer.GetChildCount() > 0){
			lastcard = hBoxContainer.GetChild<Card>(hBoxContainer.GetChildCount()-1);
		}
		foreach(Card card in hBoxContainer.GetChildren()){
			card.QueueFree();
		}
        //poi la rempiamo con le vecchie carte ancora presenti nel dizionario della mano
        AwaitCardFree_CardsOnGUI(handsCard, lastcard);
	}
	public async void AwaitCardFree_CardsOnGUI(HandsCard handsCard, Card lastCard){
		if (lastCard != null){
			await ToSignal(lastCard , "tree_exited");
		}
		int i = 0;
		foreach(Card card in handsCard.CardsInHand){
			if (card == null){
				continue; // se è nullo salta alla prossima iterazione
			}
			var c = ResourceLoader.Load("res://Deck/Card/Cards-Scene/"+ card.CardName + ".tscn") as PackedScene; //prendiamo il packedscne attraverso il nome della carta
			hBoxContainer.AddChild(c.Instantiate()); //istanziamo la carta
			hBoxContainer.GetChild<Card>(i).CardHandPosition = card.CardHandPosition; //diamo posizione della mano alla carta
			hBoxContainer.GetChild<Card>(i).CardDeckPosition = card.CardDeckPosition; //diamo posizione del deck alla carta
			i++;
		}
		ReSizeCardCollsion(i); //ridimensioniamo le collisioni delle carte
		EmitSignal("ConnectInputToCards", this); //connettiamo le carte all'input
		EmitSignal("CardsOnGUIUpdated"); //segnaliamo che le carte sono state aggiornate
	}
	public async void _on_Able_Cards_Collision(Boolean value, bool afterCardsOnGUI){ //disabilita le collisioni delle carte per non poterle selezionare
		//aspettiamo il segnlae CardsONGUIUpdated per evitare di attivare le collisioni di carte che sono in queuefree
		if (afterCardsOnGUI == true){
			await ToSignal(this, "CardsOnGUIUpdated");
		}
		//altrimenti se non c'è da aspettare si esegue subito
		foreach(Card card in hBoxContainer.GetChildren()){
			card.GetNode<CollisionShape2D>("Area2D/CollisionShape2D").SetDeferred("disabled", !(value));
		}
    }

	public void _on_Partial_Able_Cards_Collision(Card card){ //disabilita le collisioni di tutte le carte tranne quella passata
		//serve quando viene selezionata una carta
		foreach(Card c in hBoxContainer.GetChildren()){
			if (c == card){
				c.GetNode<CollisionShape2D>("Area2D/CollisionShape2D").SetDeferred("disabled", false);
			}else{
				c.GetNode<CollisionShape2D>("Area2D/CollisionShape2D").SetDeferred("disabled", true);
			}
		}
	}
	public void _on_restart_button_pressed(){ //segnale per il bottone restart
		GetTree().ReloadCurrentScene();
	}
	#endregion

	#region GETTER/SETTER ———————————————————————————————————————————————————————————————————————————
	public HBoxContainer HBoxContainer{
		get{return hBoxContainer;}
		set{hBoxContainer = value;}
	}
	#endregion

}
