using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Kursach
{
    public class Agregation
    {
        ClassBox From { get; set; }
        ClassBox To { get; set; }

        public Agregation(ClassBox From, ClassBox To)
        {
            this.From = From;
            this.To = To;
        }

        public void draw (Graphics g)
        {
            Pen Black = new Pen(new SolidBrush(Color.Black),2);
            if ((From.X < To.X - From.Width))
            {
                g.DrawLine(Black, From.X + From.Width, From.Y + From.Height / 2, To.X, To.Y + To.Height / 2);
            }
            else if ((From.X + From.Width >= To.X) && (From.X <= To.X + To.Width))
                if (From.Y < To.Y)
                {
                    g.DrawLine(Black, From.X + From.Width / 2, From.Y + From.Height, To.X + To.Width / 2, To.Y);
                }
                else
                {
                    g.DrawLine(Black, From.X + From.Width / 2, From.Y, To.X + To.Width / 2, To.Y + From.Height);
                }
            else
            {
                g.DrawLine(Black, From.X, From.Y + From.Height / 2, To.X + To.Width, To.Y + To.Height / 2);
            }
        }
    }
}
