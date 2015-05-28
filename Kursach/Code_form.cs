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
    public partial class Code_form : Form
    {
        string _pathFileEdit = string.Empty;
        UML_diagram_form udf;
        public Code_form()
        {
            InitializeComponent();
        }

        public void AddFiles(UML_diagram_form _udf)
        {
            listView1.Items.Clear();
            foreach (string _path in _udf._pathToCreatingFile)
            {
                ListViewItem lvi = new ListViewItem(_path.Remove(0,_path.LastIndexOf('\\') + 1));
                lvi.SubItems.Add(_path);
                listView1.Items.Add(lvi);
            }
            udf = _udf;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_pathFileEdit.Length != 0)
            {
                using (StreamWriter sw = new StreamWriter(_pathFileEdit))
                {
                    sw.Write(textBox1.Text);
                }
                udf.logger.Add(new LoggerUserAction(Action.SaveFiles, DateTime.Now, _pathFileEdit.Remove(0, _pathFileEdit.LastIndexOf('\\') + 1)));
            }
            else
            {
                MessageBox.Show("Не выбран файл для редактирования!");
            }
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices[0] != -1)
            {
                textBox1.Text = "";
                _pathFileEdit = listView1.Items[listView1.SelectedIndices[0]].SubItems[1].Text;
                using (StreamReader file = new StreamReader(_pathFileEdit))
                {
                    textBox1.Lines = file.ReadToEnd().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    file.Close();
                }
            }
        }

        private void Code_form_Resize(object sender, EventArgs e)
        {
            button1.Size = new Size(this.Size.Width / 6, button1.Size.Height);
            textBox1.Location = new Point(button1.Size.Width + 10, 5);
            textBox1.Size = new Size((this.Size.Width / 6) * 4, this.Size.Height - 10);
            listView1.Location = new Point(((this.Size.Width / 6) * 5) + 10, 5);
            listView1.Size = new Size((this.Size.Width / 6), this.Size.Height / 2);
            listView1.Columns[0].Width = listView1.Size.Width - 5;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0)
            {
                _pathFileEdit = string.Empty;
            }
        }

    }
}
