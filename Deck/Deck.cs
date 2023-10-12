using Godot;
using System;
using System.Collections.Generic;

[GlobalClass] public partial class Deck : Resource
{
	//Risorsa di carte da allegare al battledeck
	//serve per contenere le carte fisse e per poterle passare al battledeck
	//Export
	[Export] int capacity;
	//ATTRIBUTI
	private Dictionary<int, Card> cards; //dizionario di carte

	//FUNZIONI
	public void AddCard(Card card){ //aggiunge una carta al mazzo se non è pieno
		if (cards.Count < capacity){
			cards.Add(card.CardId, card);
		}
	}
	public void RemoveCard(Card card){ //rimuove una carta
		cards.Remove(card.CardId);
	}

	//Getter-Setter
	public Dictionary<int, Card> Cards{
		get{return cards;}
		set{cards = value;}
	}
	public int Capacity{
		get{return capacity;}
		set{capacity = value;}
	}
}
