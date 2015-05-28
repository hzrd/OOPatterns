using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Kursach
{
    public class ClassBox 
    {
        public int X { set; get; }
        public int Y { set; get; }
        public int Width { set; get; }
        public int Height { set; get; }

        Pen Border, Select, black;
        SolidBrush Background;

        public string Name { set; get; }
        public List<C_Variables> Variables = new List<C_Variables>();
        public List<C_Methods> Methods = new List<C_Methods>();
        public List<C_Methods> VirtualMethods = new List<C_Methods>();
        public static int Count = 0;

        public List<ClassBox> AgregatedClasses = new List<ClassBox>();
        public List<ClassBox> CompositedClasses = new List<ClassBox>();
        public List<ClassBox> ParentClasses = new List<ClassBox>();

        public bool isSelected { set; get; }
        //public bool isAgregated { set; get; }

        public ClassBox()
        {
            Count++;
            Border = new Pen(new SolidBrush(Color.Black), 2);
            Select = new Pen(new SolidBrush(Color.LightGreen), 2);
            black = new Pen(new SolidBrush(Color.Black));
            Background = new SolidBrush(Color.LightBlue);
            //isAgregated = false;
            isSelected = false;
        }

        public ClassBox(string _name)
        {
            Name = _name;
            Count++;
            Border = new Pen(new SolidBrush(Color.Black), 2);
            Select = new Pen(new SolidBrush(Color.LightGreen), 2);
            black = new Pen(new SolidBrush(Color.Black));
            Background = new SolidBrush(Color.LightBlue);
            //isAgregated = false;
            isSelected = false;
        }

        public ClassBox(int PosX, int PosY, int Width, int Height)
        {
            TestValues(); //Тестовые значения при инициализации
            X = PosX;
            Y = PosY;
            this.Width = Width;
            this.Height = Height;
            Name = "Class_" + Count;
            Count++;
            Border = new Pen(new SolidBrush(Color.Black),2);
            Select = new Pen(new SolidBrush(Color.LightGreen),2);
            black = new Pen(new SolidBrush(Color.Black));
            Background = new SolidBrush(Color.LightBlue);
            //isAgregated = false;
            isSelected = false;
        }

        ~ClassBox()
        {
            Count--;
        }

        public void AddListVariables(List<C_Variables> _lcv)
        {
            foreach (C_Variables cv in _lcv)
            {
                Variables.Add(new C_Variables(cv.Type, cv.Name));
            }
        }

        public void draw(Graphics g)
        {
            Resize();
            Rectangle R = new Rectangle(X, Y, Width, Height);
            if (Methods.Count == 0 && VirtualMethods.Count != 0)
            {
                g.FillRectangle(new SolidBrush(Color.LightYellow), R);
            }
            else
            {
                g.FillRectangle(Background, R);
            }
            if (isSelected)
            {
                g.DrawRectangle(Select, R);
                g.DrawLine(Select, X, Y + 19, X + Width, Y + 19);
            }
            else
            {
                g.DrawRectangle(Border, R);
                g.DrawLine(Border, X, Y + 19, X + Width, Y + 19);
            }
            g.DrawString(Name, new Font("Arial",10), new SolidBrush(Color.Black), R.X+2, R.Y+2);
            int dY = 19;
            foreach (C_Variables cv in Variables)
            {
                g.DrawString(cv.Type+" "+cv.Name, new Font("Arial",10), new SolidBrush(Color.Black), R.X+2, R.Y+dY);
                dY+=15;
            }
            if (dY != 19)
            {
                if (isSelected)
                {
                    g.DrawLine(Select, X, Y + dY + 2, X + Width, Y + dY + 2);
                }
                else
                {
                    g.DrawLine(Border, X, Y + dY + 2, X + Width, Y + dY + 2);
                }
            }
            dY += 2;
            foreach (C_Methods cm in Methods)
            {
                g.DrawString(cm.Type + " " + cm.Name+"()", new Font("Arial", 10), new SolidBrush(Color.Black), R.X + 2, R.Y + dY);
                dY += 15;
            }
            foreach (C_Methods cm in VirtualMethods)
            {
                g.DrawString("virtual " + cm.Type + " " + cm.Name + "()", new Font("Arial", 10), new SolidBrush(Color.Black), R.X + 2, R.Y + dY);
                dY += 15;
            }

            foreach (ClassBox c in AgregatedClasses)
                drawConnection(g, c, 0);
            foreach (ClassBox c in CompositedClasses)
                drawConnection(g, c, 1);
            foreach (ClassBox c in ParentClasses)
                drawConnection(g, c, 2);
        }

        void drawConnection(Graphics g, ClassBox From, int Type)
        {
            SolidBrush sb = new SolidBrush(Color.Black);
            Pen Black = new Pen(sb,2);
            int x1, y1, x2, y2;
            if (From.X+From.Width/2<X+Width/2)
            {
                x1 = From.X + From.Width+10;
                y1 = From.Y + From.Height / 2;
                x2 = X-20;
                y2 = Y + Height / 2;
                g.DrawLine(Black, x1-10, y1, x1, y1);
                if (Type == 0)
                    g.DrawPolygon(Black, new Point[] { new Point(x2, y2), new Point(x2 + 10, y2 + 6), new Point(x2 + 20, y2), new Point(x2 + 10, y2 - 6) });
                if (Type == 1)
                    g.FillPolygon(sb, new Point[] { new Point(x2, y2), new Point(x2 + 10, y2 + 6), new Point(x2 + 20, y2), new Point(x2 + 10, y2 - 6) });
                if (Type == 2)
                {
                    g.DrawLine(Black,x2,y2,x2 +20, y2);
                    g.DrawLine(Black,x2+10,y2+6,x2+20,y2);
                    g.DrawLine(Black,x2+10,y2-6,x2+20,y2);
                }
            }
            else
            {
                x1 = From.X-10;
                y1 = From.Y + From.Height / 2;
                x2 = X+Width+20;
                y2 = Y + Height / 2;
                g.DrawLine(Black, x1, y1, x1 + 10, y1);
                if (Type == 0)
                    g.DrawPolygon(Black, new Point[] { new Point(x2, y2), new Point(x2 - 10, y2 + 6), new Point(x2 - 20, y2), new Point(x2 - 10, y2 - 6) });
                if (Type == 1)
                    g.FillPolygon(sb, new Point[] { new Point(x2, y2), new Point(x2 - 10, y2 + 6), new Point(x2 - 20, y2), new Point(x2 - 10, y2 - 6) });
                if (Type == 2)
                {
                    g.DrawLine(Black, x2, y2, x2 - 20, y2);
                    g.DrawLine(Black, x2 - 10, y2 + 6, x2 - 20, y2);
                    g.DrawLine(Black, x2 - 10, y2 - 6, x2 - 20, y2);
                }
            }

            g.DrawLine(Black, x1, y1, x2, y2); //Соединение
        }

        public void Resize()
        {
            //Узнаем длину имени класса
            Width = TextRenderer.MeasureText(Name, new Font("Arial", 10)).Width;
            foreach (C_Variables c in Variables)
            {
                int temp = TextRenderer.MeasureText(c.Type + " " + c.Name, new Font("Arial", 10)).Width;
                if (Width < temp)
                {
                    Width = temp;
                }
            }
            foreach (C_Methods m in Methods)
            {
                int temp = TextRenderer.MeasureText(m.Type + " " + m.Name + "()", new Font("Arial", 10)).Width;
                if (Width < temp)
                {
                    Width = temp;
                }
            }
            foreach (C_Methods vm in VirtualMethods)
            {
                int temp = TextRenderer.MeasureText("virtual " + vm.Type + " " + vm.Name + "()", new Font("Arial", 10)).Width;
                if (Width < temp)
                {
                    Width = temp;
                }
            }
            Height = 15 * (Variables.Count + Methods.Count + VirtualMethods.Count + 2);
        }

        public void TestValues()
        {
            Variables.Add(new C_Variables("int", "Length"));
            Variables.Add(new C_Variables("int", "Heigth"));
            Variables.Add(new C_Variables("int", "Weigth"));
            Methods.Add(new C_Methods("void", "Run"));
            Methods[0].AddVariable(new C_Variables("int", "speed"));
            Methods[0].AddVariable(new C_Variables("int", "deeps"));
            Methods.Add(new C_Methods("void", "Jump"));
            Methods[1].AddVariable(new C_Variables("int", "heigth"));
            Methods[1].AddVariable(new C_Variables("int", "htgieh"));
        }

        public void Check()
        {
            if (Name.Length * 7 > Width)
                Width = Name.Length * 7;
        }
        
    }

}
