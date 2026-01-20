using Godot;
using System;

public partial class GameManager : Node
{
	private int _score = 0;
	// Use the [Export] attribute to assign the Label node from the editor
	[Export]
	public Label ScoreLabel { get; set; }

	public override void _Ready()
	{
		UpdateScoreText();
		ScoreLabel = GetNode<Label>("ScoreLabel");
	}

	public void AddScore(int points)
	{
		_score += points; // Increment the score by the given points
		UpdateScoreText();
	}

	private void UpdateScoreText()
	{
		if (ScoreLabel != null)
		{
			// Convert the score integer to a string and update the Label's Text property
			ScoreLabel.Text = $"Score: {_score}";
		}
	}
}
