using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using flocking.animal;
using System.Diagnostics;

namespace flocking.behavior {
    public class RandomBehavior : Behavior {
        private static BehaviorType type = BehaviorType.Any;
        private Random rand;

        public RandomBehavior() {
            rand = new Random();
        }

        public BehaviorType BehaviorType { get { return type; } }
        public bool isReactive(Animal me, Animal you, ref Vector2 dir, float dist) {
            return true;
        }
        public Vector2 react(Animal me, Animal you, ref Vector2 dir, float dist) {
            float angle = (float)rand.NextDouble() * MathHelper.TwoPi;
            float ratio = me.AnimalSpec.RandomDirectionSensitivity;
            Vector2 result =
                new Vector2(ratio * (float)Math.Cos(angle), ratio * (float)Math.Sin(angle));
            float len = result.Length();
            Debug.Assert(0 < len && len < 1e5);
            return result;
        }
    }
}
