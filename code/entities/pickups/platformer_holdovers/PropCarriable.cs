﻿
using Sandbox;
using SandboxEditor;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TDD.entities.pickups.platformer_holdovers;

namespace TDD;

[Library( "plat_prop_carriable" )]
[Display( Name = "Prop Carriable", GroupName = "Platformer", Description = "A model the player can carry." ), Category( "Gameplay" ), Icon( "shopping_bag" )]
[HammerEntity]
public partial class PropCarriable : Prop, IUse {
    public enum PropType {
        Wood,
        Cardboard
    }

    //public enum SpawnType
    //{
    //	Coin,
    //	Life,
    //	Health
    //}

    [Property( "model_properties", Title = "Break Type" ), Net]
    public PropType BreakType { get; set; } = PropType.Wood;

    [Property( Title = "Spawn Pickups on Destroyed." ), Net]
    public bool SpawnOnDeath { get; set; } = false;

    //Maybe revist this later.
    //[Property( "spawn_properties", Title = "Break Type" ), Net]
    //public SpawnType PickUpType { get; set; } = SpawnType.Coin;

    [Property( Title = "Amount to Spawn" ), Net]
    public int AmountToSpawn { get; set; } = 0;


    public string SoundBreak = "break.wood";

    public string ParticleBreak = "particles/break/break.wood.vpcf";

    public override void Spawn() {
        base.Spawn();

        Transmit = TransmitType.Always;

        if ( BreakType == PropType.Wood ) {
            SoundBreak = "break.wood";
            ParticleBreak = "particles/break/break.wood.vpcf";
        }

        if ( BreakType == PropType.Cardboard ) {
            SoundBreak = "break.cardboard";
            ParticleBreak = "particles/break/break.cardboard.vpcf";
        }

        Tags.Add( "PropCarry" );
    }

    public void Drop( Vector3 velocity ) {
        if ( !Parent.IsValid() ) return;

        Velocity = velocity;
        EnableCollisionWithDelay( .1f );

        SetParent( null );
    }

    public bool IsUsable( Entity user ) => !Parent.IsValid();

    public bool OnUse( Entity user ) {
        if ( user is not Player p ) return false;
        if ( p.HeldBody.IsValid() ) return false;

        SetParent( p );

        p.HeldBody = this;
        EnableAllCollisions = false;

        LocalPosition = Vector3.Up * 30 + Vector3.Forward * Model.RenderBounds.Size.x * 1.1f;
        LocalRotation = Rotation.Identity;

        return true;
    }

    public override void OnKilled() {
        DeathEffect();
        base.OnKilled();
    }

    public void DeathEffect() {
        Platformer.PropCarryBreak( Position, ParticleBreak, SoundBreak );

        if ( SpawnOnDeath ) {
            for ( int i = 0; i < AmountToSpawn; i++ ) {
                var Coins = new CoinPickup {
                    Position = Position + Rotation.Up * new Random().Next( 40, 60 ),
                };

                Coins.SpawnWithPhys();
                Coins.Tags.Add( "Weapon" );
                //Coins.SetInteractsAs( CollisionLayer.Debris );
            }
        }
    }

    private async void EnableCollisionWithDelay( float delay ) {
        await Task.DelaySeconds( delay );

        EnableAllCollisions = true;
    }

    protected override void OnPhysicsCollision( CollisionEventData eventData ) {
        if ( eventData.This.Entity is Player p ) return;

        base.OnPhysicsCollision( eventData );
    }

}
