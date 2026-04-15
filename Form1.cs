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
    public partial class driveSphere_R : Form
    {
        public driveSphere_R()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Login f4 = new Login();
            f4.Show();
            this.Hide();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
