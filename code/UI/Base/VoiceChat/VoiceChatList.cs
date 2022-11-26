﻿using Sandbox;
using Sandbox.UI;
using System;
using System.Linq;

namespace TDD.ui;

public class VoiceChatList : Panel {
    public static VoiceChatList Current { get; internal set; }

    public VoiceChatList() {
        Current = this;
        StyleSheet.Load( "/UI/Base/VoiceChat/VoiceChatList.scss" );
    }

    public void OnVoicePlayed( long steamId, float level ) {
        var entry = ChildrenOfType<VoiceChatEntry>().FirstOrDefault( x => x.Friend.Id == steamId );
        if ( entry == null ) entry = new VoiceChatEntry( this, steamId );

        entry.Update( level );
    }
}
