using Godot;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public Node3D Pivot { get; set; }

	public Camera3D Camera { get; set; }

	public override void _Ready()
	{
		Pivot = GetNode<Node3D>("Pivot");
		Camera = GetNode<Camera3D>("Pivot/Camera3D");
		base._Ready();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else if (Input.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		if (Input.MouseMode == Input.MouseModeEnum.Captured
			&& @event is InputEventMouseMotion)
		{
			var mouseEvent = @event as InputEventMouseMotion;
			Pivot.RotateY((float)(-mouseEvent.Relative.X * 0.01));
			Camera.RotateX((float)(-mouseEvent.Relative.Y * 0.01));
			Camera.Rotation = new Vector3(Mathf.Clamp(Camera.Rotation.X, Mathf.DegToRad(-30), Mathf.DegToRad(60)),
				Camera.Rotation.Y, Camera.Rotation.Z);
		}
		base._UnhandledInput(@event);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("Left", "Right", "Up", "Down");
		Vector3 direction = (Pivot.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
