using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace flocking.spec {
    public class FishSpec : Spec {
        public static float RadialVelocityOriginal = 10f;
        public  float RadialVelocity { get; set; }
        

        public float DetectionDistance { get; private set; }
        public float SeparationDistance { get; private set; }

        public float InertiaDirectionSensitivity { get; private set; }
        public float RandomDirectionSensitivity { get; private set; }
        public float MemberSensitivity { get; private set; }
        public float AlienSensitivity { get; private set; }
        public float AlienSeparationDistance{get; private set;}

        public float RotationLimitation { get; private set; }

        public FishSpec() {

            RadialVelocity = 10f;

            DetectionDistance = 70f;
            SeparationDistance = 60f;
            InertiaDirectionSensitivity = 1.0f;
            MemberSensitivity = 1.0f;
            AlienSensitivity = 10000f;//5000f;// 50f;
            AlienSeparationDistance = 50000f;//600f;

            RandomDirectionSensitivity = 0.05f;

            RotationLimitation = 5.0f;
        }
    }
}
