using Godot;
using System;

[GlobalClass] public partial class Characters_Battle : Sprite2D
{
	#region SIGNALS ———————————————————————————————————————————————————————————————————————————
	[Signal] public delegate void PrepareBattleDeckSignalEventHandler(); //Segnale per preparare i deck(risorse) dei characters e poi il BattleDeck, prima dell'inizio della battaglia
	//Collegato con Player -> Godot -> BattleDeck, Enemy -> Godot -> BattleDeck
	[Signal] public delegate void SetStatsGUIEventHandler(Texture2D icon, string name, int maxShield, int maxLife, int maxMana); //Segnale per settare le stats del character nella gui
	//Collegato con Player -> BattleScene Code -> Stats_GUI, Enemy ->  BattleScene Code -> Stats_GUI
	//emesso nel battlescene dopo essersi assicurati che le statsGUI siano state create
	[Signal] public delegate void UpdateStatsGUIEventHandler(int shield, int life, int mana, Boolean isOnFire, Boolean isOnIce, Boolean isOnPoison, Boolean isOnEarth); //Segnale per aggiornare le stats del character nella gui
	//Collegato con Player -> BattleScene Code -> Stats_GUI, Enemy ->  BattleScene Code -> Stats_GUI
	[Signal] public delegate void CheckStatusBattleSignalEventHandler(); //Segnale per controllare se il character è morto o ha ucciso qualcuno
	//Collegato con Player -> Godot -> BattleScene, Enemy -> BattleScene Code -> BattleScene

	//[Signal] public delegate void CharacterIsDeadEventHandler(); //Segnale per dire che il character è morto
	//Collegato con Player -> Godot -> BattleScene, Enemy -> BattleScene Code -> BattleScene

	#endregion
	
	#region NODI ———————————————————————————————————————————————————————————————————————————
	private AnimationPlayer animationPlayer_char;
	#endregion

	#region ATTRIBUTI ———————————————————————————————————————————————————————————————————————————
	[Export] private string char_name;
	[Export] private int life;
	[Export] private int max_life;
	[Export] private int mana;
	[Export] private int max_mana;
	[Export] private int shield;
	[Export] private int max_shield;
	private BattleDeck battleDeck;
	private Boolean isTurn;
	private Boolean is_attacking; //controlla se il player sta attaccando
	private bool firstTurn; //se è il primo turno
	[Export] private Texture2D icon;
	#endregion
	
	#region STATI ELEMENTALI ———————————————————————————————————————————————————————————————————————————
	private Boolean isOnFire;
	private Boolean isOnIce;
	private Boolean isOnPoison;
	private Boolean isOnEarth;
	private Vector2I fireDamage; //x = danni, y = turni
	private int iceTurns; //turni da far saltare
	private Vector2I poisonDamage; //x = danni, y = turni
	#endregion

	#region READY ———————————————————————————————————————————————————————————————————————————
	public override void _Ready(){
		//inizializzo i nodi
		battleDeck = GetNode<BattleDeck>("BattleDeck");
		animationPlayer_char = GetNode<AnimationPlayer>("AnimationPlayer");
		//emetto il segnale per preparare il battle deck
		EmitSignal("PrepareBattleDeckSignal"); //emette il segnale per preparare le risorse deck e poi il battledeck
	}
	#endregion

	#region FUNZIONI ———————————————————————————————————————————————————————————————————————————
	public void TakeDamage(int value){
		//se lo scudo è maggiore di 0, lo scudo assorbe il danno
		if (shield > 0){
			shield -= value;
			if (shield < 0){ //se lo scudo è stato distrutto, il danno in eccesso non viene calcolato
				//life += shield;
				shield = 0;
			}
		}else {//se lo scudo è 0, il danno viene sottratto alla vita
			life -= value;
		}
		//controlliamo se la vita è arrivata a 0 e nel caso emettiamo il segnale per dire che il character è morto
		if(life <= 0){
				life = 0;

		}
		EmitSignal("UpdateStatsGUI", shield, life, mana, isOnFire, isOnIce, isOnPoison, isOnEarth); //emette il segnale per aggiornare le statsGUI
		animationPlayer_char.Play("TakeDamage");
		//TO-DO: PROBABILE AWAIT
		EmitSignal("CheckStatusBattleSignal"); //emette il segnale per controllare se il character è morto o ha ucciso qualcuno
	}
	public void Animate(string animationName){
		animationPlayer_char.Play(animationName);
	}
	public void AddLife(int value){
		if(life + value > max_life){
			life = max_life;
		} else {
			life += value;
		}
		//controlliamo se la vita è arrivata a 0 e nel caso emettiamo il segnale per dire che il character è morto
		if(life <= 0){
				life = 0;
			}
		EmitSignal("UpdateStatsGUI", shield, life, mana, isOnFire, isOnIce, isOnPoison, isOnEarth); //emette il segnale per aggiornare le statsGUI
	}
	public void AddMana(int value){ //non dovrebbe servire
		if (mana + value > max_mana){
			mana = max_mana;
		} else {
			mana += value;
		}
		EmitSignal("UpdateStatsGUI", shield, life, mana, isOnFire, isOnIce, isOnPoison, isOnEarth); //emette il segnale per aggiornare le statsGUI
	}
	public void UseMana(int value){
		if (mana - value < 0){
			mana = 0;
		} else {
			mana -= value;
		}
		EmitSignal("UpdateStatsGUI", shield, life, mana, isOnFire, isOnIce, isOnPoison, isOnEarth); //emette il segnale per aggiornare le statsGUI
	}
	public void ResetMana(){
		if (isOnPoison){
			mana = max_mana/2;
		} else {
			mana = max_mana;
		}
		EmitSignal("UpdateStatsGUI", shield, life, mana, isOnFire, isOnIce, isOnPoison, isOnEarth); //emette il segnale per aggiornare le statsGUI
	}
	public void AddShield(int value){
		if( shield + value > max_shield){
			shield = max_shield;
		} else {
			shield += value;
		}
		EmitSignal("UpdateStatsGUI", shield, life, mana, isOnFire, isOnIce, isOnPoison, isOnEarth); //emette il segnale per aggiornare le statsGUI
	}
	public void DrawCard(){
		battleDeck.Draw();
	}
	
	public Boolean CheckElementalStatus(){ //funzione per controllare se il character è in uno stato elementale
	//il character può trovarsi in tutti gli stati elementali
	//ritorna false se non salta il turno, ritorna true se lo salta.
		if (isOnFire){
			if (fireDamage.Y > 0){
				TakeDamage(fireDamage.X); //arrechiamo danno
				fireDamage.Y--; //diminuiamo il turno
			} else {
				isOnFire = false;
			}
			return false;
		}
		if (isOnIce && iceTurns > 0){
			iceTurns--; //diminuiamo il turno
			if (iceTurns == 0){ // se il turno arriva a zero, lo stato si disattiva
				isOnIce = false;
			}
			/*TESTING*/GD.Print(Name + " ice: "+ isOnIce.ToString());
			return true;
		}
		if (isOnPoison){
			if (poisonDamage.Y > 0){
				AddLife(- poisonDamage.X); //arrechiamo danno solo alla vita aggiungengo un valore negativo
				poisonDamage.Y--; //diminuiamo il turno
				//mana dimezzata nella funzione ResetMana()				
			} else {
				isOnPoison = false;
			}
			return false;
		}
		if (isOnEarth){
			//TO-DO
		}
		EmitSignal("UpdateStatsGUI", shield, life, mana, isOnFire, isOnIce, isOnPoison, isOnEarth); //emette il segnale per aggiornare le statsGUI
		EmitSignal("CheckStatusBattleSignal"); //emette il segnale per controllare se il character è morto o ha ucciso qualcuno
		return false; // Aggiunto return false per gestire il caso in cui nessuno dei precedenti if venga soddisfatto
	}
	public Boolean CheckIfOnStatus(){ //serve a controllare se il character è in uno stato elementale
		if (isOnFire || isOnIce || isOnPoison || isOnEarth){
			return true;
		} else {
			return false;
		}
	}
	//funzioni per settare gli stati elementali
	public void SetOnFire(Boolean value, int damage, int turns){ //SET FIRE
		isOnFire = value;
		fireDamage = new Vector2I(damage, turns);
		EmitSignal("UpdateStatsGUI", shield, life, mana, isOnFire, isOnIce, isOnPoison, isOnEarth); //emette il segnale per aggiornare le statsGUI
	}
	public void SetOnIce(Boolean value, int turns){ //SET ICE
		isOnIce = value;
		iceTurns = turns;
		EmitSignal("UpdateStatsGUI", shield, life, mana, isOnFire, isOnIce, isOnPoison, isOnEarth); //emette il segnale per aggiornare le statsGUI
	}
	public void SetOnPoison(Boolean value, int damage, int turns){ //SET POISON
		isOnPoison = value;
		poisonDamage = new Vector2I(damage, turns);
		EmitSignal("UpdateStatsGUI", shield, life, mana, isOnFire, isOnIce, isOnPoison, isOnEarth); //emette il segnale per aggiornare le statsGUI
	}
	public void SetOnEarth(Boolean value){ //SET EARTH
		isOnEarth = value;
		EmitSignal("UpdateStatsGUI", shield, life, mana, isOnFire, isOnIce, isOnPoison, isOnEarth); //emette il segnale per aggiornare le statsGUI
	}
	#endregion

	#region GETTER/SETTER ———————————————————————————————————————————————————————————————————————————
	public string Char_Name{
		get{return char_name;}
		set{char_name = value;}
	}
	public int Life{
		get{return life;}
		set{life = value;}
	}
	public int Max_Life{
		get{return max_life;}
		set{max_life = value;}
	}
	public int Mana{
		get{return mana;}
		set{mana = value;}
	}
	public int Max_Mana{
		get{return max_mana;}
		set{max_mana = value;}
	}
	public int Shield{
		get{return shield;}
		set{shield = value;}
	}
	public int Max_Shield{
		get{return max_shield;}
		set{max_shield = value;}
	}
	public BattleDeck BattleDeck{
		get{return battleDeck;}
		set{battleDeck = value;}
	}
	public Boolean IsTurn{
		get{return isTurn;}
		set{isTurn = value;}
	}
	public AnimationPlayer AnimationPlayer_char{
		get{return animationPlayer_char;}
		set{animationPlayer_char = value;}
	}
	public Boolean Is_attacking{
		get{return is_attacking;}
		set{is_attacking = value;}
	}
	public Boolean IsOnFire{
		get{return isOnFire;}
		set{isOnFire = value;}
	}
	public Boolean IsOnIce{
		get{return isOnIce;}
		set{isOnIce = value;}
	}
	public Boolean IsOnPoison{
		get{return isOnPoison;}
		set{isOnPoison = value;}
	}
	public Boolean IsOnEarth{
		get{return isOnEarth;}
		set{isOnEarth = value;}
	}
	public bool FirstTurn{
		get{return firstTurn;}
		set{firstTurn = value;}
	
	}
	public Texture2D Icon{
		get{return icon;}
		set{icon = value;}
	}
	#endregion
}
interface DeckUse {
	void _on_BattleStart_Signal();
	void _on_battle_scene_is_turn_signal();
}