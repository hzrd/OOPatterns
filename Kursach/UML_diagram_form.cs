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
        public List<Agregation> Agregations = new List<Agregation>();
        public List<LoggerUserAction> logger = new List<LoggerUserAction>();

        public List<string> _pathToCreatingFile = new List<string>();
        int PosX, PosY;  //Координаты в которых будет расположен следующий объект
        int dX, dY;      //Для точного перетаскивания мышкой
        Bitmap buffImage;
        Graphics g, gBuffer;
        bool isClicked = false;  //Нажатие мышкой на объект
        int set = 0;

        public UML_diagram_form()
        {
            InitializeComponent();
        }

        //Алгоритм проверки свободного места на диаграмме. Находит позицию с координатами PosX, PosY в которой можно рисовать объект---------
        private bool GetFreePosition()
        {
            bool free = true;
            for (PosY = 5; PosY < pictureBox1.Height; PosY += 10)
            {
                for (PosX = 5; PosX < pictureBox1.Width; PosX += 10)
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
            return free;
        }

        private void Add_class_button_Click(object sender, EventArgs e)
        {           
            try
            {
                if (!GetFreePosition())
                {
                    throw new Exception("На полотне недостаточно места для размещения объекта!");
                }
                //Если есть место то добавляем объект-----------
                Classes.Add(new ClassBox(PosX, PosY, 100, 100));
                logger.Add(new LoggerUserAction(Action.AddClass, DateTime.Now, Classes[Classes.Count - 1].Name));

                if ((PosX + 210) < pictureBox1.Width)
                    PosX += 110;
                else
                {
                    PosX = 5;
                    PosY += 110;
                }
                Redraw();
                Redraw();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CheckConnections(ClassBox a, ClassBox b)
        {
            foreach (ClassBox c in a.AgregatedClasses)
                if (c.Name == b.Name)
                    return false;
            foreach (ClassBox c in a.CompositedClasses)
                if (c.Name == b.Name)
                    return false;
            foreach (ClassBox c in a.ParentClasses)
                if (c.Name == b.Name)
                    return false;
            foreach (ClassBox c in b.AgregatedClasses)
                if (c.Name == a.Name)
                    return false;
            foreach (ClassBox c in b.CompositedClasses)
                if (c.Name == a.Name)
                    return false;
            foreach (ClassBox c in b.ParentClasses)
                if (c.Name == a.Name)
                    return false;
            return true;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (set == 0)
                {
                    int number = 0;
                    foreach (ClassBox C in Classes)
                        C.isSelected = false;

                    foreach (ClassBox C in Classes) //выводим объект поверх других и выделяем
                    {
                        if ((e.X > C.X) && (e.X < C.X + C.Width))       //если попали
                            if ((e.Y > C.Y) && (e.Y < C.Y + C.Height))  //в него мышкой
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
                }
                else
                {
                    int index1 = 0; //Выделенный класс
                    int index2 = 0; //Класс от которого идет связь
                    foreach (ClassBox c in Classes)
                    {
                        if (c.isSelected)
                            break;
                        index1++;
                    }
                    foreach (ClassBox C in Classes)
                    {
                        if ((e.X > C.X) && (e.X < C.X + C.Width))
                            if ((e.Y > C.Y) && (e.Y < C.Y + C.Height))
                                break;
                        index2++;
                    }
                    if (index1 != index2)
                    {
                        try
                        {
                            if (CheckConnections(Classes[index1], Classes[index2]))
                            {
                                if (set == 1)
                                {

                                    Classes[index1].AgregatedClasses.Add(Classes[index2]);
                                    Classes[index1].Variables.Add(new C_Variables(Classes[index2].Name, Classes[index2].Name.ToLower()));

                                }
                                if (set == 2)
                                {
                                    Classes[index1].CompositedClasses.Add(Classes[index2]);
                                    Classes[index1].Variables.Add(new C_Variables(Classes[index2].Name, Classes[index2].Name.ToLower()));
                                }
                                if (set == 3)
                                    Classes[index1].ParentClasses.Add(Classes[index2]);
                                logger.Add(new LoggerUserAction(Action.AddAggregation, DateTime.Now, Classes[index1].Name));
                            }
                            else
                                MessageBox.Show("Между этими объектами уже установлена связь");


                        }
                        catch
                        {
                            MessageBox.Show("Нужно выбрать объект!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Нельзя установить связь с самим собой!");
                    }
                    set = 0;
                }
                Redraw();
                //----------------------------------------------------
            }
            else
            {
                contextMenuStrip1.Show(pictureBox1, e.X, e.Y);
            }
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

            foreach (Agregation A in Agregations)
            {
                A.draw(gBuffer);
            }

            try //Какой то странный баг...
            {
                g.DrawImage(buffImage, 0, 0);
            }
            catch
            {
                gBuffer = Graphics.FromImage(buffImage);
                g = pictureBox1.CreateGraphics();
            }
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
            if (f.ShowDialog() == DialogResult.OK)
            {
                CodeGeneration_module cgmWrite = new CodeGeneration_module();
                foreach (ClassBox c in Classes)
                {
                    if (c.VirtualMethods.Count != 0 && c.Methods.Count == 0)
                    {
                        cgmWrite.CodeToFile(c, f.SelectedPath, true);
                        _pathToCreatingFile.Add(f.SelectedPath + "\\" + c.Name + ".h");
                    }
                    else
                    {
                        cgmWrite.CodeToFile(c, f.SelectedPath, false);
                        _pathToCreatingFile.Add(f.SelectedPath + "\\" + c.Name + ".h");
                        _pathToCreatingFile.Add(f.SelectedPath + "\\" + c.Name + ".cpp");
                    }
                    logger.Add(new LoggerUserAction(Action.SaveFiles, DateTime.Now, c.Name));
                }
            }
        }

        private void addAgregationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            set = 1;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            CodeGeneration_module cgmRead = new CodeGeneration_module();
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string[] masFiles = Directory.GetFiles(fbd.SelectedPath);
                foreach (string file in masFiles)
                {
                    if (file.Remove(0, file.LastIndexOf('.') + 1) == "h")
                    {
                        try
                        {
                            if (GetFreePosition())
                            {
                                Classes.Add(cgmRead.ReadFile(file));
                                logger.Add(new LoggerUserAction(Action.AddClass, DateTime.Now, Classes[Classes.Count - 1].Name));
                            }
                            Classes[Classes.Count - 1].X = PosX;
                            Classes[Classes.Count - 1].Y = PosY;
                            Classes[Classes.Count - 1].Width = 100;
                            Classes[Classes.Count - 1].Height = 150;

                        }
                        catch
                        {
                            MessageBox.Show("Возникла ошибка, связанная со структурой файла!\nВ данной версии продукта не поддерживается ввод переменных в метод без указания имени переменной! \nФайл:" + file,
                                            "Внимание!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                        }
                    }
                    Redraw();
                }
                //Проверяем правильность наброса связей
                foreach (ClassBox _cb in Classes)
                {
                    //Проверяем в остальных классах
                    for (int i = 0; i < Classes.Count; i++)
                    {
                        //Просматриваем список родителей
                        for (int j = 0; j < Classes[i].ParentClasses.Count; j++)
                        {
                            if (_cb.Name == Classes[i].ParentClasses[j].Name)
                            {
                                Classes[i].ParentClasses.RemoveAt(j);
                                Classes[i].ParentClasses.Add(_cb);
                                break;
                            }
                        }
                        //Проверяем агрегацию
                        for (int v = 0; v< Classes[i].Variables.Count; v++)
                        {
                            if (_cb.Name == Classes[i].Variables[v].Type)
                            {
                                Classes[i].AgregatedClasses.Add(_cb);
                                break;
                            }
                        }
                    }
                }
                //Проверям доступные классы\интерфейсы и убираем ненужные связи
                for (int i = 0; i < Classes.Count; i++)
                {
                    bool temp = false;
                    //Смотрим всех родителей
                    for (int indexParent = 0; indexParent < Classes[i].ParentClasses.Count; indexParent++)
                    {
                        //Просматриваем все классы, пока не найдем нужный --> parent
                        for (int j = 0; j < Classes.Count; j++)
                        {
                            if (Classes[i].ParentClasses[indexParent].Name == Classes[j].Name)
                            {
                                temp = true;
                                break;
                            }
                        }
                        //Если не нашли
                        if (!temp)
                        {
                            Classes[i].ParentClasses.Remove(Classes[i].ParentClasses[indexParent]);
                            if (indexParent != 0)
                            {
                                indexParent--;
                            }
                        }
                    }
                }
                Redraw();
            }
        }


        private void DeleteFromDiagram()
        {
            bool findSomething = false;
            int index = 0;
            foreach (ClassBox c in Classes)
            {
                if (c.isSelected)
                {
                    findSomething = true;
                    break;
                }
                index++;
            }
            if (findSomething)
            {
                if (MessageBox.Show("Вы действительно хотите удалить " + Classes[index].Name + " ?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    logger.Add(new LoggerUserAction(Action.DeleteClass, DateTime.Now, Classes[index].Name));
                    Classes.RemoveAt(index);
                    Redraw();
                }
            }
            else
                MessageBox.Show("Ничего не выбрано");
        }

        private void UML_diagram_form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteFromDiagram();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteFromDiagram();
        }

        private void deleteAgregationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ClassBox c in Classes)
            {
                if (c.isSelected)
                {
                    int i1 = c.Variables.Count - 1;
                    int i2 = i1 - c.AgregatedClasses.Count;
                    for (int i = i1; i > i2; i--)
                        c.Variables.RemoveAt(i);
                    c.AgregatedClasses.Clear();
                    break;
                }
            }
            Redraw();
        }

        private void addCompositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            set = 2;
        }

        private void addParentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            set = 3;
        }
    }
}
