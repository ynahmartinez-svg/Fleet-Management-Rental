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
    public partial class Client_Dashboard : Form
    {
        public Client_Dashboard()
        {
            InitializeComponent();
            this.FormClosed += Client_Dashboard_FormClosed;
        }
        private void Client_Dashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void label33_Click(object sender, EventArgs e)
        {

        }

        private void label40_Click(object sender, EventArgs e)
        {

        }

        private void Client_Dashboard_Load(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            Client_Dashboard cd = new Client_Dashboard();   
            cd.Show();
            this.Hide();
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
            Payments_and_Billing pAB = new Payments_and_Billing();
            pAB.Show();
            this.Hide();    
        }

        private void button3_Click(object sender, EventArgs e)
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
                this.Close();

                Login loginForm = new Login();
                loginForm.ShowDialog();
                this.Hide();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BookingReq br = new BookingReq();
            br.Show();  
            this.Hide();
        }
    }
}
