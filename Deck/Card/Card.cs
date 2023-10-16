using Godot;
using System;

public partial class Card : TextureRect
{
	//Export
	[Export] PackedScene cardAnimation; //animazione da eseguire, nodo esterno da allegare
	[Export] int cardId; //id della carta
	[Export] int mana_value;
	[Export] private string card_name;
	private int card_deck_position; //posizione della carta nel deck
	private int card_hand_position; //posizione della carta nella mano
	//NODI
	//private Label descrizione;
	
	//GETTER-SETTER
	public string CardName{
		get{return card_name;}
		set{card_name = value;}
	}
	public int ManaValue{
		get{return mana_value;}
		set{mana_value = value;}
	}
	public PackedScene CardAnimation{
		get{return cardAnimation;}
		set{cardAnimation = value;}
	}
	public int CardId{
		get{return cardId;}
		set{cardId = value;}
	}
	public int CardDeckPosition{
		get{return card_deck_position;}
		set{card_deck_position = value;}
	}
	public int CardHandPosition{
		get{return card_hand_position;}
		set{card_hand_position = value;}
	}
}
