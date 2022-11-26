﻿using TDD.movement;

namespace TDD.player.mechanics.movement {
    class WaterMove : BasePlayerMechanic {

        public float WaterAcceleration => 10;

        public override bool TakesOverMovement => true;

        public WaterMove( PlayerController ctrl )
            : base( ctrl ) {

        }

        protected override bool TryActivate() {
            if ( ctrl.Pawn.WaterLevel > 0.6f )
                return true;
            return false;
        }

        public override void Simulate() {
            if ( ctrl.Pawn.WaterLevel <= 0.6f ) {
                IsActive = false;
                return;
            }

            if ( InputActions.Jump.Down() )
                ctrl.Velocity = ctrl.Velocity.WithZ( 100 );

            // ctrl.ApplyFriction( 1 );

            var wishdir = ctrl.WishVelocity.Normal;
            var wishspeed = ctrl.WishVelocity.Length;

            wishspeed *= 0.8f;

            ctrl.Accelerate( wishdir, wishspeed, 100, WaterAcceleration );
            ctrl.Velocity += ctrl.BaseVelocity;
            ctrl.Move();
            ctrl.Velocity -= ctrl.BaseVelocity;
        }

    }
}
