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
    public partial class My_rentals : Form
    {
        public My_rentals()
        {
            InitializeComponent();
            this.FormClosed += My_rentals_FormClosed;

        }
        private void My_rentals_FormClosed(object sender, FormClosedEventArgs e)
        {
           
              Client_Dashboard cd = new Client_Dashboard();
               cd.Show();
               this.Hide();
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void My_rentals_Load(object sender, EventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
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

        private void button17_Click(object sender, EventArgs e)
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

        private void button5_Click(object sender, EventArgs e)
        {
            Completed_Client cc = new Completed_Client();
            cc.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Upcoming_Client uc = new Upcoming_Client();
            uc.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Active_Client ac = new Active_Client();
            ac.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Booking_Details bd = new Booking_Details();
            bd.Show();
            this.Hide();
        }
    }
}
