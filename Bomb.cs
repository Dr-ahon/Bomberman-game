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
    public class Bomb
    {
        public PictureBox picbox;
        private List<PictureBox> _flames;
        private Form1 _form;
        public string _owner;
        private bool detonated;
        private List<Bomb> bombsToExplode;
        private List<Ghost> ghostsToKill;
        private int _lifetime;
        public int Lifetime => _lifetime;

        public int DecreaseLifetime(int by)
        {
            _lifetime -= by;
            return Lifetime;
        }

        

        public Bomb(Point loc, Form1 form, string owner)
        {
            this.picbox = new PictureBox
            {
                Location = loc,
                BackgroundImageLayout = ImageLayout.Zoom,
                Size = new Size(50, 50),
                Visible = true
            };
            if (owner == "hero") picbox.BackgroundImage = Properties.Resources.bomb;
            else picbox.BackgroundImage = Properties.Resources.ghostBomb;
            this._owner = owner;
            this.detonated = false;
            this._flames = new List<PictureBox>();
            this.bombsToExplode = new List<Bomb>();
            this.ghostsToKill = new List<Ghost>();
            form.Controls.Add(this.picbox);
            this._form = form;
            this._lifetime = 3000;
            _form.bombs.Add(this);
        }

        public void BombAction(int time)
        {
           switch(time)
           {
                case 0:
                    Explode();
                    break;
                case -1:
                    EndBomb();
                    break;
           }
        }

        private void Explode()
        {
            if (!_form.bombs.Contains(this)) return;
            
            MakeFlames();
           
            DetonateOrDie();
        }

        private void MakeFlames()
        {
            PictureBox flame = new PictureBox();
            this.detonated = true;
            for (int b = 1; b <= 4; b++)
            {

                //delka plamenu je tri pole
                for (int a = 50; a <= 150; a += 50)
                {
                    bool didBomb = false;
                    flame = new PictureBox
                    {
                        Size = new Size(50, 50),
                        BackgroundImage = Properties.Resources.flame,
                        BackgroundImageLayout = ImageLayout.Stretch
                    };
                    flame.BringToFront();
                    flame.Visible = true;

                    _form.Controls.Add(flame);
                    this._flames.Add(flame);


                    switch (b)
                    {
                        case 1:
                            flame.Location = new Point(this.picbox.Location.X - a, this.picbox.Location.Y);
                            break;
                        case 2:
                            flame.Location = new Point(this.picbox.Location.X, this.picbox.Location.Y - a);
                            break;
                        case 3:
                            flame.Location = new Point(this.picbox.Location.X + a, this.picbox.Location.Y);
                            break;
                        case 4:
                            flame.Location = new Point(this.picbox.Location.X, this.picbox.Location.Y + a);
                            break;


                    }
                    foreach (PictureBox box in _form.pictureBoxes)
                    {
                        if (box.Location == flame.Location) didBomb = true;
                    }
                    if (didBomb) break;
                }
            }
        }
        public void DetonateOrDie()
        {
            //vezmu nejstarsi plameny a vyhodnotim

            foreach (PictureBox a in this._flames)
            {
                foreach (PictureBox b in _form.rBoxes)
                {
                    if (b.Location == a.Location)
                    {
                        _form.pictureBoxes.Remove(b);
                        _form.Controls.Remove(b);
                        _form.rBoxes.Remove(b);
                        b.Dispose();
                        break;
                    }
                }

                foreach (Ghost g in _form.ghosts)
                {
                    if(g.picbox.Location == a.Location && this._owner == "hero") ghostsToKill.Add(g);
                }

                foreach (Ghost g in ghostsToKill)
                {
                    _form.Controls.Remove(g.picbox);
                    g.picbox.Dispose();
                    _form.ghosts.Remove(g);
                    if (_form.ghosts.Count == 0) _form.YouWon();
                }

                    _form.bombs
                       .Where(w => !w.detonated)
                       .Where(oldBomb => oldBomb.picbox.Location == a.Location)
                       .ToList()
                       .ForEach(b => { b.DecreaseLifetime(b.Lifetime); b.detonated = true; });
                

                if (_form.IsHeroThere(a.Location)) _form.GameOver();
            }


        }
        private void EndBomb()
        {
            foreach (PictureBox oldFlame in this._flames)
            {
                _form.Controls.Remove(oldFlame);
                oldFlame.Dispose();
            }
            _form.Controls.Remove(this.picbox);
            _form.bombs.Remove(this);
        }

    }
}
