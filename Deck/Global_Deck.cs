using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class Global_Deck : Node
{
	//RISORSE
	private Deck playerDeck = ResourceLoader.Load("res://BattleScene/Player_BattleScene/PlayerDeck.tres") as Deck; //percorso di deck del player
	private string playerDeckCards_JSON = "user://PlayerDeckCards.json"; //informazioni delle carte salvate nel deck del player
	private string starterPlyaerDeckCards_JSON = "res://BattleScene/Player_BattleScene/StarterPlayerDeckCards.json"; //informazioni delle carte base d'inizio del player

	//FUNZIONI
	public void InitiazlizePlayerDeckFromJson(){ //serve per inizializzare il deck del player
		if (FileAccess.FileExists(playerDeckCards_JSON)){
			playerDeck.InitForPlayer(playerDeckCards_JSON);
		} else { //se non esiste inizializziamo 
			playerDeck.InitForPlayer(starterPlyaerDeckCards_JSON);
			SavePlayerDeckInJson();
		}
	}

	//FUNZIONE PER SALVARE LE CARTE DEL PLAYER
public void SavePlayerDeckInJson(){ //salva le carte presenti nel deck nel file json
		var options = new JsonSerializerOptions
    	{
        	WriteIndented = true
    	};
		var cards_address = new Dictionary<int, string>();
		foreach(Card card in playerDeck.Cards.Values){
			cards_address.Add(card.CardDeckPosition, "res://Deck/Card/Cards-Scene/" + card.CardName + ".tscn");
		}
    	var json = JsonSerializer.Serialize(cards_address, options);
    	var file = FileAccess.Open(playerDeckCards_JSON, FileAccess.ModeFlags.Write);
    	file.StoreString(json);
	}
}
