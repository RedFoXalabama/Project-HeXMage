using Godot;
using System;

public partial class Enemy_BattleScene : Characters_Battle, DeckUse
{
	#region SIGNALS ———————————————————————————————————————————————————————————————————————————
	[Signal] public delegate void EndTurnSignalEventHandler(); //Segnale per far finire il turno
	//connesso da Enemy_BattleScene -> Codice Battlescene -> BattleScene
	[Signal] public delegate void IsBeenSelectedSignalEventHandler(Enemy_BattleScene enemy_BattleScene); //Segnale per passare il nemico selezionato al player
	//connesso da Enemy_BattleScene -> Codice Battlescene -> Player_BattleScene
	#endregion

	#region ATTRIBUTI ———————————————————————————————————————————————————————————————————————————
	private CollisionShape2D collisionShape2D;
	#endregion
	
	#region READY ———————————————————————————————————————————————————————————————————————————
	public override void _Ready(){
		//inizializzo i nodi
		BattleDeck = GetNode<BattleDeck>("BattleDeck");
		AnimationPlayer_char = GetNode<AnimationPlayer>("AnimationPlayer");
		collisionShape2D = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
		//emetto il segnale per preparare il battle deck
		EmitSignal("PrepareBattleDeckSignal"); //emette il segnale per preparare le risorse deck e poi il battledeck
		ToBeUnselected(); //disattiva la collisione cosi da non poter ricevere input quando non desiderato
	}
	#endregion
	
	#region FUNZIONI ———————————————————————————————————————————————————————————————————————————
	public void EndTurn(){ //termina il turno
		IsTurn = false;
		EmitSignal("EndTurnSignal"); //invia il segnale per terminare il turno e passare al successivo
	}
	public void SelectCard(){
	}

	public void UseCard(){

	}
	public void SelectTarget(){

	}

	public void ToBeSelected(){ //funzione per attivare la collisione cosi da poter ricevere input
		collisionShape2D.SetDeferred("disabled", false);
	}
	public void ToBeUnselected(){ //funzione per disattivare la collisione cosi da non poter ricevere input
		collisionShape2D.SetDeferred("disabled", true);
	}
	#endregion

	#region SIGNALS ———————————————————————————————————————————————————————————————————————————
	public void _on_BattleStart_Signal(){ //funzione del segnale BattleStart, emesso da BattleScene (collegato Godot)
		BattleDeck._on_BattleStart();//Prepara il battle deck
	}

	public void _on_battle_scene_is_turn_signal(){ //funzione del segnale IsTurnSignal per eseguire la mossa quando è il turno del nemico
        if (IsTurn){
           //funzione che gestisce la mossa

			//INZIO TEST
			Animate("Attack");
			//FINE TEST
			
			EndTurn();
        }
    }

	public void _on_area_2d_input_event(Node viewport, InputEvent @event, long shapeIdx){ //funzione del segnale input_event, collegato con Godot
		if (@event.IsActionPressed("ui_select")){ //se selezionato col tasto sinistro del mouse inva il segnale IsBeenSelectedSignal al player passando il nemico
			EmitSignal("IsBeenSelectedSignal", this); //serve per passare il nemico selezionato al player cosi da poter interagire con esso
		}
	}
	#endregion

	#region GETTER/SETTER ———————————————————————————————————————————————————————————————————————————

	#endregion
}
