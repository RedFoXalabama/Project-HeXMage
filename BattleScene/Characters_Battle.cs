using Godot;
using System;

[GlobalClass] public partial class Characters_Battle : Sprite2D
{
	//Export
	[Export] private int life;
	[Export] private int mana;
	[Export] private int shield;
	[Export] BattleDeck battleDeck;


	//FUNZIONI
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
	}
	public void AddLife(int value){
		life += value;
	}
	public void AddMana(int value){
		mana += value;
	}
	public void UseMana(int value){
		mana -= value;
	}
	public void AddShield(int value){
		shield += value;
	}
	public void DrawCard(){
		battleDeck.Draw();
	}

	//GETTER-SETTER
	public BattleDeck BattleDeck{
		get{return battleDeck;}
		set{battleDeck = value;}
	}
}
interface DeckUse {
	void UseCard(Card card);
	void SelectCard();
	void SelectTarget();
}