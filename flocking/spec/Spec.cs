using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace flocking.spec {
    public interface Spec {
        float RadialVelocity { get; }

        float DetectionDistance { get; }
        float SeparationDistance { get; }

        float InertiaDirectionSensitivity { get; }
        float RandomDirectionSensitivity { get; }
        float MemberSensitivity { get; }
        float AlienSensitivity { get; }

        float RotationLimitation { get; }
    }
}
