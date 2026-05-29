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

namespace Fleet_Management_Rental
    {
        public partial class Booking_Details : Form
        {
        private long motorcycleId;
        private long bookingClientId;
        private long rentalId;
        private bool isExtension = false;

        public Booking_Details(long mid, long cid, long rid = 0, bool extensionMode = false)
        {
            InitializeComponent();
            motorcycleId = mid;
            bookingClientId = cid;  
            rentalId = rid;
            this.isExtension = extensionMode;
            this.FormClosed += Booking_Details_FormClosed;
            this.Load += Booking_Details_Load;
        }

        private void Booking_Details_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
            private void panel2_Paint(object sender, PaintEventArgs e)
            {

            }

        private void Booking_Details_Load(object sender, EventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT model_name, brand, plate_num, price_per_day, image_path " +
                             "FROM motorcycle_management WHERE motorcycle_id = @id";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", motorcycleId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblModel.Text = reader.GetString(0);
                            lblBrand.Text = reader.GetString(1);
                            lblPlate.Text = reader.GetString(2);
                            lblPrice.Text = "₱" + reader.GetDecimal(3).ToString("0.00") + "/Day";

                            string imagePath = reader.GetString(4);
                            if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                                pictureMotor.Image = Image.FromFile(imagePath);
                        }
                    }
                }

                // Only load IDs for normal booking/edit
                if (!isExtension)
                {
                    LoadClientValidIds();

                    if (cmbLisense.Items.Count == 0)
                    {
                        // fallback static list
                        cmbLisense.Items.AddRange(new object[] {
            "Driver’s License", "Passport", "PhilSys", "National ID",
            "UMID (SSS/GSIS ID)", "PRC ID", "Voter’s ID",
            "Postal ID", "Company ID"
            });
                    }

                    if (cmbLisense.Items.Count > 0)
                        cmbLisense.SelectedIndex = 0;

                    dtpStart.MinDate = DateTime.Today;
                    dtpEnd.MinDate = DateTime.Today;
                }
            }

            if (isExtension && rentalId > 0)
            {
                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    string sqlRental = @"SELECT start_date, return_date, pickup_location, pickup_time,
                                    return_location, return_time, valid_id, id_image_path
                             FROM rentals
                             WHERE rental_id = @rid AND client_id = @cid";

                    using (var cmd = new NpgsqlCommand(sqlRental, conn))
                    {
                        cmd.Parameters.AddWithValue("@rid", rentalId);
                        cmd.Parameters.AddWithValue("@cid", bookingClientId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                dtpStart.Value = reader.GetDateTime(0);
                                dtpEnd.Value = reader.GetDateTime(1);

                                txtPick.Text = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                timePickup.Value = reader.IsDBNull(3) ? DateTime.Now : DateTime.Today.Add(reader.GetTimeSpan(3));
                                txtReturn.Text = reader.IsDBNull(4) ? "" : reader.GetString(4);
                                timeReturn.Value = reader.IsDBNull(5) ? DateTime.Now : DateTime.Today.Add(reader.GetTimeSpan(5));

                                cmbLisense.Text = reader.IsDBNull(6) ? "" : reader.GetString(6);
                                uploadedIdPath = reader.IsDBNull(7) ? "" : reader.GetString(7);

                                if (!string.IsNullOrEmpty(uploadedIdPath) && File.Exists(uploadedIdPath))
                                    pictureBoxID.Image = Image.FromFile(uploadedIdPath);

                                string validId = reader.IsDBNull(6) ? "" : reader.GetString(6);

                                // ensure the combo has items
                                if (!cmbLisense.Items.Contains(validId))
                                    cmbLisense.Items.Add(validId);

                                cmbLisense.SelectedItem = validId; // ✅ instead of just Text
                                cmbLisense.Enabled = false;
                            }
                        }
                    }
                }

                // Disable fields not needed for extension
                cmbLisense.Enabled = false;
                btnUpload.Enabled = false;
                txtPick.Enabled = false;
                txtReturn.Enabled = false;
                timePickup.Enabled = false;
                timeReturn.Enabled = false;

                lblBanner.Text = "Extension Mode: Only update the return date.";
            }

        }



        private void LoadClientValidIds()
        {
            long clientId = SessionData.LoggedInClientId;

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                string sql = @"SELECT valid_id 
                       FROM rentals 
                       WHERE client_id = @cid AND status = 'Pending'";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cid", clientId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        cmbLisense.Items.Clear();
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                string validId = reader.GetString(0);
                                cmbLisense.Items.Add(validId);
                            }
                        }
                    }
                }
            }
        }
        private string uploadedIdPath;
        private void btnUpload_Click(object sender, EventArgs e)
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    uploadedIdPath = ofd.FileName;
                    pictureBoxID.Image = Image.FromFile(uploadedIdPath);

                }
                }
            }

        private void btnBook_Click(object sender, EventArgs e)
        {
            long clientId = SessionData.LoggedInClientId;
            DateTime startDate = dtpStart.Value;
            DateTime endDate = dtpEnd.Value;

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                // ✅ Check availability before booking/extension
                string sqlCheck = @"SELECT COUNT(*) 
                            FROM rentals 
                            WHERE motorcycle_id = @mid 
                              AND status IN ('Approved','Pending')
                              AND ((@start BETWEEN start_date AND return_date)
                                OR (@end BETWEEN start_date AND return_date)
                                OR (start_date BETWEEN @start AND @end)
                                OR (return_date BETWEEN @start AND @end))";

                using (var cmdCheck = new NpgsqlCommand(sqlCheck, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@mid", motorcycleId);
                    cmdCheck.Parameters.AddWithValue("@start", startDate);
                    cmdCheck.Parameters.AddWithValue("@end", endDate);

                    int conflictCount = Convert.ToInt32(cmdCheck.ExecuteScalar());
                    if (conflictCount > 0)
                    {
                        MessageBox.Show("This motorcycle is unavailable for the selected dates.",
                                        "Booking Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                if (rentalId == 0)
                {
                    // ✅ New booking → full validation
                    if (string.IsNullOrEmpty(uploadedIdPath) || cmbLisense.SelectedItem == null)
                    {
                        MessageBox.Show("Please fill in all required fields and upload your ID.",
                                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (endDate <= startDate)
                    {
                        MessageBox.Show("Return date must be after start date.",
                                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string sql = @"INSERT INTO rentals 
                           (client_id, motorcycle_id, start_date, return_date, status, 
                            pickup_location, pickup_time, return_location, return_time, valid_id, id_image_path)
                           VALUES (@cid, @mid, @start, @end, 'Pending', 
                                   @pickup, @pickuptime, @return, @returntime, @validid, @idimage)";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@cid", bookingClientId);
                        cmd.Parameters.AddWithValue("@mid", motorcycleId);
                        cmd.Parameters.AddWithValue("@start", startDate);
                        cmd.Parameters.AddWithValue("@end", endDate);
                        cmd.Parameters.AddWithValue("@pickup", txtPick.Text);
                        cmd.Parameters.AddWithValue("@pickuptime", timePickup.Value);
                        cmd.Parameters.AddWithValue("@return", txtReturn.Text);
                        cmd.Parameters.AddWithValue("@returntime", timeReturn.Value);
                        cmd.Parameters.AddWithValue("@validid", cmbLisense.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@idimage", uploadedIdPath);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Rental request submitted successfully! Please wait for admin approval.",
                                    "Booking Submitted", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Hide();
                    My_rentals b = new My_rentals();
                    b.Show();
                }
                else if (isExtension)
                {
                    // ✅ Extension → only check return date
                    if (endDate <= startDate)
                    {
                        MessageBox.Show("Return date must be after start date.",
                                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string sql = @"UPDATE rentals 
                           SET return_date = @end,
                               status = 'Pending'
                           WHERE rental_id = @rid AND client_id = @cid";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@end", endDate);
                        cmd.Parameters.AddWithValue("@rid", rentalId);
                        cmd.Parameters.AddWithValue("@cid", bookingClientId);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Rental extended successfully! Please wait for admin approval.",
                                    "Extension Submitted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide();
                    My_rentals b = new My_rentals();
                    b.Show();
                }
                else
                {
                    // ✅ Edit booking → full validation
                    if (string.IsNullOrEmpty(uploadedIdPath) || cmbLisense.SelectedItem == null)
                    {
                        MessageBox.Show("Please fill in all required fields and upload your ID.",
                                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (endDate <= startDate)
                    {
                        MessageBox.Show("Return date must be after start date.",
                                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string sql = @"UPDATE rentals 
                           SET return_date = @end, 
                               pickup_location = @pickup, 
                               pickup_time = @pickuptime, 
                               return_location = @return, 
                               return_time = @returntime, 
                               valid_id = @validid,
                               id_image_path = @idimage
                           WHERE rental_id = @rid AND client_id = @cid";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@end", endDate);
                        cmd.Parameters.AddWithValue("@pickup", txtPick.Text);
                        cmd.Parameters.AddWithValue("@pickuptime", timePickup.Value);
                        cmd.Parameters.AddWithValue("@return", txtReturn.Text);
                        cmd.Parameters.AddWithValue("@returntime", timeReturn.Value);
                        cmd.Parameters.AddWithValue("@validid", cmbLisense.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@idimage", uploadedIdPath);
                        cmd.Parameters.AddWithValue("@rid", rentalId);
                        cmd.Parameters.AddWithValue("@cid", bookingClientId);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Rental updated successfully! Please wait for admin approval.",
                                    "Booking Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Hide();
                    My_rentals b = new My_rentals();
                    b.Show();
                }
            }
        }


        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (isExtension)
            {
                // ✅ Extension mode → only allow editing the return date
                dtpEnd.Enabled = true;

                MessageBox.Show("You can now extend your rental by adjusting the return date.");
            }
            else
            {
                // ✅ Normal booking/edit → allow full editing
                timePickup.Enabled = true;
                timeReturn.Enabled = true;
                txtPick.ReadOnly = false;
                txtReturn.ReadOnly = false;
                dtpStart.Enabled = true;
                dtpEnd.Enabled = true;
                btnUpload.Enabled = true;
                cmbLisense.Enabled = true;

                MessageBox.Show("You can now edit your booking information.");
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
            {
                DialogResult result = MessageBox.Show("Do you want to cancel booking?", "Are you Sure?",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    this.Hide();
                    new Client_Dashboard().Show();
                }
            }
            private void dtpStart_ValueChanged(object sender, EventArgs e)
            {

            }

            private void dtpEnd_ValueChanged(object sender, EventArgs e)
            {

            }

            private void cmbLisense_SelectedIndexChanged(object sender, EventArgs e)
            {

            }

        private void button2_Click(object sender, EventArgs e)
        {
            booking bk = new booking();
            bk.Show();
            this.Hide();
        }

        private void pictureMotor_Click(object sender, EventArgs e)
        {

        }
    }
    }
