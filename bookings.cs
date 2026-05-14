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
    public partial class booking : Form
    {
        public booking()
        {
            InitializeComponent();
            this.FormClosed += booking_FormClosed;
        }
        private void booking_FormClosed(object sender, FormClosedEventArgs e)
        {
            Client_Dashboard cd = new Client_Dashboard();
            cd.Show();
            this.Hide();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }





        private void button7_Click(object sender, EventArgs e)
        {
            Client_Account ca = new Client_Account();
            ca.Show();
            this.Hide();
        }



        private void button15_Click(object sender, EventArgs e)
        {
            Booking_Details br = new Booking_Details();
            br.Show();
            this.Hide();
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            Client_Dashboard cd = new Client_Dashboard();
            cd.Show();
            this.Hide();
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            My_rentals r = new My_rentals();
            r.Show();
            this.Hide();
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            Payments_and_Billing pAB = new Payments_and_Billing();
            pAB.Show();
            this.Hide();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            Client_Account ca = new Client_Account();
            ca.Show();
            this.Hide();
        }

        private void button8_Click_1(object sender, EventArgs e)
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
            Booking_Details br = new Booking_Details();
            br.Show();
            this.Hide();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Booking_Details br = new Booking_Details();
            br.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Booking_Details br = new Booking_Details();
            br.Show();
            this.Hide();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Booking_Details br = new Booking_Details();
            br.Show();
            this.Hide();
        }
    }
}
