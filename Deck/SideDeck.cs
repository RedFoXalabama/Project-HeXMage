using Godot;
using System;

[GlobalClass] public partial class SideDeck : Deck
{
    //Deck che eredita tutte le funzioni del Deck, con un limite inferiore e con la funzione per pulirlo quando finisce il dungeon
    //FUNZIONI
    public void ClearDeck(){
        Cards.Clear();
    }
}
