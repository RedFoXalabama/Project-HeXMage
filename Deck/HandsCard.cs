using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]public partial class HandsCard : Resource
{
    #region ATTRIBUTI ———————————————————————————————————————————————————————————————————————————
    private int capacity = 4; //capacità della mano
    private Card[] cardsInHand = new Card[4]; //array di carte in mano
    #endregion

    #region FUNZIONI ———————————————————————————————————————————————————————————————————————————
    public void AddCard(Card card){ //aggiunge una carta alla mano, dizionario di carte
        //contiamo le carte presenti nella mano
        var count = CountCardsInHand();
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

    public void SortCardsInHands(){ //funzione per ordinare le carte nella mano spostando gli spazii vuoti alla fine
        Card[] sortedCards = new Card[cardsInHand.Length];
        int cardIndex = 0;
        foreach (Card card in cardsInHand) {
            if (card != null) {
                sortedCards[cardIndex] = card;
                cardIndex++;
            }
        }
        cardsInHand = sortedCards;
    }

    public int CountCardsInHand(){ //serve a contare quante carte ci sono nella mano
    //non si può usare cardsInHand.Length perchè ci sono spazi vuoti null, ma che vengono conteggiati perchè hanno info di godot
        int count = 0;
        for(int i = 0; i < cardsInHand.Length; i++){
            if(cardsInHand[i] != null){
                count++;
            }
        }
        return count;
    }
    #endregion

    #region GETTER-SETTER ———————————————————————————————————————————————————————————————————————————
    public int Capacity{
        get{return capacity;}
        set{capacity = value;}
    }
    public Card[] CardsInHand{
        get{return cardsInHand;}
        set{cardsInHand = value;}
    }
    #endregion
}
