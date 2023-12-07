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
		Opponent = 1 << 2, // 2^1=2
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
	[ExportSubgroup("Specs")]
	[Export] public int damageValue;
	[Export] public int specValue;
	[Export] public int probValue;
	public int enemyNumValue;
	[Export] public bool foreachEnemy; //0 se non ha valore, 1 se ha valore
	[Export] public int mana_value;
	[ExportSubgroup("Attack Type")]
	[Export(PropertyHint.Flags, "Self,Opponent")] public int CardTarget { get; set; } = 0;
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

		//HARDCODED
		if(card_name.Equals("LOL_Card")){
			probValue = 100;
		}
		//HARDCODED
	}
	#endregion

	#region FUNZIONI ———————————————————————————————————————————————————————————————————————————
	public void Animate(string animationName){
		if (animationPlayer != null){ //serve per controllare se la carta è una scena (è una carta del player), o è solo un oggetto (carta ell'enemy)
			animationPlayer.Play(animationName); //da errore se la carta che stai cercando di animare non è una scena, ma solo un oggetto
		}
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
				enemy.TakeDamage(damageValue + specValue);
				break;
			case 2: //AtkHard
				enemy.TakeDamage(2 * damageValue + specValue);
				break;
		}
		switch(StatsType){
			case 1: //Heal
				if (foreachEnemy == true) {
					enemy.AddLife(enemyNumValue * (damageValue + specValue)); //DA CAMBIARE
				} else if (foreachEnemy == false) {
					enemy.AddLife(damageValue + specValue); //DA CAMBIARE
				}
				break;
			case 2: //Shield
				if (foreachEnemy == true) {
					enemy.AddShield(enemyNumValue * (damageValue + specValue)); //DA CAMBIARE
				} else if (foreachEnemy == false){
					enemy.AddShield(damageValue + specValue); //DA CAMBIARE
				}
				break;
			case 3: //HealAndShield
				if (foreachEnemy == true) {
					enemy.AddLife(enemyNumValue * (damageValue + specValue)); //DA CAMBIARE
				} else if (foreachEnemy == false) {
					enemy.AddLife(damageValue + specValue); //DA CAMBIARE
				}
				if (foreachEnemy == true) {
					enemy.AddShield(enemyNumValue * (damageValue + specValue)); //DA CAMBIARE
				} else if (foreachEnemy == false){
					enemy.AddShield(damageValue + specValue); //DA CAMBIARE
				}
				break;
		}
		//probabilità di applicare l'effetto elementale
		int random = new Random().Next(0, 100);
		if (random <= probValue){
			switch (ElementType)
			{
				case 1: //Fire
					enemy.SetOnFire(true, damageValue, specValue);
					break;
				case 2: //Ice
					enemy.SetOnIce(true, specValue);
					//DA CAMBIARE
					break;
				case 4: //Poison
					enemy.SetOnPoison(true, damageValue, specValue);
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
	}
	public void ExecuteCard(Player_BattleScene player){
		switch(AttackType){
			case 1: //AtkSoft
				player.TakeDamage(damageValue + specValue);
				break;
			case 2: //AtkHard
				player.TakeDamage(2 * damageValue + specValue);
				break;
		}
		switch(StatsType){
			case 1: //Heal
				if (foreachEnemy == true) {
					player.AddLife(enemyNumValue * (damageValue + specValue)); //DA CAMBIARE
				} else if (foreachEnemy == false) {
					player.AddLife(damageValue + specValue); //DA CAMBIARE
				}
				break;
			case 2: //Shield
				if (foreachEnemy == true) {
					player.AddShield(enemyNumValue * (damageValue + specValue)); //DA CAMBIARE
				} else if (foreachEnemy == false){
					player.AddShield(damageValue + specValue); //DA CAMBIARE
				}
				break;
			case 3: //HealAndShield
				if (foreachEnemy == true) {
					player.AddLife(enemyNumValue * (damageValue + specValue)); //DA CAMBIARE
				} else if (foreachEnemy == false) {
					player.AddLife(1 + damageValue + specValue); //DA CAMBIARE
				}
				if (foreachEnemy == true) {
					player.AddShield(enemyNumValue * (damageValue + specValue)); //DA CAMBIARE
				} else if (foreachEnemy == false){
					player.AddShield(damageValue + specValue); //DA CAMBIARE
				}
				break;
		}
		//probabilità di applicare l'effetto elementale
		int random = new Random().Next(0, 100);
		if (random <= probValue) {
			switch (ElementType) {
				case 1: //Fire
					player.SetOnFire(true, damageValue, specValue);
					break;
				case 2: //Ice
					player.SetOnIce(true, specValue);
					//DA CAMBIARE
					break;
				case 4: //Poison
					player.SetOnPoison(true, damageValue, specValue);
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
	}
	public Card DeepCopy(){ //funzione per creare una copia della risorsa
	  //necessaria nella creazione del TempDeck perchè altrimenti le carte sarebbero le stesse, copiando solo il riferimento alla risorsa e non duplicando la carta
		Card newCard = new Card();
		newCard.cardId = this.cardId;
		newCard.card_name = this.card_name;
		newCard.damageValue = this.damageValue;
		newCard.card_deck_position = this.card_deck_position;
		newCard.specValue = this.specValue;
		newCard.mana_value = this.mana_value;
		newCard.CardTarget = this.CardTarget;
		newCard.StatsType = this.StatsType;
		newCard.AttackType = this.AttackType;
		newCard.ElementType = this.ElementType;
		newCard.Passive = this.Passive;
		newCard.cardAnimation = this.cardAnimation;
		// Copia altri campi e proprietà qui
		return newCard;
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
	public int EnemyNumValue
	{
		get { return enemyNumValue; }
		set { enemyNumValue = value; }
	}
	#endregion
}
