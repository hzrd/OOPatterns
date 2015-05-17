using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Kursach
{
    public class ClassBox
    {
        public int X{set;get;}
        public int Y{set;get;}
        public int Width{set;get;}
        public int Height { set; get; }
        Pen Contour,Select,black;
        SolidBrush Background;

        string Name;
        public List<C_Variables> Variables = new List<C_Variables>();
        public List<C_Methods> Methods = new List<C_Methods>();
        public static int Count = 0;

        bool isSelected;
        bool Agregation;
        ClassBox AgregatedClassBox;


        public ClassBox(int PosX, int PosY, int Width, int Height)
        {
            TestValues(); //Тестовые значения при инициализации
            X = PosX;
            Y = PosY;
            this.Width = Width;
            this.Height = Height;
            Name = "Class " + Count;
            Count++;
            Contour = new Pen(new SolidBrush(Color.Black),2);
            Select = new Pen(new SolidBrush(Color.LightGreen),2);
            black = new Pen(new SolidBrush(Color.Black));
            Background = new SolidBrush(Color.LightBlue);
            Agregation = false;
            isSelected = false;
        }

        ~ClassBox()
        {
            Count--;
        }

        public void draw(Graphics g)
        {
            Rectangle R = new Rectangle(X, Y, Width, Height);
            g.FillRectangle(Background, R);
            g.DrawRectangle(Contour, R);           
            if (isSelected)
                g.DrawRectangle(Select, R);
            if (Agregation)
                DrawAgregation(g);
            g.DrawString(Name, new Font("Arial",10), new SolidBrush(Color.Black), R.X+2, R.Y+2);
            int dY = 17;
            foreach (C_Variables cv in Variables)
            {
                g.DrawString(cv.Type+" "+cv.Name, new Font("Arial",10), new SolidBrush(Color.Black), R.X+2, R.Y+dY);
                dY+=15;
            }
            foreach (C_Methods cm in Methods)
            {
                g.DrawString(cm.Type + " " + cm.Name, new Font("Arial", 10), new SolidBrush(Color.Black), R.X + 2, R.Y + dY);
                dY += 15;
            }
        }

        public void draw(Graphics g, int PosX, int PosY)
        {
            Rectangle R = new Rectangle(PosX, PosY, Width, Height);
            g.FillRectangle(Background, R);
            g.DrawRectangle(Contour, R);
            if (isSelected)
                g.DrawRectangle(Select, R);
            if (Agregation)
                DrawAgregation(g);
            g.DrawString(Name, new Font("Arial", 10), new SolidBrush(Color.Black), R.X + 2, R.Y + 2);
            int dY = 17;
            foreach (C_Variables cv in Variables)
            {
                g.DrawString(cv.Type + " " + cv.Name, new Font("Arial", 10), new SolidBrush(Color.Black), R.X + 2, R.Y + dY);
                dY += 15;
            }
            foreach (C_Methods cm in Methods)
            {
                g.DrawString(cm.Type + " " + cm.Name, new Font("Arial", 10), new SolidBrush(Color.Black), R.X + 2, R.Y + dY);
                dY += 15;
            }
        }

        public void DrawAgregation(Graphics g)
        {
            if ((X < AgregatedClassBox.X - Width))
                g.DrawLine(black, X + Width, Y + Height / 2, AgregatedClassBox.X, AgregatedClassBox.Y + AgregatedClassBox.Height / 2);
            else if ((X+Width >= AgregatedClassBox.X) && (X <= AgregatedClassBox.X + AgregatedClassBox.Width))
                if (Y < AgregatedClassBox.Y)
                    g.DrawLine(black, X + Width / 2, Y + Height, AgregatedClassBox.X + AgregatedClassBox.Width / 2, AgregatedClassBox.Y);
                else
                    g.DrawLine(black, X + Width / 2, Y, AgregatedClassBox.X + AgregatedClassBox.Width / 2, AgregatedClassBox.Y + Height);
            else
                g.DrawLine(black, X, Y + Height / 2, AgregatedClassBox.X+AgregatedClassBox.Width, AgregatedClassBox.Y + AgregatedClassBox.Height / 2);
        }

        public void TestValues()
        {
            Variables.Add(new C_Variables("int", "Length"));
            Variables.Add(new C_Variables("int", "Heigth"));
            Variables.Add(new C_Variables("int", "Weigth"));
            Methods.Add(new C_Methods("void", "Run()"));
            Methods[0].AddVariable(new C_Variables("int", "speed"));
            Methods[0].AddVariable(new C_Variables("int", "deeps"));
            Methods.Add(new C_Methods("void", "Jump()"));
            Methods[1].AddVariable(new C_Variables("int", "heigth"));
            Methods[1].AddVariable(new C_Variables("int", "htgieh"));
        }

        public void Check()
        {
            if (Name.Length * 7 > Width)
                Width = Name.Length * 7;
        }

        public string ClassName
        {
            set { Name = value; }
            get { return Name; }
        }


        public bool Selected
        {
            set { isSelected = value; }
            get { return isSelected; }
        }

        public bool Agregated
        {
            set { Agregation = value; }
            get { return Agregation; }
        }

        public ClassBox AgregatedClass
        {
            set { AgregatedClassBox = value; }
            get { return AgregatedClassBox; }
        }
        
    }

}
