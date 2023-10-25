using Godot;
using System;
using System.Collections.Generic;

public partial class BattleScene : Node2D
{
	#region SIGNAL ———————————————————————————————————————————————————————————————————————————
	[Signal] public delegate void BattleStartEventHandler(); //Segnale per iniziare la battaglia, usato per far preparare i BattleDeck ai characters
	//Collegato con Godot -> Player, Connect -> Enemy
	[Signal] public delegate void IsTurnSignalEventHandler(); //Segnale per far fare la mossa al character che ha il turno
	//Collegato con Godot -> Player, Connect -> Enemy
	[Signal] public delegate void AbleCardsCollisionEventHandler(Boolean value); //Segnale per disabilitare le collisioni delle carte
    //Collegato Player -> Godot -> HandsCard_Gui
	#endregion
	//Export
	[Export] PackedScene[] enemy_packedScene; //array di packedScene dei nemici 

	#region NODE ———————————————————————————————————————————————————————————————————————————
	Player_BattleScene player;
	Marker2D playerPosition; //Posizione del player
	Marker2D enemyPosition1; //Posizione dei nemici
	Marker2D enemyPosition2;
	Marker2D enemyPosition3;
	TextureButton turnButton;
	#endregion
	//ATTRIBUTI
	Enemy_BattleScene[] enemys; //array di nemici
	Queue<Marker2D> turnQueue; //coda dei turni, prendo le loro posizioni cosi da poterli tenere tutti in una sola coda

	#region Ready ———————————————————————————————————————————————————————————————————————————
	public override void _Ready(){
		//inizializzo i nodi
		playerPosition = GetNode<Marker2D>("PlayerPosition");
		player = GetNode<Player_BattleScene>("PlayerPosition/Player");
		enemyPosition1 = GetNode<Marker2D>("EnemyPosition1");
		enemyPosition2 = GetNode<Marker2D>("EnemyPosition2");
		enemyPosition3 = GetNode<Marker2D>("EnemyPosition3");
		turnButton = GetNode<TextureButton>("HandCards_GUI/TurnButton");

		//inizializzo gli attributi
		turnQueue = new Queue<Marker2D>();

		//creo i nemici
		enemys = new Enemy_BattleScene[enemy_packedScene.Length];
		CreateEnemy(enemy_packedScene);
		EmitSignal("BattleStart");
		PrepareTurnQueue();

	}
	#endregion

	#region FUNZIONI PER LA CREAZIONE DELLA BATTAGLIA ———————————————————————————————————————————————————————————————————————————
	public void CreateEnemy(PackedScene[] enemy){ //dato il paccheto di tscn di nemici, li istanzia e li mette nelle posizioni giuste
		var enemiesPosition = new Marker2D[] {enemyPosition1, enemyPosition2, enemyPosition3};
		for (int i = 0; i < enemy.Length; i++){ //istanzia e collega il segnale BattleStart di ognuno dei nemici
			enemiesPosition[i].AddChild(enemy[i].Instantiate());
			enemys[i] = enemiesPosition[i].GetChild<Enemy_BattleScene>(0);
			Connect("BattleStart", new Callable(enemiesPosition[i].GetChild<Enemy_BattleScene>(0), "_on_BattleStart_Signal"));
			Connect("IsTurnSignal", new Callable(enemiesPosition[i].GetChild<Enemy_BattleScene>(0), "_on_battle_scene_is_turn_signal"));
			enemiesPosition[i].GetChild<Enemy_BattleScene>(0).ConnectSignalsForBattle();
		}
	}
	#endregion

	#region FUNZIONI PER LA GESTIONE DELLA BATTAGLIA ———————————————————————————————————————————————————————————————————————————
	public void PrepareTurnQueue(){
		//funzione che prepara la coda dei turni
		//FUNZIONE DA SCRIVERE PER IL CALCOLO DI CHI PRIMA INIZIA
		//per ora metto per primo il player e poi tutti i nemici
		turnQueue.Enqueue(playerPosition);
		turnQueue.Enqueue(enemyPosition1);
		turnQueue.Enqueue(enemyPosition2);
		turnQueue.Enqueue(enemyPosition3);
		ChangeTurn();
	}
	public void ChangeTurn(){ //funzione collegata tramite segnale Connect in enemy per il cambio turno
		Marker2D char_turn = turnQueue.Dequeue();
		switch (char_turn.Name){
			case "PlayerPosition":
				player.IsTurn = true; //abilita il turno del player a fare la mossa
				turnButton.Disabled = false; //abilita il bottone per terminare il turno
				turnQueue.Enqueue(char_turn); //rimetto il player in coda
				EmitSignal("IsTurnSignal"); //segnale per far fare la mossa al player
				EmitSignal("AbleCardsCollision", true); //abilita le collisioni delle carte
				break;
			case "EnemyPosition1":
				char_turn.GetChild<Enemy_BattleScene>(0).IsTurn = true; //abilita il turno del nemico a fare la mossa
				turnQueue.Enqueue(char_turn); //rimetto il nemico in coda
				EmitSignal("IsTurnSignal"); //segnale per far fare la mossa al nemico
				break;
			case "EnemyPosition2":
				char_turn.GetChild<Enemy_BattleScene>(0).IsTurn = true; //abilita il turno del nemico a fare la mossa
				turnQueue.Enqueue(char_turn); //rimetto il nemico in coda
				EmitSignal("IsTurnSignal"); //segnale per far fare la mossa al nemico
				break;
			case "EnemyPosition3":
				char_turn.GetChild<Enemy_BattleScene>(0).IsTurn = true; //abilita il turno del nemico a fare la mossa
				turnQueue.Enqueue(char_turn); //rimetto il nemico in coda
				EmitSignal("IsTurnSignal"); //segnale per far fare la mossa al nemico
				break;
		}
	}
	public void LoopBattle(){
		while (player.Life > 0){
			//player.Turn();
			//player.DrawCard(); //pesca una carta
			//enemy.Turn();
			
		}
	}

	public Boolean CheckStatusBattle(){
		//return false se la battaglia non è finita, ritorna true se la battaglia è finita
		CheckEnemysLife(); // controlla quali nemici sono morti e li rimuove
		if (player.Life <= 0){
			//gameover
			GameOver();
			return false;
		} else if (enemys.Length == 0){
			//vittoria
			Win();
			return false;
		} else {
			//La battaglia non è finita
			return true;
		}
	}
	
	public void Win(){//funzione vittoria da definire
		//vittoria
	}
	public void GameOver(){//funzione gameover da definire
		//gameover
	}
	public void CheckEnemysLife(){// controlla quali nemici sono morti e li rimuove
		for (int i = 0; i < enemys.Length; i++){
			if (enemys[i].Life <= 0){
				enemys[i].QueueFree();
				enemys[i] = null;
			}
		}
	}
	#endregion

	#region SIGNALS ———————————————————————————————————————————————————————————————————————————
	public void _on_turn_button_pressed(){
		if (player.IsTurn){
			player.IsTurn = false;
			turnButton.Disabled = true; //lo riattiva quando è nuovamente il turno del player
			EmitSignal("AbleCardsCollision", false); //disabilita le collisioni delle carte
			ChangeTurn();
		}
	}
	#endregion
}
