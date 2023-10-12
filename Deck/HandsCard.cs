using Godot;
using System;
using System.Collections.Generic;

public partial class HandsCart : Resource
{
    //ATTRIBUTI
    private int capacity = 4;
    private Dictionary<int, Card> cardsInHand;

    //FUNZIONI
    public void AddCard(Card card){ //aggiunge una carta alla mano
        if(cardsInHand.Count < capacity){
            cardsInHand.Add(card.CardId, card);
        }
    }
    public void RemoveCard(Card card){
        cardsInHand.Remove(card.CardId);
    }

    //GETTER-SETTER
    public int Capacity{
        get{return capacity;}
        set{capacity = value;}
    }
    public Dictionary<int, Card> Cards{
        get{return cardsInHand;}
        set{cardsInHand = value;}
    }
}
