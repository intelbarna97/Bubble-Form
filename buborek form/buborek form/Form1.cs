using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace buborek_form
{
    public partial class Form1 : Form
    {

        readonly static int RUN_TIME = 299;  // egy buborék kifestés kb 17millisecundum, ezért 17 millisecundumra sleepelem, mivel 588*17=9996 ami kb 10 sec
        readonly static int SLEEP_TIME = 17;
        Graphics g;
        Random r = new Random();
        public Form1()
        {
            InitializeComponent();

            g = pictureBox1.CreateGraphics();

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //kattintásra indul a buborék
            Thread t = new Thread(() => BubbleFirst(e));
            t.Start();
        }

        private void BubbleFirst(MouseEventArgs e)
        {
            int diameter = r.Next(50, 120);
            Rectangle rect = new Rectangle(e.X, e.Y, diameter, diameter);
            //a buborék "sebességét" és irányát egy pontban tárolom el
            Point velocity = new Point(r.Next(-5, 5), r.Next(-5, 5));
            Bubble(rect, 0, velocity);
        }

        private bool BubbleEND(Rectangle rectangle, int count)
        {
            //a falhoz érés vizsgálata
            if(count > RUN_TIME || (rectangle.X<=0 || (rectangle.X+rectangle.Width>=pictureBox1.Width) || rectangle.Y<=0 || (rectangle.Y+rectangle.Height>=pictureBox1.Height)))
            {
                return true;
            }

            return false;
        }

        private void Bubble(Rectangle rectangle, int count, Point velocity)
        {
            lock (g)
            {
                g.DrawEllipse(new Pen(Color.FromArgb(r.Next(256), r.Next(256), r.Next(256)), 2), rectangle);
            }

            Thread.Sleep(SLEEP_TIME);

            if(BubbleEND(rectangle,count))
            {
                lock (g)
                {
                    g.DrawEllipse(new Pen(Form1.DefaultBackColor, 2), rectangle);
                }
                //itt pukkad ki, sok kis bubira
                for (int i = 0; i < r.Next(8, 20); i++) {
                    Thread t = new Thread(
                        () =>
                        {
                            int a = r.Next(8, rectangle.Width/2);
                            Rectangle r2 = new Rectangle(
                                r.Next(rectangle.X, rectangle.Width+rectangle.X),
                                r.Next(rectangle.Y, rectangle.Height+rectangle.Y),
                                a,
                                a
                                );

                            BubbleBurst(r2);
                        }

                        );
                    t.Start();
                }
                return;
            }

            lock (g)
            {
                g.DrawEllipse(new Pen(Form1.DefaultBackColor, 2), rectangle);
            }
            rectangle.X += velocity.X;
            rectangle.Y += velocity.Y;

            count++;
            //rekurzió
            Bubble(rectangle, count, velocity);


        }

        //pukkadás animáció
        private void BubbleBurst(Rectangle rectangle)
        {
            lock (g)
            {
                g.DrawEllipse(new Pen(Color.FromArgb(r.Next(256), r.Next(256), r.Next(256)), 2), rectangle);
            }            


            Thread.Sleep(SLEEP_TIME);



            lock (g)
            {
                g.DrawEllipse(new Pen(Form1.DefaultBackColor, 2), rectangle);
            }

            if(rectangle.Width<=1)
            {
                return;
            }

            //amíg össze nem megy 1 pixelnyire

            rectangle.Width -= 2;
            rectangle.Height -= 2;
            rectangle.X++;
            rectangle.Y++;
            //rekurzív pukkanás
            BubbleBurst(rectangle);

        }

        
    }
}
