using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using flocking.animal;

namespace flocking {
    public struct Compass {
        public int quad;
        public Animal anm;
        public Compass(Animal a, int q) {
            this.anm = a;
            this.quad = q;
        }
    }
    public struct AABB {
        public Vector2 min;
        public Vector2 max;
        public AABB(Vector2 _min, Vector2 _max) {
            this.min = _min;
            this.max = _max;
        }
        public float squareDistance(Vector2 cnt, float r) {
            float sdist = 0;
            if (cnt.X < min.X) {
                float dx = min.X - cnt.X;
                sdist += dx * dx;
            } else if (max.X < cnt.X) {
                float dx = cnt.X - max.X;
                sdist += dx * dx;
            }
            if (cnt.Y < min.Y) {
                float dy = min.Y - cnt.Y;
                sdist += dy * dy;
            } else if (max.Y < cnt.Y) {
                float dy = cnt.Y - max.Y;
                sdist += dy * dy;
            }
            return sdist - r * r;
        }
    }

    public class Grid {
        public Vector2 PMin { get; private set; }
        public Vector2 PMax { get; private set; }
        public int Slits { get; private set; }
        public int TotalGrids { get; private set; }
        public LinkedList<Animal>[] Cells { get; private set; }
        public Vector2 Step { get; private set; }

        public Grid(Rectangle box, int slts) {
            this.PMin = new Vector2(box.Left, box.Top);
            this.PMax = new Vector2(box.Right, box.Bottom);
            this.Slits = slts;
            this.TotalGrids = slts *slts;
            this.Cells = new LinkedList<Animal>[TotalGrids];
            for (int i = 0; i < this.TotalGrids; i++)
                this.Cells[i] = new LinkedList<Animal>();
            this.Step = new Vector2(box.Width / (float)slts, box.Height / (float)slts);
        }

        public IEnumerable<Animal> encompassNeighbors(Animal cnt, float dist) {
            List<Animal> ngh = new List<Animal>();
            Vector2 offset = new Vector2(dist, dist);
            int minx, miny;
            resolve(cnt.Position - offset, out minx, out miny);
            int maxx, maxy;
            resolve(cnt.Position + offset, out maxx, out maxy);

            for (int y = miny; y <= maxy; y++) {
                for (int x = minx; x <= maxx; x++) {
                    if (isCollide(cnt.Position, dist, x, y) == false)
                        continue;
                    int pos = index(x,y);
                    ngh.AddRange(Cells[pos]);
                }
            }
            
            return ngh;
        }

        public bool isCollide(Vector2 cnt, float dist, int nx, int ny) {
            AABB box = getBox(nx, ny);
            return (box.squareDistance(cnt, dist) < 0);
        }
        public AABB getBox(int nx, int ny) {
            AABB box;
            box.min = PMin + new Vector2(Step.X * nx, Step.Y * ny);
            box.max = box.min + Step;
            return box;
        }

        public int index(int nx, int ny) {
            int i = nx + ny * Slits;
            Debug.Assert(0 <= i && i < TotalGrids);
            return i;
        }
        public int index(Vector2 point) {
            int nx, ny;
            resolve(point, out nx, out ny);
            return index(nx, ny);
        }
        public void resolve(Vector2 point, out int nx, out int ny) {
            Vector2 offset = point - PMin;
            nx = (int)(offset.X / Step.X);
            ny = (int)(offset.Y / Step.Y);
            nx = Math.Max(0, Math.Min(Slits-1, nx));
            ny = Math.Max(0, Math.Min(Slits-1, ny));
        }

        public void add(Animal newOne) {
            int pos = index(newOne.Position);
            Cells[pos].AddFirst(newOne);
        }
        public bool remove(Animal oldOne) {
            int pos = index(oldOne.Position);
            return Cells[pos].Remove(oldOne);
        }
    }
}
