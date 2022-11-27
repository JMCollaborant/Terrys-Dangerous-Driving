
using Sandbox;
using TDD.player.mechanics.movement.platformer_holdovers;
using TDD.player.mechanics.movement;

namespace TDD.movement {
    partial class DoubleJump : BasePlayerMechanic {

        public override string HudName => "Double Jump";
        public override string HudDescription => $"Press {InputActions.Jump.GetButtonOrigin()} in air";

        public float DoubleJumpStrength => 240f;
        public override bool TakesOverMovement => false;
        public override bool AlwaysSimulate => true;

        public int DoubleJumpsRemaining { get; set; }

        public TimeUntil TimeUntilCanDoubleJump;
        private bool justJumped;

        public DoubleJump( PlayerController controller ) : base( controller ) {
        }

        public override void PostSimulate() {
            base.PostSimulate();

            if ( justJumped && !InputActions.Jump.Down() ) {
                justJumped = false;
            }
        }

        public override void PreSimulate() {
            base.PreSimulate();

            if ( ctrl.GroundEntity != null ) {
                TimeUntilCanDoubleJump = .25f;
                DoubleJumpsRemaining = 1;

                if ( InputActions.Jump.Pressed() ) {
                    justJumped = true;
                }
            }

            if ( justJumped ) return;
            if ( ctrl.GroundEntity != null ) return;
            if ( !InputActions.Jump.Pressed() ) return;
            if ( TimeUntilCanDoubleJump > 0 ) return;
            if ( ctrl.GetMechanic<Glide>()?.Gliding ?? false ) return;
            if ( ctrl.GetMechanic<CrouchJump>()?.IsDuckjumping ?? false == true ) return;
            if ( DoubleJumpsRemaining <= 0 ) return;

            ctrl.Velocity = ctrl.Velocity.WithZ( DoubleJumpStrength );
            DoubleJumpsRemaining--;


            var groundslam = ctrl.GetMechanic<GroundPound>();
            if ( groundslam != null && groundslam.IsActive ) {
                groundslam.Cancel();
                ctrl.Velocity = ctrl.Velocity.WithZ( 220 );
            }

            DoubleJumpEffect();
        }

        private void DoubleJumpEffect() {
            if ( !ctrl.Pawn.IsServer ) return;

            using var _ = Prediction.Off();

            ctrl.AddEvent( "jump" );

            Particles.Create( "particles/gameplay/player/doublejump/doublejump.vpcf", ctrl.Pawn );
            Sound.FromWorld( "player.djump", ctrl.Pawn.Position );
        }

    }
}
