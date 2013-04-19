/******************************************************************************************
 * Shape.cs (modified for Missile Command)
 * Shape Class Declaration and Definition of member functions
 *  Includes: Rectangles10C, Lines10C  (missiles), Ellipses10C(explosions)
 * Edwin Wong, PIC 10C, 5/23/11
 * *****************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
//Extra libraries I added.
using System.Drawing;
using System.Drawing.Drawing2D;


namespace misscom
{
    /******************************************************/
    /* Shape10C -- General base class for PIC 10C shapes. */
    public abstract class Shape10C
    {
        protected Color shapeColor;
        public bool isVisible = false;  //Sometimes we want to hide shapes.
        
        public abstract void Move(int dx, int dy);
        public abstract void Draw(Graphics g);
        public abstract bool IsPointInside(Point p);
        public abstract void MouseDownCreating(Point p, Graphics g);
        public abstract void MouseMoveCreating(Point p, Graphics g);
        

        /*******************************************************************************
         * Get_color(): returns color of shape
         ******************************************************************************/
        public Color Get_color()
        {
            return shapeColor;
        }
        /*******************************************************************************
         * Set_color(): changes color of shape
         ******************************************************************************/
        public void Set_color(Color c)
        {
            shapeColor = c;
        }
    }  //end Shape10C class

    /*******************************************/
    /* Rectangle10C -- PIC 10C Rectangle class */
    /*      not used for the missile command game */
    public class Rectangle10C : Shape10C
    {
        protected PointF upperLeftCorner;
        protected float width;
        protected float height;


        /*******************************************************************************
         * Rectangle10C(): Rectangle10C default constructor
         ******************************************************************************/
        public Rectangle10C()
        {
            isVisible = false;
            shapeColor = Color.White;
            upperLeftCorner = new PointF(0, 0);
            width = 0;
            height = 0;
        }
        /*******************************************************************************
         * Rectangle10C(Color c): constructor that sets only color
         ******************************************************************************/
        public Rectangle10C(Color c)
        {
            isVisible = false;
            shapeColor = c;
            upperLeftCorner = new PointF(0, 0);
            width = 0;
            height = 0;
        }

        /***********************************************************************************
        * Rectangle10C(Color c, PointF p, int w, int h): constructor that sets all parameters
        *           Parameters: Color c= color of rectangle
        *                      PointF p= location up upperleftcorner
        *                      int w = width of rectangle
        *                      int h = height of rectangle
        ************************************************************************************/
        public Rectangle10C(Color c, PointF p, int w, int h)
        {
            isVisible = true;
            shapeColor = c;
            upperLeftCorner = p;
            width = w;
            height = h;
        }
        /***********************************************************************************************
        * Move(int dx, int dy): moves  the upperLeftCorner upon each draw/delete cycle 
        *                       (simulates rectangle movement)
        ************************************************************************************************/
        public override void Move(int dx, int dy) {
            upperLeftCorner.X += dx;
            upperLeftCorner.Y += dy;
        }
        /**********************************************
        * Draw(Graphics g): draws the rectangle
        ***********************************************/
        public override void Draw(Graphics g)
        {
            SolidBrush myBrush = new SolidBrush(shapeColor);
            g.FillRectangle(myBrush, upperLeftCorner.X, upperLeftCorner.Y, width, height);
        }
        /**************************************************************
        * IsPointInside(PointF p): checks if click is inside rectangle
        ***************************************************************/
        public override bool IsPointInside(Point p)
        {
            return (p.X >= upperLeftCorner.X && p.Y >= upperLeftCorner.Y
                && p.X <= upperLeftCorner.X + width && p.Y <= upperLeftCorner.Y + height);
        }

        /********************************************************************************
        * MouseDownCreating: prepares creation of rectangle when mouse click is held down;
        *                   sets location of upper left corner
        *********************************************************************************/
        public override void MouseDownCreating(Point p, Graphics g)
        {
            isVisible = true;
            upperLeftCorner = p;
        }
        /**********************************************************************************************
        * MouseMoveCreating: constantly updates width and height of rectangle based on current
        *                   mouse down location; allows for continuous simulated stretching of rectangle
        *********************************************************************************************/
        public override void MouseMoveCreating(Point p, Graphics g)
        {
            width = (float)(p.X - upperLeftCorner.X);
            height = (float)(p.Y - upperLeftCorner.Y);
        }
    } //end Rectangle10C class


    
    /*******************************************/
    /* Line10C -- PIC 10C Rectangle class */
    /*         Modified into a missile class*/
    public class Line10C : Shape10C
    {
        protected PointF first_point;//initial point (eg. from a base)
        protected PointF final_point;//final point (eg. user click)
        protected PointF one_point;//first increment point (incremental start location)
        protected PointF sec_point;//end of increment point (incremental end location)
        protected int thickness;//thickness of line
        protected int length;//sum of lengths of second line
        protected bool should_remove;//indicate line removal
        protected bool isCluster;//indicate cluster bomb


        /*******************************************************************************
         * Line10C(): Line10C default constructor
         ******************************************************************************/
        public Line10C()
        {
            shapeColor = Color.White;
            thickness = 1;
            length = 0;
            first_point = new PointF(0, 0);
            final_point = new PointF(0,0);
            one_point = new PointF(0, 0);
            sec_point = new PointF(0, 0);
            should_remove = false;
            isCluster = false;
            
        }
        /*******************************************************************************
         * Line10C(): Line10C sets color
         ******************************************************************************/
        public Line10C(Color c)
        {
            shapeColor = c;
            thickness = 1;
            length = 0;
            first_point = new PointF(0, 0);
            final_point = new PointF(0, 0);
            one_point = first_point;
            sec_point = new PointF(0, 0);
            should_remove = false;
            isCluster = false;
        }

        /*******************************************************************************
        * Line10C: constructor that sets all variables
        * Parameters: Color c= color of rectangle
        *             int thick = set thickness
        *             PointF start= start point of line
        *             PointF last= end point of line
        *******************************************************************************/
        public Line10C(Color c, int thick, PointF start, PointF last)
        {
            thickness = thick;
            length = 0;
            shapeColor = c;
            first_point = start;
            final_point = last;
            one_point = start;
            sec_point = one_point;
            should_remove = false;
            isCluster = false;
        }
        /*******************************************************************************
        * get_currentPoint: return current location
        *******************************************************************************/
        public PointF get_currentPoint()
        {
            return sec_point;
        }
        /*****************************************************************
         * set_removal: for use in a list of lines
         *              upon next update of list, the list will remove
         *              this line and stop it from being drawn
         * parameter: remove-> if true, this line will be removed from list
         ****************************************************************/
        public void set_removal(bool remove)
        {
            should_remove=remove;
        }

        /*****************************************************************
         * get_removal(): accessor for removal status
         ****************************************************************/
        public bool get_removal()
        {
            return should_remove;
        }
        /***************************************************************************
         * get_fire_point(): returns the original starting location of missile
         **************************************************************************/
        public PointF get_fire_point()
        {
            return first_point;//*note: missiles from center base moves faster
        }
        /***************************************************************************
         * get_final_point(): returns the point that the missile will end in/
         *                    is heading to
         **************************************************************************/
        public PointF get_final_point()
        {
            return final_point;
        }

        /***************************************************************************
         * set_cluster(): sets missile as a cluster bomb
         **************************************************************************/
        public void set_cluster(bool missile_status)
        {
            isCluster = missile_status;
        }
        /***************************************************************************
         * is_cluster(): gets cluster bomb status
         **************************************************************************/
        public bool is_cluster()
        {
            return isCluster;
        }

        /*******************************************************************************
        * Move(int dx, int dy): constructor that sets all variables
        ******************************************************************************/
        public override void Move(int dx, int dy)
        {
            one_point.X += dx;
            sec_point.Y += dy;
            one_point.X += dx;
            sec_point.Y += dy;
        }


        /****************************************************************************************
         * Move_pslope(float dx): moves the second increment point by dx and updates it's Y value
         *                      uses the principles of similar triangles to draw increments
         ***************************************************************************************/
        public void Move_pslope(float increment)
        {   //get length of triangle formed by click
            float d = (float)Math.Sqrt(Math.Pow((double)(final_point.X-first_point.X), 2.0) + 
                Math.Pow((double)(final_point.Y-first_point.Y), 2.0));
            //get the increments
            float x_update = ((final_point.X - first_point.X) * increment) / d;
            float y_update = ((final_point.Y-first_point.Y)/ d)*increment;
            length += (int)(0.5+Math.Sqrt(Math.Pow((double)x_update,2.0)+Math.Pow((double)y_update,2.0)));
            sec_point.X += x_update;
            sec_point.Y += y_update;
        }

        /****************************************************************************
         * atfinalpos(): bool that indicates where to stop missile movement animation
         ****************************************************************************/
        public bool atfinalpos()
        {
            if (final_point.X - first_point.X > 0)//the original distance was pos
            {
                if (final_point.X - sec_point.X < 0)//if the pos changes signs then final point reached
                {
                    return true;
                }
                else return false;
            }
            else if (final_point.X - first_point.X < 0)//original distance was negative
            {
                if (final_point.X - sec_point.X > 0)//sign changed
                {
                    return true;
                }
                else return false;

            }
            else {//for case when user fires at 90 angle w/o changing x
                if (final_point.Y - first_point.Y > 0)//y difference was pos
                {
                    if (final_point.Y - sec_point.Y < 0)//signs change so final point reached
                    {
                        return true;
                    }
                    else return false;
                }
                else//in case user fires below missile(not implemented)
                {
                    if (final_point.Y - sec_point.Y > 0)
                    {
                        return true;
                    }
                    else return false;
                }
            }
        }

        /**************************************************************************************
        *point_slope: (not implemented in code)
        *             returns a PointF object containing the values a,b for y=ax+b;
        *              note that in c# the y axis is inverted so *(-1) to results
        ****************************************************************************************/
        public PointF point_slope()
        {
            float slope = (-1*final_point.Y - -1*first_point.Y) / (final_point.X - first_point.X);//correct for inverted y
            float b = slope * first_point.X * -1 - first_point.Y;
            float a = slope;
            return new PointF(-1*a, -1*b);
        }

        /*******************************************************************************
         * Draw(Graphics g): draws the line
         ******************************************************************************/
        public override void Draw(Graphics g)
        {
            Pen myPen = new Pen(shapeColor,thickness);//set all line thickness to 12
            if (!atfinalpos())//missile not at final positoin
            {
                g.DrawLine(myPen, first_point, sec_point);
            }
            one_point = sec_point;//update location of missile
        }


        /*********************************************************************************
         * IsPointInside(Point p):(not used) checks if click is within a line
         *                  Uses the equation/suggestions given by Pic Prof
         *                  based on values of distance d from click to line and variable t
         ********************************************************************************/
        public override bool IsPointInside(Point p)
        {
            double t = ((p.X - first_point.X) * (final_point.X - first_point.X) +
                (p.Y - first_point.Y) * (final_point.Y - first_point.Y)) /
                (Math.Pow((final_point.X - first_point.X), 2.0) +
                Math.Pow((final_point.Y - first_point.Y), 2.0));
            Pen myPen = new Pen(shapeColor, 12);
            double d = Math.Sqrt(Math.Pow((p.X - first_point.X - t * final_point.X + t * first_point.X), 2.0)
                + Math.Pow((p.Y - first_point.Y - t * final_point.Y + t * first_point.Y), 2));
            return (0 < t && t < 1 && d<6);
        }

        /********************************************************************************
        * MouseDownCreating(PointF p, Graphics g): (not used)prepares creation of line when mouse click is held down;
        *                   sets location of initial point
        *********************************************************************************/
        public override void MouseDownCreating(Point p, Graphics g)
        {
            isVisible = true;
            first_point = p;
        }
        /****************************************************************************************
        * MouseMoveCreating: (not used)constantly updates the "final" point;
        *****************************************************************************************/
        public override void MouseMoveCreating(Point p, Graphics g)
        {
            final_point = p;
        }
    }
    /***********************************************
     * Ellipse10C: modified class for explosions
     ***********************************************/
    public class Ellipse10C : Shape10C
    {
        protected PointF upperLeftCorner;//upper left corner of explosion
        public bool keep_expand;//status for if explosion should keep expanding
        protected int max_size;//max explosion size
        protected int width;
        protected int height;


        /*******************************************************************************
        * Ellipse10C(): Ellipse10C default constructor
        ******************************************************************************/
        public Ellipse10C()
        {
            isVisible = false;
            shapeColor = Color.White;
            upperLeftCorner = new PointF(0, 0);
            width = 0;
            height = 0;
            max_size = 0;
        }
        /*******************************************************************************
         * Ellipse10C(Color c): constructor that sets only color
         ******************************************************************************/
        public Ellipse10C(Color c)
        {
            isVisible = false;
            shapeColor = c;
            upperLeftCorner = new PointF(0, 0);
            width = 0;
            height = 0;
            max_size = 0;
        }
        /**************************************************************************************
        * Ellipse10C: constructor that sets all parameters
        * Parameters: Color c= color of rectangle
        *             Point p= location up upperleftcorner
        *             int w = width of ellipse (x)axis
        *             int h = height of ellipse (y)axis
        *             int limit = size limit to explosion
        ****************************************************************************************/
        public Ellipse10C(Color c, PointF p, int w, int h, int limit)
        {
            isVisible = true;
            shapeColor = c;
            upperLeftCorner = p;
            width = w;
            height = h;
            max_size = limit;
            keep_expand = true;//explosion just made so keep expanding
        }


        /**************************************************************************************
        * Move(int dx, int dy):(not used) updates upperLeftCorner location to allow for simulation of movement
        ****************************************************************************************/
        public override void Move(int dx, int dy)
        {
            upperLeftCorner.X += (float)dx;
            upperLeftCorner.Y += (float)dy;
        }

        /**************************************************************************************************
        * expanding_circle: updates size of explosion
        ****************************************************************************************************/
        public void expanding_circle(float dx)
        {
            upperLeftCorner.X -= dx*(float).5;
            upperLeftCorner.Y -= dx*(float).5;
            width += (int)dx;
            height += (int)dx;
        }

        /**************************************************************************************************
        * contracting_circle: decrease size of circle
        ****************************************************************************************************/
        public void contracting_circle(float dx)
        {
            upperLeftCorner.X += dx * (float).5;
            upperLeftCorner.Y += dx * (float).5;
            width -= (int)dx;
            height -= (int)dx;
        }

        /**************************************************************************************************
        * reached_max_size: bool that returns wheter or not the max size of explosion was reached
        ****************************************************************************************************/
        public bool reached_max_size()
        {
            if (max_size - width < 0)
            {
                return true;
            }
            else return false;
        }

        /**************************************************************************************************
        * reached_min_size: bool that returns wheter or not the min size of explosion was reached
        ****************************************************************************************************/
        public bool reached_min_size()
        {
            if (width<=0)
            {
                return true;
            }
            else return false;
        }
        /**************************************************************************************************
       * set_color: sets color of explosion
       ****************************************************************************************************/
        public void set_color(Color a_color)
        {
            shapeColor = a_color;
        }
        /**************************************************************************************
        *Draw(Graphics g): draws a filled ellipse/explosion
        ****************************************************************************************/
        public override void Draw(Graphics g)
        {
            SolidBrush myBrush = new SolidBrush(shapeColor);
            g.FillEllipse(myBrush, upperLeftCorner.X, upperLeftCorner.Y, width, height);
            
        }

        /*******************************************************************************
         * IsPointInside: (not used)checks if the click is inside the ellipse
         *                based on ellipse equation 
         *                (center-X / semiXaxis)^2 + (center-y/semiXaxis)^2
         * Parameter = point p (user click)
         * returns bool (less than or equal to 1 if inside ellipse)
         * *****************************************************************************/
        public override bool IsPointInside(Point p)
        {
            double center_x = (2 * upperLeftCorner.X + width)/2;
            double center_y = (2 * upperLeftCorner.Y + height)/2;
            double location_inside = Math.Pow((p.X - center_x) / (0.5 * width), 2.0)
                + Math.Pow((p.Y - center_y) / (0.5 * height),2.0);
            //location inside is less than or equal to one if click is on the border or inside ellipse
            return (location_inside <= 1);
        }

        /*******************************************************************************
         * IsPointInside: (float version of above function) used for missile explosion
         * Parameter = point p (the location of the missile)
         * returns bool (less than or equal to 1 if inside ellipse)
         * *****************************************************************************/
        public bool IsPointFInside(PointF p)
        {
            double center_x = (2 * upperLeftCorner.X + width) / 2;
            double center_y = (2 * upperLeftCorner.Y + height) / 2;
            double location_inside = Math.Pow((p.X - center_x) / (0.5 * width), 2.0)
                + Math.Pow((p.Y - center_y) / (0.5 * height), 2.0);
            //location inside is less than or equal to one if click is on the border or inside ellipse
            return (location_inside <= 1);
        }


        /********************************************************************************
        * MouseDownCreating: (not used)prepares creation of ellipse when mouse click is held down;
        *                   sets location of upper left corner
        *********************************************************************************/
        public override void MouseDownCreating(Point p, Graphics g)
        {
            isVisible = true;
            upperLeftCorner = p;
        }

        /**********************************************************************************************************
        * MouseMoveCreating: (not used)constantly updates thea axes of the ellipse based on current mouse 
        *                          down location; allows for continuous expansion of ellipse
        **************************************************************************************************************/
        public override void MouseMoveCreating(Point p, Graphics g)
        {
            width = (int)(p.X - upperLeftCorner.X);
            height = (int)(p.Y - upperLeftCorner.Y);
        }

    }
}
