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
    public partial class Admin_Accounts : Form
    {
        public Admin_Accounts()
        {
            InitializeComponent();
            this.FormClosed += Admin_Accounts_FormClosed;
        }

        private void Admin_Accounts_FormClosed(object sender, FormClosedEventArgs e)
        {
            Admin_DashBoard ad = new Admin_DashBoard();
            ad.Show();
            this.Hide();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void Admin_Accounts_Load(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {
            User_Management um = new User_Management();
            um.Show();
            this.Hide();
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            Motorcycle_Management mm = new Motorcycle_Management();
            mm.Show();
            this.Hide();
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            Admin_DashBoard ad = new Admin_DashBoard();
            ad.Show();
            this.Hide();
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            Rental_Management rm = new Rental_Management();
            rm.Show();
            this.Hide();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            BookingReq bq = new BookingReq();
            bq.Show();
            this.Hide();
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            Payment_Billing pb = new Payment_Billing();
            pb.Show();
            this.Hide();
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            Reports_Analytics1 ra = new Reports_Analytics1();
            ra.Show();
            this.Hide();
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
            "Do you want to log out?", "Logout Confirmation",
            MessageBoxButtons.YesNo,MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                MessageBox.Show("Logged out successfully!");

                Login loginForm = new Login();
                this.Hide();
                loginForm.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Logout cancelled.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
