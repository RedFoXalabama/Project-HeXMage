using Godot;
using System;

public partial class HandCards_GUI : CanvasLayer
{
	//NODI
	HBoxContainer hBoxContainer; //contenitore di carte che si hanno in mano

	//READY
	public override void _Ready(){
		hBoxContainer = GetNode<HBoxContainer>("HBoxContainer");
	}

	//SEGNALI
	public void _on_player_cards_on_gui(HandsCard handsCard){ //segnale richiamato dal player per mostrare le carte a schermo, collegato con Godot
		foreach(Card card in handsCard.Cards.Values){
			var c = ResourceLoader.Load("res://Deck/Card/Cards-Scene/"+card.CardName + ".tscn") as PackedScene;
			hBoxContainer.AddChild(c.Instantiate());
		}
	}
}
