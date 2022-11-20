using Sandbox;
using System;
using System.Linq;
using System.Numerics;
using Utilities;
using static Sandbox.MiscExtensions;

namespace Sandbox.PlayerController;

partial class Player : AnimatedEntity {

	static float jumpVelocity = 400.0f;
	static float doubleJumpVelocity = 250.0f;
	static Vector3 jumpUpGravity = new Vector3( 0, 0, -15f );
	static Vector3 fallDownGravity = new Vector3( 0, 0, -16f );

	static float groundMaxSpeed = 200.0f;
	static float groundAcceleration = 30f;

	static float airMaxSpeed = 150.0f;
	static float airAcceleration = 20f;

	static float groundRotationSpeed = 10f;
	static float airRotationSpeed = 3.5f;

	static Vector3 hullSize = new Vector3( 16, 16, 34.0f );
	static Vector3 hullMins = new Vector3( -hullSize.x, -hullSize.y, 0 );
	static Vector3 hullMaxs = hullSize.WithZ( hullSize.z * 2 );

	static float groundFriction = 10.0f;

	static float stepDownHeight = 10.0f;
	static float stepUpHeight = 4.0f;

	CameraController cameraController;

	Rotation goalRotation;

	MoveHelper GetMoveHelper() {
		MoveHelper moveHelper = new MoveHelper();
		moveHelper.Trace = moveHelper.Trace.Size( hullMins, hullMaxs );
		moveHelper.Trace.Ignore( this );
		moveHelper.Velocity = Velocity;
		moveHelper.Position = Position;
		moveHelper.MaxStandableAngle = 46f;

		return moveHelper;
	}

	/// <summary>
	/// Called when the entity is first created 
	/// </summary>
	public override void Spawn() {
		base.Spawn();

		SetModel( "models/characters/homer/homer.vmdl" );
		SetAnimGraph( "animgraphs/homer2.vanmgrph" );

		EnableDrawing = true;

		cameraController = Components.Create<CameraController>();
		Components.Add( cameraController );

		goalRotation = Rotation;
	}

	/// <summary>
	/// Called every tick, clientside and serverside.
	/// </summary>
	public override void Simulate( Client cl ) {
		base.Simulate( cl );

		this.Scale = 0.4f;

		UpdateMovement();
		UpdateIsGrounded();
		UpdateAnimations();
	}

	public override void FrameSimulate( Client cl ) {
		base.FrameSimulate( cl );

		UpdateMovement();
		UpdateIsGrounded();
		UpdateJumping();
		UpdateAnimations();
	}

	Vector3 GetRemappedInputMovement() {
		float remappedForward = MathF.Abs( Input.Forward ).Remap( 0.0f, 1, 0, 1 ) * MathF.Sign( Input.Forward );
		float remappedLeft = MathF.Abs( Input.Left ).Remap( 0.0f, 1, 0, 1 ) * MathF.Sign( Input.Left );
		return new Vector3( remappedForward, remappedLeft, 0 ).ClampLength( 0, 1 );
	}

	void UpdateAnimations() {
		Vector3 movementDirection = GetRemappedInputMovement();

		this.SetAnimParameter( "IsGrounded", isGrounded );
		this.SetAnimParameter( "Speed", MathF.Abs( movementDirection.Length ) );
		this.SetAnimParameter( "ButtonPushJump", justJumped );
		this.SetAnimParameter( "ButtonPushDoubleJump", hasDoubleJumped );
	}
}
