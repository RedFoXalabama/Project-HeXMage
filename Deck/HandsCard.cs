using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]public partial class HandsCard : Resource
{
    //ATTRIBUTI
    private int capacity = 4;
    private Dictionary<int, Card> cardsInHand = new Dictionary<int, Card>();

    //FUNZIONI
    public void AddCard(Card card){ //aggiunge una carta alla mano
        if(cardsInHand.Count < capacity){
            card.CardHandPosition = cardsInHand.Count;
            cardsInHand.Add(card.CardHandPosition, card);
        }
        //FUNZIONE PER AGGIORNARE LA GUI
    }
    public void RemoveCard(Card card){ //rimuove una carta dalla mano
        cardsInHand.Remove(card.CardHandPosition);
        //FUNZIONE PER AGGIORNARE LA GUI
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
