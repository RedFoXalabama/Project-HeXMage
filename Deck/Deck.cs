using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

//Risorsa di carte da allegare al battledeck
//serve per contenere le carte fisse e per poterle passare al battledeck
[GlobalClass] public partial class Deck : Resource
{
	//Export
	[Export] int capacity; //capacità del mazzo
	[Export] PackedScene[] exportedCards = new PackedScene[10]; //array dei tcsn delle carte. Permette di creare il mazzo tramite codice
	//ATTRIBUTI
	private Dictionary<int, Card> cards = new Dictionary<int, Card>(); //dizionario di carte

	//FUNZIONI
	public void InitForPlayer(string deckCardFile){ //inizializza il dizionario di carte per il player
		//prima ci assiucriamo che non ci siano residui
		cards.Clear(); //pulisce il dizionario
		//Riempiamo il packedscene di carte tramite il json in cui sono salvate le carte del player
		var file = FileAccess.Open(deckCardFile, FileAccess.ModeFlags.Read);
		var json = file.GetAsText();
   		var options = new JsonSerializerOptions {
			PropertyNameCaseInsensitive = true
		};
		var deserializedDeck =JsonSerializer.Deserialize<Dictionary<int, string>>(json, options);
		//Riempiamo il packedscene di carte tramite il json in cui sono salvate le carte del player
		foreach(KeyValuePair<int, string> card in deserializedDeck){
			exportedCards[card.Key] = ResourceLoader.Load(card.Value) as PackedScene;
		}
		//Inizializzamo il deck(main o side)(risorsa) del player
		for(int i = 0; i < exportedCards.Length; i++){ //per ogni tscn nel packedscene[]
			if(exportedCards[i] != null){ //se l'oggetto non è diverso da nulla
				Card card = (Card)exportedCards[i].Instantiate(); //crea un oggetto delal carta
				card.CardDeckPosition = cards.Count; //gli da la posizione nel mazzo come chiave
				cards.Add(card.CardDeckPosition, card); //aggiunge al mazzo la carta
			}
		}
	}
	public void InitForEnemy(){ //Inizializzamo il deck(main o side)(risorsa) dell'enemy
		for(int i = 0; i < exportedCards.Length; i++){ //per ogni tscn nel packedscene[]
			if(exportedCards[i] != null){ //se l'oggetto non è diverso da nulla
				Card card = (Card)exportedCards[i].Instantiate(); //crea un oggetto delal carta
				card.CardDeckPosition = cards.Count; //gli da la posizione nel mazzo come chiave
				cards.Add(card.CardDeckPosition, card); //aggiunge al mazzo la carta
			}
		}
	}
	public void AddCard(Card card){ //aggiunge una carta al mazzo se non è pieno
		if (cards.Count < capacity){
			card.CardDeckPosition = cards.Count;
			cards.Add(card.CardDeckPosition, card);
		}
	}
	public void RemoveCard(Card card){ //rimuove una carta
		cards.Remove(card.CardDeckPosition);

		//aggiungere funzione per rimuovere la carta dal packedscene e dal json
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
