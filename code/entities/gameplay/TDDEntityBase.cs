using Sandbox;

namespace TDD.entities.gameplay {
    internal class TDDEntityBase : ModelEntity {

        public TDDEntityBase() : base() {}

        [Event.Frame]
        private void OnEveryFrame() {
            if ( BasePlayerController.Debug ) {
                DrawDebugInfo();
            }
        }

        public virtual void DrawDebugInfo() {
            DebugOverlay.Text( this.ClassName, this.Position + Vector3.Up * this.PhysicsBody.GetBounds().Size.z );
            DebugOverlay.Box( this, Color.Blue );
        }

    }
}
