/***************************************************************************
 * city
 * class definitions for bomber class for the missile command game
 * Edwin Wong, Pic 10C, 5/23/11
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace misscom
{
    class Bomber
    {
        private PointF bomber_loc;

        public Bomber()
        {
            bomber_loc = new PointF(0,0);
        }
        /***********************************************************
         * bomber: constructs bomber location
         * Parameter: 
         *           PointF location: location of bomber
         ************************************************************/
        public Bomber(PointF location)
        {
            bomber_loc = location;
        }

        /***********************************************************
         * move: move bomber by increment
         ************************************************************/
        public void move(float increment)
        {
            bomber_loc.X += increment;
        }
        /***********************************************************
         * Draw: draw bomber at location with size of 40x30
         ************************************************************/
        public void Draw(Graphics g)
        {
            g.DrawImage(Properties.Resources.bomber, bomber_loc.X, bomber_loc.Y, 40, 30);
        }
        /**************************************************************************************
         * get_location: get location of bomber
         * Parameter:  hit_location: index that indicates which point on the bomber to return
         ************************************************************************************/
        public PointF get_location(int hit_location)
        {
            PointF fixed_location = new PointF(0,0);
            if (hit_location == 1)//hit the center of bomber so return center
            {
                fixed_location = new PointF(bomber_loc.X + 20, bomber_loc.Y + 15);

            }
            else if (hit_location == 0)//hit the back of bomber so return back point
            {
                fixed_location = new PointF(bomber_loc.X, bomber_loc.Y + 15);
            }
            else fixed_location = new PointF(bomber_loc.X + 40, bomber_loc.Y + 15);//hit front of bomber
            return fixed_location;
        }

    }
}
