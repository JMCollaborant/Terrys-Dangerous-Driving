
using Sandbox;
using SandboxEditor;
using System.ComponentModel.DataAnnotations;
using TDD.entities.gameplay;

namespace TDD;

[Library( "Phonebooth", Description = "Spawns cars that you own." )]
[Display( Name = "Phone Booth", GroupName = "Terry's Dangerous Driving", Description = "Spawns cars that you own." ), Category( "Gameplay" ), Icon( "phone" )]
[HammerEntity]
internal partial class PhoneBooth : TDDEntityBase, IUse {

    public override void Spawn() {
        base.Spawn();
        SetModel( "models/sbox_props/office_chair/office_chair.vmdl" );

        Transmit = TransmitType.Always;

        SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

        EnableAllCollisions = true;
        EnableSolidCollisions = false;
    }

    public bool IsUsable( Entity user ) {
        if ( user.GetType() != typeof( Player ) || user.Health <= 0 ) {
            return false;
        }
        return true;
    }

    public bool OnUse( Entity user ) {
        Log.Info( "We got used!" );
        return false;
    }
}
