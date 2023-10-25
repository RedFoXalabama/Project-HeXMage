using Godot;
using System;

public partial class Card : TextureRect
{
	#region ATTRIBUTI ———————————————————————————————————————————————————————————————————————————
	[Export] int cardId; //id della carta
	[Export] int mana_value;
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

	#region READY ———————————————————————————————————————————————————————————————————————————
	public override void _Ready(){
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		collisionShape2D = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
	}
	#endregion

	#region FUNZIONI ———————————————————————————————————————————————————————————————————————————
	public void Animate(string animationName){
		animationPlayer.Play(animationName);
	}
	public void Expire(){ //da chiamare quando la carta viene usata
		QueueFree();
	}

	public void ReSizeCollsion(Vector2 size, Vector2 pos){
		collisionShape2D.Shape.SetDeferred("size", size);
		collisionShape2D.SetDeferred("position", pos);
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
	#endregion
}
