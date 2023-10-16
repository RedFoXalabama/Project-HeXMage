using Godot;
using System;

public partial class Enemy_BattleScene : Characters_Battle, DeckUse
{
	public void _on_BattleStart_Signal(){ //funzione del segnale BattleStart, emesso da BattleScene (collegato Godot)
		BattleDeck._on_BattleStart();//Prepara il battle deck
	}
	public void SelectCard(){
	}

	public void UseCard(Card card){

	}
	public void SelectTarget(){

	}

}
