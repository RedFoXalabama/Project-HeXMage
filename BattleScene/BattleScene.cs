using Godot;
using System;

public partial class BattleScene : Node2D
{
	//SIGNAL
	[Signal] public delegate void BattleStartEventHandler(); //Segnale per iniziare la battaglia, usato per far preparare i BattleDeck ai characters
	//Collegato con Godot -> Player, Connect -> Enemy
	//Export
	[Export] PackedScene[] enemy_packedScene; //array di packedScene dei nemici 
	//NODI
	Player_BattleScene player;
	Marker2D enemyPosition1; //Posizione dei nemici
	Marker2D enemyPosition2;
	Marker2D enemyPosition3;

	//READY
	public override void _Ready(){
		//inizializzo i nodi
		player = GetNode<Player_BattleScene>("PlayerPosition/Player");

		//creo i nemici
		CreateEnemy(enemy_packedScene);
		EmitSignal("BattleStart");
	}

	//FUNZIONI
	public void CreateEnemy(PackedScene[] enemy){ //dato il paccheto di tscn di nemici, li istanzia e li mette nelle posizioni giuste
		enemyPosition1 = GetNode<Marker2D>("EnemyPosition1");
		enemyPosition2 = GetNode<Marker2D>("EnemyPosition2");
		enemyPosition3 = GetNode<Marker2D>("EnemyPosition3");
		var enemiesPosition = new Marker2D[] {enemyPosition1, enemyPosition2, enemyPosition3};
		for (int i = 0; i < enemy.Length; i++){ //istanzia e collega il segnale BattleStart di ognuno dei nemici
			enemiesPosition[i].AddChild(enemy[i].Instantiate());
			//enemiesPosition[i].GetChild<Enemy_BattleScene>(0).Connect("BattleStart", new Callable(enemiesPosition[i].GetChild<Enemy_BattleScene>(0), "_on_BattleStart_Signal"));
			Connect("BattleStart", new Callable(enemiesPosition[i].GetChild<Enemy_BattleScene>(0), "_on_BattleStart_Signal"));
		}
	}

	public void LoopBattle(){
		
	}
}
