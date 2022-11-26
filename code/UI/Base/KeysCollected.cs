

using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;
using TDD.UI;

namespace TDD.ui {
    public partial class KeysCollected : Panel {

        private List<KeyPanel> KeyPanels = new();
        private bool Built;

        public override void Tick() {
            // build this once on first tick, theoretically all entities exist by now
            if ( !Built ) {
                Built = true;

                foreach ( var key in Entity.All.OfType<KeyPickup>() ) {
                    KeyPanels.Add( Add.KeyPanel( $"{key.KeyIcon}", "key1", key.KeyNumber ) );
                }
            }
        }
    }
}
