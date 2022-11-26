using Sandbox;
using TDD.movement;

namespace TDD.player.mechanics.movement.platformer_holdovers {
    class SideBoost : BasePlayerMechanic {

        public override bool TakesOverMovement => true;

        private TimeSince timeSinceSideBoost;
        private TimeSince timeSinceMoveLeft;
        private TimeSince timeSinceMoveRight;

        public SideBoost( PlayerController ctrl )
            : base( ctrl ) {

        }

        protected override bool TryActivate() {
            if ( timeSinceSideBoost < .8f )
                return false;

            if ( ctrl.GroundEntity == null )
                return false;

            var moveLeft = Input.Pressed( InputButton.Left );
            var moveRight = Input.Pressed( InputButton.Right );

            if ( !moveLeft && !moveRight )
                return false;

            var timeSince = 0f;

            if ( moveLeft ) {
                timeSince = timeSinceMoveLeft;
                timeSinceMoveLeft = 0;
                timeSinceMoveRight = 100f;
            }

            if ( moveRight ) {
                timeSince = timeSinceMoveRight;
                timeSinceMoveRight = 0;
                timeSinceMoveLeft = 100f;
            }

            if ( timeSince > .3f )
                return false;

            ctrl.ClearGroundEntity();

            var boostVelocity = new Vector3( 0, Input.Left, 0 ) * Input.Rotation;
            boostVelocity *= 325f;
            boostVelocity += Vector3.Up * 130;

            ctrl.Velocity += boostVelocity;
            timeSinceSideBoost = 0;

            return true;
        }

        public override void Simulate() {
            base.Simulate();

            ctrl.Move();

            if ( timeSinceSideBoost < .15f )
                return;

            IsActive = false;
        }

    }
}
