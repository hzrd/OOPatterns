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
    public partial class Authorization : Form
    {
        Main m;
        public Authorization()
        {
            InitializeComponent();
        }

        public Authorization(Main _mainForm)
        {
            InitializeComponent();
            m = _mainForm;
        }

        private void Authorization_FormClosed(object sender, FormClosedEventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "Admin" || textBox2.Text != "Admin")
            {
                MessageBox.Show("Данные введены не верно!");
            }
            else
            {
                m.admin = true;
                MessageBox.Show("Вы вошли под администратором. Теперь вам доступны функции редактирования и добавления шаблонов.");
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
