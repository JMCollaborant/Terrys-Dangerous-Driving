using Sandbox;
using System;
using System.Linq;

namespace Sandbox;

partial class Pawn : AnimatedEntity {

	private Vector3 gravity = new Vector3( 0, 0, -10 );

	/// <summary>
	/// Called when the entity is first created 
	/// </summary>
	public override void Spawn() {
		base.Spawn();

		//
		// Use a watermelon model
		//
		SetModel( "models/sbox_props/watermelon/watermelon.vmdl" );

		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	private void UpdateCamera() {
		float yaw = Input.Rotation.Yaw() / 10;

		float cameraDistance = 75f;

		Vector3 cameraPos = new Vector3( MathF.Cos( yaw ), MathF.Sin( yaw ), 0 ) * cameraDistance;

		EyePosition = Position + cameraPos;
		EyeRotation = ( Position - EyePosition ).EulerAngles.ToRotation();
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

		if ( Input.Down( InputButton.Forward ) ) {
			Velocity += EyeRotation.Forward * 10f;
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


		UpdateCamera();
	}

	public override void FrameSimulate( Client cl ) {
		base.FrameSimulate( cl );

		UpdateCamera();
	}
}
