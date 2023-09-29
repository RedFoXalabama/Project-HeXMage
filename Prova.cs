using Godot;
using System;

public class Prova : Control{
    public override void _Ready(){
        GetNode<TextEdit>("TextEdit").Hide();
        GetNode<PopupMenu>("PopupMenu").Popup_();
    }
    
    public void _on_PopupMenu_id_pressed(int id){
        switch(id){
            case 0:
                GetNode<TextEdit>("TextEdit").Show();
                break;
        }
    }
}
