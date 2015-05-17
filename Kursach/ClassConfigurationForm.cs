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
    public partial class ClassConfigurationForm : Form
    {
        Graphics g;
        int index;
        UML_diagram_form f;
        Bitmap bmp;

        //Массив переменных и методов
        List<C_Variables> vars = new List<C_Variables>();
        List<C_Methods> meths = new List<C_Methods>();

        //Массив с запрещенными символами в начале имени, для проверки "не начинается ли имя с цифры"
        char[] firstSymbol = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '#','.',',','\\','/','|','-','*','+','@','"','№',';','$','%','?','(',')','<','>','`','~'};
        //Массив системных имен
        string[] systemName = {"system","using","namespace","int","double","float","bool","char","string"};
        public ClassConfigurationForm()
        {
            InitializeComponent();
        }

        public ClassConfigurationForm(int ind)
        {
            InitializeComponent();
            index = ind;
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }
        //--------------------------------------------Добавление переменных и методов------------------------------------------------------------------------
        private void AddVariables(C_Variables var)
        {
            //Проверяем не начинается ли переменная с цифры
            int count = 0;
            foreach (char fs in firstSymbol)
            {
                if (var.Name[0] == fs)
                {
                    count++;
                    break;
                }
            }
            //ПРОВЕРКА СИСТЕМНЫХ ИМЕН---------------------------------------------
            if (count == 0)
            {
                foreach (string s in systemName)
                {
                    if (var.Name == s)
                    {
                        count++;
                        break;
                    }
                }
            }
            //--------------------------------------------------------------------
            if (count == 0)
            {
                //Добавляем переменную в массив

                vars.Add(var);
                //И на форму
                ListViewItem item = new ListViewItem(var.Type);
                item.SubItems.Add(var.Name);
                listView1.Items.Add(item);
            }
            else
            {
                MessageBox.Show("Имя переменной не должно начинаться с цифры или совпадать с системным именем!");
            }
        }

        private void AddMethods(C_Methods meth)
        {
            //Проверяем не начинается ли метод с цифры
            int count = 0;
            foreach (char fs in firstSymbol)
            {
                if (meth.Name[0] == fs)
                {
                    count++;
                    break;
                }
            }
            //ПРОВЕРКА СИСТЕМНЫХ ИМЕН---------------------------------------------
            if (count == 0)
            {
                foreach (string s in systemName)
                {
                    if (meth.Name == s)
                    {
                        count++;
                        break;
                    }
                }
            }
            //--------------------------------------------------------------------
            if (count == 0)
            {
                //Добавляем метод в массив
                meths.Add(meth);
                //И на форму
                ListViewItem item = new ListViewItem(meth.Type);
                item.SubItems.Add(meth.Name);
                listView2.Items.Add(item);
            }
            else
            {
                MessageBox.Show("Имя метода не должно начинаться с цифры или совпадать с системным именем!");
            }
        }

        private void AddVariablesInMethods(C_Methods meth,C_Variables var)
        {
            //Проверяем не начинается ли переменная с цифры
            int count = 0;
            foreach (char fs in firstSymbol)
            {
                if (var.Name[0] == fs)
                {
                    count++;
                    break;
                }
            }
            //ПРОВЕРКА СИСТЕМНЫХ ИМЕН---------------------------------------------
            if (count == 0)
            {
                foreach (string s in systemName)
                {
                    if (var.Name == s)
                    {
                        count++;
                        break;
                    }
                }
            }
            //--------------------------------------------------------------------
            if (count == 0)
            {
                //Добавляем переменные для метода в массив самого метода
                meth.AddVariable(var);
                //И выводим на форму
                ListViewItem item = new ListViewItem(var.Type);
                item.SubItems.Add(var.Name);
                listView3.Items.Add(item);
            }
            else
            {
                MessageBox.Show("Имя переменной не должно начинаться с цифры или совпадать с системным именем!");
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------Проверка на совпадение----------------------------------------------------------------------
        private int Check(ListView lv1, ListView lv2,string sName)
        {
            for (int i = 0; i < lv1.Items.Count; i++)
            {
                if (lv1.Items[i].SubItems[1].Text == sName)
                {
                    throw new Exception("Введенное имя совпадает с именем переменной!");
                }
            }
            for (int i = 0; i < lv2.Items.Count; i++)
            {
                if (lv2.Items[i].SubItems[1].Text == sName)
                {
                    throw new Exception("Введенное имя совпадает с именем метода!");
                }
            }
            return 1;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------Удаление переменных и методов-------------------------------------------------------------------
        private void DeleteVariable(string sNameVariable)
        {
            int count = 0;
            //Находим позицию, на которой стоит переменная
            for (int i = 0; i < vars.Count; i++)
            {
                if (vars[i].Name == sNameVariable)
                {
                    count = i;
                    break;
                }
            }
            //Передвигаем, не сбивая порядок
            for (int i = count; i < vars.Count - 1; i++)
            {
                C_Variables temp = vars[i];
                vars[i] = vars[i + 1];
                vars[i + 1] = temp;
            }
            //Убираем ненужную переменную, путем уменьшения длины массива <--- Это пиздежь, мы просто удаляем из листа
            vars.RemoveAt(vars.Count - 1);
            //Перезагружаем переменные
            listView1.Items.Clear();
            foreach (C_Variables v in vars)
            {
                ListViewItem item = new ListViewItem(v.Type);
                item.SubItems.Add(v.Name);
                listView1.Items.Add(item);
            }
        }

        private void DeleteMethod(string sNameMethod)
        {
            int count = 0;
            //Находим позицию, на которой стоит метод
            for (int i = 0; i < meths.Count; i++)
            {
                if (meths[i].Name == sNameMethod)
                {
                    count = i;
                    break;
                }
            }
            //Передвигаем, не сбивая порядок
            for (int i = count; i < meths.Count - 1; i++)
            {
                C_Methods temp = meths[i];
                meths[i] = meths[i + 1];
                meths[i + 1] = temp;
            }
            //Убираем ненужный метод, путем уменьшения длины массива  <-- ОПЯТЬ ПИЗДЕЖЬ
            meths.RemoveAt(meths.Count - 1);
            //Перезагружаем методы
            listView2.Items.Clear();
            foreach (C_Methods m in meths)
            {
                ListViewItem item = new ListViewItem(m.Type);
                item.SubItems.Add(m.Name);
                listView2.Items.Add(item);
            }
        }

        private void DeleteMethodVariables(string sNameMethod, string sNameVariable)
        {
            int numberMethod = 0;
            int numberVariable = 0;
            //Находим нужный метод в массиве
            for (int i = 0; i < meths.Count; i++)
            {
                if (meths[i].Name == sNameMethod)
                {
                    numberMethod = i;
                    break;
                }
            }
            //В данном методе находим нужную переменную
            for (int i = 0; i < meths[numberMethod].Variables.Count; i++)
            {
                if (meths[numberMethod].Variables[i].Name == sNameVariable)
                {
                    numberVariable = i;
                    break;
                }
            }
            //Удаляем данную переменную
            meths[numberMethod].DeleteVariable(meths[numberMethod].Variables[numberVariable]);
            listView3.Items.Clear();
            foreach (C_Variables v in meths[numberMethod].Variables)
            {
                ListViewItem item = new ListViewItem(v.Type);
                item.SubItems.Add(v.Name);
                listView3.Items.Add(item);
            }
        }
        //------------------------------------------------------Обработка нажатий кнопок---------------------------------------------------------------------
        private void button1_Click(object sender, EventArgs e)
        {
            //Выполняем добавление переменной
            if (comboBox1.SelectedIndex != -1 && string.IsNullOrEmpty(textBox1.Text) == false && button1.Text == "+")
            {
                C_Variables var = new C_Variables();
                var.Type = comboBox1.Text;
                var.Name = textBox1.Text;
                try
                {
                    if (Check(listView1, listView2, var.Name) == 1)
                    {
                        AddVariables(var);
                        comboBox1.SelectedIndex = -1;
                        textBox1.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //Выполняем редактирование переменной
            if (comboBox1.SelectedIndex != -1 && string.IsNullOrEmpty(textBox1.Text) == false && button1.Text == "E")
            {
                try
                {
                    if (Check(listView1, listView2, textBox1.Text) == 1)
                    {
                        DialogResult dialogResult = MessageBox.Show("Сохранить изменения в переменной \"" + listView1.FocusedItem.Text + " "
                                            + listView1.FocusedItem.SubItems[1].Text + "\" на +\"" +
                                            comboBox1.Text + " " + textBox1.Text + "\"?"
                                            , "Изменение переменной", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.Yes)
                        {
                            //Находим нужную переменную
                            foreach (C_Variables v in vars)
                            {
                                //Заменяем данные и выходим с цикла
                                if (v.Name == listView1.FocusedItem.SubItems[1].Text)
                                {
                                    v.Name = textBox1.Text;
                                    v.Type = comboBox1.Text;
                                    break;
                                }
                            }
                            //Загружаем переменные
                            listView1.Items.Clear();
                            foreach (C_Variables v in vars)
                            {
                                ListViewItem item = new ListViewItem(v.Type);
                                item.SubItems.Add(v.Name);
                                listView1.Items.Add(item);
                            }
                            //Очищаем вспомогательные данные
                            button1.Text = "+";
                            comboBox1.SelectedIndex = -1;
                            textBox1.Text = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            g = pictureBox1.CreateGraphics();
            f.Classes[index].draw(g,50,50);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex != -1 && string.IsNullOrEmpty(textBox2.Text) == false && button2.Text == "+")
            {
                C_Methods meth = new C_Methods();
                meth.Type = comboBox2.Text;
                meth.Name = textBox2.Text;
                try
                {
                    if (Check(listView1, listView2, meth.Name) == 1)
                    {
                        AddMethods(meth);
                        comboBox2.SelectedIndex = -1;
                        textBox2.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (comboBox2.SelectedIndex != -1 && string.IsNullOrEmpty(textBox2.Text) == false && button2.Text == "E")
            {
                try
                {
                    if (Check(listView1, listView2, textBox2.Text) == 1)
                    {
                        DialogResult dialogResult = MessageBox.Show("Сохранить изменения в методе \"" + listView2.FocusedItem.Text + " "
                                                    + listView2.FocusedItem.SubItems[1].Text + "\" на +\"" +
                                                    comboBox2.Text + " " + textBox2.Text + "\"?"
                                                    , "Изменение метода", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.Yes)
                        {
                            foreach (C_Methods m in meths)
                            {
                                if (m.Name == listView2.FocusedItem.SubItems[1].Text)
                                {
                                    m.Name = textBox2.Text;
                                    m.Type = comboBox2.Text;
                                    break;
                                }
                            }
                            listView2.Items.Clear();
                            foreach (C_Methods m in meths)
                            {
                                ListViewItem item = new ListViewItem(m.Type);
                                item.SubItems.Add(m.Name);
                                listView2.Items.Add(item);
                            }
                            button2.Text = "+";
                            comboBox2.SelectedIndex = -1;
                            textBox2.Text = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                f.Classes[index].draw(g,50,50);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Добавляем переменную в метод
            if (comboBox3.SelectedIndex != -1 && string.IsNullOrEmpty(textBox3.Text) == false && listView2.FocusedItem.Selected && button3.Text == "+")
            {
                C_Variables var = new C_Variables();
                var.Type = comboBox3.Text;
                var.Name = textBox3.Text;
                try
                {
                    if (Check(listView3, listView2, var.Name) == 1)
                    {
                        foreach (C_Methods m in meths)
                        {
                            if (m.Name == listView2.FocusedItem.SubItems[1].Text)
                            {
                                AddVariablesInMethods(m, var);
                                comboBox3.SelectedIndex = -1;
                                textBox3.Text = "";
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (comboBox3.SelectedIndex != -1 && string.IsNullOrEmpty(textBox3.Text) == false && listView2.FocusedItem.Selected && button3.Text == "E")
            {
                try
                {
                    if (Check(listView3, listView2, textBox3.Text) == 1)
                    {
                        DialogResult dialogResult = MessageBox.Show("Сохранить изменения в переменной \"" + listView3.FocusedItem.Text + " "
                                                    + listView3.FocusedItem.SubItems[1].Text + "\" на +\"" +
                                                    comboBox3.Text + " " + textBox3.Text + "\" метода \"" + listView2.FocusedItem.Text + "\" "
                                                    + listView2.FocusedItem.SubItems[1].Text + "?"
                                                    , "Изменение переменной метода", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.Yes)
                        {
                            //Ищем нужный метод
                            foreach (C_Methods m in meths)
                            {
                                if (m.Name == listView2.FocusedItem.SubItems[1].Text)
                                {
                                    //Ищем нужную переменную
                                    foreach (C_Variables v in m.Variables)
                                    {
                                        //Заменяем данные
                                        if (v.Name == listView3.FocusedItem.SubItems[1].Text)
                                        {
                                            v.Name = textBox3.Text;
                                            v.Type = comboBox3.Text;
                                            break;
                                        }
                                    }
                                    //Перезагружаем переменные
                                    listView3.Items.Clear();
                                    foreach (C_Variables v in m.Variables)
                                    {
                                        ListViewItem item = new ListViewItem(v.Type);
                                        item.SubItems.Add(v.Name);
                                        listView3.Items.Add(item);
                                    }
                                    button3.Text = "+";
                                    comboBox3.SelectedIndex = -1;
                                    textBox3.Text = "";
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                f.Classes[index].draw(g,50,50);
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------Обработка смены индекса listView-------------------------------------------------------------
        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Если потеряли фокус - ничего не редактируем
            button2.Text = "+";
            comboBox2.SelectedIndex = -1;
            textBox2.Text = "";
            listView3.Items.Clear();
            try
            {
                //Если есть выбранный элемент
                if (listView2.FocusedItem.Selected)
                {
                    //Находим метод
                    foreach (C_Methods m in meths)
                    {
                        if (m.Name == listView2.FocusedItem.SubItems[1].Text)
                        {
                            //Открываем доступ
                            listView3.Enabled = true;
                            comboBox3.Enabled = true;
                            textBox3.Enabled = true;
                            button3.Enabled = true;
                            //Загружаем список переменных метода
                            foreach (C_Variables v in m.Variables)
                            {
                                ListViewItem item = new ListViewItem(v.Type);
                                item.SubItems.Add(v.Name);
                                listView3.Items.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    //Иначе делаем недоступными
                    comboBox3.SelectedIndex = -1;
                    textBox3.Text = "";
                    listView3.Enabled = false;
                    comboBox3.Enabled = false;
                    textBox3.Enabled = false;
                    button3.Enabled = false;
                }
            }
            catch(Exception ex)
            {
                //...
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Если потеряли фокус - ничего не редактируем
            button1.Text = "+";
            comboBox1.SelectedIndex = -1;
            textBox1.Text = "";
        }

        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Если потеряли фокус - ничего не редактируем
            button3.Text = "+";
            comboBox3.SelectedIndex = -1;
            textBox3.Text = "";
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------Обработка контекстного меню------------------------------------------------------------------
        
        //---------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------Обработка кнопки ToolStrip-------------------------------------------------------------------
        private void добавитьСвойТипДляПеременнойToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Даже не трогай меня больше :) \nNumber of lines of code at the moment: 666.\nNumber after writing the code for this button: ~999");
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void редактироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //Находим нужный элемент, с которого было вызвано контекстное меню и изменяем соответсвующие контролы
                if ((contextMenuStrip1.SourceControl as ListView).Name == listView1.Name)
                {
                    if (listView1.SelectedIndices[0] != -1)
                    {
                        comboBox1.Text = listView1.FocusedItem.Text;
                        textBox1.Text = listView1.FocusedItem.SubItems[1].Text;
                        button1.Text = "E";
                    }
                }
                if ((contextMenuStrip1.SourceControl as ListView).Name == listView2.Name)
                {
                    if (listView2.SelectedIndices[0] != -1)
                    {
                        comboBox2.Text = listView2.FocusedItem.Text;
                        textBox2.Text = listView2.FocusedItem.SubItems[1].Text;
                        button2.Text = "E";
                    }
                }
                if ((contextMenuStrip1.SourceControl as ListView).Name == listView3.Name)
                {
                    if (listView3.SelectedIndices[0] != -1)
                    {
                        comboBox3.Text = listView3.FocusedItem.Text;
                        textBox3.Text = listView3.FocusedItem.SubItems[1].Text;
                        button3.Text = "E";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Та иди ты нахер с такими запросами. Вообще в голове ветер? Выбери ченить, че ты как не мужик..");
            }
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //Находим нужный элемент, с которого было вызвано контекстное меню и изменяем соответсвующие контролы
                if ((contextMenuStrip1.SourceControl as ListView).Name == listView1.Name)
                {
                    if (listView1.SelectedIndices[0] != -1)
                    {
                        DialogResult dialogResult = MessageBox.Show("Удалить переменную \"" + listView1.FocusedItem.Text + " "
                                                                    + listView1.FocusedItem.SubItems[1].Text + "\"?"
                                                                    , "Удаление переменной", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            DeleteVariable(listView1.FocusedItem.SubItems[1].Text);
                        }
                    }
                }
                if ((contextMenuStrip1.SourceControl as ListView).Name == listView2.Name)
                {
                    if (listView2.SelectedIndices[0] != -1)
                    {
                        DialogResult dialogResult = MessageBox.Show("Удалить метод \"" + listView2.FocusedItem.Text + " "
                                                                    + listView2.FocusedItem.SubItems[1].Text + "\"?"
                                                                    , "Удаление метода", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            DeleteMethod(listView2.FocusedItem.SubItems[1].Text);
                        }
                    }
                }
                if ((contextMenuStrip1.SourceControl as ListView).Name == listView3.Name)
                {
                    if (listView3.SelectedIndices[0] != -1)
                    {
                        DialogResult dialogResult = MessageBox.Show("Удалить переменную \"" + listView3.FocusedItem.Text + " "
                                                                    + listView3.FocusedItem.SubItems[1].Text + "\" в методе \"" + listView2.FocusedItem.Text + "\" "
                                                                    + listView2.FocusedItem.SubItems[1].Text + "?"
                                                                    , "Удаление переменной метода", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            DeleteMethodVariables(listView2.FocusedItem.SubItems[1].Text, listView3.FocusedItem.SubItems[1].Text);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Та иди ты нахер с такими запросами. Вообще в голове ветер? Выбери ченить, че ты как не мужик..");
            }
        }

        private void ClassConfigurationForm_Load(object sender, EventArgs e)
        {
            f = this.Owner as UML_diagram_form;
            //Загружаем переменные в форму
            foreach (C_Variables v in f.Classes[index].Variables)
            {
                AddVariables(v);
            }
            //Загружаем методы
            foreach (C_Methods m in f.Classes[index].Methods)
            {
                AddMethods(m);
            }
            g = Graphics.FromImage(bmp);
            f.Classes[index].draw(g,50,50);
            pictureBox1.Image = bmp;
        }
    }
}
