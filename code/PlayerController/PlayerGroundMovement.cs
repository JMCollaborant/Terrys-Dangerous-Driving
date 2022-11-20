using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Sandbox.PlayerController {
    internal partial class Player {

        // Tracks several recent readings of ground detection.
		// These readings are aggregated to produce the final isGrounded boolean
		private CircularList<bool> groundReadingsList = new Utilities.CircularList<bool>( 3 );

		// Start not grounded so we aren't standing on air when we spawn
		public bool isGrounded = false;

		private void UpdateIsGrounded() {
			MoveHelper moveHelper = GetMoveHelper();
			TraceResult collisionInfo = moveHelper.TraceFromTo( Position, Position + Vector3.Down * stepDownHeight );

			// Add our estimation of whether or not we're grounded to the list of previous readings
			if ( collisionInfo.Hit ) {
				//Position = collisionInfo.HitPosition;
				groundReadingsList.Insert( true );
			} else {
				groundReadingsList.Insert( false );
			}

			// Used to determine if we have transitioned from midair to ground
			bool previousIsGrounded = isGrounded;

			// To change grounded states, all elements in the grounded list MUST concurr against the current state
			for ( int i = 0; i < groundReadingsList.length; i++ ) {

				// If we agree with the current state, we cannot be changing state
				if ( groundReadingsList[ i ] == isGrounded ) {
					break;
				}

				// If this is the last element and none of them have agreed with the current state, change state
				if ( i == groundReadingsList.length - 1 ) {
					isGrounded = !isGrounded;
				}

			}

			// If we just landed
			justLanded = !previousIsGrounded && isGrounded;
		}


		Vector3 GetDesiredMoveSpeed( Vector3 currentVelocity, float maxSpeed, float maxAcceleration ) {

			Vector3 movementDirection = GetRemappedInputMovement();
			float cameraYaw = Input.Rotation.Yaw();
			Vector3 movementDirectionAdjustedforCamera = movementDirection.Normal * Rotation.FromYaw( cameraYaw );

			float speedCap = Math.Max( maxSpeed * movementDirection.Length, currentVelocity.Length );
			float speedCapNoZ = Math.Max( maxSpeed * movementDirection.Length, currentVelocity.WithZ( 0 ).Length );

			Vector3 newDesiredSpeed = currentVelocity + movementDirectionAdjustedforCamera * maxAcceleration * movementDirection.Length;

			if ( newDesiredSpeed.Length > speedCapNoZ ) {
				newDesiredSpeed = newDesiredSpeed.Normal * speedCap;
			}

			return newDesiredSpeed;
		}

		void UpdateMovement() {

			MoveHelper moveHelper = GetMoveHelper();
			moveHelper.Velocity = Velocity;

			Vector3 movement = GetRemappedInputMovement();

			float cameraYaw = Input.Rotation.Yaw();
			Vector3 movementDirectionAdjustedforCamera = movement.Normal * Rotation.FromYaw( cameraYaw );

			// Rotate to face our direction of movement
			if ( movement.Length > 0f ) {
				goalRotation = movementDirectionAdjustedforCamera.EulerAngles.ToRotation();
			}

			// Grounded behavior
			if ( isGrounded ) {

				// Friction
				// Only apply friction if we didn't just jump to avoid undesirable jumping behavior
				if ( movement.Length <= 0 ) {
					moveHelper.ApplyFriction( groundFriction, Time.Delta );

					// Ground movement
				} else {
					moveHelper.Velocity = GetDesiredMoveSpeed( moveHelper.Velocity, groundMaxSpeed, groundAcceleration );
					if ( justLanded ) {
						moveHelper.Velocity = moveHelper.Velocity.ClampLength( groundMaxSpeed );
					}
				}

				// Air behavior
			} else {

				// Max air speed
				//DebugOverlay.Circle( Position + Vector3.Up * 2f, Rotation.FromAxis( Vector3.Right, 90 ), airMaxSpeed, Color.Blue.WithAlpha( 0.25f ), 0, true );

				// Air movement
				moveHelper.Velocity = GetDesiredMoveSpeed( moveHelper.Velocity, airMaxSpeed, airAcceleration );

				// Gravity
				if ( moveHelper.Velocity.z >= 0 && isJumping ) {
					moveHelper.Velocity += jumpUpGravity;
				} else {
					moveHelper.Velocity += fallDownGravity;
				}

				// Double jumping
				bool doubleJumpDelayTimeHasElapsed = ( Time.Now - minDoubleJumpTime ) > minDoubleJumpTime;
				if ( Input.Pressed( InputButton.Jump ) && !hasDoubleJumped && doubleJumpDelayTimeHasElapsed && isJumping ) {
					hasDoubleJumped = true;
					moveHelper.Velocity = moveHelper.Velocity.WithZ( doubleJumpVelocity );
				}
			}

			//DebugOverlay.Box( Position + hullMins, Position + hullMaxs );


			// Player rotation
			if ( isGrounded ) {
				Rotation = Rotation.Slerp( Rotation, goalRotation, groundRotationSpeed * Time.Delta );
			} else {
				Rotation = Rotation.Slerp( Rotation, goalRotation, airRotationSpeed * Time.Delta );
			}


			// Move the player towards their velocity
			moveHelper.TryMoveWithStep( Time.Delta, stepUpHeight );

			Log.Info( Velocity.z );

			Velocity = moveHelper.Velocity;
			Position = moveHelper.Position;
		}

	}
}
