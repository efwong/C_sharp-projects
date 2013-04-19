/******************************************************************************************
 * Shape.cs
 * Shape Class Declaration and Definition of member functions
 *  Includes: Rectangles, Lines, Ellipses
 * Edwin Wong, PIC 10C, 5/6/11
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
    public class Line10C : Shape10C
    {
        protected Point first_point;//initial point
        protected Point final_point;//final point
        protected Point one_point;//first increment point
        protected Point sec_point;//end of increment point
        protected int thickness;
        protected int length;//sum of lengths of second line


        /*******************************************************************************
         * Line10C(): Line10C default constructor
         ******************************************************************************/
        public Line10C()
        {
            shapeColor = Color.White;
            thickness = 1;
            length = 0;
            first_point = new Point(0, 0);
            final_point = new Point(0,0);
            one_point = new Point(0, 0);
            sec_point = new Point(0, 0);
        }
        /*******************************************************************************
         * Line10C(): Line10C sets color
         ******************************************************************************/
        public Line10C(Color c)
        {
            shapeColor = c;
            thickness = 1;
            length = 0;
            first_point = new Point(0, 0);
            final_point = new Point(0, 0);
            one_point = first_point;
            sec_point = new Point(0, 0);
        }

        /*******************************************************************************
        * Line10C(Color c, PointF left, PointF right): constructor that sets all variables
        * Parameters: Color c= color of rectangle
        *             PointF left= left point of line
        *             PointF right= right point of line
        *******************************************************************************/
        public Line10C(Color c, int thick, Point start, Point last)
        {
            thickness = thick;
            length = 0;
            shapeColor = c;
            first_point = start;
            final_point = last;
            one_point = start;
            sec_point = one_point;
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
            sec_point.X += (int)x_update;
            sec_point.Y += (int)y_update;
            //if (final_point.Y - first_point.Y <= 0)
            //{
                
            //}
            /*else { 
                sec_point.Y += y_update; 
            }*/
            //sec_point.Y = point_slope().X * sec_point.X + point_slope().Y;
        }

        /**************************************************************************************
        *point_slope(Line10C p): returns a PointF object containing the values a,b for y=ax+b;
        *                       note that in c sharp the y axis is inverted so *(-1) to results
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
            g.DrawLine(myPen, one_point, sec_point);
            one_point = sec_point;
        }


        /*********************************************************************************
         * IsPointInside(PointF p): checks if click is within a line
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
            //note there are other variants to the d equation that one might want to use
            //but if t fails, then d fails anyways so doesnt matter
            return (0 < t && t < 1 && d<6);
        }

        /********************************************************************************
        * MouseDownCreating(PointF p, Graphics g): prepares creation of line when mouse click is held down;
        *                   sets location of initial point
        *********************************************************************************/
        public override void MouseDownCreating(Point p, Graphics g)
        {
            isVisible = true;
            first_point = p;
        }
        /****************************************************************************************
        * MouseMoveCreating: constantly updates the "final" point;
        *****************************************************************************************/
        public override void MouseMoveCreating(Point p, Graphics g)
        {
            final_point = p;
        }
    }

    public class Ellipse10C : Shape10C
    {
        protected Point upperLeftCorner;
        protected int width;
        protected int height;


        /*******************************************************************************
        * Ellipse10C(): Ellipse10C default constructor
        ******************************************************************************/
        public Ellipse10C()
        {
            isVisible = false;
            shapeColor = Color.White;
            upperLeftCorner = new Point(0, 0);
            width = 0;
            height = 0;
        }
        /*******************************************************************************
         * Ellipse10C(Color c): constructor that sets only color
         ******************************************************************************/
        public Ellipse10C(Color c)
        {
            isVisible = false;
            shapeColor = c;
            upperLeftCorner = new Point(0, 0);
            width = 0;
            height = 0;
        }
        /**************************************************************************************
        * Ellipse10C(Color c, Point p, int w, int h): constructor that sets all parameters
        * Parameters: Color c= color of rectangle
        *             Point p= location up upperleftcorner
        *             int w = width of ellipse (x)axis
        *             int h = height of ellipse (y)axis
        ****************************************************************************************/
        public Ellipse10C(Color c, Point p, int w, int h)
        {
            isVisible = true;
            shapeColor = c;
            upperLeftCorner = p;
            width = w;
            height = h;
        }


        /**************************************************************************************
        * Move(int dx, int dy): updates upperLeftCorner location to allow for simulation of movement
        ****************************************************************************************/
        public override void Move(int dx, int dy)
        {
            upperLeftCorner.X += dx;
            upperLeftCorner.Y += dy;
        }

        /**************************************************************************************
        *Draw(Graphics g): draws a filled ellipse
        ****************************************************************************************/
        public override void Draw(Graphics g)
        {
            SolidBrush myBrush = new SolidBrush(shapeColor);
            //draw a filled ellipse with width=width of ellipse, height= height of ellipse
            g.FillEllipse(myBrush, upperLeftCorner.X, upperLeftCorner.Y, width, height);
        }

        /*******************************************************************************
         * IsPointInside: checks if the click is inside the ellipse
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


        /********************************************************************************
        * MouseDownCreating: prepares creation of ellipse when mouse click is held down;
        *                   sets location of upper left corner
        *********************************************************************************/
        public override void MouseDownCreating(Point p, Graphics g)
        {
            isVisible = true;
            upperLeftCorner = p;
        }

        /**********************************************************************************************************
        * MouseMoveCreating: constantly updates thea axes of the ellipse based on current mouse 
        *                          down location; allows for continuous expansion of ellipse
        **************************************************************************************************************/
        public override void MouseMoveCreating(Point p, Graphics g)
        {
            width = p.X - upperLeftCorner.X;
            height = p.Y - upperLeftCorner.Y;
        }



    }
}
