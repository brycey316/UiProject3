using Godot;

public partial class GameState : Node
{
	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.IsActionPressed("esc"))
		{
			TogglePause();
		}
		
	}
	public void TogglePause()
	{
		GetTree().Paused = !GetTree().Paused;
	}
	public int Score { get; private set; } = 0;

	public void AddScore(int points)
	{
		Score += points;
		EmitSignal(SignalName.ScoreChanged, Score);
	}

	[Signal]
	public delegate void ScoreChangedEventHandler(int newScore);

	// Classic singleton pattern (optional but very convenient in C#)
	private static GameState _instance;
	public static GameState Instance => _instance;

	public override void _Ready()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else
		{
			QueueFree(); // prevent duplicate instances
		}
	}
}
