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

namespace Fleet_Management_Rental
{
    public partial class Login : Form
    {
        public static class DbHelper
        {
            public static NpgsqlConnection GetConnection()
            {
                return new NpgsqlConnection(DbConfig.ConnectionString);
            }
        }

        public static class DbConfig
        {
            public static string ConnectionString = "Host=smart1-fleetdb-25755.j77.aws-ap-southeast-1.cockroachlabs.cloud;" +
        "Port=26257;" +
        "Database=fms_rental;" +
        "Username=joohn;" +
        "Password=XANnoM1UEQoQ2IJ2-Jp5aw;" +
        "SSL Mode=VerifyFull;" +
              "Trust Server Certificate=true";
        }

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
                MessageBox.Show("Client login successful!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Client_Dashboard Dashboard = new Client_Dashboard();
                Dashboard.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid email or password!", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                txtPass.Clear();
                txtPass.Focus();
            }
        }

        private string ValidateUserRole(string email, string password)
        {
            try
            {
                using (NpgsqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    string hashedPassword = ComputeSha256Hash(password);


                    string adminQuery = "SELECT COUNT(*) FROM adminprofile WHERE LOWER(email) = LOWER(@mail) AND passwordhash = @pass";
                    using (var cmd = new NpgsqlCommand(adminQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@mail", email);
                        cmd.Parameters.AddWithValue("@pass", password);
                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            return "Admin";
                    }

                    string clientQuery = "SELECT COUNT(*) FROM clientprofile WHERE LOWER(email) = LOWER(@mail) AND passwordhash = @pass";
                    using (var cmd = new NpgsqlCommand(clientQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@mail", email);
                        NpgsqlParameter npgsqlParameter = cmd.Parameters.AddWithValue("@pass", password);
                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            return "Client";
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
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
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
