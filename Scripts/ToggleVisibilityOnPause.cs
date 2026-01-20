using Godot;
using System;

public partial class ToggleVisibilityOnPause : CanvasLayer
{
	[Export] bool visibleOnPause = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PauseManager.Instance.GamePauseToggle += ToggleVisibility;
		
		if(!visibleOnPause) return;
		Hide();
	}
	
	private void ToggleVisibility(bool isPaused)
	{
		if(visibleOnPause == isPaused)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}
}
