using Godot;
using System;

public partial class Card : TextureRect
{
	#region SIGNAL ———————————————————————————————————————————————————————————————————————————
	[Signal] public delegate void CardExpiredEventHandler(); //Segnale per dire che la carta è sparita
	#endregion

	#region ATTRIBUTI ———————————————————————————————————————————————————————————————————————————
	[ExportGroup("Card Proprieties")]
	[Export] int cardId; //id della carta
	[Export] private string card_name;

	private int card_deck_position; //posizione della carta nel deck
	private int card_hand_position; //posizione della carta nella mano
	private Boolean is_focused; //se la carta è focussata
	private Boolean is_input_connected; //se la carta è connessa al segnale
	#endregion

	#region NODI ———————————————————————————————————————————————————————————————————————————
	[Export] PackedScene cardAnimation; //animazione da eseguire, nodo esterno da allegare
	private AnimationPlayer animationPlayer; //starta con stan-by da godot
	private CollisionShape2D collisionShape2D;
	#endregion

	#region EFFECT (All exported) ———————————————————————————————————————————————————————————————————————————
	//i valori a destra sono gli esponenti del 2 - 1, (2^{n-1}), le combinazioni (sembrano essere) la somma dei due valori
	[Flags] public enum CardTargetEnum {
		Self = 1 << 1, // 2^0=1
   		Enemy = 1 << 2, // 2^1=2
	}
	[Flags] public enum AttackTypeEnum {
    	AtkSoft = 1 << 1,
    	AtkHard = 1 << 2,
	}
	[Flags] public enum StatsTypeEnum {
		Heal = 1 << 1,
		Shield = 1 << 2,
		HealAndShield = Heal | Shield, // 2^0+2^1=3
	}
	[Flags] public enum ElementTypeEnum {
		Fire = 1 << 1,
		Ice = 1 << 2,
		Poison = 1 << 3, // 2^2=4
		Earth = 1 << 4, // 2^3=8
	}
	[Flags] public enum PassiveEnum {
		Rage = 1 << 1,
	}

	[ExportGroup("EFFECT")]
		[ExportSubgroup("Target")]
			[Export] public int cardLevel;
			[Export] public int specValue;
			[Export] public int mana_value;
		[ExportSubgroup("Attack Type")]
			[Export(PropertyHint.Flags, "Self,Enemy")] public int CardTarget { get; set; } = 0;
			[Export(PropertyHint.Flags, "Heal,Shield")] public int StatsType { get; set; } = 0;
			[Export(PropertyHint.Flags, "AtkSoft,AtkHard")] public int AttackType { get; set; } = 0;
		[ExportSubgroup("Element Type")]
			[Export(PropertyHint.Flags, "Fire,Ice,Poison,Earth")] public int ElementType { get; set; } = 0;
		[ExportSubgroup("Passive")]
			[Export(PropertyHint.Flags, "Rage")] public int Passive { get; set; } = 0;
	#endregion

	#region READY ———————————————————————————————————————————————————————————————————————————
	public override void _Ready(){
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		collisionShape2D = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
	}
	#endregion

	#region FUNZIONI ———————————————————————————————————————————————————————————————————————————
	public void Animate(string animationName){
		animationPlayer.Play(animationName); //da errore se la carta che stai cercando di animare non è una scena, ma solo un oggetto
	}
	public void Expire(){ //da chiamare quando la carta viene usata
		EmitSignal("CardExpired");
		QueueFree();
	}

	public void ReSizeCollsion(Vector2 size, Vector2 pos){ //reimposta la collisione della carta
		collisionShape2D.Shape.SetDeferred("size", size);
		collisionShape2D.SetDeferred("position", pos);
	}

	//di seguito le funzioni castate dalle carte
	//le funzioni sono divise per nemici e player. Se la definizione contiene il nemico farà danni o curerà il nemico e cosi quella con il player
	//se si vuole fare danno al nemico il player chiamerà la def con enemy, se vuole curare se stesso la def con player, viceversa il nemico
	//la scelta della funzione da chiamare viene fatta al momento del cast della carta con una if(hasEnemy)
	public void ExecuteCard(Enemy_BattleScene enemy){
		switch(AttackType){
			case 1: //AtkSoft
				enemy.TakeDamage(1+cardLevel);
				break;
			case 2: //AtkHard
				enemy.TakeDamage(3+2*cardLevel);
				break;
		}
		switch(StatsType){
			case 1: //Heal
				enemy.AddLife(1+cardLevel); //DA CAMBIARE
				break;
			case 2: //Shield
				enemy.AddShield(1+cardLevel); //DA CAMBIARE
				break;
			case 3: //HealAndShield
				enemy.AddLife(1+cardLevel); //DA CAMBIARE
				enemy.AddShield(1+cardLevel); //DA CAMBIARE
				break;
		}
		switch(ElementType){
			case 1: //Fire
				enemy.SetOnFire(true, cardLevel, specValue);
				break;
			case 2: //Ice
				enemy.SetOnIce(true, specValue);
				//DA CAMBIARE
				break;
			case 4: //Poison
				enemy.SetOnPoison(true, cardLevel, specValue);
				break;
			case 8: //Earth
				//TO-DO
				break;
		}
		switch (Passive){
			case 1: //Rage
				//TO-DO
				break;
		}
	}
	public void ExecuteCard(Player_BattleScene player){
		switch(AttackType){
			case 1: //AtkSoft
				player.TakeDamage(1+cardLevel);
				break;
			case 2: //AtkHard
				player.TakeDamage(3+2*cardLevel);
				break;
		}
		switch(StatsType){
			case 1: //Heal
				player.AddLife(1+cardLevel); //DA CAMBIARE
				break;
			case 2: //Shield
				player.AddShield(1+cardLevel); //DA CAMBIARE
				break;
			case 3: //HealAndShield
				player.AddLife(1+cardLevel); //DA CAMBIARE
				player.AddShield(1+cardLevel); //DA CAMBIARE
				break;
		}
		switch(ElementType){
			case 1: //Fire
				player.SetOnFire(true, cardLevel, specValue);
				break;
			case 2: //Ice
				player.SetOnIce(true, specValue);
				break;
			case 4: //Poison
				player.SetOnPoison(true, cardLevel, specValue);
				break;
			case 8: //Earth
				//TO-DO
				break;
		}
		switch (Passive){
			case 1: //Rage
				//TO-DO
				break;
		}
	}
	#endregion

	#region SEGNALI ———————————————————————————————————————————————————————————————————————————
	public void _on_area_2d_mouse_entered(){
		is_focused = true;
		Animate("Glow");
	}
	public void _on_area_2d_mouse_exited(){
		is_focused = false;
		Animate("Stand-by");
		//Animate("Draw");
	}
	#endregion

    #region GETTER-SETTER ———————————————————————————————————————————————————————————————————————————
    public string CardName{
		get{return card_name;}
		set{card_name = value;}
	}
	public int ManaValue{
		get{return mana_value;}
		set{mana_value = value;}
	}
	public PackedScene CardAnimation{
		get{return cardAnimation;}
		set{cardAnimation = value;}
	}
	public int CardId{
		get{return cardId;}
		set{cardId = value;}
	}
	public int CardDeckPosition{
		get{return card_deck_position;}
		set{card_deck_position = value;}
	}
	public int CardHandPosition{
		get{return card_hand_position;}
		set{card_hand_position = value;}
	}
	public Boolean IsFocused{
		get{return is_focused;}
		set{is_focused = value;}
	}
	public Boolean Is_input_connected{
		get{return is_input_connected;}
		set{is_input_connected = value;}
	}
	public AnimationPlayer AnimationPlayer_card{
		get{return animationPlayer;}
		set{animationPlayer = value;}
	}
	public PackedScene CardAnimation_card{
		get{return cardAnimation;}
		set{cardAnimation = value;}
	}

	#endregion
}
