using Godot;
using System;

public partial class CardAnimation : Sprite2D
{
	//Scena da allegare alla Card, è l'animazione che viene poi chiamata dalla Battle scene, castata ed animata
	public void PlayAnimation(){
		AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("BattleCardAnimation");
	}
}
