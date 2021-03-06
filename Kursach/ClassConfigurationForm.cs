﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Kursach
{
    public partial class ClassConfigurationForm : Form
    {
        Graphics g;
        //Индекс класса, с которым работаем
        int index;
        //Ссылка на форму
        UML_diagram_form f;
        //Рисунок объекта 
        Bitmap bmp;

        //Временный объект для редактирования
        ClassBox temp;

        //Вспомогательная переменная для ввода символов в textBox
        bool backspace = false;

        public ClassConfigurationForm()
        {
            InitializeComponent();
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------Обработка загрузки данных на форму-----------------------------------------------------------
        public ClassConfigurationForm(int ind)
        {
            InitializeComponent();
            index = ind;
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }

        private void ClassConfigurationForm_Load(object sender, EventArgs e)
        {
            f = this.Owner as UML_diagram_form;
            temp = new ClassBox(50,50,100,100);
            temp.Variables.Clear();
            temp.Methods.Clear();
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
            foreach(C_Methods m in f.Classes[index].VirtualMethods)
            {
                AddVirtualMethod(m);
            }
            temp.Name = f.Classes[index].Name;
            temp.Resize();
            temp.X = (pictureBox1.Width / 2) - (temp.Width / 2);
            g = Graphics.FromImage(bmp);
            temp.draw(g);
            textBox4.Text = temp.Name;
            pictureBox1.Image = bmp;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------Добавление переменных и методов------------------------------------------------------------------------
        private bool CheckSystemName(string _Name)
        {
            //Массив системных имен
            string[] systemName = { "system", "using", "namespace", "int", "double", "float", "bool", "char", "string","virtual" };

            foreach (string sN in systemName)
            {
                if (sN == _Name)
                {
                    return false;
                }
            }
            return true;
        }

        private void AddVariables(C_Variables var)
        {
            if (CheckSystemName(var.Name))
            {
                //Добавляем переменную в list
                temp.Variables.Add(var);
                //И на форму
                ListViewItem item = new ListViewItem(var.Type);
                item.SubItems.Add(var.Name);
                listView1.Items.Add(item);
            }
            else
            {
                MessageBox.Show("Имя переменной не должно совпадать с системным именем!");
            }
        }

        private void AddMethods(C_Methods meth)
        {
            //--------------------------------------------------------------------
            if (CheckSystemName(meth.Name))
            {
                //Добавляем метод в list
                temp.Methods.Add(meth);
                //И на форму
                ListViewItem item = new ListViewItem(meth.Type);
                item.SubItems.Add(meth.Name);
                listView2.Items.Add(item);
            }
            else
            {
                MessageBox.Show("Имя метода не должно совпадать с системным именем!");
            }
        }

        private void AddVariablesInMethods(C_Methods meth,C_Variables var)
        {
            if (CheckSystemName(var.Name))
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
                MessageBox.Show("Имя переменной не должно совпадать с системным именем!");
            }
        }

        private void AddVirtualMethod(C_Methods meth)
        {
            if (CheckSystemName(meth.Name))
            {
                //Добавляем метод в list
                temp.VirtualMethods.Add(meth);
                //И на форму
                ListViewItem item = new ListViewItem(meth.Type);
                item.SubItems.Add(meth.Name);
                listView2.Items.Add(item);
                checkBox1.Checked = true;
            }
            else
            {
                MessageBox.Show("Имя метода не должно совпадать с системным именем!");
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------Проверка на совпадение----------------------------------------------------------------------
        private int CheckList(ListView lv1, ListView lv2,string sName)
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
        private void EnabledListView3()
        {
            listView3.Items.Clear();
            comboBox3.SelectedIndex = -1;
            textBox3.Text = "";
            listView3.Enabled = false;
            comboBox3.Enabled = false;
            textBox3.Enabled = false;
            button3.Enabled = false;
        }
        private void Delete(object sender)
        {
            try
            {
                //Находим нужный элемент, с которого было вызвано контекстное меню и изменяем соответсвующие контролы
                if ((sender as ListView).Name == listView1.Name)
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
                if ((sender as ListView).Name == listView2.Name)
                {
                    if (listView2.SelectedIndices[0] != -1)
                    {
                        DialogResult dialogResult = MessageBox.Show("Удалить метод \"" + listView2.FocusedItem.Text + " "
                                                                    + listView2.FocusedItem.SubItems[1].Text + "\"?"
                                                                    , "Удаление метода", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            DeleteMethod(listView2.FocusedItem.SubItems[1].Text);
                            EnabledListView3();
                        }
                    }
                }
                if ((sender as ListView).Name == listView3.Name)
                {
                    if (listView3.SelectedIndices[0] != -1)
                    {
                        DialogResult dialogResult = MessageBox.Show("Удалить переменную \"" + listView3.FocusedItem.Text + " "
                                                                    + listView3.FocusedItem.SubItems[1].Text + "\" в методе \"" + listView2.FocusedItem.Text + "\" "
                                                                    + listView2.FocusedItem.SubItems[1].Text + "?"
                                                                    , "Удаление переменной метода", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            if (checkBox1.Checked)
                            {
                                DeleteMethodVariables(listView2.FocusedItem.SubItems[1].Text, listView3.FocusedItem.SubItems[1].Text, true);
                            }
                            else
                            {
                                DeleteMethodVariables(listView2.FocusedItem.SubItems[1].Text, listView3.FocusedItem.SubItems[1].Text, false);
                            }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Не выбран элемент для удаления!");
            }
        }
        
        private void DeleteVariable(string sNameVariable)
        {
            //Убираем ненужную переменную
            temp.Variables.RemoveAt(temp.Variables.Count - 1);
            //Перезагружаем переменные
            listView1.Items.Clear();
            foreach (C_Variables v in temp.Variables)
            {
                ListViewItem item = new ListViewItem(v.Type);
                item.SubItems.Add(v.Name);
                listView1.Items.Add(item);
            }
        }

        private void DeleteMethod(string sNameMethod)
        {
            //Убираем ненужный метод
            for (int i = 0; i < temp.Methods.Count; i++)
            {
                if (temp.Methods[i].Name == sNameMethod)
                {
                    temp.Methods.RemoveAt(i);
                    break;
                }
            }
            //Перезагружаем методы
            listView2.Items.Clear();
            foreach (C_Methods m in temp.Methods)
            {
                ListViewItem item = new ListViewItem(m.Type);
                item.SubItems.Add(m.Name);
                listView2.Items.Add(item);
            }
        }

        private void DeleteVirtualMethod(string sNameMethod)
        {
            //Убираем ненужный метод
            for (int i = 0; i < temp.VirtualMethods.Count; i++)
            {
                if (temp.VirtualMethods[i].Name == sNameMethod)
                {
                    temp.VirtualMethods.RemoveAt(i);
                    break;
                }
            }
            //Перезагружаем методы
            listView2.Items.Clear();
            foreach (C_Methods m in temp.VirtualMethods)
            {
                ListViewItem item = new ListViewItem(m.Type);
                item.SubItems.Add(m.Name);
                listView2.Items.Add(item);
            }
        }

        private void DeleteMethodVariables(string sNameMethod, string sNameVariable, bool _virtual)
        {
            int numberMethod = 0;
            int numberVariable = 0;
            //Находим нужный метод в массиве
            if (!_virtual)
            {
                for (int i = 0; i < temp.Methods.Count; i++)
                {
                    if (temp.Methods[i].Name == sNameMethod)
                    {
                        numberMethod = i;
                        break;
                    }
                }
                //В данном методе находим нужную переменную
                for (int i = 0; i < temp.Methods[numberMethod].Variables.Count; i++)
                {
                    if (temp.Methods[numberMethod].Variables[i].Name == sNameVariable)
                    {
                        numberVariable = i;
                        break;
                    }
                }
                //Удаляем данную переменную
                temp.Methods[numberMethod].DeleteVariable(temp.Methods[numberMethod].Variables[numberVariable]);
                //Перезагружаем данные
                listView3.Items.Clear();
                foreach (C_Variables v in temp.Methods[numberMethod].Variables)
                {
                    ListViewItem item = new ListViewItem(v.Type);
                    item.SubItems.Add(v.Name);
                    listView3.Items.Add(item);
                }
            }
            else
            {
                for (int i = 0; i < temp.VirtualMethods.Count; i++)
                {
                    if (temp.VirtualMethods[i].Name == sNameMethod)
                    {
                        numberMethod = i;
                        break;
                    }
                }
                //В данном методе находим нужную переменную
                for (int i = 0; i < temp.VirtualMethods[numberMethod].Variables.Count; i++)
                {
                    if (temp.VirtualMethods[numberMethod].Variables[i].Name == sNameVariable)
                    {
                        numberVariable = i;
                        break;
                    }
                }
                //Удаляем данную переменную
                temp.VirtualMethods[numberMethod].DeleteVariable(temp.VirtualMethods[numberMethod].Variables[numberVariable]);
                //Перезагружаем данные
                listView3.Items.Clear();
                foreach (C_Variables v in temp.VirtualMethods[numberMethod].Variables)
                {
                    ListViewItem item = new ListViewItem(v.Type);
                    item.SubItems.Add(v.Name);
                    listView3.Items.Add(item);
                }
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
                    if (CheckList(listView1, listView2, var.Name) == 1)
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
                    if (CheckList(listView1, listView2, textBox1.Text) == 1)
                    {
                        DialogResult dialogResult = MessageBox.Show("Сохранить изменения в переменной \"" + listView1.FocusedItem.Text + " "
                                            + listView1.FocusedItem.SubItems[1].Text + "\" на +\"" +
                                            comboBox1.Text + " " + textBox1.Text + "\"?"
                                            , "Изменение переменной", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.Yes)
                        {
                            //Находим нужную переменную
                            foreach (C_Variables v in temp.Variables)
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
                            foreach (C_Variables v in temp.Variables)
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
            g.Clear(Color.Silver);
            temp.draw(g);
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
                    if (CheckList(listView1, listView2, meth.Name) == 1)
                    {
                        if (checkBox1.Checked)
                        {
                            AddVirtualMethod(meth);
                        }
                        else
                        {
                            AddMethods(meth);
                        }
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
                    if (CheckList(listView1, listView2, textBox2.Text) == 1)
                    {
                        DialogResult dialogResult = MessageBox.Show("Сохранить изменения в методе \"" + listView2.FocusedItem.Text + " "
                                                    + listView2.FocusedItem.SubItems[1].Text + "\" на +\"" +
                                                    comboBox2.Text + " " + textBox2.Text + "\"?"
                                                    , "Изменение метода", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.Yes)
                        {
                            if (checkBox1.Checked)
                            {
                                foreach (C_Methods m in temp.VirtualMethods)
                                {
                                    if (m.Name == listView2.FocusedItem.SubItems[1].Text)
                                    {
                                        m.Name = textBox2.Text;
                                        m.Type = comboBox2.Text;
                                        break;
                                    }
                                }
                                listView2.Items.Clear();
                                foreach (C_Methods m in temp.VirtualMethods)
                                {
                                    ListViewItem item = new ListViewItem(m.Type);
                                    item.SubItems.Add(m.Name);
                                    listView2.Items.Add(item);
                                }
                            }
                            else
                            {
                                foreach (C_Methods m in temp.Methods)
                                {
                                    if (m.Name == listView2.FocusedItem.SubItems[1].Text)
                                    {
                                        m.Name = textBox2.Text;
                                        m.Type = comboBox2.Text;
                                        break;
                                    }
                                }
                                listView2.Items.Clear();
                                foreach (C_Methods m in temp.Methods)
                                {
                                    ListViewItem item = new ListViewItem(m.Type);
                                    item.SubItems.Add(m.Name);
                                    listView2.Items.Add(item);
                                }
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
            }
            g = pictureBox1.CreateGraphics();
            g.Clear(Color.Silver);
            temp.draw(g);
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
                    if (CheckList(listView3, listView2, var.Name) == 1)
                    {
                        if (checkBox1.Checked)
                        {
                            foreach (C_Methods m in temp.VirtualMethods)
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
                        else
                        {
                            foreach (C_Methods m in temp.Methods)
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
                    if (CheckList(listView3, listView2, textBox3.Text) == 1)
                    {
                        DialogResult dialogResult = MessageBox.Show("Сохранить изменения в переменной \"" + listView3.FocusedItem.Text + " "
                                                    + listView3.FocusedItem.SubItems[1].Text + "\" на +\"" +
                                                    comboBox3.Text + " " + textBox3.Text + "\" метода \"" + listView2.FocusedItem.Text + "\" "
                                                    + listView2.FocusedItem.SubItems[1].Text + "?"
                                                    , "Изменение переменной метода", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.Yes)
                        {
                            //Ищем нужный метод
                            if (checkBox1.Checked)
                            {
                                foreach (C_Methods m in temp.VirtualMethods)
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
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (C_Methods m in temp.Methods)
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
                                            break;
                                        }
                                    }
                                }
                            }
                            button3.Text = "+";
                            comboBox3.SelectedIndex = -1;
                            textBox3.Text = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                g = pictureBox1.CreateGraphics();
                g.Clear(Color.Silver);
                temp.draw(g);
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
                    if (checkBox1.Checked)
                    {
                        foreach (C_Methods m in temp.VirtualMethods)
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
                        foreach (C_Methods m in temp.Methods)
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
                }
                else
                {
                    //Иначе делаем недоступными
                    EnabledListView3();
                }
            }
            catch
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
        //------------------------------------------------------Обработка кнопки ToolStrip-------------------------------------------------------------------
        private void добавитьСвойТипДляПеременнойToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Даже не трогай меня больше :) \nNumber of lines of code at the moment: 950.\nNumber after writing the code for this button: ~999.\nComming soon:)");
        }

        private void сохранитьToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Save();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------Обработка контекстного меню------------------------------------------------------------------
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
            catch
            {
                MessageBox.Show("Та иди ты нахер с такими запросами. Вообще в голове ветер? Выбери ченить, че ты как не мужик..");
            }
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Delete(contextMenuStrip1.SourceControl);
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------Обработка ввода имени переменной/метода----------------------------------------------------------------------
        private void CheckSymbol(KeyPressEventArgs e, int length)
        {
            //Массив с запрещенными символами в начале имени
            char[] firstSymbol = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '#','.',',','\\',
                               '/','|','-','*','+','@','"','№',';','$','%','?','(',')','<','>','`',
                               '~','^',':','№','!'};
            Regex pattern = new Regex("[a-z0-9_]", RegexOptions.IgnoreCase);
            //if (length > 15)
            //{
            //    e.Handled = true;
            //    return;
            //}
            if (length == 0)
            {
                foreach (char c in firstSymbol)
                {
                    if (c == e.KeyChar || !pattern.IsMatch(e.KeyChar.ToString()))
                    {
                        e.Handled = true;
                    }
                }
            }
            else
                if (!pattern.IsMatch(e.KeyChar.ToString()))
                {
                    if (!backspace)
                    {
                        e.Handled = true;
                    }
                }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckSymbol(e, textBox1.Text.Length);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckSymbol(e, textBox2.Text.Length);
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckSymbol(e, textBox3.Text.Length);
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckSymbol(e, textBox4.Text.Length);
        }

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            backspace = false;
            if (e.KeyData == Keys.Back)
            {
                backspace = true;
            }
        }

        private void textBox2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            backspace = false;
            if (e.KeyData == Keys.Back)
            {
                backspace = true;
            }
        }

        private void textBox3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            backspace = false;
            if (e.KeyData == Keys.Back)
            {
                backspace = true;
            }
        }

        private void textBox4_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            backspace = false;
            if (e.KeyData == Keys.Back)
            {
                backspace = true;
            }
            if (e.Control == true && e.KeyCode == Keys.S)
            {
                Save();
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------Обработка нажатия Del для удаления имени, переменной/метода---------------------------------------------------
        private void listView1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                Delete(sender);
            }
            if (e.Control == true && e.KeyCode == Keys.S)
            {
                Save();
            }
        }

        private void listView2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                Delete(sender);
            }
            if (e.Control == true && e.KeyCode == Keys.S)
            {
                Save();
            }
        }

        private void listView3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                Delete(sender);
            }
            if (e.Control == true && e.KeyCode == Keys.S)
            {
                Save();
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------Имя, виртуальные методы---------------------------------------------------------------------------
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            temp.Name = textBox4.Text;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            if (checkBox1.Checked)
            {
                foreach (C_Methods m in temp.VirtualMethods)
                {
                    ListViewItem item = new ListViewItem(m.Type);
                    item.SubItems.Add(m.Name);
                    listView2.Items.Add(item);
                }
            }
            else
            {
                foreach (C_Methods m in temp.Methods)
                {
                    ListViewItem item = new ListViewItem(m.Type);
                    item.SubItems.Add(m.Name);
                    listView2.Items.Add(item);
                }
            }
            EnabledListView3();
        }

        private void Save()
        {
            f.Classes[index].Methods = temp.Methods;
            f.Classes[index].Variables = temp.Variables;
            f.Classes[index].VirtualMethods = temp.VirtualMethods;
            f.Classes[index].Name = temp.Name;
            f.Redraw();
            this.Close();
        }
    }
}
