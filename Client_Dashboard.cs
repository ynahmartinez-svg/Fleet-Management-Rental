using Npgsql;
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
            if (this.Owner != null)
            {
                this.Owner.Show();
            }
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
            long clientId = SessionData.LoggedInClientId;

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                // 🔧 Auto-transition Approved → Active if start_date is today or earlier
                string sqlUpdate = @"UPDATE rentals
                             SET status = 'Active'
                             WHERE client_id = @cid
                               AND status = 'Approved'
                               AND start_date <= CURRENT_DATE";
                using (var cmdUpdate = new NpgsqlCommand(sqlUpdate, conn))
                {
                    cmdUpdate.Parameters.AddWithValue("@cid", clientId);
                    cmdUpdate.ExecuteNonQuery();
                }

                // Total rentals for this client
                string sqlTotal = "SELECT COUNT(*) FROM rentals WHERE client_id = @cid";
                using (var cmdTotal = new NpgsqlCommand(sqlTotal, conn))
                {
                    cmdTotal.Parameters.AddWithValue("@cid", clientId);
                    lblTotal.Text = Convert.ToInt32(cmdTotal.ExecuteScalar()).ToString();
                }

                // Total motorcycles available
                string sqlAvailable = "SELECT COUNT(*) FROM motorcycle_management WHERE status = 'Available'";
                using (var cmdAvail = new NpgsqlCommand(sqlAvailable, conn))
                {
                    lblActive.Text = Convert.ToInt32(cmdAvail.ExecuteScalar()).ToString();
                }

                // Current rental details (Active or Approved)
                string sqlCurrent = @"SELECT m.model_name, m.plate_num, r.start_date, r.return_date, r.status, m.image_path
                              FROM rentals r
                              JOIN motorcycle_management m ON r.motorcycle_id = m.motorcycle_id
                              WHERE r.client_id = @cid 
                                AND (r.status = 'Active' OR r.status = 'Approved')
                              ORDER BY r.start_date ASC
                              LIMIT 1";

                using (var cmd = new NpgsqlCommand(sqlCurrent, conn))
                {
                    cmd.Parameters.AddWithValue("@cid", clientId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblModel.Text = "Model: " + reader.GetString(0);
                            lblPlate.Text = "Plate No.: " + reader.GetString(1);
                            lblStart.Text = "Start: " + reader.GetDateTime(2).ToShortDateString();
                            lblReturn.Text = "Return: " + reader.GetDateTime(3).ToShortDateString();
                            lblStatus.Text = "Status: " + reader.GetString(4);

                            string imagePath = reader.GetString(5);
                            if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                                pictureMotor.Image = Image.FromFile(imagePath);
                            else
                                pictureMotor.BackColor = Color.LightGray;
                        }
                        else
                        {
                            lblModel.Text = "Model: none";
                            lblPlate.Text = "Plate No.: none";
                            lblStart.Text = "Start: none";
                            lblReturn.Text = "Return: none";
                            lblStatus.Text = "Status: none";
                            pictureMotor.BackColor = Color.LightGray;
                        }
                    }
                }

                // Most rented sample buttons
                string sqlMio = "SELECT motorcycle_id FROM motorcycle_management WHERE model_name ILIKE '%MIO%' LIMIT 1";
                using (var cmdMio = new NpgsqlCommand(sqlMio, conn))
                {
                    var result = cmdMio.ExecuteScalar();
                    if (result != null)
                    {
                        btnRent1.Tag = Convert.ToInt64(result);
                    }
                }

                string sqlKeeway = "SELECT motorcycle_id FROM motorcycle_management WHERE model_name ILIKE '%KEEWAY%' LIMIT 1";
                using (var cmdKeeway = new NpgsqlCommand(sqlKeeway, conn))
                {
                    var result = cmdKeeway.ExecuteScalar();
                    if (result != null)
                    {
                        btnRent2.Tag = Convert.ToInt64(result);
                    }
                }
            }
        }


        private void button10_Click(object sender, EventArgs e)
        {
            Client_Dashboard cd = new Client_Dashboard();   
            cd.Show();
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
           
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (((Button)sender).Tag != null)
            {
                long motorcycleId = Convert.ToInt64(((Button)sender).Tag);
                long clientId = SessionData.LoggedInClientId; // always logged-in client

                Booking_Details bd = new Booking_Details(motorcycleId, clientId);
                bd.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("No motorcycle available for booking.",
                                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            My_rentals r = new My_rentals();
            r.Show();
            this.Hide();
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            booking b = new booking();
            b.Show();
            this.Hide();
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            Payments_and_Billing PB = new Payments_and_Billing();
            PB.Show();
            this.Hide();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            Client_Account ca = new Client_Account();
            this.Hide(); 
            ca.ShowDialog();
            this.Show();
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                       "Do you want to log out?", "Logout Confirmation",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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

        private void button10_Click_1(object sender, EventArgs e)
        {

        }

        private void btnRent2_Click(object sender, EventArgs e)
        {
            if (((Button)sender).Tag != null)
            {
                long motorcycleId = Convert.ToInt64(((Button)sender).Tag);
                long clientId = SessionData.LoggedInClientId; // always logged-in client

                Booking_Details bd = new Booking_Details(motorcycleId, clientId);
                bd.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("No motorcycle available for booking.",
                                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void btnNotification_Click(object sender, EventArgs e)
        {
            Client_Notification cn = new Client_Notification();
            cn.Show();
            this.Hide();
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnMap_Click(object sender, EventArgs e)
        {
            Client_Map map = new Client_Map();
            map.Show();
            this.Hide();
        }
    }
}
