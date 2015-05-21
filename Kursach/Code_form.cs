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
        public Code_form()
        {
            InitializeComponent();
        }

        public void AddFiles(UML_diagram_form udf)
        {
            listView1.Items.Clear();
            foreach (string _path in udf._pathToCreatingFile)
            {
                ListViewItem lvi = new ListViewItem(_path.Remove(0,_path.LastIndexOf('\\') + 1));
                lvi.SubItems.Add(_path);
                listView1.Items.Add(lvi);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void listView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices[0] != -1)
            {
                textBox1.Text = "";
                using (StreamReader file = new StreamReader(listView1.Items[listView1.SelectedIndices[0]].SubItems[1].Text))
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

    }
}
