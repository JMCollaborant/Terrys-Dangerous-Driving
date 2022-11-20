using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.PlayerController {
    internal partial class Player {

		bool justLanded = false;
		bool justJumped = false;
		bool isJumping = false;
		float lastJumpTime = Time.Now;

		bool hasDoubleJumped = false;
		float minDoubleJumpTime = 0.8f;

		// Min time between jumps
		static float minJumpDelay = 0.5f;

		void UpdateJumping() {

			hasDoubleJumped = false;
			isJumping = false;

			bool jumpTimeIsRight = ( Time.Now - lastJumpTime ) > minJumpDelay;
			if ( Input.Pressed( InputButton.Jump ) && jumpTimeIsRight ) {
				moveHelper.Velocity = moveHelper.Velocity.WithZ( jumpVelocity );
				justJumped = true;
				lastJumpTime = Time.Now;
				isJumping = true;
			} else {
				justJumped = false;
			}

			this.SetAnimParameter( "ButtonPushJump", justJumped );

        }

    }
}
