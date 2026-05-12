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
    public partial class Active_Client : Form
    {
        public Active_Client()
        {
            InitializeComponent();
            this.FormClosed += Active_Client_FormClosed;
        }
        private void Active_Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            Client_Dashboard cd = new Client_Dashboard();
            cd.Show();
            this.Hide();
        }
        private void Active_Client_Load(object sender, EventArgs e)
        {

        }
    }
}
