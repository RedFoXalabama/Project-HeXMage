using Godot;
using System;

public partial class IslandButton : TextureButton
{
	#region SIGNALS ———————————————————————————————————————————————————————————————————————————
	public void _on_pressed(){
		GetTree().ChangeSceneToFile("res://BattleScene/Dungeon.tscn");
	}
	#endregion
}
