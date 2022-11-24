using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Utility {
    internal static class TransformExtensions {

        public static Vector3 RelativeVector( this Transform transform, float forward, float right, float up ) {
            return RelativeVector( transform, new Vector3( forward, right, up ) );
        }

        public static Vector3 RelativeVector( this Transform transform, Vector3 motion ) {
            return transform.Rotation.Forward * motion.x + transform.Rotation.Right * motion.y + transform.Rotation.Up * motion.z;
        }

        public static Vector3 RelativeVectorToWorld( this Transform transform, float forward, float right, float up ) {
            return transform.Position + transform.RelativeVector( forward, right, up );
        }

        public static Vector3 RelativeVectorToWorld( this Transform transform, Vector3 motion ) {
            return transform.Position + transform.RelativeVector( motion );
        }

    }
}
