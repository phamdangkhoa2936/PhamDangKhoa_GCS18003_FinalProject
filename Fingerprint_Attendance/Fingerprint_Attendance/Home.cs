using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fingerprint_Attendance
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Check_Attendance tform3 = new Check_Attendance();
            tform3.Show();

            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Registration tform2 = new Registration();
            tform2.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            Registration tform2 = new Registration();
            tform2.Close();
            Check_Attendance tform3 = new Check_Attendance();
            tform3.Close();
            Environment.Exit(1);
        }

        private void Home_Load(object sender, EventArgs e)
        {

        }
    }
}
