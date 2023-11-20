using Godot;
using System;

public partial class Stats_GUI : Control{
	#region NODI ———————————————————————————————————————————————————————————————————————————
	private TextureRect charIcon;
	private Label charName;
	private TextureProgressBar ShieldBar;
	private TextureProgressBar LifeBar;
	private TextureProgressBar ManaBar;
	private TextureRect fireIcon;
	private TextureRect iceIcon;
	private TextureRect poisonIcon;
	private TextureRect earthIcon;
	#endregion

	#region READY - PROCESS ———————————————————————————————————————————————————————————————————————————
	public override void _Ready(){
		charIcon = GetNode<TextureRect>("HBoxContainer/CharIcon");
		charName = GetNode<Label>("HBoxContainer/BarContainer/CharName");
		ShieldBar = GetNode<TextureProgressBar>("HBoxContainer/BarContainer/ShieldBar");
		LifeBar = GetNode<TextureProgressBar>("HBoxContainer/BarContainer/LifeBar");
		ManaBar = GetNode<TextureProgressBar>("HBoxContainer/BarContainer/ManaBar");
		fireIcon = GetNode<TextureRect>("HBoxContainer/BarContainer/StatusContainer/Fire");
		iceIcon = GetNode<TextureRect>("HBoxContainer/BarContainer/StatusContainer/Ice");
		poisonIcon = GetNode<TextureRect>("HBoxContainer/BarContainer/StatusContainer/Poison");
		earthIcon = GetNode<TextureRect>("HBoxContainer/BarContainer/StatusContainer/Earth");
	}
	#endregion

	#region SEGNALI ———————————————————————————————————————————————————————————————————————————
	public void _on_character_set_stats(Texture2D icon, string name, int maxShield, int maxLife, int maxMana){ //segnale per settare la prima volta le Stats
		charIcon.Texture = icon;
		charName.Text = name;
		ShieldBar.MaxValue = maxShield;
		LifeBar.MaxValue = maxLife;
		ManaBar.MaxValue = maxMana;
		SetDeferred("custom_minimum_size", GetNode<HBoxContainer>("HBoxContainer").Size + icon.GetSize());
	}

	public void _on_character_update_stats(int shield, int life, int mana, Boolean isOnFire, Boolean isOnIce, Boolean isOnPoison, Boolean isOnEarth){ //segnale per aggiornare le stats
		ShieldBar.Value = shield;
		LifeBar.Value = life;
		ManaBar.Value = mana;
		fireIcon.Visible = isOnFire;
		iceIcon.Visible = isOnIce;
		poisonIcon.Visible = isOnPoison;
		earthIcon.Visible = isOnEarth;
	}
	#endregion
}
