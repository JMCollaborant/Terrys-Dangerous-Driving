using Sandbox;

namespace TDD.entities.gameplay {
    internal class TDDEntityBase : ModelEntity {

        public TDDEntityBase() : base() {

        }


        public override void Simulate( Client cl ) {
            base.Simulate( cl );

            if ( BasePlayerController.Debug ) {
                DebugOverlay.Text( this.Name, this.Position + Vector3.Up * this.PhysicsBody.GetBounds().Size.z );
                DebugOverlay.Box( this, Color.Blue );
            }

        }


    }
}
