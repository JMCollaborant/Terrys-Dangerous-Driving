using TDD.utility;
using System;
using System.Linq;
using Sandbox;
using TDD.movement;

namespace TDD.player.movement.mechanics {
    internal class GroundKick : BasePlayerMechanic {

        public override string HudName => "Grounded Kick";
        public override string HudDescription => $"Press {InputActions.LeftClick.GetButtonOrigin()} while on the ground to commit an assault";

        public override bool TakesOverMovement => true;
        public override bool TakesOverRotation => true;
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

        // How hard should we physically throw things that get kicked?
        private float kickForce = 1f;

        public GroundKick( PlayerController controller ) : base( controller ) {

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
            if ( ctrl.Pawn is not Player pl ) {
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

            if ( BasePlayerController.Debug ) {
                DebugOverlay.Sphere( kickPos, kickRadius, Color.Red, 3f );
            }

            var ents = Entity.FindInSphere( kickPos, kickRadius ).OfType<ModelEntity>();

            foreach ( ModelEntity ent in ents ) {
                if ( ent == ctrl.Pawn ) continue;
                var dmgtype = ent is Player ? DamageFlags.Sonic : DamageFlags.Generic;
                var dmgAmount = ent is Player ? 2 : 80;

                // Unfreeze anything that might have been frozen before we kicked it a lot
                if ( ent.PhysicsBody.BodyType == PhysicsBodyType.Dynamic ) {
                    ent.PhysicsBody.MotionEnabled = true;
                }

                if ( BasePlayerController.Debug ) {
                    Color color = Color.Red;

                    switch ( ent.PhysicsBody.BodyType ) {
                        case PhysicsBodyType.Static: {
                            color = Color.White;
                            break;
                        }
                        case PhysicsBodyType.Dynamic: {
                            color = Color.Green;
                            break;
                        }
                        case PhysicsBodyType.Keyframed: {
                            color = Color.Blue;
                            break;
                        }
                    }

                    if ( ent.GetType() == typeof( ModelBreakPiece ) ) {
                        color = Color.Orange;
                    }

                    DebugOverlay.Box( ent, color, 3f );
                }

                // Grab the gib list before we hurt the entity, lest it disappear on us
                ModelBreakPiece[] gibs = ent.Model.GetData<ModelBreakPiece[]>() ?? Array.Empty<ModelBreakPiece>();

                // Do damage to the entity
                ent.TakeDamage( new DamageInfo() {
                    Attacker = ctrl.Pawn,
                    Flags = dmgtype,
                    Damage = dmgAmount,
                    Force = this.ctrl.Rotation.Forward * ent.PhysicsBody.Mass * kickForce,
                    Body = ent.PhysicsBody,
                    Position = ent.PhysicsBody.MassCenter
                } );

                // Gibs don't currently inherit their parent's velocity if the parent was non-dynamic so we have to do this manually
                bool hasGibs = gibs.Length > 0;
                bool hasDied = ent is not null && ent.Health <= 0;
                if ( hasGibs && hasDied ) {
                    var gibList = Entity.FindInSphere( kickPos, kickRadius * 1.75f ).OfType<PropGib>();

                    foreach ( PropGib gib in gibList ) {
                        float mass = Math.Max( gib.PhysicsBody.Mass, 10 );

                        gib.ApplyAbsoluteImpulse( this.ctrl.Rotation.Forward * mass * ( kickForce * 1.25f ) );
                    }
                }
            }

        }


    }
}
