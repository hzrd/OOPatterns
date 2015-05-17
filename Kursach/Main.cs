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
    public partial class Main : Form
    {
        Code_form cf;
        UML_diagram_form udf;

        public Main()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            cf.Hide();
            udf.Show();
            udf.Refresh();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            udf.Hide();
            cf.Show();
            cf.Refresh();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            cf = new Code_form();
            cf.MdiParent = this;
            cf.Dock = DockStyle.Fill;
            udf = new UML_diagram_form();
            udf.MdiParent = this;
            udf.Dock = DockStyle.Fill;
        }
    }
}
