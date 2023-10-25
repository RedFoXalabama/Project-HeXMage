using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]public partial class HandsCard : Resource
{
    //ATTRIBUTI
    private int capacity = 4;
    //private Dictionary<int, Card> cardsInHand = new Dictionary<int, Card>();
    private Card[] cardsInHand = new Card[4];

    //FUNZIONI
    public void AddCard(Card card){ //aggiunge una carta alla mano, dizionario di carte
        /*if(cardsInHand.Count < capacity){
            card.CardHandPosition = cardsInHand.Count;
            cardsInHand.Add(card.CardHandPosition, card);
        }*/
        int count = 0;
        for(int i = 0; i < cardsInHand.Length; i++){
            if(cardsInHand[i] != null){
                count++;
            }
        }
        if(count < capacity){
            card.CardHandPosition = count;
            cardsInHand[card.CardHandPosition] = card;
        }
        //FUNZIONE PER AGGIORNARE LA GUI
    }
    public void RemoveCard(Card card){ //rimuove una carta dalla mano
        cardsInHand[card.CardHandPosition] = null;
        foreach (Card pointedCard in cardsInHand){
            if ((pointedCard != null) && (pointedCard.CardHandPosition > card.CardHandPosition)){
                pointedCard.CardHandPosition -= 1;
                cardsInHand[pointedCard.CardHandPosition] = pointedCard;
                cardsInHand[pointedCard.CardHandPosition + 1] = null;
            }
        }
        card.Animate("Expire");
    }
    //GETTER-SETTER
    public int Capacity{
        get{return capacity;}
        set{capacity = value;}
    }
    public Card[] CardsInHand{
        get{return cardsInHand;}
        set{cardsInHand = value;}
    }
}
