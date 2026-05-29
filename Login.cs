using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Fleet_Management_Rental.Client_Dashboard;

namespace Fleet_Management_Rental
{
    public partial class Login : Form
    {

        public Login()
        {
            InitializeComponent();
            this.FormClosed += Login_FormClosed;
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Login_Load(object sender, EventArgs e)
        {
            txtPass.UseSystemPasswordChar = true;
            cbPass.Checked = true;

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SignUp SU = new SignUp();
            SU.Show();
            this.Hide();
        }

        private void Log1n_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPass.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter email and password!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string role = ValidateUserRole(email, password);

            if (role == "Admin")
            {
                MessageBox.Show("Admin login successful!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Admin_DashBoard Dashboard_Admin = new Admin_DashBoard();
                Dashboard_Admin.Show();
                this.Hide();

            }
            else if (role == "Client")
            {
                if (IsClientProfileComplete(SessionData.LoggedInClientId))
                {
                    MessageBox.Show("Client login successful!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Client_Dashboard Dashboard = new Client_Dashboard();
                    Dashboard.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Login successful! Please complete your profile first before accessing the dashboard.",
                    "Profile Required", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Client_NewAccount newAccForm = new Client_NewAccount();
                    newAccForm.Show();
                    this.Hide();
                }
            }
            else
            {
                MessageBox.Show("Invalid email or password!", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                txtPass.Clear();
                txtPass.Focus();
            }
        }
        private bool IsClientProfileComplete(long clientId)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT first_name, last_name, phone_no, date_of_birth, valid_id,
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
                            // If any required field is NULL or empty, profile is incomplete
                            return !(reader.IsDBNull(0) || string.IsNullOrWhiteSpace(reader.GetString(0)) ||
                                     reader.IsDBNull(1) || string.IsNullOrWhiteSpace(reader.GetString(1)) ||
                                     reader.IsDBNull(2) || string.IsNullOrWhiteSpace(reader.GetString(2)) ||
                                     reader.IsDBNull(3) ||
                                     reader.IsDBNull(4) || string.IsNullOrWhiteSpace(reader.GetString(4)) ||
                                     reader.IsDBNull(5) || string.IsNullOrWhiteSpace(reader.GetString(5)) ||
                                     reader.IsDBNull(6) || string.IsNullOrWhiteSpace(reader.GetString(6)) ||
                                     reader.IsDBNull(7) || string.IsNullOrWhiteSpace(reader.GetString(7)));
                        }
                    }
                }
            }
            return false;
        }

        private string ValidateUserRole(string email, string password)
        {
            try
            {
                using (NpgsqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    // Check Admin
                    string adminQuery = "SELECT COUNT(*) FROM adminprofile WHERE LOWER(email) = LOWER(@mail) AND passwordhash = @pass";
                    using (var cmd = new NpgsqlCommand(adminQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@mail", email);
                        cmd.Parameters.AddWithValue("@pass", password);
                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            return "Admin";
                    }

                    // Check Client
                    string clientQuery = @"SELECT client_id FROM clientprofile 
                       WHERE LOWER(email) = LOWER(@mail) AND passwordhash = @pass";
                    using (var cmd = new NpgsqlCommand(clientQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@mail", email);
                        cmd.Parameters.AddWithValue("@pass", password);

                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            SessionData.LoggedInRole = "Client";
                            SessionData.LoggedInClientId = Convert.ToInt64(result);
                            return "Client";
                        }
                    }


                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }


        private void cbPass_CheckedChanged(object sender, EventArgs e)
        {

            txtPass.UseSystemPasswordChar = cbPass.Checked;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ForgotPassw FP = new ForgotPassw();
            FP.Show();
            this.Hide();
        }
    }
}

