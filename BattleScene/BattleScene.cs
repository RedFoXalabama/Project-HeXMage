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
	//Collegato Godot -> HandsCard_Gui
	[Signal] public delegate void PassEnemysToSelectedEventHandler(Marker2D[] enemys); //Segnale per passare i nemici selezionabili al player
	//Collegato Godot -> Player, facendo cosi ogni volta viene inviata un nuova versione aggiornata dell'array di nemici
	#endregion

	#region PACKEDSCENE ———————————————————————————————————————————————————————————————————————————
	[Export] PackedScene[] enemy_packedScene; //array di packedScene dei nemici
	#endregion

	#region NODE ———————————————————————————————————————————————————————————————————————————
	private Player_BattleScene player;
	private Marker2D playerPosition; //Posizione del player
	private Marker2D enemyPosition1; //Posizione dei nemici
	private Marker2D enemyPosition2;
	private Marker2D enemyPosition3;
	private HandCards_GUI handCards_GUI; //GUI delle carte in mano
	private TextureButton turnButton; //bottone per terminare il turno
	#endregion

	#region ATTRIBUTI ———————————————————————————————————————————————————————————————————————————
	private Enemy_BattleScene[] enemys; //array di nemici
	private Queue<Marker2D> turnQueue; //coda dei turni, prendo le loro posizioni cosi da poterli tenere tutti in una sola coda
	#endregion

	#region READY ———————————————————————————————————————————————————————————————————————————
	public override void _Ready(){
		//inizializzo i nodi
		playerPosition = GetNode<Marker2D>("PlayerPosition");
		player = GetNode<Player_BattleScene>("PlayerPosition/Player");
		enemyPosition1 = GetNode<Marker2D>("EnemyPosition1");
		enemyPosition2 = GetNode<Marker2D>("EnemyPosition2");
		enemyPosition3 = GetNode<Marker2D>("EnemyPosition3");
		handCards_GUI = GetNode<HandCards_GUI>("HandCards_GUI");
		turnButton = handCards_GUI.GetNode<TextureButton>("TurnButton");

		//inizializzo gli attributi
		turnQueue = new Queue<Marker2D>();

		//creo i nemici
		enemys = new Enemy_BattleScene[enemy_packedScene.Length];
		CreateEnemy(enemy_packedScene);
		//emetto il segnale per iniziare la battaglia che fa preparare i mazzi temp dei characters
		EmitSignal("BattleStart");
		//prepato la turnazione, da modificare in base alla formulla della velocità ancora da elaborare
		PrepareTurnQueue();
	}
	#endregion

	#region FUNZIONI PER LA CREAZIONE DELLA BATTAGLIA ———————————————————————————————————————————————————————————————————————————
	public void CreateEnemy(PackedScene[] enemy){ //dato il paccheto di tscn di nemici, li istanzia e li mette nelle posizioni giuste
		var enemiesPosition = new Marker2D[] {enemyPosition1, enemyPosition2, enemyPosition3};
		for (int i = 0; i < enemy.Length; i++){ //istanzia e collega il segnale BattleStart, IsTurnSignal, IsBeenSelected, EndTurnSignal di ognuno dei nemici
			enemiesPosition[i].AddChild(enemy[i].Instantiate());
			enemys[i] = enemiesPosition[i].GetChild<Enemy_BattleScene>(0);
			Connect("BattleStart", new Callable(enemiesPosition[i].GetChild<Enemy_BattleScene>(0), "_on_BattleStart_Signal"));
			Connect("IsTurnSignal", new Callable(enemiesPosition[i].GetChild<Enemy_BattleScene>(0), "_on_battle_scene_is_turn_signal"));
			enemiesPosition[i].GetChild<Enemy_BattleScene>(0).Connect("IsBeenSelectedSignal", new Callable(player, "_on_Enemy_Is_Been_Selected_Signal"));
			enemiesPosition[i].GetChild<Enemy_BattleScene>(0).Connect("EndTurnSignal", new Callable(this, "ChangeTurn"));
		}
	}
	#endregion

	#region FUNZIONI PER LA GESTIONE DELLA BATTAGLIA ———————————————————————————————————————————————————————————————————————————
	public void PrepareTurnQueue(){ //funzione che prepara la coda dei turni
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
				if (player.CheckElementalStatus()){  //controlla se il player è in uno stato elementale e nel caso salta il turno
					GD.Print("Iced"); //TO-DO EFFETTO GRAFICO ICED
					turnButton.EmitSignal("pressed"); //funziona solo con pressed e non Pressed
				};
				EmitSignal("IsTurnSignal"); //segnale per far fare la mossa al player
				EmitSignal("PassEnemysToSelected", enemys); //passa i nemici selezionati
				break;
			case "EnemyPosition1":
				char_turn.GetChild<Enemy_BattleScene>(0).IsTurn = true; //abilita il turno del nemico a fare la mossa
				turnQueue.Enqueue(char_turn); //rimetto il nemico in coda
				if (char_turn.GetChild<Enemy_BattleScene>(0).CheckElementalStatus()){  //controlla se il player è in uno stato elementale e nel caso salta il turno
					char_turn.GetChild<Enemy_BattleScene>(0).EndTurn();
				};
				EmitSignal("IsTurnSignal"); //segnale per far fare la mossa al nemico
				break;
			case "EnemyPosition2":
				char_turn.GetChild<Enemy_BattleScene>(0).IsTurn = true; //abilita il turno del nemico a fare la mossa
				turnQueue.Enqueue(char_turn); //rimetto il nemico in coda
				if (char_turn.GetChild<Enemy_BattleScene>(0).CheckElementalStatus()){  //controlla se il player è in uno stato elementale e nel caso salta il turno
					char_turn.GetChild<Enemy_BattleScene>(0).EndTurn();
				};
				EmitSignal("IsTurnSignal"); //segnale per far fare la mossa al nemico
				break;
			case "EnemyPosition3":
				char_turn.GetChild<Enemy_BattleScene>(0).IsTurn = true; //abilita il turno del nemico a fare la mossa
				turnQueue.Enqueue(char_turn); //rimetto il nemico in coda
				if (char_turn.GetChild<Enemy_BattleScene>(0).CheckElementalStatus()){  //controlla se il player è in uno stato elementale e nel caso salta il turno
					char_turn.GetChild<Enemy_BattleScene>(0).EndTurn();
				};
				EmitSignal("IsTurnSignal"); //segnale per far fare la mossa al nemico
				break;
		}
	}

	public void LoopBattle(){ //WORKING ON
		while (player.Life > 0){
			//player.Turn();
			//player.DrawCard(); //pesca una carta
			//enemy.Turn();
			
		}
	}

	public Boolean CheckStatusBattle(){ //controlla lo stato della battaglia 
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
	public void _on_turn_button_pressed(){ //segnale connesso da HandsCards_GUI -> Godot -> BattleScene
	//se il bottone per terminare il turno viene premuto, allora il turno del player finisce
		if (player.IsTurn){
			player.IsTurn = false;
			turnButton.Disabled = true; //lo riattiva quando è nuovamente il turno del player
			EmitSignal("AbleCardsCollision", false); //disabilita le collisioni delle carte
			ChangeTurn(); //passa al turno successivo
		}
	}
	public void _on_player_animate_card_on_enemy(Card card, Enemy_BattleScene enemy_BattleScene){ //segnale collegato da Player_BattleScene -> Godot -> BattleScene
		//serve per passare la carta e il nemico selezionato, dalla carta si prende il packedscene dell'animazione e lo istanzia sul nemico
		enemy_BattleScene.GetParent<Marker2D>().AddChild(card.CardAnimation.Instantiate());
		//connetto il segnale per abilitare le collisioni delle carte così che possa inviarlo l'animazione una volta finita
		enemy_BattleScene.GetParent<Marker2D>().GetChild<CardAnimation>(1).Connect("AbleCardsCollision", new Callable(handCards_GUI, "_on_Able_Cards_Collision"));
		//avvio l'animazione
		enemy_BattleScene.GetParent<Marker2D>().GetChild<CardAnimation>(1).PlayAnimation();
	}
	public void _on_player_animate_card_on_player(Card card){
		playerPosition.AddChild(card.CardAnimation.Instantiate());
		playerPosition.GetChild<CardAnimation>(1).Connect("AbleCardsCollision", new Callable(handCards_GUI, "_on_Able_Cards_Collision"));
		playerPosition.GetChild<CardAnimation>(1).PlayAnimation();
	}
	#endregion
}
