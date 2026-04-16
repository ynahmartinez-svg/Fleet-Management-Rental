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
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Fleet_Management_Rental
{
    public partial class SignUp : Form
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
            public static string ConnectionString = "Host=fleetm-2026-24709.j77.aws-ap-southeast-1.cockroachlabs.cloud;Port=26257;Database=fms_rental;Username=stephens;Password=gLId5nipIimPiL-zjB_9oA;SSL Mode = Require; Trust Server Certificate=true";

        }
        public SignUp()
        {
            InitializeComponent();
            this .FormClosed += SignUp_FormClosed;
        }

        private void SignUp_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPass.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("All fields are required!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                using (var conn = Login.DbHelper.GetConnection())
                {
                    conn.Open();

                    // email already exists
                    string checkEmail = "SELECT COUNT(*) FROM clientprofile WHERE LOWER(email) = LOWER(@mail)";
                    using (var cmd = new NpgsqlCommand(checkEmail, conn))
                    {
                        cmd.Parameters.AddWithValue("@mail", email);
                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("This email is already registered. Please use another email.",
                                "Duplicate Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    string sql = "INSERT INTO clientprofile (name, email, passwordhash) VALUES (@name, @mail, @pass)";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@mail", email);
                        cmd.Parameters.AddWithValue("@pass", password);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Account created successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Login loginForm = new Login();
                loginForm.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Hide();
        }
    }
}
