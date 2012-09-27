using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using flocking.animal;
using System.Diagnostics;

namespace flocking.behavior
{
    public class DraftBehavior : Behavior
    {
        private static BehaviorType type = BehaviorType.Member;

        public DraftBehavior()
        {
        }

        public BehaviorType BehaviorType { get { return type; } }
        public bool isReactive(Animal me, Animal you, ref Vector2 dir, float dist) {
            return (me.AnimalType == you.AnimalType);
        }
        public Vector2 react(Animal me, Animal you, ref Vector2 dir, float dist) {
            Vector2 result = me.AnimalSpec.MemberSensitivity * new Vector2(1.0f, 0.0f);
            float len = result.Length();
            Debug.Assert(0 < len && len < 1e4);
            return result;
        }
    }
}
