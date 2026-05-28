using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;


namespace Fleet_Management_Rental
{
    public partial class Client_Map : Form
    {
        public Client_Map()
        {
            InitializeComponent();
            this.FormClosed += Client_Map_FormClosed;
        }

        private void Client_Map_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Client_Map_Load(object sender, EventArgs e)
        {
            string path = System.IO.Path.Combine(Application.StartupPath, "philippines_map.html");
            webMap.Source = new Uri(@"C:\Users\user\source\repos\Fleet-Management-Rental\philippines_map.html");

            webMap.WebMessageReceived += (s, ev) =>
            {
                var coords = ev.WebMessageAsJson;
            };
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Client_Dashboard client_Dashboard = new Client_Dashboard();
            client_Dashboard.Show();
            this.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Do you want to log out?",
                "Logout Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                MessageBox.Show("Logged out successfully!");

                Login loginForm = new Login();
                loginForm.Show();

                this.Dispose();
            }
            else if (result == DialogResult.No)
            {

            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            My_rentals r = new My_rentals();
            r.Show();
            this.Hide();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            booking b = new booking();
            b.Show();
            this.Hide();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Payments_and_Billing PB = new Payments_and_Billing();
            PB.Show();
            this.Hide();
        }

        private void btnNotification_Click(object sender, EventArgs e)
        {
            Client_Notification cn = new Client_Notification();
            cn.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Client_Account ca = new Client_Account();
            ca.Show();
            this.Hide();
        }
    }
}
