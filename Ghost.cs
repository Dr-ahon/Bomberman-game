using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bomberman
{
    public class Ghost
    {
        public PictureBox picbox;
        private Form1 _form;
        private static Random _rand;
        public int ID;
        public int waitTime;


        public Ghost(Point loc, Form1 form)
        {
            this.picbox = new PictureBox
            {
                Location = loc,
                BackgroundImage = Properties.Resources.ghost,
                Size = new Size(50, 50),
                BackgroundImageLayout = ImageLayout.Stretch,
            };
            this._form = form;
            _form.Controls.Add(this.picbox);
            _form.ghosts.Add(this);
            _rand = new Random();
            waitTime = 0;
        }


        public void GhostGoMethod(List<(int x, int y)> targets)
        {
            if (waitTime > 0) return;

            var result = DoesGhostSeeBomb();
            var ghostSight = result.dir;
            int x = result.bomb.Location.X;
            int y = result.bomb.Location.Y;
            var newTargets = new List<(int x, int y)>();
            switch (ghostSight)
            {
                case "right":
                case "left":
                    newTargets.Add((x + 50, y + 50));
                    newTargets.Add((x + 50, y - 50));
                    newTargets.Add((x - 50, y + 50));
                    newTargets.Add((x - 50, y - 50));
                    newTargets.Add((x + 50, y + 100));
                    newTargets.Add((x + 50, y - 100));
                    newTargets.Add((x - 50, y + 100));
                    newTargets.Add((x - 50, y - 100));
                    newTargets.Add((x + 150, y));
                    newTargets.Add((x - 150, y));
                    targets = newTargets;
                    break;
                case "up":
                case "down":
                    newTargets.Add((x + 50, y + 50));
                    newTargets.Add((x - 50, y + 50));
                    newTargets.Add((x + 50, y - 50));
                    newTargets.Add((x - 50, y - 50));
                    newTargets.Add((x + 100, y + 50));
                    newTargets.Add((x - 100, y + 50));
                    newTargets.Add((x + 100, y - 50));
                    newTargets.Add((x - 100, y - 50));
                    newTargets.Add((x, y + 150));
                    newTargets.Add((x, y - 150));
                    targets = newTargets;
                    break;
                case "far":
                    return;
                case "free":
                    break;

            }
            var temp = XFS(targets);
            var loc = (temp.x * 50, temp.y * 50);
            int whatShouldGhostDo = _rand.Next(10);

            switch (whatShouldGhostDo)
            {
                case 1:
                    if (_form.bombs.Where(a => a._owner == "ghost" + ID.ToString()).ToList().Count < 2)
                    {
                        Bomb newBomb = new Bomb(this.picbox.Location, _form, "ghost" + ID.ToString());
                    }
                    else this.GhostMove(loc);

                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    //case 11:
                    //case 12:
                    //case 13:
                    //case 14:
                    //case 15:
                    this.GhostMove(loc);
                    break;
            }

        }

        public void GhostMove((int x, int y) tuple)
        {


            if (tuple == (0, 0))
            {
                int x = this.picbox.Location.X;
                int y = this.picbox.Location.Y;
                var list = new List<(int X, int Y)> { (x, y + 50), (x, y - 50), (x + 50, y), (x - 50, y) }
                                        .Select(t => new Point(t.X, t.Y))
                                        .Where(p => _form.isFree(p))
                                        .ToList();
                if (list.Count == 0) return;
                var loc = list[_rand.Next(list.Count)];
                this.picbox.Location = loc;
            }
            else
            {
                this.picbox.Location = new Point(tuple.x, tuple.y);
            }

        }
        private bool[,] Putin()
        {
            bool[,] map = new bool[17, 14];
            foreach (PictureBox p in _form.pictureBoxes.Where(a => a != _form.hero).ToList())
            {
                map[p.Location.X / 50, p.Location.Y / 50] = true;
            }

            return map;

        }

        private (int x, int y) XFS(List<(int x, int y)> targets_)
        {
            Stav recStav = new Stav(this.picbox.Location.X / 50, this.picbox.Location.Y / 50);
            List<(int X, int Y)> targets = targets_;
            var map = new Mapa(Putin(), targets);
            var dicOfVisited = new Dictionary<(int x, int y), (int x, int y)>();

            var arrayOfVisited = new int[17, 14];
            arrayOfVisited[recStav.Quadruple().Item1, recStav.Quadruple().Item2] = 1;
            var fronta = new Queue<Stav>();
            fronta.Enqueue(recStav);

            while (fronta.Count != 0)
            {
                Stav vrchol = fronta.Dequeue();
                var (A, B) = vrchol.Quadruple();
                var length = arrayOfVisited[A, B];

                var result = map.IsTarget(vrchol.X, vrchol.Y);

                if (result.isTarget)
                {
                    return GoBack(length, result.target, dicOfVisited);
                }

                foreach (var nextVrchol in vrchol.NextStav(map))
                {
                    var (C, D) = nextVrchol.Quadruple();
                    if (arrayOfVisited[C, D] == 0)
                    {
                        dicOfVisited.Add((C, D), (A, B));
                        arrayOfVisited[C, D] = length + 1;
                        fronta.Enqueue(nextVrchol);
                    }
                }
            }
            return (0, 0);
        }
        private (int x, int y) GoBack(int length, (int, int) loc_, Dictionary<(int, int), (int, int)> dic)
        {
            var loc = loc_;
            for (int i = 1; i < length - 1; i++)
            {
                var a = StepBack(loc, dic);
                loc = a;
            }
            return loc;
        }
        private (int x, int y) StepBack((int, int) loc, Dictionary<(int, int), (int, int)> dic)
        {
            (int, int) value;
            dic.TryGetValue(loc, out value);
            return value;
        }

        private (string dir, PictureBox bomb) DoesGhostSeeBomb()
        {
            int x = this.picbox.Location.X;
            int y = this.picbox.Location.Y;


            foreach (Bomb b in _form.bombs.Where(b => b._owner == "hero"))
            {

                if (b.picbox.Location == new Point(x, y + 50) ||
                    b.picbox.Location == new Point(x, y + 100)) return ("down", b.picbox);
                else if (b.picbox.Location == new Point(x, y - 50) ||
                         b.picbox.Location == new Point(x, y - 100) ||
                         b.picbox.Location == new Point(x, y - 150)) return ("up", b.picbox);
                else if (b.picbox.Location == new Point(x + 50, y) ||
                         b.picbox.Location == new Point(x + 100, y) ||
                         b.picbox.Location == new Point(x + 150, y)) return ("right", b.picbox);
                else if (b.picbox.Location == new Point(x - 50, y) ||
                         b.picbox.Location == new Point(x - 100, y) ||
                         b.picbox.Location == new Point(x - 150, y)) return ("left", b.picbox);
                else if (b.picbox.Location == new Point(x + 50, y + 50) ||
                         b.picbox.Location == new Point(x - 50, y - 50) ||
                         b.picbox.Location == new Point(x + 50, y - 50) ||
                         b.picbox.Location == new Point(x - 50, y + 50) ||
                         b.picbox.Location == new Point(x + 100, y + 50) ||
                         b.picbox.Location == new Point(x - 100, y - 50) ||
                         b.picbox.Location == new Point(x + 100, y - 50) ||
                         b.picbox.Location == new Point(x - 100, y + 50) ||
                         b.picbox.Location == new Point(x + 50, y + 100) ||
                         b.picbox.Location == new Point(x - 50, y - 100) ||
                         b.picbox.Location == new Point(x + 50, y - 100) ||
                         b.picbox.Location == new Point(x - 50, y + 100)) return ("far", b.picbox);
            }
            return ("free", picbox);
        }
    }
}
