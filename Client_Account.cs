using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Fleet_Management_Rental
{
    public partial class Client_Account : Form
    {
        public Client_Account()
        {
            InitializeComponent();
            this.FormClosed += Client_Account_FormClosed;
        }
        private void Client_Account_FormClosed(object sender, FormClosedEventArgs e)
        {
            Client_Dashboard cd = new Client_Dashboard();
            cd.Show();  
            this.Hide();
        }

        private void Client_Account_Load(object sender, EventArgs e)
        {

        }
    }
}
