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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Fleet_Management_Rental
{
    public partial class BookingReq : Form
    {
        private long currentClientId;
        private long currentMotorcycleId;
        private DateTime currentStartDate;
        private DateTime currentReturnDate;
        public BookingReq()
        {
            InitializeComponent();
            this.FormClosed += BookingReq_FormClosed;
        }
        
        private void BookingReq_FormClosed(object sender, FormClosedEventArgs e)
        {
            Admin_DashBoard ad = new Admin_DashBoard();
            ad.Show();
            this.Hide();
        }

        private void BookingReq_Load(object sender, EventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT r.rental_id, r.client_id, r.motorcycle_id,
                              r.start_date, r.return_date, r.duration_days,
                              r.valid_id, r.pickup_location, r.id_image_path,
                              c.first_name, c.last_name, c.phone_no, c.email,
                              c.street, c.city, c.postal_code,
                              m.model_name
                              FROM rentals r
                              JOIN clientprofile c ON r.client_id = c.client_id
                              JOIN motorcycle_management m ON r.motorcycle_id = m.motorcycle_id
                              WHERE r.status = 'Pending'
                              LIMIT 1";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        currentClientId = (long)reader["client_id"];
                        currentMotorcycleId = (long)reader["motorcycle_id"];
                        currentStartDate = (DateTime)reader["start_date"];
                        currentReturnDate = (DateTime)reader["return_date"];

                        // Client info
                        lblName.Text = reader["first_name"].ToString() + " " + reader["last_name"].ToString();
                        lblPhone.Text = reader["phone_no"].ToString();
                        lblEmail.Text = reader["email"].ToString();
                        string street = reader["street"].ToString();
                        string city = reader["city"].ToString();
                        string postal = reader["postal_code"].ToString();
                        lblLocation.Text = $"{street}, {city}, {postal}";

                        // Rental info
                        lblMotorUnit.Text = reader["model_name"].ToString();
                        lblPickUp.Text = currentStartDate.ToShortDateString();
                        lblReturn.Text = currentReturnDate.ToShortDateString();
                        lblDuration.Text = reader["duration_days"].ToString() + " days";

                        // Valid ID type
                        lblValidID.Text = reader["valid_id"].ToString();

                        string idImagePath = reader["id_image_path"].ToString();
                        if (!string.IsNullOrEmpty(idImagePath) && System.IO.File.Exists(idImagePath))
                        {
                            pictureBoxID.Image = Image.FromFile(idImagePath);
                        }
                        else
                        {
                            pictureBoxID.BackColor = Color.LightGray;
                        }
                    }
                }
            }
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            long clientId = currentClientId;
            long motorcycleId = currentMotorcycleId;
            DateTime startDate = currentStartDate;
            DateTime returnDate = currentReturnDate;

            int durationDays = (returnDate - startDate).Days;

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                // Update rentals status to Approved
                string sqlRental = @"UPDATE rentals 
                             SET status = 'Approved'
                             WHERE client_id = @cid AND motorcycle_id = @mid 
                             AND start_date = @start AND return_date = @return";
                using (var cmdRental = new NpgsqlCommand(sqlRental, conn))
                {
                    cmdRental.Parameters.AddWithValue("@cid", clientId);
                    cmdRental.Parameters.AddWithValue("@mid", motorcycleId);
                    cmdRental.Parameters.AddWithValue("@start", startDate);
                    cmdRental.Parameters.AddWithValue("@return", returnDate);
                    cmdRental.ExecuteNonQuery();
                }

                // Insert notification
                string sqlNotif = @"INSERT INTO notifications (client_id, message)
                            VALUES (@cid, 'Your booking has been approved!')";
                using (var cmdNotif = new NpgsqlCommand(sqlNotif, conn))
                {
                    cmdNotif.Parameters.AddWithValue("@cid", clientId);
                    cmdNotif.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Booking approved and client notified.");
        }


        private void btnReject_Click(object sender, EventArgs e)
        {
            long clientId = currentClientId;
            long motorcycleId = currentMotorcycleId;
            DateTime startDate = currentStartDate;
            DateTime returnDate = currentReturnDate;

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                // Update rentals status to Approved
                string sqlRental = @"UPDATE rentals 
                             SET status = 'Approved'
                             WHERE client_id = @cid AND motorcycle_id = @mid 
                             AND start_date = @start AND return_date = @return";
                using (var cmdRental = new NpgsqlCommand(sqlRental, conn))
                {
                    cmdRental.Parameters.AddWithValue("@cid", clientId);
                    cmdRental.Parameters.AddWithValue("@mid", motorcycleId);
                    cmdRental.Parameters.AddWithValue("@start", startDate);
                    cmdRental.Parameters.AddWithValue("@return", returnDate);
                    cmdRental.ExecuteNonQuery();
                }

                // After updating rentals to Active
                string sqlMotor = @"UPDATE motorcycle_management
                    SET status = 'Rented'
                    WHERE motorcycle_id IN (
                        SELECT motorcycle_id FROM rentals
                        WHERE client_id = @cid AND status = 'Active'
                    )";
                using (var cmdMotor = new NpgsqlCommand(sqlMotor, conn))
                {
                    cmdMotor.Parameters.AddWithValue("@cid", clientId);
                    cmdMotor.ExecuteNonQuery();
                }


                // Insert notification
                string sqlNotif = @"INSERT INTO notifications (client_id, message)
                            VALUES (@cid, 'Your booking request has been rejected.')";
                using (var cmdNotif = new NpgsqlCommand(sqlNotif, conn))
                {
                    cmdNotif.Parameters.AddWithValue("@cid", clientId);
                    cmdNotif.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Booking rejected and client notified.");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Admin_DashBoard ad = new Admin_DashBoard();
            ad.Show();
            this.Hide();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            User_Management um = new User_Management();
            um.Show();
            this.Hide();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Motorcycle_Management mm = new Motorcycle_Management();
            mm.Show();
            this.Hide();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Rental_Management rm = new Rental_Management();
            rm.Show();
            this.Hide();
        }

        private void button9_Click(object sender, EventArgs e)
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

        private void button8_Click(object sender, EventArgs e)
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
    }
}
