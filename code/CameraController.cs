using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox {
    internal class CameraController : CameraMode {

        private Angles orbitAngle = new Angles( 0, 0, 0 );
        private float goalOrbitDistance = 125f;

        public override void Update() {
            Rotation = orbitAngle.ToRotation();

            Vector3 cameraCenter = Entity.Position + new Vector3( 0, 0, 32 );

            TraceResult trace = Trace.Ray( cameraCenter, cameraCenter - Rotation.Forward * goalOrbitDistance )
            .Size( 16 )
            .Ignore( Entity )
            .WithTag( "solid" )
            .Run();

            Position = trace.EndPosition;
        }

        public override void BuildInput( InputBuilder input ) {
            orbitAngle.pitch += input.AnalogLook.pitch;
            orbitAngle.pitch = orbitAngle.pitch.Clamp( -85, 85 );

            orbitAngle.yaw += input.AnalogLook.yaw;
            orbitAngle = orbitAngle.Normal;

            input.ViewAngles = orbitAngle;
        }
    }
}
