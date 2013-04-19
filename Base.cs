/***************************************************************************
 * Base
 * class definitions for base class for the missile command game
 * Edwin Wong, Pic 10C, 5/23/11
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace misscom
{
    class Base
    {
        private PointF Base1;//location to draw base
        private PointF Base2;
        private PointF Base3;
        private PointF fire1;//location where missile fires from
        private PointF fire2;
        private PointF fire3;
        public bool[] base_disable;//is base disabled
        public int[] base_missiles;//# of missiles remaining
        private float height;//height of base

        public Base()
        {
            Base1 = new PointF(0, 0);
            Base2 = new PointF(0, 0);
            Base3 = new PointF(0, 0);
            fire1 = new PointF(0, 0);
            fire2 = new PointF(0, 0);
            fire3 = new PointF(0, 0);
            height = 0;
            base_missiles = new int[3];
            base_disable = new bool[3];
        }

        /*************************************************************
         * Base: sets up base location and firing positions
         *************************************************************/
        public Base(int screen_w, int screen_h)
        {
            Base1 = new PointF((float)(5), (float)(screen_h - 73));
            Base2 = new PointF((float)((screen_w / (float)2.0)-65), (float)(screen_h - 73));
            Base3 = new PointF((float)(screen_w-93), (float)(screen_h - 73));
            fire1 = new PointF((float)(Base1.X + 50), Base1.Y);
            fire2 = new PointF((float)(Base2.X + 50), Base2.Y);
            fire3 = new PointF((float)((Base3.X + 50)), Base3.Y);
            height = Base3.Y;
            base_disable = new bool[3];
            base_missiles = new int[3];
            for (int i = 0; i < 3; i++)
            {//bases arent disabled and each base has 10 missiles
                base_disable[i] = false;
                base_missiles[i] = 10;
            }
        }
        /**********************************************************************
         * minus_base_missiles: increment missile count(eg. subtract when fired)
         **********************************************************************/
        public void minus_base_missiles(int base_index,int increment)
        {
            base_missiles[base_index] -= increment;
        }

        /*************************************************************
         * reset_missiles: reset missile count
         *************************************************************/
        public void reset_missiles()
        {
            for (int i = 0; i < 3; i++)
            {
                base_missiles[i] = 10;
                base_disable[i] = false;
            }

        }

        /*************************************************************
         * get_base_point: return the firing positions of the bases
         *************************************************************/
        public PointF get_base_point(int index)
        {
            if (index == 0) return fire1;

            else if (index == 1) return fire2;
            else return fire3;
        }

        /*************************************************************
         * get_base_height: return the height of the base on screen
         *************************************************************/
        public float get_base_height()
        {
            return height;
        }


        /*******************************************************************************
         *get_closest_point: returns closest fire point(btwn each base) to given pointF p
         *                   use alternative bases if closest base is disabled
         * Parameter: Point p: user click location
         * ****************************************************************************/
        public PointF get_closest_point(Point p)
        {
            //point p is closest to base 1
            if (Math.Abs(p.X - fire1.X) < Math.Abs(p.X - fire2.X) && 
                Math.Abs(p.X - fire1.X) < Math.Abs(p.X - fire3.X))
            {
                if (base_disable[0] == false) return fire1;//base 1 isnt disabled
                else if (base_disable[1] == false) return fire2;//base2 isnt disabled
                else return fire3;
            }
            //p is closest to base 2
            else if (Math.Abs(p.X - fire2.X) < Math.Abs(p.X - fire1.X) &&
                Math.Abs(p.X - fire2.X) < Math.Abs(p.X - fire3.X))
            {//is the base disabled?
                if (base_disable[1] == false) return fire2;
                else if ((base_disable[0] == false &&
                    Math.Abs(p.X - fire1.X)<Math.Abs(p.X - fire3.X))| 
                    base_disable[2]==true) return fire1;
                else return fire3;
            }
            else//p is closest to base 3
            {//is the base disabled?
                if (base_disable[2] == false) return fire3;
                else if (base_disable[1] == false) return fire2;
                else return fire1;
            }

        }
    }
}
