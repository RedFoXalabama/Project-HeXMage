using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
	#region VARIABILI --------------------------------------------------

	private const float Speed = 5.0f;
	private const float gravity = 10.0f;

	#endregion
	#region FUNZIONI --------------------------------------------------

	public override void _PhysicsProcess(double delta)
	{
		Vector3 inputDirection = Velocity;
		inputDirection.Y -= gravity * (float)delta; //Per portare il personaggio al livello del suolo

		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			inputDirection.X = direction.X * Speed;
			inputDirection.Z = direction.Z * Speed;
		}

		else {inputDirection.X = 0; inputDirection.Z = 0;}
		


        Velocity = inputDirection;
        MoveAndSlide();
	}

	#endregion
}
