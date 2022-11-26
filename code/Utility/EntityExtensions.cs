using Sandbox;

namespace TDD.utility;

internal static class EntityExtensions {

    public static BBox GetBBox( this Entity ent ) {
        BBox bBox = new BBox();

        foreach ( PhysicsBody body in ent.PhysicsGroup.Bodies ) {
            bBox.AddPoint( body.GetBounds().Maxs );
            bBox.AddPoint( body.GetBounds().Mins );
        }

        return bBox;
    }

    public static void SetRenderColorRecursive( this Entity e, Color color ) {
        if ( !e.IsValid() ) return;

        if ( e is ModelEntity m )
            m.RenderColor = color;

        foreach ( var child in e.Children ) {
            child.SetRenderColorRecursive( color );
        }
    }

}
