
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TDD.ui {
    public class CoinCurrent : Panel {

        //public Image Image;
        public Label Icon;
        public Label Number;


        public CoinCurrent() {

            //Image = Add.Image( "ui/hud/coin.png", "coinimage" );
            Icon = Add.Label( "paid", "icon" );
            Number = Add.Label( "", "coinnumber" );

        }

        public override void Tick() {


            var player = Local.Pawn;
            if ( player == null ) return;

            if ( Local.Pawn is not Player pl ) return;
            var Coin = pl.Coin;

            SetClass( "active", true );

            Number.Text = $"{Coin}";
        }
    }
}
