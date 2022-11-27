﻿using Sandbox;
using TDD.movement;

namespace TDD.player.mechanics.movement.platformer_holdovers {
    class Glide : BasePlayerMechanic {

        public float GlideGravity => 20f;
        public bool Gliding { get; set; }

        public override bool TakesOverMovement => false;
        public override bool AlwaysSimulate => true;

        private TimeSince tsJumpHold;

        public Glide( PlayerController controller ) : base( controller ) {
        }

        public override void PreSimulate() {
            base.PreSimulate();

            Gliding = false;

            if ( !ctrl.PlayerPickedUpGlider ) return;

            if ( ctrl.GroundEntity != null ) return;
            if ( ctrl.Energy == 0 ) return;
            if ( !InputActions.Jump.Down() ) {
                tsJumpHold = 0;
                return;
            }
            if ( ctrl.Velocity.z > 0 ) return;
            if ( tsJumpHold < .15f ) return;

            Gliding = true;
            ctrl.Energy = ( ctrl.Energy - ctrl.EnergyDrain * Time.Delta ).Clamp( 0f, ctrl.MaxEnergy );
            ctrl.Velocity = ctrl.Velocity.WithZ( -GlideGravity );
        }
    }
}
