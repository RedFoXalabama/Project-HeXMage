using Godot;
using System;

public partial class Player : CharacterBody3D
{

	private const float Speed = 5.0f;
	Vector3 velocity = new Vector3();
	private AnimationTree animationTree;

	private Vector2 move;


	public override void _Ready()
	{
		animationTree = GetNode<AnimationTree>("AnimationTree");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 inputDirection = Velocity;
		inputDirection.Y -= 10 * (float)delta; //Per portare il personaggio al livello del suolo

		//inputDirection = inputDirection.Normalized();

		GD.Print(animationTree.Get("parameters/Walk/blend_position"));

		animationTree.Set("parameters/conditions/isWalk", false);
		animationTree.Set("parameters/conditions/isIdle", true);
		move.X = 0;
		move.Y = 0;
		inputDirection.X = 0;
		inputDirection.Z = 0;


		if (Input.IsActionPressed("PlayerMoveLEFT")){
			inputDirection.X = -Speed;
			animationTree.Set("parameters/conditions/isIdle", false);
			animationTree.Set("parameters/conditions/isWalk", true);
			move.X = -1;
			animationTree.Set("parameters/Idle/blend_position", move);
			animationTree.Set("parameters/Walk/blend_position", move);
		}

		if (Input.IsActionPressed("PlayerMoveRIGHT")){
			inputDirection.X = Speed;
			animationTree.Set("parameters/conditions/isIdle", false);
			animationTree.Set("parameters/conditions/isWalk", true);
			move.X = 1;
			animationTree.Set("parameters/Idle/blend_position", move);
			animationTree.Set("parameters/Walk/blend_position", move);
		}

		if (Input.IsActionPressed("PlayerMoveUP")){
			inputDirection.Z = -Speed;
			animationTree.Set("parameters/conditions/isIdle", false);
			animationTree.Set("parameters/conditions/isWalk", true);
			move.Y = 1;
			animationTree.Set("parameters/Idle/blend_position", move);
			animationTree.Set("parameters/Walk/blend_position", move);
		}

		if (Input.IsActionPressed("PlayerMoveDOWN")){
			inputDirection.Z = Speed;
			move.Y = -1;
			animationTree.Set("parameters/conditions/isIdle", false);
			animationTree.Set("parameters/conditions/isWalk", true);
			animationTree.Set("parameters/Idle/blend_position", move);
			animationTree.Set("parameters/Walk/blend_position", move);
		}
		
        Velocity = inputDirection;
        MoveAndSlide();
	}
}
