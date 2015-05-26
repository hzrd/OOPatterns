using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Kursach
{
    public abstract class Item
    {
        public int X { set; get; }
        public int Y { set; get; }
        public int Width { set; get; }
        public int Height { set; get; }

        public Pen Border, Select, black;
        public SolidBrush Background;

        public string Name { set; get; }

        public bool isSelected { set; get; }

        public abstract void draw(Graphics g);
    }
}
