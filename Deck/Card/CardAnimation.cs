using Godot;
using System;

public partial class CardAnimation : Sprite2D
{
	#region SIGNAL ———————————————————————————————————————————————————————————————————————————
	[Signal] public delegate void AbleCardsCollisionEventHandler(Boolean value); //Segnale per disabilitare le collisioni delle carte
    //Collegato Player -> Godot -> HandsCard_Gui
	//Scena da allegare alla Card, è l'animazione che viene poi chiamata dalla Battle scene, castata ed animata
	#endregion

	#region FUNZIONI ———————————————————————————————————————————————————————————————————————————
	public void PlayAnimation(){ //eseguiamo l'animazione creta
		AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("BattleCardAnimation");
	}
	#endregion

	#region SIGNAL ———————————————————————————————————————————————————————————————————————————
	public void _on_animation_player_animation_finished(String anim_name){ //chiamata quando l'animazione finisce, collegato tramite Godot
		EmitSignal("AbleCardsCollision", true); //rabilitiamo la collisione delle carte per una nuova selezione
		QueueFree(); //eliminiamo l'animazione e liberiamo memoria
	}
	#endregion
}
