using Sandbox;
using SandboxEditor;
using System.ComponentModel.DataAnnotations;

namespace TDD.entities.pickups.platformer_holdovers;

[Library( "plat_glider", Description = "Glider Pickup" )]
[EditorModel( "models/gameplay/glider/handglider.vmdl", FixedBounds = true )]
[Display( Name = "Glider Pickup", GroupName = "Platformer", Description = "Glider Pickup." ), Category( "Gameplay" ), Icon( "paragliding" )]
[HammerEntity]
internal partial class GliderPickup : BaseCollectible {
    public override void Spawn() {
        base.Spawn();

        SetModel( "models/gameplay/glider/handglider.vmdl" );
    }
    protected override bool OnCollected( Player pl ) {
        base.OnCollected( pl );

        return true;
    }

    protected override void OnCollectedEffect() {
        Sound.FromEntity( "life.pickup", this );
        Particles.Create( "particles/explosion/barrel_explosion/explosion_gib.vpcf", this );
    }

    [Event.Tick.Server]
    public void Tick() {
        Rotation = Rotation.FromYaw( Rotation.Yaw() + 100 * Time.Delta );
    }
}
