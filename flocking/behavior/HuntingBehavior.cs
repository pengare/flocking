using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using flocking.animal;
using System.Diagnostics;

namespace flocking.behavior {
    public class HuntingBehavior : Behavior {
        private static BehaviorType type = BehaviorType.Alien;

        public HuntingBehavior() {
        }

        public BehaviorType BehaviorType { get { return type; } }
        public bool isReactive(Animal me, Animal you, ref Vector2 dir, float dist) {
            return (you.AnimalType == AnimalType.Fish) 
                && (dist < me.AnimalSpec.DetectionDistance);
        }
        public Vector2 react(Animal me, Animal you, ref Vector2 dir, float dist) {
            float weight = me.AnimalSpec.MemberSensitivity 
                * (dist - me.AnimalSpec.SeparationDistance) 
                / (me.AnimalSpec.DetectionDistance - me.AnimalSpec.SeparationDistance);
            Vector2 result = dir * weight;
            float len = result.Length();
            Debug.Assert(0 < len && len < 1e4);
            return result;
        }
    }
}
