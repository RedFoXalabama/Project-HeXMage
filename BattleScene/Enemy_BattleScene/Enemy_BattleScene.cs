using Godot;
using System;

public partial class Enemy_BattleScene : Characters_Battle, DeckUse
{
	#region SIGNALS ———————————————————————————————————————————————————————————————————————————
	[Signal] public delegate void EndTurnSignalEventHandler(); //Segnale per far finire il turno
	#endregion

	#region ATTRIBUTI ———————————————————————————————————————————————————————————————————————————
	//private Boolean isInBattle;
	#endregion
	
	#region READY ———————————————————————————————————————————————————————————————————————————
	/*public override void _Ready(){
		BattleDeck = GetNode<BattleDeck>("BattleDeck");
		EmitSignal("PrepareBattleDeckSignal"); //emette il segnale per preparare le risorse deck e poi il battledeck
	}*/
	#endregion
	
	#region FUNZIONI ———————————————————————————————————————————————————————————————————————————
	public void ConnectSignalsForBattle(){
		Connect("EndTurnSignal", new Callable(GetParent().GetParent(), "ChangeTurn"));
	}
	public void EndTurn(){
		IsTurn = false;
		EmitSignal("EndTurnSignal");
	}
	public void SelectCard(){
	}

	public void UseCard(Card card){

	}
	public void SelectTarget(){

	}
	#endregion

	#region SIGNALS ———————————————————————————————————————————————————————————————————————————
	public void _on_BattleStart_Signal(){ //funzione del segnale BattleStart, emesso da BattleScene (collegato Godot)
		BattleDeck._on_BattleStart();//Prepara il battle deck
	}

	public void _on_battle_scene_is_turn_signal(){
        if (IsTurn){
           //funzione che gestisce la mossa

			//INZIO TEST
			Animate("Attack");
			//FINE TEST
			
			EndTurn();
        }
    }
	#endregion

	#region GETTER/SETTER ———————————————————————————————————————————————————————————————————————————
	/*public Boolean IsInBattle{
		get{return isInBattle;}
		set{isInBattle = value;}
	}*/
	#endregion
}
