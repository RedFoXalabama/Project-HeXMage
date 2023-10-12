using Godot;
using System;
using System.Collections.Generic;

public partial class BattleDeck : Node
{
    //Export
    [Export] Deck mainDeck;
    [Export] Deck sideDeck;
    [Export] HandsCart handsCart;
    //ATTRIBUTI
    private int capacity;
    private Dictionary<int, Card> cards; //definizione stampino delle carte, MainDeck + SideDeck
    private Card[] tempCards; //mazzo temporaneo, viene riempito con le carte del cards e poi mischiato. Usato per la battaglia

    //READY
    public override void _Ready(){
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
    }

    //FUNZIONI
    public void CreateTempDeck(){ //crea il mazzo temporaneo copiandolo dalla definizione per poi ordinarlo casualmente
        foreach(KeyValuePair<int, Card> card in cards){
            int i = 0;
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
                handsCart.AddCard(tempCards[i]);
                tempCards[i] = null;
                break; //appena inserita una carta esce dal ciclo e dalla funzione
            }
        } 
        //se non ci sono carte riempe il mazzo e richiama la funzione
        CreateTempDeck();
        Draw(); 
    }

    //SEGNALI
    public void _on_BattleStart(){ //segnale da collegare con il segnale BattleStart del nodo Battle
        CreateTempDeck(); //Creiamo il mazzo
        for (int i = 0; i < handsCart.Capacity; i++){ //riempiamo la mano
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
}
