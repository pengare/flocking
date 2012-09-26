using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using flocking.animal;
using System.Diagnostics;

namespace flocking.behavior {
    public class CohesionBehavior : Behavior {
        private static BehaviorType type = BehaviorType.Member;

        public CohesionBehavior() {
        }

        public BehaviorType BehaviorType { get { return type; } }
        public bool isReactive(Animal me, Animal you, ref Vector2 dir, float dist) {
            return (me.AnimalType == you.AnimalType)
                && (dist > me.AnimalSpec.SeparationDistance);
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
