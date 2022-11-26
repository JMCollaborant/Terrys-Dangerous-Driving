
using Sandbox;
using Sandbox.UI;

namespace TDD.ui;

public partial class WaitingForPlayers : Panel {

    public static WaitingForPlayers Instance;
    public float timesince = 0;
    public Label newwaitingforplayers;

    public WaitingForPlayers() {
        StyleSheet.Load( "/ui/base/Waiting/WaitingForPlayers.scss" );

        newwaitingforplayers = AddChild<Label>( "waitingforplayers" );

        Instance = this;
    }

    public override void Tick() {
        base.Tick();

        if ( Time.Now - timesince < 5 ) {
            AddClass( "visible" );
        } else {
            RemoveClass( "visible" );
        }
    }

    public static void ShowWaitingForPlayers( string title ) {
        if ( Instance == null )
            return;

        Instance.newwaitingforplayers.SetText( title );
        Instance.timesince = Time.Now;

    }
}
