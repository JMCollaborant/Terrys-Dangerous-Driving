using Platformer;
using Platformer.Movement;
using Sandbox.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.player.movement.mechanics {
    internal class GroundKick : BaseMoveMechanic {

        public override string HudName => "Grounded Kick";
        public override string HudDescription => $"Press {InputActions.LeftClick.GetButtonOrigin()} while on the ground to commit an assault";

        public override bool TakesOverControl => true;
        public override bool AlwaysSimulate => false;

        private TimeSince freezeDuringKickTimer;
        private TimeSince kickLandDelayTimer;

        // Whether or not the kick's damage has been dealt yet
        private bool kickHasLanded = false;
        
        // How long the entire kick takes, AKA how long the player should be kept from moving
        private float kickDuration = 1.0f;

        // How long to wait after starting the kick before the animation is ready for us to deal damage
        private float kickLandDelay = 0.25f;

        // How far around the foot should we look for entities to kick?
        private float kickRadius = 30.0f;

        public GroundKick( PlatformerController controller ) : base( controller ) {
            BasePlayerController.Debug = true;
        }

        public void Cancel() {
            kickHasLanded = false;
            IsActive = false;
        }

        protected override bool TryActivate() {

            // We need to be on the ground and primary attacking
            if ( this.ctrl.GroundEntity is null ) return false;
            if ( !Input.Pressed( InputButton.PrimaryAttack ) ) return false;

            // Restart the timers
            freezeDuringKickTimer = 0f;
            kickLandDelayTimer = 0f;

            // New kick, who dis?
            kickHasLanded = false;

            // Stop the player in their tracks
            ctrl.Velocity = 0f;

            return true;
        }

        public override void Simulate() {
            base.Simulate();

            // Only REAL platformers get to use THIS cool move
            if ( ctrl.Pawn is not PlatformerPawn pl ) {
                Cancel();
                return;
            }

            // If we somehow become airborn, stop kicking
            if ( ctrl.GroundEntity is null ) {
                Cancel();
                return;
            }

            // When we finish kicking
            if ( freezeDuringKickTimer >= kickDuration ) {
                Cancel();
                return;
            }

            // Wait for us to be ready to do damage
            if ( kickLandDelayTimer < kickLandDelay ) {
                return;
            }

            // Don't kick twice
            if ( kickHasLanded ) {
                return;
            }
            kickHasLanded = true;

            Transform transform = new Transform( ctrl.Position, ctrl.Rotation );

            Vector3 kickPos = transform.RelativeVectorToWorld( 30f, 0, 20f );

            var ents = Entity.FindInSphere( kickPos, 30f );

            if ( BasePlayerController.Debug ) {
                DebugOverlay.Sphere( kickPos, 30f, Color.Red, 3f );
            }

            foreach ( var ent in ents ) {
                if ( ent == ctrl.Pawn ) continue;
                var dmgtype = ent is PlatformerPawn ? DamageFlags.Sonic : DamageFlags.Generic;
                var dmgAmount = ent is PlatformerPawn ? 2 : 80;

                ent.TakeDamage( new DamageInfo() {
                    Attacker = ctrl.Pawn,
                    Flags = dmgtype,
                    Damage = dmgAmount
                } );
            }

        }


    }
}
