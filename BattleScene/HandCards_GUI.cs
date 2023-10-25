using Godot;
using System;
using System.Linq;
using System.Threading;

public partial class HandCards_GUI : CanvasLayer
{
	#region SIGNAL ———————————————————————————————————————————————————————————————————————————
	[Signal] public delegate void ConnectInputToCardsEventHandler(HandCards_GUI handCards_GUI); //Segnale per collegare l'input alle carte in mano
	//Connesso al player tramite godot
	#endregion

	#region NODI ———————————————————————————————————————————————————————————————————————————
	HBoxContainer hBoxContainer; //contenitore di carte che si hanno in mano
	#endregion

	#region READY - PROCESS ———————————————————————————————————————————————————————————————————————————
	public override void _Ready(){
		hBoxContainer = GetNode<HBoxContainer>("HBoxContainer");
	}

	//TESTING (da sostituire con un segnale emesso quando una carta viene scartata)
	public override void _Process(double delta){
		ReSizeCardCollsion();
		//DisableCardsCollision();
	}
	//TESTING
	#endregion

	#region FUNZIONI ———————————————————————————————————————————————————————————————————————————
	public void ReSizeCardCollsion(){ //funzione da chiamare ogni qual volta si aggiornano le carti presenti in mano
		var size = new Vector2(
			x: hBoxContainer.Size.X / hBoxContainer.GetChildCount(),
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
	public Card GetFocusedCard(){
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
		//prima svuotiamo la mano per non avere problemi di doppioni
		foreach(Card card in hBoxContainer.GetChildren()){
			card.QueueFree();
		}
		//poi la rempiamo con le vecchie carte ancora presenti nel dizionario della mano
		
		int i = 0;
		foreach(Card card in handsCard.CardsInHand){
			if (card == null){
				continue; // se è nullo salta alla prossima iterazione
			}
			var c = ResourceLoader.Load("res://Deck/Card/Cards-Scene/"+card.CardName + ".tscn") as PackedScene;
			hBoxContainer.AddChild(c.Instantiate());
			var temp = card.CardHandPosition;
			hBoxContainer.GetChild<Card>(i).CardHandPosition = temp;
			hBoxContainer.GetChild<Card>(i).CardDeckPosition = card.CardDeckPosition;
			i++;
		}
		ReSizeCardCollsion();
		EmitSignal("ConnectInputToCards", this);
	}

	public void _on_Able_Cards_Collision(Boolean value){ //disabilita le collisioni delle carte per non poterle selezionare
        switch (value){
			case true:
				foreach(Card card in hBoxContainer.GetChildren()){
					card.GetNode<Area2D>("Area2D").Show();
				}
				break;
			case false:
				foreach(Card card in hBoxContainer.GetChildren()){
					card.GetNode<Area2D>("Area2D").Hide();
				}
				break;
		}
    }
	#endregion

	#region GETTER/SETTER ———————————————————————————————————————————————————————————————————————————
	public HBoxContainer HBoxContainer{
		get{return hBoxContainer;}
		set{hBoxContainer = value;}
	}
	#endregion

}
