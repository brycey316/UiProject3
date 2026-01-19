extends Control

func resume():
	get_tree().paused = false
	$AnimationPlayer.play_backwards("blah")

func pause():
	get_tree().paused = true
	$AnimationPlayer.play("blah")

func testEsc():
	if Input.is_action_just_pressed("esc") and get_tree().paused == false:
		pause()
	elif Input.is_action_just_pressed("esc") and get_tree().paused == true:
		resume()


func _on_resume_pressed() -> void:
	resume()


func _on_quit_pressed() -> void:
	get_tree().quit()

func _ready() -> void:
	testEsc()
