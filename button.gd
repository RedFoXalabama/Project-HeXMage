extends Button

signal spegni

func _on_button_down():
	#get_parent().get_node("Sprite2D").hide()
	emit_signal("spegni")
