/***************************************************************************
 * Missile Command
 * The user clicks on the screen to fire missiles and prevent enemy missiles
 * from destroying the cities and bases.  The user loses when all of the cities
 * are destroyed.
 * 
 * Edwin Wong, Pic 10C, 5/23/11
 ***************************************************************************/



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WMPLib;
using System.Media;

namespace misscom
{
    public partial class mc_Form : Form
    {
        private Bitmap myCanvas;
        private Random rand1 = new Random();
        private PointF cursor_location;//current cursor location
        private Line10C current_missile_line = new Line10C();
        private Ellipse10C current_explosion = new Ellipse10C();
        private DoubleBufferPanel DrawingPanel = new DoubleBufferPanel();//double buffered panel
        private List<Line10C> missile_list = new List<Line10C>();//list of your missiles
        private List<Line10C> enemy_missile_list = new List<Line10C>();//list of enemy missiles
        private List<Ellipse10C> explosion_list = new List<Ellipse10C>();//list of explosions
        private List<Bomber> bomber_list = new List<Bomber>();//list of bombers
        private City six_cities = new City();//the cities
        private Base newBase = new Base();//the bases
        //counters & bools to keep track of game status
        private bool wait_for_level_over = false;
        private bool end_of_level = false;
        private bool first_load = true;//bool for first time loading game
        private int level =1;
        private int time_to_next_level = 0;
        private int wait_counter = 0;
        private int difficulty = 1;
        private bool newgame = false;
        //sounds
        //wmpLib is too slow for this game and
        //SoundPlayer can play async but that doenst mean simultaneous sounds  :(
        private SoundPlayer mis_player = new SoundPlayer(Properties.Resources.miss_fire);
        private SoundPlayer explode_player = new SoundPlayer(Properties.Resources.pop);

        public mc_Form()
        {
            InitializeComponent();
        }

        private void mc_Form_Load(object sender, EventArgs e)//load form & initialize values
        {
            if (first_load == true)//only want to add drawingpanel controls the first time
            {
                DrawingPanel.Size = new System.Drawing.Size(700, this.ClientRectangle.Height);
                DrawingPanel.Location = new System.Drawing.Point(0, 0);
                DrawingPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawingPanel_Paint);
                DrawingPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(DrawingPanel_MouseClick);
                DrawingPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(DrawingPanel_MouseMove);
                DrawingPanel.MouseLeave += new System.EventHandler(DrawingPanel_MouseLeave);
                DrawingPanel.MouseEnter += new System.EventHandler(DrawingPanel_MouseEnter);
                DrawingPanel.Parent = this;
                this.Controls.Add(DrawingPanel);
            }
            //load sounds
            mis_player.LoadAsync();
            explode_player.LoadAsync();

            myCanvas = new Bitmap(DrawingPanel.Width,
               DrawingPanel.Height,
               System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(myCanvas);
            g.Clear(Color.Black);
            //initialize base & city locations
            newBase = new Base(DrawingPanel.Width, DrawingPanel.Height);
            PointF city1 = new PointF(100, DrawingPanel.Height - 65);
            PointF city2 = new PointF(170, DrawingPanel.Height - 62);
            PointF city3 = new PointF(240, DrawingPanel.Height - 58);
            PointF city4 = new PointF(387, DrawingPanel.Height - 65);
            PointF city5 = new PointF(470, DrawingPanel.Height - 71);
            PointF city6 = new PointF(545, DrawingPanel.Height - 63);
            six_cities = new City(city1, city2, city3, city4, city5, city6, 40, 20);
            
            //start timers
            glob_time.Start();
            missile_timer.Start();
            explosion_timer.Start();
        }

        private void DrawingPanel_Paint(object sender, PaintEventArgs e)//paint for panel
        {
            Graphics g = e.Graphics;
            g.DrawImage(myCanvas, 0, 0, myCanvas.Width, myCanvas.Height);
        }


        /**************************************************************************
         * glob_time_Tick: separate timer to set spawning of enemy missiles & bomber
         *                 also handles losing & winning conditions
         **************************************************************************/
        private void glob_time_Tick(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromImage(myCanvas);
            int rand_count = rand1.Next(1, 5);//random count of enemy missiles
            int rand_chance = 0;//enemy target locations
            int cluster_chance = 0;//chance that missile is a cluster bomb
            int rand_bomber = rand1.Next(0, 100);//chance that there's a bomber
            int rand_start = 0;//rand starting location
            PointF start_point = new PointF();
            List<PointF> enemy_order = new List<PointF>();
            enemy_order = generate_enemy_list();
            bool pop_up_once = false;//winning condition

            
            if (!six_cities.allcitydisabled())//cities still standing
            {   //condition for next level for when level has ended and all missiles reached destination
                if (time_to_next_level > difficulty && wait_for_level_over)
                {
                    time_to_next_level = 0;
                    level++;//update level
                    newgame = true;
                    g.Clear(Color.Black);
                    explosion_list.Clear();
                    enemy_missile_list.Clear();
                    bomber_list.Clear();
                    missile_list.Clear();
                    difficulty++;
                    pop_up_once = true;//for case when user goes over 100 levels
                }
                else if (time_to_next_level > difficulty) end_of_level = true; //level has ended
                else if (wait_for_level_over == false && time_to_next_level <= difficulty)//level not over
                {
                    for (int i = 0; i < rand_count; i++)//random spawns
                    {
                        rand_start = rand1.Next(50, DrawingPanel.Width - 50);
                        rand_chance = rand1.Next(0, 9);//9choices to for enemies to hit
                        cluster_chance = rand1.Next(0, 100);//chance that bomb is a cluster bomb
                        start_point = new PointF((float)rand_start, 0);//random starting location
                        Line10C enemy = new Line10C(Color.Red, 2, start_point, enemy_order[rand_chance]);
                        if (cluster_chance < 10)//10% chance that bomb is a cluster bomb
                        {
                            enemy.set_cluster(true);
                        }
                        enemy_missile_list.Add(enemy);
                    }
                    if (rand_bomber < 20)//add a bomber
                    {
                        bomber_list.Add(new Bomber(new PointF(-5, 100)));
                    }
                    time_to_next_level++;//one round of enemy spawn
                    newgame = false;
                }

            }
            else//player lost
            {
                LosingCondition(g, sender, e);
            }

            
            //winning conditions for players who go over level 100
            Font myfont = new Font("Courier New", 60, FontStyle.Bold, GraphicsUnit.Pixel);
            if (level > 100 && pop_up_once)
            {
                pop_up_once = false;
                glob_time.Stop();
                missile_timer.Stop();
                explosion_timer.Stop();
                g.Clear(Color.Red);
                g.DrawString("YOU WIN", myfont, new SolidBrush(Color.Blue), DrawingPanel.Width / 2 - 160, DrawingPanel.Height / 2 - 50);
                DrawingPanel.Invalidate();
                DialogResult you_win;
                you_win = MessageBox.Show("Would you like to play another game?", "You Win",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (you_win == DialogResult.Yes)//start new game
                {//clear lists and restart counters
                    explosion_list.Clear();
                    enemy_missile_list.Clear();
                    bomber_list.Clear();
                    missile_list.Clear();
                    wait_for_level_over = false;
                    end_of_level = false;
                    newgame = false;
                    wait_counter = 0;
                    time_to_next_level = 0;
                    difficulty = 1;
                    mc_Form_Load(sender, e);                    
                }
                else this.Close();
            }

            enemy_order.Clear();
            glob_time.Interval = 5000;

        }
        /******************************************************************
         * LosingCondition: player has lost... refresh lists & stop timers
         ******************************************************************/
        private void LosingCondition(Graphics g, object sender, EventArgs e)
        {
            Font myfont = new Font("Courier New", 100, FontStyle.Bold, GraphicsUnit.Pixel);
            g.Clear(Color.Red);
            glob_time.Stop();
            missile_timer.Stop();
            explosion_timer.Stop();
            explosion_list.Clear();
            enemy_missile_list.Clear();
            bomber_list.Clear();
            missile_list.Clear();
            wait_for_level_over = false;
            end_of_level = false;
            newgame = false;
            wait_counter = 0;
            time_to_next_level = 0;
            difficulty = 1;
            g.DrawString("THE END", myfont, new SolidBrush(Color.Blue), DrawingPanel.Width / 2 - 220, DrawingPanel.Height / 2 - 50);
            DrawingPanel.Invalidate();
            DialogResult you_must_lose;
            you_must_lose = MessageBox.Show("Would you like to play another game?", "No One Wins",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (you_must_lose == DialogResult.Yes)//start new game
            {
                mc_Form_Load(sender, e);
                DrawingPanel.Invalidate();
            }
            else this.Close();
        }
        /****************************************************************************************
         * generate_enemy_list(): generate a list of enemy locations
         ****************************************************************************************/
        public List<PointF> generate_enemy_list()
        {
            List<PointF> enemy_order = new List<PointF>();

            for (int i = 0; i < 7; i++)
            {
                //add the cities to the enemy's targets
                if (i < 6) enemy_order.Add(new PointF(six_cities.get_cities()[i].X + 20, six_cities.get_cities()[i].Y + 10));
                else//add the bases to the enemy's targets
                {
                    enemy_order.Add(newBase.get_base_point(0));
                    enemy_order.Add(newBase.get_base_point(1));
                    enemy_order.Add(newBase.get_base_point(2));
                }
            }
            return enemy_order;
        }

        /************************************************************************************
         * missile_timer_Tick: timer for animating missile movements and bomber movements
         ************************************************************************************/
        private void missile_timer_Tick(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromImage(myCanvas);
            if (newgame)//game has ended
            {
                wait_counter++;
                if (wait_counter < missile_timer.Interval*2)
                {//dislpay text
                    Draw_Funct();
                    Draw_level_text(g,sender,e);
                }
                else
                {//renew variables
                    time_to_next_level = 0;
                    newBase.reset_missiles();//restock missiles
                    //beginning of new level so set all checks below to false
                    newgame = false;
                    end_of_level = false;
                    wait_for_level_over = false;
                    wait_counter = 0;
                }
            }
            
            int explode_count = explosion_list.Count;
            List<PointF> enemy_location = new List<PointF>();
            List<PointF> enemy_bomb_targets = new List<PointF>();
            int rand_location = 0;
            int rand_missile_bomber = 0;
            //enemy bomber
            for (int i = 0; i < bomber_list.Count; i++)
            {
                if (bomber_list[i].get_location(1).X > DrawingPanel.Width+40)//bomber has left screen
                {
                    bomber_list.RemoveAt(i);
                }
                else//bomber still in screen
                {
                    int rand_bomb_location = rand1.Next(0, 6);
                    rand_missile_bomber = rand1.Next(0, 200);
                    //move & draw bomb
                    bomber_list[i].move(2);
                    bomber_list[i].Draw(g);
                    enemy_bomb_targets = generate_enemy_list();
                    if (rand_missile_bomber == 0)//low chance that bomber will fire(0/200)
                    {
                        enemy_missile_list.Add(new Line10C(Color.Red, 2, bomber_list[i].get_location(1),
                            enemy_bomb_targets[rand_bomb_location]));
                    }
                    for (int a = 0; a < explosion_list.Count; a++)//check if bomber in explosion
                    {
                        if (explosion_list[a].IsPointFInside(bomber_list[i].get_location(0)) ||
                            explosion_list[a].IsPointFInside(bomber_list[i].get_location(1)) ||
                            explosion_list[a].IsPointFInside(bomber_list[i].get_location(2)))
                        {//add explosion and remove bomber
                            explosion_list.Add(new Ellipse10C(Color.Red, bomber_list[i].get_location(1), 0, 0, 60));
                            bomber_list.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            //enemy missiles
            for (int i = 0; i < enemy_missile_list.Count; i++)
            {
                //missile not at final pos & did not encounter an explosion
                if (!enemy_missile_list[i].atfinalpos() && !enemy_missile_list[i].get_removal())
                {
                    //draw & move missiles
                    enemy_missile_list[i].Move_pslope(1);
                    enemy_missile_list[i].Draw(g);

                    //draw missile tip
                    g.FillEllipse(new SolidBrush(Color.White),
                    enemy_missile_list[i].get_currentPoint().X - 1, enemy_missile_list[i].get_currentPoint().Y - 1, 2, 2);

                    //cluster bomb... so remove original missile and add 3 missiles in place
                    if (enemy_missile_list[i].is_cluster() &&
                        enemy_missile_list[i].get_currentPoint().Y < DrawingPanel.Height / 2 + 100
                        && enemy_missile_list[i].get_currentPoint().Y > 200)
                    {
                        enemy_location = generate_enemy_list();//generate enemy list

                        //get random targets
                        for (int m = 0; m < 3; m++)
                        {
                            rand_location = rand1.Next(0, 9);
                            enemy_missile_list.Add(new Line10C(Color.Red, 2, enemy_missile_list[i].get_currentPoint(),
                                enemy_location[rand_location]));
                        }
                        enemy_missile_list.RemoveAt(i);
                    }
                }
                //enemy missile encountered explosion
                else if (enemy_missile_list[i].get_removal())
                {
                    enemy_missile_list.RemoveAt(i);
                }
                else//enemy at final position city/base
                {
                    bool hit_city = false;//speed up runtime by not running both city and base loops
                    for (int a = 0; a < six_cities.get_cities().Length; a++)
                    {
                        //did it hit a city
                        if (enemy_missile_list[i].get_final_point().X == six_cities.get_cities()[a].X + 20 &&
                            enemy_missile_list[i].get_final_point().Y == six_cities.get_cities()[a].Y + 10)
                        {
                            //disable city
                            six_cities.city_disable[a] = true;
                            hit_city = true;
                        }
                    }

                    if (hit_city != true)//if it didnt hit city, then it hit base
                    {
                        for (int b = 0; b < 3; b++)
                        {
                            if (enemy_missile_list[i].get_final_point() == newBase.get_base_point(b))
                            {
                                newBase.base_disable[b] = true;//base is disabled
                            }
                        }
                    }
                    //generate explosion
                    current_explosion = new Ellipse10C(Color.Red, enemy_missile_list[i].get_final_point(), 0, 0, 60);
                    explosion_list.Add(current_explosion);
                    enemy_missile_list.RemoveAt(i);
                    explode_player.Play();
                }
            }

            //your missiles
            for (int i = 0; i < missile_list.Count; i++)
            {
                if (!missile_list[i].atfinalpos())//missile not at final pos
                {
                    //missile from middle base moves faster
                    if (missile_list[i].get_fire_point() == newBase.get_base_point(1))
                    {
                        missile_list[i].Move_pslope(15);
                    }
                    //else if (missile_list[i].is_enemy()) missile_list[i].Move_pslope(1);
                    else missile_list[i].Move_pslope(8);
                    //draw missile and missile tip
                    missile_list[i].Draw(g);
                    g.FillEllipse(new SolidBrush(Color.White),
                    missile_list[i].get_currentPoint().X - 1, missile_list[i].get_currentPoint().Y - 1, 2, 2);
                }
                else//missile at final pos then make explosion
                {
                    current_explosion = new Ellipse10C(Color.Red, missile_list[i].get_final_point(), 0, 0, 60);
                    explosion_list.Add(current_explosion);
                    missile_list.RemoveAt(i);
                    explosion_timer.Start();
                    explode_player.Play();
                }

            }

            extra_explosion();//draw the chain explosions for when missiles encounter explosions
            DrawingPanel.Invalidate();
            //if no more missiles and it's the end of the level so initiate check to transfer to next level
            if (enemy_missile_list.Count == 0 && bomber_list.Count == 0 && end_of_level==true)
            {
                wait_for_level_over = true;
            }

        }
        /***********************************************************************************
         * explosion_timer_Tick: allows for explosion animation (but it rlly just increments 
         *                      explosion sizes)
         ***********************************************************************************/
        private void explosion_timer_Tick(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromImage(myCanvas);
            Draw_Funct();
            for (int i = 0; i < explosion_list.Count; i++)
            {
                //expand explosion if not at max size
                if (!explosion_list[i].reached_max_size() && explosion_list[i].keep_expand)
                {
                    explosion_list[i].expanding_circle(4);
                }
                else
                //contract explosion
                {
                    //contract the circle
                    if (!explosion_list[i].reached_min_size())
                    {
                        //g.Clear(Color.Black);
                        explosion_list[i].contracting_circle(4);
                        explosion_list[i].keep_expand = false;
                    }
                    if (explosion_list[i].reached_min_size())//explosion is gone
                    {
                        explosion_list.RemoveAt(i);
                    }
                }
            }
        }

        /**********************************************************************
         * Draw_Funct(): Draw extras that need to be on screen; 
         *              cities, background, explosions, base stock of missiles
         *              cursor
         **********************************************************************/
        private void Draw_Funct()
        {
            Graphics g = Graphics.FromImage(myCanvas);
            Font myfont = new Font("Courier New", 18, FontStyle.Bold, GraphicsUnit.Pixel);
            g.Clear(Color.Black);
            //draw the background & cities
            g.DrawImage(Properties.Resources.land2, 0, DrawingPanel.Height - 80, DrawingPanel.Width, 80);
            six_cities.Draw_cities(g);

            //draw animated explosion
            int[] color_array ={ rand1.Next(0, 255), rand1.Next(0, 255), rand1.Next(0, 255) };
            for (int i = 0; i < explosion_list.Count; i++)
            {
                explosion_list[i].set_color(Color.FromArgb(255, color_array[1], 0));
                explosion_list[i].Draw(g);
            }
            Draw_MissileCount();//draw missile count underneath bases
            for (int i = 0; i < 3; i++)
            {
                if (newBase.base_disable[i] == true)
                {
                    g.DrawString("OUT", myfont, new SolidBrush(Color.Blue), newBase.get_base_point(i).X - 20, newBase.get_base_point(i).Y + 40);
                }
            }
            //draw cursor/targets
            Pen myPen = new Pen(Color.Blue, 1);
            g.DrawLine(myPen, cursor_location.X - 5, cursor_location.Y - 5, cursor_location.X + 5, cursor_location.Y + 5);
            g.DrawLine(myPen, cursor_location.X - 5, cursor_location.Y + 5, cursor_location.X + 5, cursor_location.Y - 5);
        }
        /******************************************************************************
         * Draw_MissileCount: draw the missiles underneath each base
         *****************************************************************************/
        private void Draw_MissileCount()
        {
            Graphics g = Graphics.FromImage(myCanvas);
            float base_x = 0;
            float base_y = 0;
            int miss_count = 0;
            for (int k = 0; k < 3; k++)//draw missiles underneath bases
            {
                //set up variables for each base
                int increment = 0;
                if (k == 0)//first base
                {
                    base_x = newBase.get_base_point(0).X + 30;
                    base_y = newBase.get_base_point(0).Y - 30;
                    miss_count = newBase.base_missiles[0];
                    if (miss_count == 0)
                    {
                        newBase.base_disable[0] = true;
                    }
                }
                else if (k == 1)//2nd base
                {
                    base_x = newBase.get_base_point(1).X + 30;
                    base_y = newBase.get_base_point(1).Y - 30;
                    miss_count = newBase.base_missiles[1];
                    if (miss_count == 0) newBase.base_disable[1] = true;
                }
                else//third base
                {
                    base_x = newBase.get_base_point(2).X + 30;
                    base_y = newBase.get_base_point(2).Y - 30;
                    miss_count = newBase.base_missiles[2];
                    if (miss_count == 0) newBase.base_disable[2] = true;
                }
                //draw missiles for each base
                for (int i = 0; i < miss_count; i++)//only draw till the # of indicated missiles
                {
                    if (i == 0)//first row
                    {
                        g.DrawImage(Properties.Resources.missile_pic, base_x - 30, base_y + 30, 10, 13);
                    }
                    else if (i > 0 && i < 3)//2nd row of missiles
                    {
                        if (i == 2) increment = 20;
                        g.DrawImage(Properties.Resources.missile_pic, base_x - 40 + increment, base_y + 35, 10, 13);
                        increment = 0;
                    }
                    else if (i >= 3 && i < 6)//3rd row of missiles
                    {
                        if (i > 3) increment += 18;
                        g.DrawImage(Properties.Resources.missile_pic, base_x - 49 + increment, base_y + 43, 10, 13);

                    }
                    else//4th row of missiles
                    {
                        if (i == 6) increment = 0;
                        if (i > 6) increment += 18;
                        g.DrawImage(Properties.Resources.missile_pic, base_x - 58 + increment, base_y + 50, 10, 13);
                    }
                }
            }
        }
        /**********************************************************************************
         * extra_explosion: draws chain explosions from enemy missiles
         * ********************************************************************************/
        private void extra_explosion()
        {
            int enemy_num = enemy_missile_list.Count;
            int explode_num = explosion_list.Count;
            for (int i = 0; i < enemy_missile_list.Count; i++)
            {
                for (int a = 0; a < explosion_list.Count; a++)
                {//enemy missile encounters explosion
                    if (enemy_missile_list.Count >= 0 && explosion_list[a].IsPointFInside(enemy_missile_list[i].get_currentPoint()))
                    {
                        explosion_list.Add(new Ellipse10C(Color.Red, enemy_missile_list[i].get_currentPoint(), 0, 0, 60));
                        enemy_missile_list[i].set_removal(true);
                        explode_player.Play();
                    }
                }
            }
        }

        /********************************************************************
         * DrawingPanel_MouseClick: sets missile destination from user click
         **********************************************************************/
        private void DrawingPanel_MouseClick(object sender, MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(myCanvas);
            if (e.Y < newBase.get_base_height())//missile cannot fire below base
            {
                //create line from the appropriate starting point to click location
                PointF fire_point = newBase.get_closest_point(e.Location);
                current_missile_line = new Line10C(Color.Blue, 2, fire_point, e.Location);
                for (int i = 0; i < 3; i++)
                {
                    if (fire_point == newBase.get_base_point(i) && newBase.base_disable[i] == false)
                    {
                        missile_list.Add(current_missile_line);
                        newBase.minus_base_missiles(i, 1);
                        mis_player.Play();
                    }
                }
            }
        }

        /*****************************************************************
         * DrawingPanel_MouseMove: draw cursor location upon mouse movement
         ******************************************************************/
        private void DrawingPanel_MouseMove(object sender, MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(myCanvas);
            Pen myPen = new Pen(Color.Blue,1);
            cursor_location = e.Location;
            g.DrawLine(myPen, e.Location.X - 5, e.Location.Y - 5, e.Location.X + 5, e.Location.Y + 5);
            g.DrawLine(myPen, e.Location.X - 5, e.Location.Y + 5, e.Location.X + 5, e.Location.Y - 5);
        }
        /*****************************************************************
         * DrawingPanel_MouseEnter: hide cursor if mouse enters panel
         ******************************************************************/
        private void DrawingPanel_MouseEnter(object sender, EventArgs e)
        {
            Cursor.Hide();
        }
        /*****************************************************************
         * DrawingPanel_MouseLeave: shows cursor if mouse leaves panel
         ******************************************************************/
        private void DrawingPanel_MouseLeave(object sender, EventArgs e)
        {
            Cursor.Show();
        }
        /*****************************************************************
         * Draw_level_text: draw large text on screen indicating level
         ******************************************************************/
        private void Draw_level_text(Graphics g,object sender, EventArgs e)
        {
            Font myfont = new Font("Courier New", 60, FontStyle.Bold, GraphicsUnit.Pixel);
            string current_level = "Level " + level.ToString();
            g.DrawString(current_level, myfont, new SolidBrush(Color.Blue), DrawingPanel.Width / 2 - 160, DrawingPanel.Height / 2 - 50);
        }
        /*****************************************************************
        * quitToolStripMenuItem_Click: user quits
        ******************************************************************/
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /*****************************************************************
        * quitToolStripMenuItem_Click: user wants new game
        ******************************************************************/
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromImage(myCanvas);
            g.Clear(Color.Black);
            glob_time.Stop();
            missile_timer.Stop();
            explosion_timer.Stop();
            explosion_list.Clear();
            enemy_missile_list.Clear();
            bomber_list.Clear();
            missile_list.Clear();
            wait_for_level_over = false;
            end_of_level = false;
            newgame = false;
            wait_counter = 0;
            time_to_next_level = 0;
            difficulty = 1;
            mc_Form_Load(sender, e);
            DrawingPanel.Invalidate();
        }
        /*****************************************************************
        * aboutToolStripMenuItem_Click: user checks about screen/info
        ******************************************************************/
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            miss_about about = new miss_about();
            about.Show();
        }
    }

    /***************************************************
     * DoubleBufferPanel: double buffers the panel
     ***************************************************/
    public class DoubleBufferPanel : Panel
    {

        public DoubleBufferPanel()
        {
            // Set the value of the double-buffering style bits to true.
            // ControlStyles.UserPaint -- allows user to control painting w/o passing off the work to the operating system
            //ControlStyles.AllPaintingInWmPaint--optimize to reduce flicker but only use it if ControlStyles.UserPaint is true
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint |
             ControlStyles.AllPaintingInWmPaint, true);// | evaluates all conditions even if condition 1 is true
            this.UpdateStyles();//forces style to be applied
        }
    }

}