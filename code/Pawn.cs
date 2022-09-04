using Sandbox;
using System;
using System.Linq;
using System.Numerics;
using static Sandbox.MiscExtensions;

namespace Sandbox;

partial class Pawn : AnimatedEntity {

	private static Vector3 gravity = new Vector3( 0, 0, -200 );
	
	private static float groundMaxSpeed = 200.0f;
	private static float groundAcceleration = 30f;

	private static float airMaxSpeed = 100.0f;
	private static float airAcceleration = 5f;

	private static float groundRotationSpeed = 10f;
	private static float airRotationSpeed = 3.5f;

	private static float jumpVelocity = 200.0f;

	private static Vector3 hullSize = new Vector3( 16, 16, 34.0f );
	private static Vector3 hullMins = new Vector3( -hullSize.x, -hullSize.y, 0 );
	private static Vector3 hullMaxs = hullSize.WithZ( hullSize.z * 2 );

	private static float groundFriction = 10.0f;

	private CameraController cameraController;

	private Rotation goalRotation;

	private bool isGrounded = false;
	private bool justJumped = false;
	private float lastJumpTime = Time.Now;

	/// <summary>
	/// Minimum time between jumps, in seconds
	/// </summary>
	private static float minJumpDelay = 0.5f;

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
		float remappedForward = MathF.Abs( Input.Forward ).Remap( 0.0f, 1, 0, 1 ) * MathF.Sign( Input.Forward );
		float remappedLeft = MathF.Abs( Input.Left ).Remap( 0.0f, 1, 0, 1 ) * MathF.Sign( Input.Left );
		return new Vector3( remappedForward, remappedLeft, 0 );
	}

	private void UpdateAnimations() {
		Vector3 movement = GetRemappedMovementVector();

		this.SetAnimParameter( "Speed", MathF.Abs( movement.Length ) );
		this.SetAnimParameter( "IsGrounded", isGrounded );
		this.SetAnimParameter( "ButtonPushJump", justJumped );
	}

	private void UpdateIsGrounded() {
		MoveHelper moveHelper = GetMoveHelper();
		TraceResult collisionInfo = moveHelper.TraceFromTo( Position, Position + Vector3.Down * 2f );

		// Don't count as grounded if we're moving upward
		bool isMovingUp = Velocity.z > 0f;

        //DebugOverlay.TraceResult( collisionInfo );

		isGrounded = moveHelper.IsFloor( collisionInfo ) && !isMovingUp;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns>The player's new velocity velocity</returns>
	private Vector3 HandleMovement( Vector3 currentVelocity, float maxSpeed, float maxAcceleration ) {
		Vector3 movement = GetRemappedMovementVector();
		float cameraYaw = Input.Rotation.Yaw();
		Vector3 movementDirectionAdjustedforCamera = movement.Normal * Rotation.FromYaw( cameraYaw );

		float currentSpeed = currentVelocity.Length;
		float speedCap = Math.Max( maxSpeed * movement.Length, currentSpeed );

		Vector3 newDesiredSpeed = currentVelocity + movementDirectionAdjustedforCamera * maxAcceleration * movement.Length;

		if ( newDesiredSpeed.Length > speedCap ) {
			newDesiredSpeed = newDesiredSpeed.Normal * speedCap;
		}

		return newDesiredSpeed;
	}

	private void UpdateMovement() {

		MoveHelper moveHelper = GetMoveHelper();
		moveHelper.Velocity = Velocity;

		Vector3 movement = GetRemappedMovementVector();

		float cameraYaw = Input.Rotation.Yaw();
		Vector3 movementDirectionAdjustedforCamera = movement.Normal * Rotation.FromYaw( cameraYaw );

		// Rotate to face our direction of movement
        if ( movement.Length > 0f ) {
			goalRotation = movementDirectionAdjustedforCamera.EulerAngles.ToRotation();
        }
		
		// Grounded behavior
        if ( isGrounded ) {

			// Debug overlay
			Vector3 debugPos = Position + 3f;
			// Max ground speed
			DebugOverlay.Circle( Position + Vector3.Up * 2f, Rotation.FromAxis( Vector3.Right, 90 ), groundMaxSpeed, Color.Blue.WithAlpha( 0.25f ), 0, true );
			// Velocity
			DebugOverlay.Line( debugPos, debugPos + Velocity, Color.Red, 0, false );

			// Jumping
			bool jumpTimeIsRight = ( Time.Now - lastJumpTime ) > minJumpDelay;
			if ( Input.Pressed( InputButton.Jump ) && jumpTimeIsRight ) {
				moveHelper.Velocity = moveHelper.Velocity.WithZ( jumpVelocity );
				justJumped = true;
				lastJumpTime = Time.Now;
			} else {
				justJumped = false;
			}

			// Friction
			if ( movement.Length <= 0 ) {
				moveHelper.ApplyFriction( groundFriction, Time.Delta );
            
			// Ground movement
			} else {
				moveHelper.Velocity = HandleMovement( moveHelper.Velocity, groundMaxSpeed, groundAcceleration );
			}

		// Air behavior
		} else {

			// Air movement
			moveHelper.Velocity = HandleMovement( moveHelper.Velocity, airMaxSpeed, airAcceleration );

			// Gravity
			moveHelper.Velocity += gravity * Time.Delta;
		}

		DebugOverlay.Box( Position + hullMins, Position + hullMaxs );


        // Player rotation
        if ( isGrounded ) {
			Rotation = Rotation.Slerp( Rotation, goalRotation, groundRotationSpeed * Time.Delta );
        } else {
			Rotation = Rotation.Slerp( Rotation, goalRotation, airRotationSpeed * Time.Delta );
		}
		

		// Move the player towards their velocity
		moveHelper.TryMoveWithStep( Time.Delta, 4f );

		Velocity = moveHelper.Velocity;
		Position = moveHelper.Position;
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
