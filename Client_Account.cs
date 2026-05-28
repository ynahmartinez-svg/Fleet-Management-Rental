using Npgsql;
using System;
using System.Windows.Forms;

namespace Fleet_Management_Rental
{
    public partial class Client_Account : Form
    {
        public Client_Account()
        {
            InitializeComponent();
            this.FormClosed += Client_Account_FormClosed;
        }
        private void Client_Account_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Client_Account_Load(object sender, EventArgs e)
        {
            long clientId = SessionData.LoggedInClientId;

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
                            txtFname.Text = reader.IsDBNull(0) ? "" : reader.GetString(0);
                            txtLname.Text = reader.IsDBNull(1) ? "" : reader.GetString(1);
                            dtpDOB.Value = reader.IsDBNull(2) ? DateTime.Today : reader.GetDateTime(2);
                            txtPhone.Text = reader.IsDBNull(3) ? "" : reader.GetString(3);
                            cmbID.Text = reader.IsDBNull(4) ? "" : reader.GetString(4);
                            txtStreet.Text = reader.IsDBNull(5) ? "" : reader.GetString(5);
                            txtCity.Text = reader.IsDBNull(6) ? "" : reader.GetString(6);
                            txtPostal.Text = reader.IsDBNull(7) ? "" : reader.GetString(7);


                            lblName.Text = $"Name: {txtFname.Text} {txtLname.Text}".Trim();
                        }
                    }
                }
                string sqlTotal = "SELECT COUNT(*) FROM rentals WHERE client_id = @cid";
                using (var cmdTotal = new NpgsqlCommand(sqlTotal, conn))
                {
                    cmdTotal.Parameters.AddWithValue("@cid", clientId);
                    int totalRents = Convert.ToInt32(cmdTotal.ExecuteScalar());
                    lblRent.Text = $"{totalRents}";
                }


                string sqlActive = "SELECT COUNT(*) FROM rentals WHERE client_id = @cid AND status = 'Active'";
                using (var cmdActive = new NpgsqlCommand(sqlActive, conn))
                {
                    cmdActive.Parameters.AddWithValue("@cid", clientId);
                    int activeRents = Convert.ToInt32(cmdActive.ExecuteScalar());
                    lblActive.Text = $"{activeRents}";
                }
            }

            LoadClientInfo();
            SetEditingEnabled(false);

            if (cmbID.Items.Count == 0)
            {
                cmbID.Items.Add("Driver’s License");
                cmbID.Items.Add("Passport");
                cmbID.Items.Add("PhilSys");
                cmbID.Items.Add("National ID");
                cmbID.Items.Add("UMID (SSS/GSIS ID)");
                cmbID.Items.Add("PRC ID");
                cmbID.Items.Add("Voter’s ID");
                cmbID.Items.Add("Postal ID");
                cmbID.Items.Add("Company ID");
            }
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

        private void button10_Click(object sender, EventArgs e)
        {
            Client_Dashboard cd = new Client_Dashboard();
            cd.ShowDialog();
            this.Hide();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            My_rentals r = new My_rentals();
            r.Show();
            this.Hide();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            booking b = new booking();
            b.Show();
            this.Hide();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Payments_and_Billing PB = new Payments_and_Billing();
            this.Hide();
            PB.ShowDialog();
            this.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            txtFname.ReadOnly = false;
            txtLname.ReadOnly = false;
            txtPhone.ReadOnly = false;
            dtpDOB.Enabled = true;
            cmbID.Enabled = true;
            txtStreet.ReadOnly = false;
            txtCity.ReadOnly = false;
            txtPostal.ReadOnly = false;

            SetEditingEnabled(true);
            MessageBox.Show("You can now edit your information.");

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            long clientId = SessionData.LoggedInClientId;

            using (var conn = DbHelper.GetConnection()) 
            {
                conn.Open();

                string sql = @"UPDATE clientprofile
                               SET first_name = @fname,
                                   last_name = @lname,
                                   date_of_birth = @dob,
                                   phone_no = @phone,
                                   valid_id = @validid,
                                   street = @street,
                                   city = @city,
                                   postal_code = @postal
                               WHERE client_id = @cid";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@fname", txtFname.Text);
                    cmd.Parameters.AddWithValue("@lname", txtLname.Text);
                    cmd.Parameters.AddWithValue("@dob", dtpDOB.Value);
                    cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@validid", cmbID.Text);
                    cmd.Parameters.AddWithValue("@street", txtStreet.Text);
                    cmd.Parameters.AddWithValue("@city", txtCity.Text);
                    cmd.Parameters.AddWithValue("@postal", txtPostal.Text);
                    cmd.Parameters.AddWithValue("@cid", clientId);

                    cmd.ExecuteNonQuery();
                }
            }

            LoadClientInfo();
            SetEditingEnabled(false);
            lblName.Text = $"Name: {txtFname.Text} {txtLname.Text}";
            MessageBox.Show($"Client information updated successfully!");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            LoadClientInfo();
            SetEditingEnabled(false);
        }
        private void LoadClientInfo()
        {
            long clientId = SessionData.LoggedInClientId;

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
                            txtFname.Text = reader.IsDBNull(0) ? "" : reader.GetString(0);
                            txtLname.Text = reader.IsDBNull(1) ? "" : reader.GetString(1);
                            dtpDOB.Value = reader.IsDBNull(2) ? DateTime.Today : reader.GetDateTime(2);
                            txtPhone.Text = reader.IsDBNull(3) ? "" : reader.GetString(3);
                            cmbID.Text = reader.IsDBNull(4) ? "" : reader.GetString(4);
                            txtStreet.Text = reader.IsDBNull(5) ? "" : reader.GetString(5);
                            txtCity.Text = reader.IsDBNull(6) ? "" : reader.GetString(6);
                            txtPostal.Text = reader.IsDBNull(7) ? "" : reader.GetString(7);

                            lblName.Text = $"Name:  {txtFname.Text} {txtLname.Text}".Trim();
                        }
                    }
                }
            }
        }

        private void SetEditingEnabled(bool enabled)
        {
            txtFname.Enabled = enabled;
            txtLname.Enabled = enabled;
            dtpDOB.Enabled = enabled;
            txtPhone.Enabled = enabled;
            cmbID.Enabled = enabled;
            txtStreet.Enabled = enabled;
            txtCity.Enabled = enabled;
            txtPostal.Enabled = enabled;

            btnSave.Enabled = enabled;
            btnCancel.Enabled = enabled;
        }

        private void btnNotification_Click(object sender, EventArgs e)
        {
            Client_Notification cn = new Client_Notification();
            cn.Show();
            this.Hide();
        }

        private void btnMap_Click(object sender, EventArgs e)
        {
            Client_Map  map = new Client_Map();
            map.Show();
            this.Hide();
        }
    }
}
