using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace flocking.spec {
    public class WhaleSpec : Spec {
        public float RadialVelocityOriginal { get; private set; }
        public  float RadialVelocity { get; set; }
        
        public float DetectionDistance { get; private set; }
        public float SeparationDistance { get; private set; }

        public float InertiaDirectionSensitivity { get; private set; }
        public float RandomDirectionSensitivity { get; private set; }
        public float MemberSensitivity { get; private set; }
        public float AlienSensitivity { get; private set; }
        public float AlienSeparationDistance { get; private set; }

        public float RotationLimitation { get; private set; }

        public WhaleSpec() {
            RadialVelocityOriginal = 120f;
            RadialVelocity = 120f;

            DetectionDistance = 60f;
            SeparationDistance = 40f;
            InertiaDirectionSensitivity = 1.0f;
            MemberSensitivity = 1.0f;
            AlienSensitivity = 10000f;// 5000f;
            RandomDirectionSensitivity = 0.05f;
            AlienSeparationDistance = 50000f;// 600f;

            RotationLimitation = 2.0f;
        }
    }
}
