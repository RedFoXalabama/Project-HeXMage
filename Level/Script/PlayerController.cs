using Godot;
using System;
using System.Security;

public partial class PlayerController : CharacterBody3D
{
	#region VARIABILI --------------------------------------------------

	[Export] private const float Speed = 5.0f;

	private MeshInstance3D mesh;
	private Texture2D up;
	private Texture2D down;
	private Texture2D left;
	private Texture2D right;

	private QuadMesh quadMesh;
	private StandardMaterial3D sm3d;

	[Export] private int frame;
	Vector3 velocity = new Vector3();
	private AnimationTree animationTree;

	private AnimationNodeStateMachinePlayback animationState;

    #endregion
    #region FUNZIONI --------------------------------------------------

    public override void _Ready()
    {

		animationTree = GetNode<AnimationTree>("AnimationTree");
		//DICHIARAZIONI VARIABILI
        /*mesh = GetNode<MeshInstance3D>("Mesh");
		up = ResourceLoader.Load("res://Assets/3D Model/Texture/segnaposto.png") as Texture2D;
		down = ResourceLoader.Load("res://Assets/3D Model/Texture/segnaposto.png") as Texture2D;
		left = ResourceLoader.Load("res://Assets/3D Model/Texture/segnaposto.png") as Texture2D;
		right = ResourceLoader.Load("res://Assets/3D Model/Texture/segnaposto.png") as Texture2D;

		quadMesh = (QuadMesh)mesh.Mesh;
		sm3d = (StandardMaterial3D)quadMesh.Material;

		sm3d.AlbedoTexture = up;*/

        animationState = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");



    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 inputDirection = Velocity;
		inputDirection.Y -= 10 * (float)delta; //Per portare il personaggio al livello del suolo

		
		/*Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			inputDirection.X = direction.X * Speed;
			inputDirection.Z = direction.Z * Speed;

		}

		else {inputDirection.X = 0; inputDirection.Z = 0;}*/


    	inputDirection.X = Input.GetActionStrength("PlayerMoveRIGHT") - Input.GetActionStrength("PlayerMoveLEFT");    
        inputDirection.Z = Input.GetActionStrength("PlayerMoveDOWN") - Input.GetActionStrength("PlayerMoveUP");

		//animationTree.Set("parameters/IDLE/blend_position", vettoreZero);
		//animationTree.Set("parameters/Walk/blend_position", vettoreZero);

		inputDirection = inputDirection.Normalized();

		animationTree.Set("parameters/Walk/blend_position", new Vector2(0, 0));


		GD.Print(animationTree.Get("parameters/Walk/blend_position"));
		animationTree.Set("parameters/Walk/blend_position", new Vector2(inputDirection.X, inputDirection.Z));

		/*if (inputDirection.X != 0 && inputDirection.Z != 0){
			animationTree.Set("parameters/IDLE/blend_position", inputDirection);
			animationTree.Set("parameters/Walk/blend_position", inputDirection);
			velocity = velocity.MoveToward(inputDirection * 500, (float)(700 * delta));
			
		}else {
			velocity = velocity.MoveToward(Vector3.Zero, (float)(1000 * delta));
		}

		if (velocity.X != 0 || velocity.Z != 0){
			animationState.Travel("Walk");
		}else if (velocity.X == 0 && velocity.Z == 0){
			animationState.Travel("IDLE");
		}

		Velocity = velocity;
		MoveAndSlide();*/

		if (Input.IsActionPressed("PlayerMoveLEFT")){
			inputDirection.X = -Speed;
			animationTree.Set("parameters/Walk/blend_position", new Vector2(1, 0));
			//animationState.Travel("Walk"); 
			//GetNode<AnimationPlayer>("AnimationPlayer").Play("Left");
			}

		if (Input.IsActionPressed("PlayerMoveRIGHT")){
			inputDirection.X = Speed;
			
			animationTree.Set("parameters/Walk/blend_position", new Vector2(-1, 0));
			//animationState.Travel("Walk"); 
			//GetNode<AnimationPlayer>("AnimationPlayer").Play("Left");
			}

		if (Input.IsActionPressed("PlayerMoveUP")){
			inputDirection.Z = -Speed;
			animationTree.Set("parameters/Walk/blend_position", new Vector2(0, 1));
			//animationState.Travel("Walk"); 
			//GetNode<AnimationPlayer>("AnimationPlayer").Play("Up");
			}

		if (Input.IsActionPressed("PlayerMoveDOWN")){
			inputDirection.Z = Speed;
			animationTree.Set("parameters/Walk/blend_position", new Vector2(0, -1));
			//animationState.Travel("Walk"); 
			//GetNode<AnimationPlayer>("AnimationPlayer").Play("Down");
			}
		


        Velocity = inputDirection;
        MoveAndSlide();
	}

	#endregion
}