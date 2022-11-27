
using Sandbox;
using SandboxEditor;
using System;
using System.ComponentModel.DataAnnotations;
using TDD.entities.gameplay;

namespace TDD;

[HammerEntity]
[Library( "Phonebooth", Description = "Spawns cars that you own.", Editable = true )]
[Display( Name = "Phone Booth", GroupName = "Terry's Dangerous Driving", Description = "Spawns cars that you own." ), Category( "Gameplay" ), Icon( "phone" )]
[EditorModel( "models/sbox_props/office_chair/office_chair.vmdl" )]
[BoundsHelper( nameof( carSpawnMins ), nameof( carSpawnMaxs ), AutoCenter = false )]
internal partial class PhoneBooth : TDDEntityBase, IUse {
    private Vector3 carSpawnMins { get; set; }
    private Vector3 carSpawnMaxs { get; set; }

    public override void Spawn() {
        base.Spawn();
        SetModel( "models/sbox_props/office_chair/office_chair.vmdl" );

        Transmit = TransmitType.Always;

        SetupPhysicsFromOBB( PhysicsMotionType.Keyframed, Model.Bounds.Mins, Model.Bounds.Maxs );

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

    public override void DrawDebugInfo() {
        base.DrawDebugInfo();

        DebugOverlay.Box( Position, carSpawnMins, carSpawnMaxs, Color.Yellow );
    }
}
