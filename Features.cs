using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fleet_Management_Rental
{
    public partial class Features : Form
    {
        public Features()
        {
            InitializeComponent();
            this.FormClosed += Features_FormClosed;
        }

        private void Features_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void Features_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            driveSphere_R dsr = new driveSphere_R();
            dsr.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Features ft = new Features();
            ft.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AboutUs abt = new AboutUs();
            abt.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Login f4 = new Login();
            f4.Show();
            this.Hide();
        }
    }
}
