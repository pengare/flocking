using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace flocking.spec {
    public class WhaleSpec : Spec {
        public float RadialVelocity { get; private set; }

        public float DetectionDistance { get; private set; }
        public float SeparationDistance { get; private set; }

        public float InertiaDirectionSensitivity { get; private set; }
        public float RandomDirectionSensitivity { get; private set; }
        public float MemberSensitivity { get; private set; }
        public float AlienSensitivity { get; private set; }

        public float RotationLimitation { get; private set; }

        public WhaleSpec() {
            RadialVelocity = 120f;

            DetectionDistance = 60f;
            SeparationDistance = 40f;
            InertiaDirectionSensitivity = 1.0f;
            MemberSensitivity = 1.0f;
            AlienSensitivity = 50f;
            RandomDirectionSensitivity = 0.05f;

            RotationLimitation = 2.0f;
        }
    }
}
