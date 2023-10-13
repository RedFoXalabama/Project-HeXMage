using Godot;
using System;
using System.Collections.Generic;

public partial class BattleDeck : Node
{
    //Export
    [Export] Deck mainDeck;
    [Export] Deck sideDeck;
    //[Export] PackedScene exportedCards;
    private HandsCard handsCard;
    //ATTRIBUTI
    private int capacity;
    private Dictionary<int, Card> cards; //definizione stampino delle carte, MainDeck + SideDeck
    private Card[] tempCards; //mazzo temporaneo, viene riempito con le carte del cards e poi mischiato. Usato per la battaglia

    //READY
    public override void _Ready(){
        /* //PER TESTING
            Card testcard1 = (Card)exportedCards.Instantiate();
            testcard1.CardId = 1;
            Card testcard2 = (Card)exportedCards.Instantiate();
            testcard2.CardId = 2;
            Card testcard3 = (Card)exportedCards.Instantiate();
            testcard3.CardId = 3;
            mainDeck.AddCard(testcard1);
            mainDeck.AddCard(testcard2);
            mainDeck.AddCard(testcard3);
        //FINE TESTING */

        //inizializzo gli attributi
        capacity = mainDeck.Capacity + sideDeck.Capacity;
        cards = new Dictionary<int, Card>();
        //Inizializzo il cards
        foreach(KeyValuePair<int, Card> card in mainDeck.Cards){
            cards.Add(card.Key, card.Value);
        }
        foreach(KeyValuePair<int, Card> card in sideDeck.Cards){
            cards.Add(card.Key, card.Value);
        }
        //inizializzo il mazzo temporaneo
        tempCards = new Card[capacity];
        //inizializzo HandsCard
        handsCard = new HandsCard();

        /* //TESTING 
        CreateTempDeck();
        Draw();
        //FINE TESTING */
    }

    //FUNZIONI
    public void CreateTempDeck(){ //crea il mazzo temporaneo copiandolo dalla definizione per poi ordinarlo casualmente
        int i = 0;
        foreach(KeyValuePair<int, Card> card in cards){
            tempCards[i] = (card.Value);
            i++;
        }
        Sort();
    }
    public void Sort(){ //misca le carte del mazzo temporaneo
        Random random = new Random();
        Shuffle(random, tempCards);
    }

    public void Draw(){ //Serve a pescare una carta dal mazzo temporaneo e metterla in mano
        //cicla nel tempCards e restituisce la prima carta non nulla
        for(int i = 0; i < tempCards.Length; i++){
            if(tempCards[i] != null){
                handsCard.AddCard(tempCards[i]);
                tempCards[i] = null;
                return; //appena inserita una carta esce dal ciclo e dalla funzione
            }
        } 
        //se non ci sono carte riempe il mazzo e richiama la funzione
        CreateTempDeck();
        Draw(); 
    }

    //SEGNALI
    public void _on_BattleStart(){ //segnale da collegare con il segnale BattleStart del nodo Battle
        CreateTempDeck(); //Creiamo il mazzo
        for (int i = 0; i < handsCard.Capacity; i++){ //riempiamo la mano
            Draw();
        }
    }

    //FUNZIONI ESTENSIVE
    public void Shuffle(Random rng, Card[] array){
        int n = array.Length;
        while (n > 1) 
        {
            int k = rng.Next(n--);
            Card temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    //GETTER-SETTER
    public HandsCard HandsCard{
        get{return handsCard;}
        set{handsCard = value;}
    }
}
