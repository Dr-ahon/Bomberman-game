using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman
{
   
    public class Mapa
    {
        private bool[,] Map = new bool[15, 11];
        private List<(int X, int Y)> targets;

        public Mapa(bool[,] PreMap, List<(int X, int Y)> PreCil)
        {
            this.Map = PreMap;
            this.targets = PreCil;
        }

        public bool IsWall(int X, int Y)
        {
            if (X < 0 || Y < 0) return true;
            return Map[X, Y];
        }

        public (bool isTarget, (int, int) target) IsTarget(int X, int Y)
        {
            foreach ((int x, int y) t in targets)
            {
                if (X == t.x && Y == t.y)
                {
                    return (true, t);
                }

            }
            return (false, (0, 0));
           
        }
    }

    class Stav
    {
        public int X;
        public int Y;

        public Stav(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public (int, int) Quadruple()
        {
            return (X, Y);
        }

        public List<Stav> NextStav(Mapa Mapa)
        {
            var seznam = new List<(int X, int Y)> {
            (X + 1, Y),
            (X - 1, Y),
            (X, Y + 1),
            (X, Y - 1)
        };
            seznam = seznam
                .Where(a => !(Mapa.IsWall(a.X, a.Y)))                                     //vynda ty, co vesli do zdi
                .ToList();

            return seznam.Select(t => new Stav(t.X, t.Y)).ToList(); ;                    //vynda ty, co vesli do bedny
        }
    }

    

}


