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
        }
        private void Client_Dashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
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

                // Transition Approved → Active ONLY if start_date = today
                string sqlUpdate = @"UPDATE rentals
                     SET status = 'Active'
                     WHERE client_id = @cid
                       AND status = 'Approved'
                       AND start_date = CURRENT_DATE";
                using (var cmdUpdate = new NpgsqlCommand(sqlUpdate, conn))
                {
                    cmdUpdate.Parameters.AddWithValue("@cid", clientId);
                    cmdUpdate.ExecuteNonQuery();
                }

                // Total rentals for this client (exclude deleted/cancelled if needed)
                string sqlTotal = @"SELECT COUNT(*) 
                    FROM rentals 
                    WHERE client_id = @cid 
                    AND status = 'Active'";
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

                string sqlComplete = @"UPDATE rentals
                       SET status = 'Completed'
                       WHERE client_id = @cid
                         AND status = 'Active'
                         AND return_date < CURRENT_DATE;

                       UPDATE motorcycle_management
                       SET status = 'Available'
                       WHERE motorcycle_id IN (
                           SELECT motorcycle_id
                           FROM rentals
                           WHERE client_id = @cid
                             AND status = 'Completed'
                             AND return_date < CURRENT_DATE
                       );";

                using (var cmdComplete = new NpgsqlCommand(sqlComplete, conn))
                {
                    cmdComplete.Parameters.AddWithValue("@cid", clientId);
                    cmdComplete.ExecuteNonQuery();
                }


                // Current rental (Active today)
                string sqlCurrent = @"SELECT m.model_name, m.plate_num, r.start_date, r.return_date, r.status, m.image_path
              FROM rentals r
              JOIN motorcycle_management m ON r.motorcycle_id = m.motorcycle_id
              WHERE r.client_id = @cid 
                AND r.status = 'Active'
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

                //  Upcoming rental (Approved but future start_date)
                string sqlUpcoming = @"SELECT m.model_name, r.start_date, r.return_date
                       FROM rentals r
                       JOIN motorcycle_management m ON r.motorcycle_id = m.motorcycle_id
                       WHERE r.client_id = @cid 
                         AND r.status = 'Approved'
                         AND r.start_date > CURRENT_DATE
                       ORDER BY r.start_date ASC
                       LIMIT 1";

                using (var cmdUpcoming = new NpgsqlCommand(sqlUpcoming, conn))
                {
                    cmdUpcoming.Parameters.AddWithValue("@cid", clientId);
                    using (var reader = cmdUpcoming.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblUpcoming.Text = $"Upcoming: {reader.GetString(0)} " +
                                               $"({reader.GetDateTime(1).ToShortDateString()} → {reader.GetDateTime(2).ToShortDateString()})";
                        }
                        else
                        {
                            lblUpcoming.Text = "Upcoming: none";
                        }
                    }
                }
                string sql = @"SELECT motorcycle_id, model_name
               FROM motorcycle_management
               WHERE status = 'Available'
               LIMIT 2";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        btnRent1.Tag = reader.GetInt64(0);
                    }
                    if (reader.Read())
                    {
                        btnRent2.Tag = reader.GetInt64(0);
                    }
                }
            }
        }

        private bool IsClientProfileComplete(long clientId)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT first_name, last_name, date_of_birth, phone_no, valid_id,
                       street, city, postal_code
                FROM clientprofile
                WHERE client_id = @cid";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cid", clientId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Check if any required field is empty/null
                            return !(reader.IsDBNull(0) || reader.IsDBNull(1) || reader.IsDBNull(2) ||
                                     reader.IsDBNull(3) || reader.IsDBNull(4) ||
                                     reader.IsDBNull(5) || reader.IsDBNull(6) || reader.IsDBNull(7));
                        }
                    }
                }
            }
            return false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
        }


        private void button1_Click(object sender, EventArgs e)
        {
            long clientId = SessionData.LoggedInClientId;

            if (!IsClientProfileComplete(clientId))
            {
                MessageBox.Show("Please complete your profile information before renting a motorcycle.",
                                "Profile Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            long motorcycleId = 0;
            long rentalId = 0;
            bool isExtension = false;

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                // Query Mio motorcycle_id
                string sqlMio = "SELECT motorcycle_id FROM motorcycle_management WHERE model_name ILIKE '%MIO%' LIMIT 1";
                using (var cmdMio = new NpgsqlCommand(sqlMio, conn))
                {
                    var result = cmdMio.ExecuteScalar();
                    if (result != null)
                        motorcycleId = Convert.ToInt64(result);
                }

                if (motorcycleId > 0)
                {
                    // Check if client already has an active rental for this motorcycle
                    string sqlRental = @"SELECT rental_id 
                                 FROM rentals 
                                 WHERE client_id = @cid 
                                   AND motorcycle_id = @mid 
                                   AND status = 'Approved'";
                    using (var cmdRental = new NpgsqlCommand(sqlRental, conn))
                    {
                        cmdRental.Parameters.AddWithValue("@cid", clientId);
                        cmdRental.Parameters.AddWithValue("@mid", motorcycleId);

                        var result = cmdRental.ExecuteScalar();
                        if (result != null)
                        {
                            rentalId = Convert.ToInt64(result);
                            isExtension = true;
                        }
                    }
                }
            }

            if (motorcycleId > 0)
            {
                Booking_Details bd = new Booking_Details(motorcycleId, clientId, rentalId, isExtension);
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

                Login loginForm = new Login();
                loginForm.Show();

                this.Dispose();
            }
            else if (result == DialogResult.No)
            {

            }
        }

        private void button10_Click_1(object sender, EventArgs e)
        {

        }

        private void btnRent2_Click(object sender, EventArgs e)
        {
            long clientId = SessionData.LoggedInClientId;

            if (!IsClientProfileComplete(clientId))
            {
                MessageBox.Show("Please complete your profile information before renting a motorcycle.",
                                "Profile Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            long motorcycleId = 0;
            long rentalId = 0;
            bool isExtension = false;

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                // Query Keeway motorcycle_id
                string sqlKeeway = "SELECT motorcycle_id FROM motorcycle_management WHERE model_name ILIKE '%Keeway%' LIMIT 1";
                using (var cmdKeeway = new NpgsqlCommand(sqlKeeway, conn))
                {
                    var result = cmdKeeway.ExecuteScalar();
                    if (result != null)
                        motorcycleId = Convert.ToInt64(result);
                }

                if (motorcycleId > 0)
                {
                    // Check if client already has an active rental for this motorcycle
                    string sqlRental = @"SELECT rental_id 
                                 FROM rentals 
                                 WHERE client_id = @cid 
                                   AND motorcycle_id = @mid 
                                   AND status = 'Approved'";
                    using (var cmdRental = new NpgsqlCommand(sqlRental, conn))
                    {
                        cmdRental.Parameters.AddWithValue("@cid", clientId);
                        cmdRental.Parameters.AddWithValue("@mid", motorcycleId);

                        var result = cmdRental.ExecuteScalar();
                        if (result != null)
                        {
                            rentalId = Convert.ToInt64(result);
                            isExtension = true;
                        }
                    }
                }
            }

            if (motorcycleId > 0)
            {
                Booking_Details bd = new Booking_Details(motorcycleId, clientId, rentalId, isExtension);
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
