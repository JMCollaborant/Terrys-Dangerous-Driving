
using Sandbox;
using System;
using System.Linq;
using TDD.ui;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox.UI;

namespace TDD;

public partial class Platformer : Sandbox.Game {

    public new static Platformer Current;

    [Net]
    public float NumberOfCollectables { get; set; }

    public Platformer() {
        Current = this;

        BasePlayerController.Debug = true;
        if ( BasePlayerController.Debug ) {
            Log.Info( "-----------DEBUG ACTIVE-----------" );
        }
    }

    /// <summary>
    /// Someone is speaking via voice chat. This might be someone in your game,
    /// or in your party, or in your lobby.
    /// </summary>
    public override void OnVoicePlayed( Client cl ) {
        VoiceChatList.Current?.OnVoicePlayed( cl.PlayerId, cl.VoiceLevel );
    }

    [Event.Entity.PostSpawn]
    public void PostEntitySpawn() {
        NumberOfCollectables = All.OfType<KeyPickup>().Count();

        All.OfType<GenericPathEntity>()
            .ToList()
            .ForEach( x => x.Transmit = TransmitType.Always );
    }

    public override void ClientJoined( Client client ) {
        Log.Info( $"\"{client.Name}\" has joined the game" );
        ChatBox.AddInformation( To.Everyone, $"{client.Name} has joined", $"avatar:{client.PlayerId}" );

        Player platformerPawn = new Player( client );

        client.Pawn = platformerPawn;

        platformerPawn.Respawn();
    }

    public override void ClientDisconnect( Client client, NetworkDisconnectionReason reason ) {
        base.ClientDisconnect( client, reason );

        PlatformerChatBox.AddInformation( To.Everyone, $"{client.Name} has left the game", client.PlayerId );
    }

    public override void OnKilled( Client client, Entity pawn ) {
        base.OnKilled( client, pawn );

        var msg = Rand.FromList( killMessages );


        PlatformerChatBox.AddChatEntry( To.Everyone, client.Name, msg, client.PlayerId, null, false );
    }

    private List<string> killMessages = new()
    {
        "died",
        "couldn't stand"
    };

    [ClientRpc]
    public static void PropCarryBreak( Vector3 pos, string particle, string sound ) {
        Particles.Create( particle, pos );
        Sound.FromWorld( sound, pos );
    }

    [ClientRpc]
    public static void Alerts( string Title ) {
        NewMajorArea.ShowLandmark( Title );
    }

    [ClientRpc]
    public static void BeenTagged( string Title ) {
        WaitingForPlayers.ShowWaitingForPlayers( Title );
    }

    [ClientRpc]
    public static void Waiting( string Title ) {
        WaitingForPlayers.ShowWaitingForPlayers( Title );
    }

}
