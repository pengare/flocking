using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using flocking.animal;
using Microsoft.Xna.Framework;

namespace flocking {
    public class AnimalUpdater {
        public Formation Formation { get; set; }

        public AnimalUpdater() {
        }

        public void update(GameTime gametime) {
            IEnumerable<Animal> list = Formation.AnimalList;
            float dt = (float)gametime.ElapsedGameTime.TotalSeconds;
            foreach (Animal anm in list) {
                anm.update(dt, Formation);
            }
        }
    }
}
