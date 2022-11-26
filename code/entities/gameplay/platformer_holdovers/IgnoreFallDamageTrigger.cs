using Sandbox;
using SandboxEditor;
using System.ComponentModel.DataAnnotations;

namespace TDD.entities.gameplay.platformer_holdovers;

[Library( "plat_ignorefalldamage", Description = "Ignore Fall Damage." )]
[AutoApplyMaterial( "materials/editor/ignorefalldamage/ignorefalldamage.vmat" )]
[Display( Name = "Ignore Fall Damage", GroupName = "Platformer", Description = "Ignore Fall Damage" ), Category( "Triggers" ), Icon( "do_not_step" )]
[HammerEntity]
internal partial class IgnoreFallDamageTrigger : BaseTrigger {

    public override void StartTouch( Entity other ) {
        base.StartTouch( other );

        if ( !other.IsServer ) return;
        if ( other is not Player pl ) return;
    }

    public override void EndTouch( Entity other ) {
        base.EndTouch( other );

        if ( !other.IsServer ) return;
        if ( other is not Player pl ) return;

    }

}
