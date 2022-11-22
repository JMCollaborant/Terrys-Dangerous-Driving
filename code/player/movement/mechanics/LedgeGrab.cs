
using Sandbox;

namespace Platformer.Movement
{
	class LedgeGrab : BaseMoveMechanic
	{
		public override bool TakesOverControl => true;

		public Vector3 LedgeDestination;
		public Vector3 LedgeGrabLocation;
		public Vector3 GrabNormal;

		public Vector3 TargetLocation;

		public float TimeUntilNextGrab;
		public float TimeToDisengage = -1;

		public float GrabDistance => 16.0f;
		public float PlayerRadius => 32.0f;

		public LedgeGrab( PlatformerController controller ) : base( controller )
		{
		}

		protected override bool TryActivate()
		{
			if ( TimeUntilNextGrab > Time.Now )
				return false;

			if ( TryGrabUpperLedge() )
				return true;

			return false;
		}

		public override void Simulate()
		{
			base.Simulate();

			ctrl.SetTag( "grabbing_wall" );

			ctrl.Velocity = 0;
			ctrl.Rotation = (-GrabNormal).EulerAngles.WithPitch( 0 ).ToRotation();
			ctrl.Position = Vector3.Lerp( ctrl.Position, TargetLocation, Time.Delta * 10.0f );

			if ( TimeToDisengage > 0 )
			{
				if ( Time.Now > TimeToDisengage )
				{
					IsActive = false;
					TimeToDisengage = -1;
				}

				return;
			}

			// Drop down
			if ( InputActions.Duck.Pressed() || InputActions.Back.Pressed() )
			{
				TimeToDisengage = Time.Now;
			}

			// Climb up
			if ( InputActions.Jump.Pressed() ||
				InputActions.Forward.Pressed() ||
				InputActions.Left.Pressed() ||
				InputActions.Right.Pressed() )
			{
				// Effects
				ctrl.Pawn.PlaySound( "player.slam.land" );
				Particles.Create( "particles/gameplay/player/slamland/slamland.vpcf", ctrl.Position );

				TimeToDisengage = Time.Now + 0.5f;
				TargetLocation = LedgeDestination;
			}

		}

		internal bool TryGrabUpperLedge()
		{
			// Need to be on air to check for upper ledge
			if ( ctrl.GroundEntity != null )
				return false;

			var center = ctrl.Position;
			center.z += 48;
			var dest = (center + (ctrl.Pawn.Rotation.Forward.WithZ( 0 ).Normal * 48.0f));
			var destup = (center + (ctrl.Pawn.Rotation.Up * 48.0f));

			// Should be done.
			var trup = Trace.Ray( center, destup )
				.Ignore( ctrl.Pawn )
				.WithoutTags( "trigger" )
				.WithoutTags( "PropCarry" )
				.WithoutTags( "Platplayer" )
				.WithoutTags( "player" )
				//.WorldOnly()
				.Radius( 16 )
				.Run();

			var tr = Trace.Ray( center, dest )
				.Ignore( ctrl.Pawn )
				.WithoutTags( "PropCarry" )
				.WithoutTags( "Platplayer" )
				.WithoutTags( "trigger" )
				.WithoutTags( "player" )
				//.WorldOnly()
				.Radius( 8 )
				.Run();

			if ( trup.Hit ) return false;
			if ( tr.Hit )
			{
				var normal = tr.Normal;
				var destinationTestPos = tr.EndPosition - (normal * PlayerRadius) + (Vector3.Up * 64.0f);
				var originTestPos = tr.EndPosition + (normal * GrabDistance);

				// Trace again to check if we have a valid ground
				tr = Trace.Ray( destinationTestPos, destinationTestPos - (Vector3.Up * 64.0f) )
					.Ignore( ctrl.Pawn )
					.WithoutTags( "player" )
					.Radius( 4 )
					.Run();

				if ( tr.Hit )
				{
					// That's a valid position, set our destination pos
					destinationTestPos = tr.EndPosition;
					// Adjust grab position
					originTestPos = originTestPos.WithZ( destinationTestPos.z - 64.0f );

					// Then check if we have enough room to climb

					tr = Trace.Ray( destinationTestPos + (Vector3.Up * PlayerRadius + 1.0f), destinationTestPos + (Vector3.Up * 64.0f) )
						.Ignore( ctrl.Pawn )
						.WithoutTags( "player" )
						.Radius( PlayerRadius )
						.Run();

					if ( tr.Hit )
					{
						// We can't climb
						return false;
					}
					else
					{
						// Yeah, we can climb
						LedgeDestination = destinationTestPos;
						LedgeGrabLocation = originTestPos;
						GrabNormal = normal;
						TimeUntilNextGrab = Time.Now + 1.5f;

						//Default bottom ledge to grab
						TargetLocation = LedgeGrabLocation;

						//Effects
						ctrl.Pawn.PlaySound( "rail.slide.start" );
						Particles.Create( "particles/gameplay/player/sliding/railsliding.vpcf", LedgeGrabLocation + (Vector3.Up * 64.0f) - (normal * 16.0f) );

						return true;
					}
				}

			}

			return false;
		}

		internal bool TryGrabBottomLedge()
		{
			// Need to be on ground to check for bottom ledge
			if ( ctrl.GroundEntity == null )
				return false;
			return false;
		}


	}
}
