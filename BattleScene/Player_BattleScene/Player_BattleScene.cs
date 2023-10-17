using Godot;
using System;

public partial class Player_BattleScene : Characters_Battle, DeckUse
{
    //SIGNALS
    [Signal] public delegate void CardsOnGUIEventHandler(HandsCard handsCard); //Segnale per aggiornare la GUI delle carte in mano
    //Collegato Player -> Godot -> HandsCard_Gui

    //INTERFACE
    public void _on_BattleStart_Signal(){ //funzione del segnale BattleStart, emesso da BattleScene (collegato Godot)
        BattleDeck._on_BattleStart();//Prepara il battle deck
        EmitSignal("CardsOnGUI", BattleDeck.HandsCard);
    }
    public void SelectCard(){
    }

    public void UseCard(Card card){
        EmitSignal("CardsOnGUI", BattleDeck.HandsCard); //aggiorna la GUI delle carte in mano
    }
	public void SelectTarget(){

	}
}
