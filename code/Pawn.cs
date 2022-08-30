using Sandbox;
using System;
using System.Linq;
using System.Numerics;
using static Sandbox.MiscExtensions;

namespace Sandbox;

partial class Pawn : AnimatedEntity {

	private static Vector3 gravity = new Vector3( 0, 0, -200 );
	private static float groundSpeed = 1300.0f;
	private static float jumpVelocity = 200.0f;

	private static Vector3 hullSize = new Vector3( 16, 16, 34.0f );
	private static Vector3 hullMins = new Vector3( -hullSize.x, -hullSize.y, 0 );
	private static Vector3 hullMaxs = hullSize.WithZ( hullSize.z * 2 );

	private static float groundFriction = 4.0f;
	private static float airFriction = 1.0f;

	private CameraController cameraController;

	private Rotation goalRotation;

	private bool isGrounded = false;
	private bool ButtonPushJump = false;

	private MoveHelper GetMoveHelper() {
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

	private Vector3 GetRemappedMovementVector() {
		float remappedForward = MathF.Abs( Input.Forward ).Remap( 0.4f, 1, 0, 1 ) * MathF.Sign( Input.Forward );
		float remappedLeft = MathF.Abs( Input.Left ).Remap( 0.4f, 1, 0, 1 ) * MathF.Sign( Input.Left );
		return new Vector3( remappedForward, remappedLeft, 0 );
	}

	private void UpdateAnimations() {
		Vector3 movement = GetRemappedMovementVector();

		this.SetAnimParameter( "Speed", MathF.Abs( movement.Length ) );
		this.SetAnimParameter( "isGrounded", isGrounded );
		this.SetAnimParameter( "ButtonPushJump", ButtonPushJump );
	}

	private void UpdateIsGrounded() {
		MoveHelper moveHelper = GetMoveHelper();
		TraceResult collisionInfo = moveHelper.TraceFromTo( Position, Position + Vector3.Down * 2f );

		// Don't count as grounded if we're moving upward
		bool isMovingUp = Velocity.z > 1f;

        DebugOverlay.TraceResult( collisionInfo );

		isGrounded = moveHelper.IsFloor( collisionInfo ) && !isMovingUp;
	}

	private void UpdateMovement() {
		MoveHelper moveHelper = GetMoveHelper();

		Vector3 movement = GetRemappedMovementVector();

		float cameraYaw = Input.Rotation.Yaw();
		Vector3 movementDirectionAdjustedforCamera = movement.Normal * Rotation.FromYaw( cameraYaw );

        if ( movementDirectionAdjustedforCamera.Length > 0f ) {
			goalRotation = movementDirectionAdjustedforCamera.EulerAngles.ToRotation();
        }

		groundSpeed = 900f;

		float movementSpeed = movement.Length * groundSpeed;

        // Landing
        if ( isGrounded ) {
			ButtonPushJump = false;
		}

		// Ground movement
        if ( isGrounded ) {
			Velocity += movementDirectionAdjustedforCamera * movementSpeed * Time.Delta;
        }

		// Gravity
        if ( !isGrounded ) {
			Velocity += gravity * Time.Delta;
        }

        // Jumping
        if ( isGrounded && Input.Pressed( InputButton.Jump ) ) {
			Velocity += Vector3.Up * jumpVelocity;
			ButtonPushJump = true;
		}

		DebugOverlay.Box( Position + hullMins, Position + hullMaxs );

		// Apply movement velocity to move helper
		moveHelper.Velocity = Velocity;

		// Ground friction
		if ( isGrounded ) {
			moveHelper.ApplyFriction( groundFriction, Time.Delta );

		// Air friction
        } else {
			// We want to ignore Z friction to avoid screwing with gravity or anything
			float zVelocity = moveHelper.Velocity.z;
			moveHelper.ApplyFriction( airFriction, Time.Delta );
			moveHelper.Velocity.z = zVelocity;
		}

		float movePercent = moveHelper.TryMoveWithStep( Time.Delta, 4f );

		Velocity = moveHelper.Velocity;
		Position = moveHelper.Position;
		Rotation = Rotation.Slerp( Rotation, goalRotation, 0.25f );
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
		UpdateAnimations();
	}
}
