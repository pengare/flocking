using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using flocking.animal;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using flocking.spec;

namespace flocking.behavior {
    public class RepulsionBehavior : Behavior {
        private static BehaviorType type = BehaviorType.Alien;

        public RepulsionBehavior() {
        }

        public BehaviorType BehaviorType { get { return type; } }
        public bool isReactive(Animal me, Animal you, ref Vector2 dir, float dist) {
            return (you.AnimalType == AnimalType.Whale) && you.bActive == true
                && (dist < me.AnimalSpec.AlienSeparationDistance);
        }
        public Vector2 react(Animal me, Animal you, ref Vector2 dir, float dist) {
            Vector2 result = dir * -me.AnimalSpec.AlienSensitivity;
            float len = result.Length();
            //acclerate radialspeed
            if (me.RadialVelocity - FishSpec.RadialVelocityOriginal < 20)
            {
                me.RadialVelocity *= 10.0f;
            }
            Debug.Assert(0 < len && len < 1e5);
            return result;
        }
    }
}
