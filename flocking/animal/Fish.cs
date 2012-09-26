using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using flocking.spec;
using flocking.behavior;

namespace flocking.animal {
    public class Fish : Animal {
        public const AnimalType defaultType = AnimalType.Fish;

        private static Random rand = new Random();
        public Fish(Vector2 pos, Vector2 dir) {
            this.Position = pos;
            this.Direction = dir;
            this.AnimalSpec = new FishSpec();
            this.AnimalType = defaultType;
        }

        public override void aimAt(Animal someone, float sdist, ref Animal target, ref float tdist) {
            if (sdist < tdist) {
                target = someone;
                tdist = sdist;
            }
        }
        public override void updateColor(Animal nearest, float dist) {
            float weight = dist / AnimalSpec.SeparationDistance;
            Color = new Color(weight, weight, 1f);
        }

    }
}
