using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursach
{
    public partial class Main : Form
    {
        Code_form cf;
        UML_diagram_form udf;
        bool _eng = true;

        public Main()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            cf.Hide();
            udf.Show();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            udf.Hide();
            cf.AddFiles(udf);
            cf.Show();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            cf = new Code_form();
            cf.MdiParent = this;
            cf.Dock = DockStyle.Fill;
            udf = new UML_diagram_form();
            udf.MdiParent = this;
            udf.Dock = DockStyle.Fill;
            Language("eng");
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            string temp = string.Empty;
            foreach(LoggerUserAction lua in udf.logger)
            {
                temp += lua.Info()+"\n";
            }
            if (temp.Length != 0)
                MessageBox.Show(temp);
        }

        private void Language(string l)
        {
            string[] eng = 
            {   
                "UML-diagramm editor",
                "Code editor",
                "Log",
                "Settings",
                "Add class",
                "Save into files",
                "Load files",
                "Add Agregation",
                "Delete",
                "Delete Agregation",
                "Save",
                "Creating files"
            };
            string[] rus =
            {
                "UML-редактор диаграмм",
                "Редактор кода",
                "Лог",
                "Настройки",
                "Добавить класс",
                "Сохранить в файлы",
                "Загрузить файлы",
                "Добавить агрегацию",
                "Удалить",
                "Удалить агрегацию",
                "Сохранить",
                "Созданные файлы"
            };
            string[] temp;
            if (l == "eng")
            {
                temp = eng;
                _eng = true;
            }
            else
            {
                temp = rus;
                _eng = false;
            }
            this.toolStripButton1.Text = temp[0];
            this.toolStripButton2.Text = temp[1];
            this.toolStripButton3.Text = temp[2];
            this.toolStripButton4.Text = temp[3];

            udf.toolStripButton1.Text = temp[4];
            udf.toolStripButton2.Text = temp[5];
            udf.toolStripButton3.Text = temp[6];
            udf.addAgregationToolStripMenuItem.Text = temp[7];
            udf.deleteToolStripMenuItem.Text = temp[8];
            udf.deleteAgregationToolStripMenuItem.Text = temp[9];

            cf.button1.Text = temp[10];
            cf.listView1.Columns[0].Text = temp[11];

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (_eng)
            {
                Language("rus");
            }
            else
            {
                Language("eng");
            }
        }
    }
}
