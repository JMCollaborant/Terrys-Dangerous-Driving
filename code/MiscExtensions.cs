using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox {
    public static class MiscExtensions {
        public static Rotation RotateAroundGlobalAxis( this Rotation rotation, Transform currentlyLocalTo, Vector3 axis, float degrees ) {
            return rotation.RotateAroundAxis( currentlyLocalTo.NormalToLocal( axis.Normal ), degrees );
        }
    }
}
