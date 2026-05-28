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
using System.IO;
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

        }

        private void BookingReq_Load(object sender, EventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT r.rental_id, r.client_id, r.motorcycle_id,
                       r.start_date, r.return_date, r.duration_days,
                       r.valid_id, r.pickup_location, r.return_location, r.id_image_path,
                       c.first_name, c.last_name, c.phone_no, c.email,
                       c.street, c.city, c.postal_code,
                       m.model_name
                FROM rentals r
                JOIN clientprofile c ON r.client_id = c.client_id
                JOIN motorcycle_management m ON r.motorcycle_id = m.motorcycle_id
                WHERE r.status = 'Pending'";

                // Load all pending rentals into the DataGridView
                using (var da = new NpgsqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvRentals.DataSource = dt;
                }

                // Disable buttons until a row is selected
                btnApprove.Enabled = false;
                btnReject.Enabled = false;
            }

            // Handle row selection to show details
            dgvRentals.SelectionChanged += (s, ev) =>
            {
                if (dgvRentals.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = dgvRentals.SelectedRows[0];

                    currentClientId = (long)row.Cells["client_id"].Value;
                    currentMotorcycleId = (long)row.Cells["motorcycle_id"].Value;
                    currentStartDate = (DateTime)row.Cells["start_date"].Value;
                    currentReturnDate = (DateTime)row.Cells["return_date"].Value;

                    // Client info
                    lblName.Text = row.Cells["first_name"].Value + " " + row.Cells["last_name"].Value;
                    lblPhone.Text = row.Cells["phone_no"].Value.ToString();
                    lblEmail.Text = row.Cells["email"].Value.ToString();
                    lblLocation.Text = $"{row.Cells["street"].Value}, {row.Cells["city"].Value}, {row.Cells["postal_code"].Value}";

                    // Rental info
                    lblMotorUnit.Text = row.Cells["model_name"].Value.ToString();
                    lblPickUp.Text = currentStartDate.ToShortDateString();
                    lblReturn.Text = currentReturnDate.ToShortDateString();
                    lblDuration.Text = row.Cells["duration_days"].Value + " days";

                    // Pickup & Return locations
                    lblPickupLocation.Text = "Pickup: " + row.Cells["pickup_location"].Value.ToString();
                    lblReturnLocation.Text = "Return: " + row.Cells["return_location"].Value.ToString();

                    // Valid ID type
                    lblValidID.Text = row.Cells["valid_id"].Value.ToString();

                    // ID image
                    string idImagePath = row.Cells["id_image_path"].Value.ToString();
                    if (!string.IsNullOrEmpty(idImagePath) && System.IO.File.Exists(idImagePath))
                    {
                        pictureBoxID.Image = Image.FromFile(idImagePath);
                    }
                    else
                    {
                        pictureBoxID.Image = null;
                        pictureBoxID.BackColor = Color.LightGray;
                    }

                    // Enable buttons when a row is selected
                    btnApprove.Enabled = true;
                    btnReject.Enabled = true;
                }
            };
        }

        private void LoadPendingRentals()
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT r.rental_id, r.client_id, r.motorcycle_id,
                              r.start_date, r.return_date, r.duration_days,
                              r.valid_id, r.pickup_location, r.return_location, r.id_image_path,
                              c.first_name, c.last_name, c.phone_no, c.email,
                              c.street, c.city, c.postal_code,
                              m.model_name
                       FROM rentals r
                       JOIN clientprofile c ON r.client_id = c.client_id
                       JOIN motorcycle_management m ON r.motorcycle_id = m.motorcycle_id
                       WHERE r.status = 'Pending'";

                using (var da = new NpgsqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvRentals.DataSource = dt;
                }
            }

            // Disable buttons until a row is selected
            btnApprove.Enabled = false;
            btnReject.Enabled = false;
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
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
                           WHERE client_id = @cid 
                             AND motorcycle_id = @mid 
                             AND start_date = @start 
                             AND return_date = @return";
                    using (var cmdRental = new NpgsqlCommand(sqlRental, conn))
                    {
                        cmdRental.Parameters.AddWithValue("@cid", clientId);
                        cmdRental.Parameters.AddWithValue("@mid", motorcycleId);
                        cmdRental.Parameters.AddWithValue("@start", startDate);
                        cmdRental.Parameters.AddWithValue("@return", returnDate);
                        cmdRental.ExecuteNonQuery();
                    }

                    // Mark motorcycle as Rented once approved
                    string sqlMotor = @"UPDATE motorcycle_management
                          SET status = 'Rented'
                          WHERE motorcycle_id = @mid";
                    using (var cmdMotor = new NpgsqlCommand(sqlMotor, conn))
                    {
                        cmdMotor.Parameters.AddWithValue("@mid", motorcycleId);
                        cmdMotor.ExecuteNonQuery();
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
                LoadPendingRentals(); // Refresh the DataGridView to show updated status
            }
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

                // Update rentals status to Rejected
                string sqlRental = @"UPDATE rentals 
                           SET status = 'Rejected'
                           WHERE client_id = @cid 
                             AND motorcycle_id = @mid 
                             AND start_date = @start 
                             AND return_date = @return";

                using (var cmdRental = new NpgsqlCommand(sqlRental, conn))
                {
                    cmdRental.Parameters.AddWithValue("@cid", clientId);
                    cmdRental.Parameters.AddWithValue("@mid", motorcycleId);
                    cmdRental.Parameters.AddWithValue("@start", startDate);
                    cmdRental.Parameters.AddWithValue("@return", returnDate);
                    cmdRental.ExecuteNonQuery();
                }

                // Reset motorcycle status to Available when rejected
                string sqlReset = @"UPDATE motorcycle_management
                          SET status = 'Available'
                          WHERE motorcycle_id = @mid";
                using (var cmdReset = new NpgsqlCommand(sqlReset, conn))
                {
                    cmdReset.Parameters.AddWithValue("@mid", motorcycleId);
                    cmdReset.ExecuteNonQuery();
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
            LoadPendingRentals(); // Refresh the DataGridView to show updated status

        }

        private void button11_Click(object sender, EventArgs e)
        {
            Admin_DashBoard ad = new Admin_DashBoard();
            ad.Show();
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
            Fuel_Cost_Management fcm = new Fuel_Cost_Management();
            fcm.Show();
            this.Hide();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Payment_Billing pb = new Payment_Billing();
            pb.Show();
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                //reset all available motorcycles
                string sqlResetAll = @"UPDATE motorcycle_management
                          SET status = 'Available'
                         WHERE status <> 'Available'";
               using (var cmdResetAll = new NpgsqlCommand(sqlResetAll, conn))
               {
                  cmdResetAll.ExecuteNonQuery();
                }

                // delete rejected rentals
                string sqlClearRejected = @"DELETE FROM rentals WHERE status = 'Rejected'";
                using (var cmdClearRejected = new NpgsqlCommand(sqlClearRejected, conn))
                {
                    cmdClearRejected.ExecuteNonQuery();
                }

                // Clear notifications (optional, depends if you want a clean slate)
                string sqlClearNotif = @"TRUNCATE TABLE notifications";
                using (var cmdClearNotif = new NpgsqlCommand(sqlClearNotif, conn))
                {
                    cmdClearNotif.ExecuteNonQuery();
                }

                // Refresh DataGridView with updated rentals (only Pending + Approved remain)
                string sqlReload = @"SELECT r.rental_id, r.client_id, r.motorcycle_id,
                                    r.start_date, r.return_date, r.duration_days,
                                    r.valid_id, r.pickup_location, r.return_location, r.id_image_path,
                                    c.first_name, c.last_name, c.phone_no, c.email,
                                    c.street, c.city, c.postal_code,
                                    m.model_name
                             FROM rentals r
                             JOIN clientprofile c ON r.client_id = c.client_id
                             JOIN motorcycle_management m ON r.motorcycle_id = m.motorcycle_id
                             WHERE r.status = 'Pending'"; // show only pending requests

                using (var da = new NpgsqlDataAdapter(sqlReload, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvRentals.DataSource = dt;
                }
            }
            MessageBox.Show("Fleet reset: all motorcycles are now Available.");
           

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRentals.CurrentRow != null)
            {
                DataGridViewRow row = dgvRentals.CurrentRow;

                currentClientId = (long)row.Cells["client_id"].Value;
                currentMotorcycleId = (long)row.Cells["motorcycle_id"].Value;
                currentStartDate = (DateTime)row.Cells["start_date"].Value;
                currentReturnDate = (DateTime)row.Cells["return_date"].Value;

                // Client info
                lblName.Text = row.Cells["first_name"].Value + " " + row.Cells["last_name"].Value;
                lblPhone.Text = row.Cells["phone_no"].Value.ToString();
                lblEmail.Text = row.Cells["email"].Value.ToString();
                lblLocation.Text = $"{row.Cells["street"].Value}, {row.Cells["city"].Value}, {row.Cells["postal_code"].Value}";

                // Rental info
                lblMotorUnit.Text = row.Cells["model_name"].Value.ToString();
                lblPickUp.Text = currentStartDate.ToShortDateString();
                lblReturn.Text = currentReturnDate.ToShortDateString();
                lblDuration.Text = row.Cells["duration_days"].Value + " days";

                lblPickupLocation.Text = "Pickup: " + row.Cells["pickup_location"].Value;
                lblReturnLocation.Text = "Return: " + row.Cells["return_location"].Value;
                lblValidID.Text = row.Cells["valid_id"].Value.ToString();

                // ID image
                string idImagePath = row.Cells["id_image_path"].Value.ToString();
                if (!string.IsNullOrEmpty(idImagePath) && File.Exists(idImagePath))
                    pictureBoxID.Image = Image.FromFile(idImagePath);
                else
                    pictureBoxID.Image = null; // clear previous image
                pictureBoxID.BackColor = Color.LightGray;

                // Enable buttons
                btnApprove.Enabled = true;
                btnReject.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Admin_Notifications an = new Admin_Notifications();
            an.Show();
            this.Close();
        }
    }
}
