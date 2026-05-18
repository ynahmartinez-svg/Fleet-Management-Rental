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
        public partial class Booking_Details : Form
        {
        private long motorcycleId;
        private long bookingClientId;
        private long rentalId;

        public Booking_Details(long mid, long cid, long rid = 0)
        {
            InitializeComponent();
            motorcycleId = mid;
            bookingClientId = cid;  
            rentalId = rid;          
            this.FormClosed += Booking_Details_FormClosed;
            this.Load += Booking_Details_Load;
        }

        private void Booking_Details_FormClosed(object sender, FormClosedEventArgs e)
        {
            {
                if (this.Owner != null)
                {
                    this.Owner.Show();  
                }
            }
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
                }

                LoadClientValidIds();

                if (cmbLisense.Items.Count == 0)
                {
                    cmbLisense.Items.Add("Driver’s License");
                    cmbLisense.Items.Add("Passport");
                    cmbLisense.Items.Add("PhilSys");
                    cmbLisense.Items.Add("National ID");
                    cmbLisense.Items.Add("UMID (SSS/GSIS ID)");
                    cmbLisense.Items.Add("PRC ID");
                    cmbLisense.Items.Add("Voter’s ID");
                    cmbLisense.Items.Add("Postal ID");
                    cmbLisense.Items.Add("Company ID");
                }

                if (cmbLisense.Items.Count > 0)
                    cmbLisense.SelectedIndex = 0;
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
            if (string.IsNullOrEmpty(uploadedIdPath))
            {
                MessageBox.Show("Please fill in all required fields and upload your ID.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbLisense.SelectedItem == null)
            {
                MessageBox.Show("Please select a valid ID type.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            long clientId = SessionData.LoggedInClientId;
            DateTime startDate = dtpStart.Value;
            DateTime endDate = dtpEnd.Value;

            if (endDate <= startDate)
            {
                MessageBox.Show("Return date must be after start date.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                if (rentalId == 0)
                {
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
                }
                else
                {
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
                    booking b = new booking();
                    b.Show();
                }
            }
        }
            private void btnEdit_Click(object sender, EventArgs e)
            {
                timePickup.Enabled = true;
                timeReturn.Enabled = true;
                txtPick.ReadOnly = false;
                txtReturn.ReadOnly = false;
                dtpStart.Enabled = true;
                dtpEnd.Enabled = true;
                btnUpload.Enabled = true;
                cmbLisense.Enabled = true;
                dtpStart.Enabled = true;
                dtpEnd.Enabled = true;
                btnUpload.Enabled = true;

                MessageBox.Show("You can now edit your information.");
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
    }
    }
