using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Kursach
{
    public partial class UML_diagram_form : Form
    {
        public List<ClassBox> Classes = new List<ClassBox>();
        int PosX, PosY;  //Координаты в которых будет расположен следующий объект
        int dX, dY;      //Для точного перетаскивания мышкой
        Bitmap buffImage;
        Graphics g, gBuffer;
        bool isClicked = false;  //Нажатие мыши на объект

        public UML_diagram_form()
        {
            InitializeComponent();
        }

        private void Add_class_button_Click(object sender, EventArgs e)
        {
            //Алгоритм проверки свободного места на диаграмме. Находит позицию с координатами PosX, PosY в которой можно рисовать объект---------
            bool free = true;
            for (PosY=5; PosY < pictureBox1.Height; PosY += 10)
            {
                for (PosX=5; PosX < pictureBox1.Width; PosX += 10)
                {
                    free = true;
                    foreach (ClassBox c in Classes)
                        if (((PosX > c.X - 110) && (PosX < c.X + c.Width + 10)) && ((PosY > c.Y - 110) && (PosY < c.Y + c.Height + 10)))
                        {
                            free = false;
                            break;
                        }
                    if (free)
                        break;
                }
                if ((PosX + 100 > pictureBox1.Width) || (PosY + 100 > pictureBox1.Height))
                    free = false;
                if (free)
                    break;
            }
            //---------------------------------------------------------------------------------------------------------------------------

            try
            {
                if (!free)
                {
                    throw new Exception("MESTA NETU #$!@%");
                }


                //Если есть место то добавляем объект-----------
                Classes.Add(new ClassBox(PosX, PosY, 100, 100));
                Classes[ClassBox.Count - 1].draw(gBuffer);

                if ((PosX + 210) < pictureBox1.Width)
                    PosX += 110;
                else
                {
                    PosX = 5;
                    PosY += 110;
                }

                g.DrawImage(buffImage, 0, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int number = 0;
                foreach (ClassBox C in Classes)
                    C.isSelected = false;

                foreach (ClassBox C in Classes) //выводим объект поверх других и выделяем
                {
                    if ((e.X > C.X) && (e.X < C.X + C.Width))       //если попали
                        if ((e.Y > C.Y) && (e.Y < C.Y + C.Height))  //в него мышей
                        {
                            isClicked = true;
                            dX = e.X - C.X;
                            dY = e.Y - C.Y;
                            Classes.Add(Classes[number]);
                            Classes.RemoveAt(number);
                            Classes[Classes.Count - 1].isSelected = true;
                            break;
                        }
                    number++;
                }
                Redraw();
                //----------------------------------------------------
            }
            else
                contextMenuStrip1.Show(e.X,e.Y);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isClicked)
            {
                Classes[Classes.Count-1].X = e.X - dX;
                Classes[Classes.Count-1].Y = e.Y - dY;
                Redraw();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isClicked = false;          
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = 0;
            foreach (ClassBox C in Classes)
            {
                
                if ((e.X > C.X) && (e.X < C.X + C.Width))
                    if ((e.Y > C.Y) && (e.Y < C.Y + C.Height))
                    {
                        ClassConfigurationForm CCF = new ClassConfigurationForm(index);
                        CCF.Owner = this;
                        CCF.ShowDialog();
                        break;
                    }
                index++;
            }
        }

        public void Redraw()
        {
            gBuffer.Clear(Color.Silver);
            foreach (ClassBox C in Classes)
            {
                C.draw(gBuffer);
            }
            g.DrawImage(buffImage, 0, 0);
        }

        private void UML_diagram_form_Load(object sender, EventArgs e)
        {
            buffImage = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            gBuffer = Graphics.FromImage(buffImage);
            pictureBox1.Image = buffImage;
            g = pictureBox1.CreateGraphics();

        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            buffImage = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            gBuffer = Graphics.FromImage(buffImage);
            g = pictureBox1.CreateGraphics();
            Redraw();
        }

        private void generate_code_button_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog f = new FolderBrowserDialog();
            string path = "";
            if (f.ShowDialog() == DialogResult.OK)
            {
                foreach (ClassBox c in Classes)
                {
                    StreamWriter sw = new StreamWriter(f.SelectedPath+"\\"+c.ClassName + ".txt");
                    sw.WriteLine("Class " + c.ClassName);
                    sw.WriteLine("{");
                    foreach (C_Variables v in c.Variables)
                        sw.WriteLine(v.Type + " " + v.Name + ";");
                    sw.WriteLine();
                    foreach (C_Methods m in c.Methods)
                    {
                        sw.Write(m.Type + " " + m.Name + "( ");
                        string vars = "";
                        foreach (C_Variables mv in m.Variables)
                        {
                            vars += mv.Type + " " + mv.Name + ", ";
                        }
                        vars = vars.Remove(vars.Length - 2, 2);
                        sw.WriteLine(vars + " );");
                        sw.WriteLine();
                    }
                    sw.WriteLine("}");
                    sw.Close();
                }
            }
        }

    }
}
