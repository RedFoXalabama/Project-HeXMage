using Godot;
using System;

public partial class CardAnimation : Sprite2D //Scena da allegare alla Card, è l'animazione che viene poi chiamata dalla Battle scene, castata ed animata
{
	#region SIGNAL ———————————————————————————————————————————————————————————————————————————
	[Signal] public delegate void AbleCardsCollisionEventHandler(Boolean value); //Segnale per disabilitare le collisioni delle carte
    //Collegato Player -> Godot -> HandsCard_Gui
	[Signal] public delegate void EnemyCardAnimationFinishedEventHandler(); //Segnale per dire che l'animazione è finita
	//per ora solo castato, serve in un await in BattleScene
	[Signal] public delegate void PlayerCardAnimationFinishedEventHandler(); //Segnale per dire che l'animazione è finita
	//per ora solo castato, serve in un await in BattleScene
	#endregion

	#region ATTRIBUTI ———————————————————————————————————————————————————————————————————————————
	private string caster; //nome del caster
	#endregion

	#region FUNZIONI ———————————————————————————————————————————————————————————————————————————
	public void PlayAnimation(string caster){ //eseguiamo l'animazione creta
		this.caster = caster; //salviamo il caster, serve a capire chi esegue l'animazione
		AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("BattleCardAnimation");
	}
	#endregion

	#region SIGNAL ———————————————————————————————————————————————————————————————————————————
	public void _on_animation_player_animation_finished(String anim_name){ //chiamata quando l'animazione finisce, collegato tramite Godot
		QueueFree(); //eliminiamo l'animazione e liberiamo memoria
	}

	public void _on_tree_exited(){ //chiamata quando l'animazione finisce, collegato tramite Godot
		switch (caster){
			case "player":
				//prima dichiaro che è finito e sblocco l'await
				EmitSignal("PlayerCardAnimationFinished"); //segnale per dire che l'animazione è finita
				//dopo abilito le collisioni delle carte
				EmitSignal("AbleCardsCollision", true, true); //se il caster è il player, riabilitiamo la collisione delle carte per una nuova selezione
				break;
			case "enemy":
				EmitSignal("EnemyCardAnimationFinished"); //segnale per dire che l'animazione è finita
				break;
		}
	}
	#endregion
}
