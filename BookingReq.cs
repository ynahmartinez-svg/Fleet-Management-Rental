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
        private long currentRentalId;
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
                string sql = @"
            SELECT r.rental_id, r.client_id, r.motorcycle_id,
                   r.start_date, r.return_date, r.duration_days,
                   r.valid_id, r.pickup_location, r.return_location, r.id_image_path,
                   c.first_name, c.last_name, c.phone_no, c.email,
                   c.street, c.city, c.postal_code,
                   m.model_name,
                   CASE 
                       WHEN EXISTS (
                           SELECT 1 FROM rentals r2
                           WHERE r2.client_id = r.client_id
                             AND r2.motorcycle_id = r.motorcycle_id
                             AND r2.status = 'Approved'
                       )
                       THEN 'Extension Request'
                       ELSE 'New Booking'
                   END AS request_type
            FROM rentals r
            JOIN clientprofile c ON r.client_id = c.client_id
            JOIN motorcycle_management m ON r.motorcycle_id = m.motorcycle_id
            WHERE r.status = 'Pending';";

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
                string sql = @"
            SELECT r.rental_id, r.client_id, r.motorcycle_id,
                   r.start_date, r.return_date, r.duration_days,
                   r.valid_id, r.pickup_location, r.return_location, r.id_image_path,
                   c.first_name, c.last_name, c.phone_no, c.email,
                   c.street, c.city, c.postal_code,
                   m.model_name,
                   CASE 
                       WHEN EXISTS (
                           SELECT 1 FROM rentals r2
                           WHERE r2.client_id = r.client_id
                             AND r2.motorcycle_id = r.motorcycle_id
                             AND r2.status = 'Approved'
                       )
                       THEN 'Extension Request'
                       ELSE 'New Booking'
                   END AS request_type
            FROM rentals r
            JOIN clientprofile c ON r.client_id = c.client_id
            JOIN motorcycle_management m ON r.motorcycle_id = m.motorcycle_id
            WHERE r.status = 'Pending';";

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
            var result = MessageBox.Show("Approve this booking request?",
                                         "Confirm Approval",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                long rentalId = currentRentalId;
                long clientId = currentClientId;
                long motorcycleId = currentMotorcycleId;

                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    // Approve rental
                    string sqlRental = @"UPDATE rentals 
                                 SET status = 'Approved'
                                 WHERE rental_id = @rid";
                    using (var cmdRental = new NpgsqlCommand(sqlRental, conn))
                    {
                        cmdRental.Parameters.AddWithValue("@rid", rentalId);
                        cmdRental.ExecuteNonQuery();
                    }

                    // Check request type (same logic as LoadPendingRentals)
                    string sqlCheck = @"SELECT CASE 
                                    WHEN EXISTS (
                                        SELECT 1 FROM rentals r2
                                        WHERE r2.client_id = r.client_id
                                          AND r2.motorcycle_id = r.motorcycle_id
                                          AND r2.status = 'Approved'
                                          AND r2.rental_id <> r.rental_id
                                    )
                                    THEN 'Extension Request'
                                    ELSE 'New Booking'
                                END
                                FROM rentals r
                                WHERE r.rental_id = @rid";
                    string requestType;
                    using (var cmdCheck = new NpgsqlCommand(sqlCheck, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@rid", rentalId);
                        requestType = cmdCheck.ExecuteScalar()?.ToString();
                    }

                    if (requestType == "New Booking")
                    {
                        // Mark motorcycle as rented for new bookings
                        string sqlMotor = @"UPDATE motorcycle_management
                                    SET status = 'Rented'
                                    WHERE motorcycle_id = @mid";
                        using (var cmdMotor = new NpgsqlCommand(sqlMotor, conn))
                        {
                            cmdMotor.Parameters.AddWithValue("@mid", motorcycleId);
                            cmdMotor.ExecuteNonQuery();
                        }
                    }

                    // Notify client
                    string message = (requestType == "Extension Request")
                        ? "Your extension request has been approved!"
                        : "Your booking has been approved!";
                    string sqlNotif = @"INSERT INTO notifications (client_id, message)
                                VALUES (@cid, @msg)";
                    using (var cmdNotif = new NpgsqlCommand(sqlNotif, conn))
                    {
                        cmdNotif.Parameters.AddWithValue("@cid", clientId);
                        cmdNotif.Parameters.AddWithValue("@msg", message);
                        cmdNotif.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Booking approved and client notified.");
                LoadPendingRentals();
            }
        }



        private void btnReject_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Reject this booking request?",
                                         "Confirm Rejection",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                long rentalId = currentRentalId;
                long clientId = currentClientId;
                long motorcycleId = currentMotorcycleId;

                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    // Reject rental
                    string sqlRental = @"UPDATE rentals 
                                 SET status = 'Rejected'
                                 WHERE rental_id = @rid";
                    using (var cmdRental = new NpgsqlCommand(sqlRental, conn))
                    {
                        cmdRental.Parameters.AddWithValue("@rid", rentalId);
                        cmdRental.ExecuteNonQuery();
                    }

                    // Check request type (same logic as LoadPendingRentals)
                    string sqlCheck = @"SELECT CASE 
                                    WHEN EXISTS (
                                        SELECT 1 FROM rentals r2
                                        WHERE r2.client_id = r.client_id
                                          AND r2.motorcycle_id = r.motorcycle_id
                                          AND r2.status = 'Approved'
                                          AND r2.rental_id <> r.rental_id
                                    )
                                    THEN 'Extension Request'
                                    ELSE 'New Booking'
                                END
                                FROM rentals r
                                WHERE r.rental_id = @rid";
                    string requestType;
                    using (var cmdCheck = new NpgsqlCommand(sqlCheck, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@rid", rentalId);
                        requestType = cmdCheck.ExecuteScalar()?.ToString();
                    }

                    if (requestType == "New Booking")
                    {
                        // Reset motorcycle only for new bookings
                        string sqlReset = @"UPDATE motorcycle_management
                                    SET status = 'Available'
                                    WHERE motorcycle_id = @mid";
                        using (var cmdReset = new NpgsqlCommand(sqlReset, conn))
                        {
                            cmdReset.Parameters.AddWithValue("@mid", motorcycleId);
                            cmdReset.ExecuteNonQuery();
                        }
                    }
                    // For extension requests → motorcycle stays rented

                    // Notify client
                    string message = (requestType == "Extension Request")
                        ? "Your extension request has been rejected."
                        : "Your booking request has been rejected.";
                    string sqlNotif = @"INSERT INTO notifications (client_id, message)
                                VALUES (@cid, @msg)";
                    using (var cmdNotif = new NpgsqlCommand(sqlNotif, conn))
                    {
                        cmdNotif.Parameters.AddWithValue("@cid", clientId);
                        cmdNotif.Parameters.AddWithValue("@msg", message);
                        cmdNotif.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Booking rejected and client notified.");
                LoadPendingRentals();
            }
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


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRentals.CurrentRow != null)
            {
                DataGridViewRow row = dgvRentals.CurrentRow;

                currentRentalId = (long)row.Cells["rental_id"].Value;
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
