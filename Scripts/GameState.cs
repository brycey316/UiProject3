using Godot;

public partial class GameState : Node
{
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("Pause"))
		{
			GetTree().ChangeSceneToFile("res://pause.tscn");;
		}
		if (@event.IsActionPressed("pause"))
		{
			GetTree().Paused =true;
		}
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
