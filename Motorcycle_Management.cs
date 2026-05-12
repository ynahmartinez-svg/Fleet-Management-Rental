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
    public partial class Motorcycle_Management : Form
    {
        public Motorcycle_Management()
        {
            InitializeComponent();
            this.FormClosed += Motorcycle_Management_FormClosed;

        }
        private void Motorcycle_Management_FormClosed(object sender, FormClosedEventArgs e)
        {
            Admin_DashBoard ad = new Admin_DashBoard();
            ad.Show();
            this.Hide();
        }

        private void Motorcycle_Management_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox7_Click(object sender, EventArgs e)
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

        private void button15_Click(object sender, EventArgs e)
        {
            Admin_Notification an = new Admin_Notification();
            an.Show();
            this.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Admin_Accounts aa = new Admin_Accounts();
            aa.Show();
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

        private void button2_Click(object sender, EventArgs e)
        {
            Motorcycle_management2 mm2 = new Motorcycle_management2();
            mm2.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Motorcycle_management2 mm2 = new Motorcycle_management2();
            mm2.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Motorcycle_management2 mm2 = new Motorcycle_management2();
            mm2.Show();
            this.Hide();
        }
    }
}
