using Godot;
using System;

[GlobalClass] public partial class Characters_Battle : Sprite2D
{
	#region SIGNALS ———————————————————————————————————————————————————————————————————————————
	[Signal] public delegate void PrepareBattleDeckSignalEventHandler(); //Segnale per preparare i deck(risorse) dei characters e poi il BattleDeck, prima dell'inizio della battaglia
	//Collegato con Player -> Godot -> BattleDeck, Enemy -> Godot -> BattleDeck
	#endregion
	
	#region NODI ———————————————————————————————————————————————————————————————————————————
	private AnimationPlayer animationPlayer_char;
	#endregion

	#region ATTRIBUTI ———————————————————————————————————————————————————————————————————————————
	[Export] private int life;
	[Export] private int mana;
	[Export] private int max_mana;
	[Export] private int shield;
	BattleDeck battleDeck;
	Boolean isTurn;
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
			if (shield < 0){ //se lo scudo è stato distrutto, il danno in eccesso viene sottratto alla vita
				life += shield;
				shield = 0;
			}
		}else {//se lo scudo è 0, il danno viene sottratto alla vita
			life -= value;
		}
		animationPlayer_char.Play("TakeDamage");
	}
	public void Animate(string animationName){
		animationPlayer_char.Play(animationName);
	}
	public void AddLife(int value){
		life += value;
	}
	public void AddMana(int value){ //non dovrebbe servire
		mana += value;
	}
	public void UseMana(int value){
		mana -= value;
	}
	public void ResetMana(){
		mana = max_mana;
	}
	public void AddShield(int value){
		shield += value;
	}
	public void DrawCard(){
		battleDeck.Draw();
	}
	#endregion

	#region GETTER/SETTER ———————————————————————————————————————————————————————————————————————————
	public int Life{
		get{return life;}
		set{life = value;}
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
	#endregion
}
interface DeckUse {
	void UseCard(Card card);
	//void SelectCard(); serve una definizione diversa degli argomenti per il player e il nemico
	void SelectTarget();
	void _on_BattleStart_Signal();
	void _on_battle_scene_is_turn_signal();
}