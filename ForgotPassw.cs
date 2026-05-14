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
    public partial class ForgotPassw : Form
    {
        private bool isNavigatingToLogin = false;

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
                "Password=XANnoM1UEQoQ2IJ2-Jp5aw;" + // <--- CHANGE THIS IMMEDIATELY
                "SSL Mode=VerifyFull;" +
                "Trust Server Certificate=true";
        }
        public ForgotPassw()
        {
            InitializeComponent();
            this.FormClosed += ForgotPassw_FormClosed;
        }

        private void ForgotPassw_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void ForgotPassw_Load(object sender, EventArgs e)
        {
            cbPass.Checked = true;
            txtConfirmedPass.UseSystemPasswordChar = true;
        }

        private void Log1n_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtNewPass.Text) || string.IsNullOrEmpty(txtConfirmedPass.Text))
            {
                MessageBox.Show("All fields are required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtNewPass.Text != txtConfirmedPass.Text)
            {
                MessageBox.Show("Passwords do not match!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = DbHelper.GetConnection())
                {
                    // 1. You must explicitly open the pipeline before building commands
                    conn.Open();

                    string sql = "UPDATE clientprofile SET passwordhash = @pass WHERE LOWER(email) = LOWER(@mail)";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@pass", txtNewPass.Text);
                        cmd.Parameters.AddWithValue("@mail", txtEmail.Text.Trim());

                        // 2. This execution will now succeed because the pipeline is open
                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Password reset successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Login log = new Login();
                            log.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Email address not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                } // 3. The using block automatically closes and disposes the connection here
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Login Log = new Login();
            Log.Show();
            this.Hide();
        }

        private void cbPass_CheckedChanged(object sender, EventArgs e)
        {
            txtNewPass.UseSystemPasswordChar = cbPass.Checked;
            txtConfirmedPass.UseSystemPasswordChar= cbPass.Checked;
        }
    }
}
