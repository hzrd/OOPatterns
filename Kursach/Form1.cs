using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursach
{
    //Добавил комментарий
    public partial class UML_diagram_form : Form
    {
        public List<ClassBox> Classes = new List<ClassBox>();
        int mX, mY;
        int PosX=5, PosY=5;
        int dX, dY, number;
        Bitmap buffImage;
        Graphics g, gBuffer;
        bool isClicked = false;
        bool Agregation = false;

        public UML_diagram_form()
        {
            InitializeComponent();
            buffImage = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            gBuffer = Graphics.FromImage(buffImage);
            g = pictureBox1.CreateGraphics();
        }

        private void Add_class_button_Click(object sender, EventArgs e)
        {
            bool free = true;
            for (PosY = 10; PosY < pictureBox1.Height; PosY += 10)
            {
                for (PosX = 10; PosX < pictureBox1.Width; PosX += 10)
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

            try
            {
                if (!free)
                {
                    throw new Exception("MESTA NETU #$!@%");
                }


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
                gBuffer.Clear(Color.White);

                if (Agregation)
                {
                    foreach (ClassBox C in Classes)
                        if ((e.X > C.X) && (e.X < C.X + C.Width))
                            if ((e.Y > C.Y) && (e.Y < C.Y + C.Height))
                            {
                                Classes[Classes.Count - 1].AgregatedClass = C;
                                Agregation = false;
                            }
                }
                else
                {


                    number = 0;
                    foreach (ClassBox C in Classes)
                        C.Selected = false;

                    foreach (ClassBox C in Classes)
                    {
                        if ((e.X > C.X) && (e.X < C.X + C.Width))
                            if ((e.Y > C.Y) && (e.Y < C.Y + C.Height))
                            {
                                ClassBox temp = Classes[number];
                                isClicked = true;
                                dX = e.X - C.X;
                                dY = e.Y - C.Y;
                                Classes.Remove(Classes[number]);
                                Classes.Add(temp);
                                Classes[Classes.Count - 1].Selected = true;
                                break;
                            }
                        number++;
                    }
                }


                foreach (ClassBox C in Classes)
                {
                    C.draw(gBuffer);
                }
                g.DrawImage(buffImage, 0, 0);

            }
            else
                contextMenuStrip1.Show(e.X,e.Y);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isClicked)
            {
                gBuffer.Clear(Color.White);
                Classes[Classes.Count-1].X = e.X - dX;
                Classes[Classes.Count-1].Y = e.Y - dY;
                foreach (ClassBox C in Classes)
                {
                    C.draw(gBuffer);
                }

                mX = e.X;
                mY = e.Y;
            }
            g.DrawImage(buffImage, 0, 0);
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

        private void button1_Click(object sender, EventArgs e)
        {
            gBuffer.Clear(Color.White);
            Classes[0].Height += 20;
            foreach (ClassBox C in Classes)
            {
                C.draw(gBuffer);
            }
            g.DrawImage(buffImage, 0, 0);
        }

        public void Redraw()
        {
            gBuffer.Clear(Color.White);
            foreach (ClassBox C in Classes)
            {
                C.draw(gBuffer);
            }
            g.DrawImage(buffImage, 0, 0);
        }

        private void addAgregationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Agregation = true;
            foreach (ClassBox C in Classes)
            {
                if (C.Selected)
                    C.Agregated = true;
            }
            MessageBox.Show("Select Class to agregate");
        }

    }
}
