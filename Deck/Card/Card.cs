using Godot;
using System;

public partial class Card : Node2D
{
	//Export
	[Export] PackedScene cardAnimation; //animazione da eseguire, nodo esterno da allegare
	[Export] int cardId; //id della carta
	[Export] int danno;
	//ATTRIBUTI
	private string card_name;
	private int mana_value;
	//NODI
	//private Sprite2D sfondo;
	//private Sprite2D immagine;
	private Label nome_label;
	//private Label descrizione;
	private Label mana_label;

	public override void _Ready(){
		//inizializzo i nodi
		//sfondo = GetNode<Sprite2D>("Sfondo");
		//immagine = GetNode<Sprite2D>("Immagine");
		nome_label = GetNode<Label>("Nome");
		//descrizione = GetNode<Label>("Descrizione");
		mana_label = GetNode<Label>("Mana");
		//inizializzo gli attributi
		card_name = nome_label.Text;
		mana_value = Convert.ToInt32(mana_label.Text);
	}
	
	//GETTER-SETTER
	public string CardName{
		get{return card_name;}
		set{card_name = value;}
	}
	public int Danno{
		get{return danno;}
		set{danno = value;}
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
}
