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
    public partial class Client_NewAccount : Form
    {
        public Client_NewAccount()
        {
            InitializeComponent();
        }

        private void Client_NewAccount_Load(object sender, EventArgs e)
        {
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            long clientId = SessionData.LoggedInClientId;

            // Basic validation
            if (string.IsNullOrWhiteSpace(txtFname.Text) ||
                string.IsNullOrWhiteSpace(txtLname.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtStreet.Text) ||
                string.IsNullOrWhiteSpace(txtCity.Text) ||
                string.IsNullOrWhiteSpace(txtPostal.Text) ||
                string.IsNullOrWhiteSpace(cmbID.Text))
            {
                MessageBox.Show("Please fill in all required fields (*) before saving.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                string sql = @"UPDATE clientprofile
                       SET first_name = @fname,
                           last_name = @lname,
                           phone_no = @phone,
                           date_of_birth = @dob,
                           valid_id = @validid,
                           street = @street,
                           city = @city,
                           postal_code = @postal
                       WHERE client_id = @cid";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@fname", txtFname.Text.Trim());
                    cmd.Parameters.AddWithValue("@lname", txtLname.Text.Trim());
                    cmd.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@dob", dtpDOB.Value);
                    cmd.Parameters.AddWithValue("@validid", cmbID.Text.Trim());
                    cmd.Parameters.AddWithValue("@street", txtStreet.Text.Trim());
                    cmd.Parameters.AddWithValue("@city", txtCity.Text.Trim());
                    cmd.Parameters.AddWithValue("@postal", txtPostal.Text.Trim());
                    cmd.Parameters.AddWithValue("@cid", clientId);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Profile saved successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Redirect to dashboard
            Client_Dashboard dashboard = new Client_Dashboard();
            dashboard.Show();
            this.Hide();
        }

    }
}

