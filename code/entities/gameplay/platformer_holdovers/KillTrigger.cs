using Sandbox;
using SandboxEditor;
using System.ComponentModel.DataAnnotations;


namespace TDD.entities.gameplay.platformer_holdovers;

[Library( "kill_trigger", Description = "Spawns cars that you own." )]
[Display( Name = "kill_trigger", GroupName = "Terry's Dangerous Driving", Description = "Spawns cars that you own." ), Category( "Gameplay" ), Icon( "phone" )]
[HammerEntity]
internal partial class KillTrigger : BaseTrigger {

    public override void Touch( Entity other ) {
        base.Touch( other );

        if ( !other.IsServer ) return;
        if ( other is not Player pl ) return;

        Game.Current.DoPlayerSuicide( pl.Client );
    }

}
