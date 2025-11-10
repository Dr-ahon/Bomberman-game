using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//poradi funkci mozna trochu nedava smysl, snad se v tom vyznas

namespace bomberman
{


    public partial class Form1 : Form
    {

        //vsechny steny
        public List<PictureBox> pictureBoxes;
        //cihlove steny - znicitelne
        public List<PictureBox> rBoxes;
        //polozene bomby
        public List<Bomb> bombs;
        public List<Ghost> ghosts;
        private List<Bomb> _timerBombs;
        private Random _rand = new Random();
        private int _IDSetter;
        private int ghostNumber;


        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;
            this.AutoSize = true;
            bombs = new List<Bomb>();
            ghosts = new List<Ghost>();
            pictureBoxes = new List<PictureBox>();
            KeyUp += new KeyEventHandler(Form1_KeyUp);
            _timerBombs = new List<Bomb>();
            _IDSetter = 1;
            hero.BackgroundImage = Properties.Resources.robotHead;
            hero.BackgroundImageLayout = ImageLayout.Zoom;

        }
        
        public List<PictureBox> makeList()
        {
            // vsechny picboxy, co nejsou hrac (orange) tj. steny
            return this.Controls.OfType<PictureBox>().ToList().Where(a => a.Name != "hero").ToList();
        }
        public bool IsHeroThere(Point here)
        {
            return hero.Location == here;
        }
        private void NEWGAME_Click(object sender, EventArgs e)
        {
            if (!NEWGAME.Visible) return;
            System.Diagnostics.Process.Start(Application.ExecutablePath); // to start new instance of application
            this.Close();
        }
        private void START_MouseClick(object sender, MouseEventArgs e)
        {
            hero.Visible = true;
            PAUSE.Visible = true;
            bool canContinue = false;
            switch (GHOSTLIST.SelectedIndex)
            {
                case 0:
                    ghostNumber = 1;
                    canContinue = true;
                    break;
                case 1:
                    ghostNumber = 2;
                    canContinue = true;
                    break;
                case 2:
                    ghostNumber = 3;
                    canContinue = true;
                    break;
                case 3:
                    ghostNumber = 4;
                    canContinue = true;
                    break;
                case 4:
                    ghostNumber = 5;
                    canContinue = true;
                    break;
                default:
                    WARNING.Visible = true;
                    break;
            }
            if (canContinue)
            {
                START.Visible = false;
                BOMBERMAN.Visible = false;
                PLEASE.Visible = false;
                GHOSTLIST.Visible = false;
                WARNING.Visible = false;
                ghostGo_timer.Enabled = true;

                //ziskani pevnych zdi
                pictureBoxes = makeList();
                //vlozeni nahodnych odbombnutych zdi
                randomBoxes();
            }
        }
        private void randomBoxes()
        {
            rBoxes = new List<PictureBox>();
            bool ghostPlanted = false;

            for (int i = 1; i <= ghostNumber ; i++)
            {
                while (ghostPlanted != true)
                {
                    int x = _rand.Next(1, 14);
                    int y = _rand.Next(1, 11);

                    if ((x == 1 && y == 1) ||
                        (x == 1 && y == 2) ||
                        (x == 2 && y == 1)) continue;

                    if (isFree(new Point(x * 50, y * 50)))
                    {
                        Ghost newGhost = new Ghost(new Point(x * 50, y * 50), this);
                        ghostPlanted = true;
                        newGhost.ID = _IDSetter;
                    }
                }
                _IDSetter++;
                ghostPlanted = false;
            }


            for (int i = 0; i < 100; i++)
            {
                int x = _rand.Next(0, 15) * 50;
                int y = _rand.Next(0, 12) * 50;

                bool isOccupated = false;

                var loc = new Point(x, y);

                if (!isFree(loc)) continue;
                foreach (PictureBox box in pictureBoxes)
                {
                    if ((x == box.Location.X && y == box.Location.Y) /* || ( x == ghost.Location.X && y == ghost.Location.Y)*/ )
                    {
                        isOccupated = true;
                        break;
                    }
                }
                //pokud by se mela cihlova zed vytvorit na obsazenem miste, jdu na dalsi iteraci - vytvorim novou zed
                if (isOccupated == true) continue;

                PictureBox wall = new PictureBox
                {
                    BackgroundImageLayout = ImageLayout.Stretch,
                    Name = "rBox" + i.ToString(),
                    Location = new Point(x, y)
                };
                //zakazuji vytvorit steny na prvnich trech polich, aby se hras nemusel hned zabit
                if ((wall.Location.X == 50 && wall.Location.Y == 50) ||
                    (wall.Location.X == 50 && wall.Location.Y == 100) ||
                    (wall.Location.X == 100 && wall.Location.Y == 50)) continue;

                wall.Size = new Size(50, 50);
                wall.BackgroundImage = Properties.Resources.brick;
                //wall.SizeMode = PictureBoxSizeMode.StretchImage;
                wall.Visible = true;
                this.Controls.Add(wall);
                pictureBoxes.Add(wall);
                rBoxes.Add(wall);
            }

        }
        public bool isFree(Point loc)
        {
            bool free = true;

            foreach (var a in pictureBoxes)
            {
                if (a.Location == loc) free = false;
            }

            this.Text = hero.Location.ToString();

            if (free) return true;
            else return false;
            //kontrolni vypis
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (START.Visible) return;
            if (PAUSED.Visible) return;

            int x = hero.Location.X;
            int y = hero.Location.Y;

            if (e.KeyCode == Keys.Right) x += 50;
            else if (e.KeyCode == Keys.Left) x -= 50;
            else if (e.KeyCode == Keys.Up) y -= 50;
            else if (e.KeyCode == Keys.Down) y += 50;
            else if (e.KeyCode == Keys.Space)
            {
                if (!(bombs.Where(b => b._owner == "hero").ToList().Count <= 1)) return;
                
                 Bomb newBomb = new Bomb(hero.Location, this, "hero");

            }


            //kontrola, aby nesel do zdi
            var loc = new Point(x, y);
            if (isFree(loc)) hero.Location = loc;

           

        }
        private void KillingGhost()
        {
            foreach (Ghost g in ghosts)
            {
                if (hero.Location == g.picbox.Location) GameOver();
            }
        }
        private void GhostGo_timer_Tick(object sender, EventArgs e)
        {
            
            foreach (Ghost g in ghosts)
            {
                List<(int x, int y)> targets = new List<(int x, int y)>();
                targets.Add((hero.Location.X/50, hero.Location.Y/50));
                g.GhostGoMethod(targets);
            }
        }
        private void LifeTimer_Tick(object sender, EventArgs e)
        {
            KillingGhost();
            foreach (Bomb b in bombs)
            {
                b.DetonateOrDie();
            }
            foreach (Bomb b in bombs)
            {
                switch (b.Lifetime)
                {
                    case 0:
                        b.BombAction(0);
                        break;
                    case -1000:
                        _timerBombs.Add(b);
                        break;
                }
                b.DecreaseLifetime(100);
            }
            foreach(Bomb b in _timerBombs)
            {
                b.BombAction(-1);
            }
        }
        public void GameOver()
        {
            if (YOUWON.Visible) return;
            hero.Dispose();
            GAMEOVER.Visible = true;
            NEWGAME.Visible = true;
        }
        public void YouWon()
        {
            YOUWON.Visible = true;
            NEWGAME.Visible = true;

        }
       

        private void PAUSE_Click(object sender, EventArgs e)
        {
            if (BOMBERMAN.Visible) return;
            PAUSED.Visible = true;
            CONTINUE.Visible = true;
            NEWGAME.Visible = true;
            lifeTimer.Enabled = false;
            ghostGo_timer.Enabled = false;
            ENDGAME.Visible = true;
        }

        private void CONTINUE_Click(object sender, EventArgs e)
        {
            PAUSED.Visible = false;
            CONTINUE.Visible = false;
            NEWGAME.Visible = false;
            lifeTimer.Enabled = true;
            ghostGo_timer.Enabled = true;
            ENDGAME.Visible = false;
        }

        private void ENDGAME_Click(object sender, EventArgs e)
        {
            if (!ENDGAME.Visible) return;
            this.Close();
        }
    }






}


