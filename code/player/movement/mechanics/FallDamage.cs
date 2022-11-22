﻿
using Sandbox;
using System;

namespace Platformer.Movement
{
	class FallDamage : BaseMoveMechanic
	{

		public override bool AlwaysSimulate => true;
		public override bool TakesOverControl => false;

		private float prevFallSpeed;
		private bool prevGrounded;

		public FallDamage( PlatformerController controller )
			: base( controller )
		{

		}

		public override void PreSimulate()
		{
			base.PreSimulate();

			prevGrounded = ctrl.GroundEntity != null;
			prevFallSpeed = ctrl.Velocity.z;
		}

		public override void PostSimulate()
		{
			base.PostSimulate();

			if ( ctrl.GroundEntity == null || prevGrounded ) return;

			Sound.FromWorld( "player.land1", ctrl.Pawn.Position );

			if ( ctrl.Pawn is not PlatformerPawn p ) return;
			if ( p.IgnoreFallDamage ) return;

			var dmg = GetFallDamage( prevFallSpeed );

			if ( dmg == 0 ) return;

			p.TakeDamage( new DamageInfo() { Damage = dmg } );
			p.SetInvulnerable( 2f );

			FallDamageEffect();
		}

		private void FallDamageEffect()
		{
			if ( !ctrl.Pawn.IsServer ) return;
			using var _ = Prediction.Off();

			Sound.FromWorld( "player.fall1", ctrl.Pawn.Position );
		}

		private int GetFallDamage( float fallspeed )
		{
			fallspeed = Math.Abs( fallspeed );

			if ( fallspeed < 700 ) return 0;
			if ( fallspeed < 1000 ) return 1;
			if ( fallspeed < 1300 ) return 2;
			if ( fallspeed < 1600 ) return 3;

			return 4;
		}

	}
}
