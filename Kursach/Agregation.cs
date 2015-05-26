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
            int x1, y1, x2, y2;
            if (From.X+From.Width/2<To.X+To.Width/2)
            {
                x1 = From.X + From.Width+10;
                y1 = From.Y + From.Height / 2;
                x2 = To.X-20;
                y2 = To.Y + To.Height / 2;
                g.DrawLine(Black, x1-10, y1, x1, y1);
                g.DrawPolygon(Black, new Point[] { new Point(x2, y2), new Point(x2 + 10, y2 + 6), new Point(x2 + 20, y2), new Point(x2 + 10, y2 - 6) });
            }
            else
            {
                x1 = From.X-10;
                y1 = From.Y + From.Height / 2;
                x2 = To.X+To.Width+20;
                y2 = To.Y + To.Height / 2;
                g.DrawLine(Black, x1, y1, x1 + 10, y1);
                g.DrawPolygon(Black, new Point[] { new Point(x2, y2), new Point(x2 - 10, y2 + 6), new Point(x2 - 20, y2), new Point(x2 - 10, y2 - 6) });
            }

            g.DrawLine(Black, x1, y1, x2, y2); //Соединение
        }
    }
}
