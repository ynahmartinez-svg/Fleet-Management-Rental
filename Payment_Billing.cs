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
    public partial class Payment_Billing : Form
    {
        public Payment_Billing()
        {
            InitializeComponent();
            this.FormClosed += Payment_Billing_FormClosed;
        }
        private void Payment_Billing_FormClosed(object sender, FormClosedEventArgs e)
        {
            Admin_DashBoard ad = new Admin_DashBoard();
            ad.Show();
            this.Hide();
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void Payment_Billing_Load(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            Admin_DashBoard ad = new Admin_DashBoard();
            ad.Show();
            this.Hide();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            User_Management um = new User_Management();
            um.Show();
            this.Hide();    
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Motorcycle_Management mm = new Motorcycle_Management();
            mm.Show();
            this.Hide();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Rental_Management rm = new Rental_Management();
            rm.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Payment_Billing pb = new Payment_Billing(); 
            pb.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Reports_Analytics1 ra = new Reports_Analytics1();
            ra.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Admin_Accounts aa = new Admin_Accounts();
            aa.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Admin_Notification an = new Admin_Notification();
            an.Show();
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
    }
}
