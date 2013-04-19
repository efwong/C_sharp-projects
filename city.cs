/***************************************************************************
 * city
 * class definitions for city class for the missile command game
 * Edwin Wong, Pic 10C, 5/23/11
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace misscom
{
    class City
    {
        private PointF[] city_point;//location of each city
        public bool[] city_disable;//are cities disabled
        private int width;
        private int height;
        
        public City()
        {
            city_point = new PointF[6];
            city_disable = new bool[6];
            width = 0;
            height = 0;
        }
        /*************************************************************
         * City: sets up city locations and default conditions
         *************************************************************/
        public City(PointF city1, PointF city2, PointF city3, PointF city4, PointF city5, PointF city6,
            int city_width, int city_height)
        {
            city_point = new PointF[6] { city1, city2, city3, city4, city5, city6 };//array of city locations
            width = city_width;
            height = city_height;
            city_disable = new bool[6];
            for (int i = 0; i < 6; i++)//cities arent disabled
            {
                city_disable[i] = false;
            }
        }
        /*************************************************************
         * Draw_cities: draw the cities if they arent disabled
         *************************************************************/
        public void Draw_cities(Graphics g)
        {
            for (int i = 0; i < 6; i++)
            {
                if (city_disable[i] == false)//not disabled so draw
                {
                    g.DrawImage(Properties.Resources.city, city_point[i].X, city_point[i].Y, width, height);
                }
            }
        }
        /*************************************************************
         * get_cities: return all city locations
         *************************************************************/
        public PointF[] get_cities()
        {
            return city_point;
        }
        /*************************************************************************
         * allcitydisabled: bool for all city status(eg. are they all disabled?)
         *************************************************************************/
        public bool allcitydisabled()
        {
            return (city_disable[0] && city_disable[1] && city_disable[2] && city_disable[3]
                && city_disable[4] && city_disable[5]);
        }
    }
}
