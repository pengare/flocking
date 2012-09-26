using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using flocking.animal;
using Microsoft.Xna.Framework;

namespace flocking {
    public class Formation {
        public List<Animal> AnimalList { get; private set; }
        public Rectangle Screen { get; private set; }
        public Grid Grid { get; private set; }

        public Formation(Rectangle scr, int slits) {
            this.AnimalList = new List<Animal>();
            this.Screen = scr;
            this.Grid = new Grid(scr, slits);
        }

        public IEnumerable<Animal> findNeighbors(Animal cnt) {
            IEnumerable<Animal> seq;
            float dst = cnt.AnimalSpec.DetectionDistance;
            seq = Grid.encompassNeighbors(cnt, dst);
            return seq;
        }

        public void normalize(ref Vector2 pos, out Vector2 normalized) {
            normalized = pos;
            if (pos.X < Screen.Left)
                normalized.X += Screen.Width;
            else if (Screen.Right <= pos.X)
                normalized.X -= Screen.Width;

            if (pos.Y < Screen.Top)
                normalized.Y += Screen.Height;
            else if (Screen.Bottom <= pos.Y)
                normalized.Y -= Screen.Height;
        }

        public void move(Animal anm, Vector2 newPos) {
            Grid.remove(anm);
            anm.Position = newPos;
            Grid.add(anm);
        }
        public void add(Animal newone) {
            AnimalList.Add(newone);
            Grid.add(newone);
        }
        public void remove(Animal oldone) {
            AnimalList.Remove(oldone);
            Grid.remove(oldone);
        }
    }
}
