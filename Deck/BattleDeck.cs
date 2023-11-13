using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

public partial class BattleDeck : Node
{
    #region ATTRIBUTI ———————————————————————————————————————————————————————————————————————————
    [Export] Deck mainDeck;
    [Export] Deck sideDeck;
    private HandsCard handsCard;
    //ATTRIBUTI
    private int capacity;
    private Dictionary<int, Card> cards; //definizione stampino delle carte, MainDeck + SideDeck
    private Card[] tempCards; //mazzo temporaneo, viene riempito con le carte del cards e poi mischiato. Usato per la battaglia
    #endregion

    #region FUNZIONI ———————————————————————————————————————————————————————————————————————————
    public void InitBattleDeck(){ //funzione per creare il mazzo (stampino) e il mazzo temp, inizializza anche HandsCard
        //definisco la capacità del mazzo e lo inizializzo
        capacity = mainDeck.Capacity + sideDeck.Capacity;
        cards = new Dictionary<int, Card>();
        //Inizializzo il cards, prendendo le carte dal maindeck e dal side deck
        foreach(KeyValuePair<int, Card> card in mainDeck.Cards){
            cards.Add(card.Key, card.Value);
        }
        foreach(KeyValuePair<int, Card> card in sideDeck.Cards){
            cards.Add(mainDeck.Capacity + card.Key, card.Value);
        }
        //inizializzo il mazzo temporaneo
        tempCards = new Card[capacity];
        //inizializzo HandsCard
        handsCard = new HandsCard();
    }
    public void CreateTempDeck(){ //crea il mazzo temporaneo copiandolo dalla definizione per poi ordinarlo casualmente
        int i = 0;
        foreach(KeyValuePair<int, Card> card in cards){
            tempCards[i] = card.Value;
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
    #endregion

    #region SEGNALI ———————————————————————————————————————————————————————————————————————————
    public void _on_BattleStart(){ //segnale da collegare con il segnale BattleStart del nodo Battle
        CreateTempDeck(); //Creiamo il mazzo
        for (int i = 0; i < handsCard.Capacity; i++){ //riempiamo la mano
            Draw();
        }
    }
    public void _on_PrepareBattleDeck_Player(){ //prepare le risorse maindeck e sidedeck del player tramite json e poi inizializza il battle deck
        GetNode<Global_Deck>("/root/GlobalDeck").InitiazlizePlayerDeckFromJson(); //funzione definita nel global_deck
        //non inizializzo ancora il sidedeck, perchè dovrebbe essere un mazzo temporaneo
        InitBattleDeck();
    }

    public void _on_PrepareBattleDeck_Enemy(){ //prepara il battle deck del nemico
        mainDeck.InitForEnemy();
        sideDeck.InitForEnemy();
        InitBattleDeck();
    }
    #endregion

    #region FUNZIONI ESTENSIVE ———————————————————————————————————————————————————————————————————————————
    public void Shuffle(Random rng, Card[] array){ //funzione per mischiare le carte
        int n = array.Length;
        while (n > 1) 
        {
            int k = rng.Next(n--);
            Card temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
    #endregion

    #region GETTER-SETTER ———————————————————————————————————————————————————————————————————————————
    public HandsCard HandsCard{
        get{return handsCard;}
        set{handsCard = value;}
    }
    public Deck Maindeck{
        get{return mainDeck;}
        set{mainDeck = value;}
    }
    public Deck SideDeck{
        get{return sideDeck;}
        set{sideDeck = value;}
    }
    #endregion
}
