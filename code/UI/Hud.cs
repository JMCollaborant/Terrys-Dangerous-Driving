
using Sandbox.UI;

using Sandbox;

namespace TDD.UI;

[UseTemplate]
public class Hud : RootPanel
{

	public Hud()
	{
		Local.Hud = this;
	}

	public override void Tick()
	{
		//SetClass( "game-end", Platformer.GameState == GameStates.GameEnd );
		//SetClass( "game-warmup", Platformer.GameState == GameStates.Warmup );
	}

}
