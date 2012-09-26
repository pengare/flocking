using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using flocking.spec;
using flocking.behavior;

namespace flocking.animal {
    public class Whale : Animal {
        public const AnimalType defaultType = AnimalType.Whale;

        private static Random rand = new Random();
        public Whale(Vector2 pos, Vector2 dir) {
            this.Position = pos;
            this.Direction = dir;
            this.AnimalSpec = new WhaleSpec();
            this.AnimalType = defaultType;
        }

        public override void aimAt(Animal someone, float sdist, ref Animal target, ref float tdist) {
            if (sdist < tdist && someone.AnimalType == AnimalType.Fish) {
                target = someone;
                tdist = sdist;
            }
        }
        public override void updateColor(Animal nearest, float dist) {
            float weight = dist / AnimalSpec.SeparationDistance;
            Color = new Color(1f, weight, weight);
        }
     }
}
