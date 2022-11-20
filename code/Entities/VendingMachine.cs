using SandboxEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Entities {

    [HammerEntity]
    [Title( "Vending Machine" ), Category( "Hit & Run" )]
    [EditorModel( "models/props/vending machine/vending_machine_remesh1.vmdl" )]
    class VendingMachine : AnimatedEntity, IUse {
        public VendingMachine() {
            this.SetModel( "models/props/vending machine/vending_machine_remesh1.vmdl" );
        }

        public bool IsUsable( Entity user ) {
            return true;
        }

        public bool OnUse( Entity user ) {
            Log.Info( "Used vending machine" );
            return true;
        }
    }

}
