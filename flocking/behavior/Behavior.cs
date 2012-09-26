using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using flocking.animal;
using Microsoft.Xna.Framework;

namespace flocking.behavior {
    public enum BehaviorType { Any, Member, Alien }
    public interface Behavior {
        BehaviorType BehaviorType { get; }
        bool isReactive(Animal me, Animal you, ref Vector2 dir, float dist);
        Vector2 react(Animal me, Animal you, ref Vector2 dir, float dist);
    }
}
