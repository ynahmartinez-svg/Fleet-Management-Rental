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
    public partial class Client_Notification : Form
    {
        public Client_Notification()
        {
            InitializeComponent();
            this.FormClosed += Client_Notification_FormClosed;
        }

        private void Client_Notification_FormClosed(object sender, FormClosedEventArgs e)
        {
            Client_Dashboard cd = new Client_Dashboard();
            cd.Show();
            this.Hide();
        }
        private void Client_Notification_Load(object sender, EventArgs e)
        {

        }
    }
}
