using Sandbox;
using System;
using System.Linq;
using static Sandbox.MiscExtensions;

namespace Sandbox;

partial class Pawn : AnimatedEntity {

	private Vector3 gravity = new Vector3( 0, 0, -10 );

	private CameraController cameraController;

	/// <summary>
	/// Called when the entity is first created 
	/// </summary>
	public override void Spawn() {
		base.Spawn();

		SetModel( "models/citizen_props/oldoven.vmdl" );

		EnableDrawing = true;

		cameraController = Components.Create<CameraController>();
		Components.Add( cameraController );
    }

	/// <summary>
	/// Called every tick, clientside and serverside.
	/// </summary>
	public override void Simulate( Client cl ) {
		base.Simulate( cl );

		Velocity += gravity;

		if ( Input.Pressed( InputButton.Jump ) ) {
			Velocity = Velocity.WithZ( 100f );
		}


		float moveSpeed = 10f;

		Vector3 movementVector = new Vector3( Input.Forward, Input.Left, 0 ).Normal;
		Vector3 movementAdjustedforCamera = movementVector * Input.Rotation.Normal;
		Velocity += movementAdjustedforCamera * moveSpeed;

        if ( movementVector.Length != 0 ) {
			Rotation = Rotation.FromAxis( Vector3.Up, movementAdjustedforCamera.EulerAngles.yaw );
		}

		MoveHelper helper = new MoveHelper( Position, Velocity );
		Vector3 hullSize = new Vector3( 16, 16, 32 );
		helper.Trace = helper.Trace.Size( -hullSize, hullSize );

		DebugOverlay.Box( Position + -hullSize, Position + hullSize );

		float moveDistance = helper.TryMoveWithStep( Time.Delta, 10f );

		if ( moveDistance > 0 ) {
			if ( moveDistance > 1 ) {
				Velocity -= gravity;
			}

			Position = helper.Position;
		}

		Velocity *= 0.955f;
	}

	public override void FrameSimulate( Client cl ) {
		base.FrameSimulate( cl );
	}
}
