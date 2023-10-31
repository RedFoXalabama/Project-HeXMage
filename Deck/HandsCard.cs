using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]public partial class HandsCard : Resource
{
    //ATTRIBUTI
    private int capacity = 4; //capacità della mano
    private Card[] cardsInHand = new Card[4]; //array di carte in mano

    //FUNZIONI
    public void AddCard(Card card){ //aggiunge una carta alla mano, dizionario di carte
        //contiamo le carte presenti nella mano
        int count = 0;
        for(int i = 0; i < cardsInHand.Length; i++){
            if(cardsInHand[i] != null){
                count++;
            }
        }
        //Se c'è spazio aggiungiamo una carta e segniamo la sua posizione nella mano come quella del count
        if(count < capacity){
            card.CardHandPosition = count;
            cardsInHand[card.CardHandPosition] = card;
        }
    }
    public void RemoveCard(Card card){ //rimuove una carta dalla mano
        cardsInHand[card.CardHandPosition] = null; //impostiamo a null il suo posto nell'array
        foreach (Card pointedCard in cardsInHand){ //rispostiamo ogni carta in modo da occupare i primi spazi e non avere spazi vuoti in mezzo
            if ((pointedCard != null) && (pointedCard.CardHandPosition > card.CardHandPosition)){
                pointedCard.CardHandPosition -= 1;
                cardsInHand[pointedCard.CardHandPosition] = pointedCard;
                cardsInHand[pointedCard.CardHandPosition + 1] = null;
            }
        }
        card.Animate("Expire"); //animazione di sparizione
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
