using Godot;
using System;

public partial class Dungeon : Node
{
	#region SIGNALS ———————————————————————————————————————————————————————————————————————————
	#endregion

	#region ATTRIBUTI ———————————————————————————————————————————————————————————————————————————
	[Export] PackedScene[] rooms = new PackedScene[10]; //valore settato a 10, ma non si è obbligati a riempirlo tutto
	private int room_number = 0;
	#endregion

	#region NODI —————————————————————————————————————————————————————————————————————————————
	private Node roomContainer;
	private Control gameover_Screen;
	private Control battleWin_Screen;
	#endregion

	#region READY ————————————————————————————————————————————————————————————————————————————
	public override void _Ready(){
		roomContainer = GetNode("RoomContainer");
		gameover_Screen = GetNode<Control>("Win_OverContainer/GameOver_Screen");
		battleWin_Screen = GetNode<Control>("Win_OverContainer/BattleWin_Screen");

		gameover_Screen.Hide();
		battleWin_Screen.Hide();
	}
	#endregion

	#region FUNCTION ———————————————————————————————————————————————————————————————————————————
	public void ChangeRoomTo(int room_number){
		if (roomContainer.GetChildCount() > 0){
			roomContainer.GetChild<BattleScene>(0).QueueFree(); //elimino la stanza precedente
		}
		//carico la scena della stanza successiva
		roomContainer.AddChild(rooms[room_number].Instantiate());
		roomContainer.GetChild<BattleScene>(0).Connect("GameOverSignal", new Callable(this, "GameOver"));
		roomContainer.GetChild<BattleScene>(0).Connect("WinSignal", new Callable(this, "Win"));
	}
	public void GameOver(){
		//blocco il processamento della stanza, senza eliminarla
		roomContainer.GetChild<BattleScene>(0).ProcessMode = Godot.Node.ProcessModeEnum.Disabled;
		//carico la scena del gameover
		gameover_Screen.Show();
	}
	public void Win(){
		//blocco il processamento della stanza, senza eliminarla
		roomContainer.GetChild<BattleScene>(0).ProcessMode = Godot.Node.ProcessModeEnum.Disabled;
		//incremento il numero della stanza di 1
		room_number++;
		if (room_number == CountRooms()){ //nel caso il numero della stanza sia uguale alla lunghezza dell'array, significa che ho finito le stanze
			//carico la scena della vittoria
			battleWin_Screen.Show();
		} else {
			//carico la scena della stanza successiva
			ChangeRoomTo(room_number);
		}
		
	}
	#endregion

	#region SIGNAL CALLBACKS ———————————————————————————————————————————————————————————————————
	public void _on_start_button_pressed(){
		ChangeRoomTo(0);
		GetNode<Control>("StartView").Hide();
	}
	#endregion

	#region FUNCTIONALITY ————————————————————————————————————————————————————————————————————
	public int CountRooms(){
		int count = 0;
        for(int i = 0; i < rooms.Length; i++){
            if(rooms[i] != null){
                count++;
            }
        }
        return count;
	}
	#endregion
}
