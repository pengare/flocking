using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using flocking.animal;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace flocking.behavior {
    public class InertiaBehavior : Behavior {
        private static BehaviorType type = BehaviorType.Any;

        public InertiaBehavior() {
        }

        public BehaviorType BehaviorType { get { return type; } }
        public bool isReactive(Animal me, Animal you, ref Vector2 dir, float dist) {
            return true;
        }
        public Vector2 react(Animal me, Animal you, ref Vector2 dir, float dist) {
            Vector2 result = me.Direction * me.AnimalSpec.InertiaDirectionSensitivity;
            float len = result.Length();
            Debug.Assert(0 < len && len < 1e5);
            return result;
        }
    }
}
