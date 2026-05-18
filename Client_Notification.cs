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
    public partial class Client_Notification : Form
    {
        public Client_Notification()
        {
            InitializeComponent();
            this.FormClosed += Client_Notification_FormClosed;
        }
        private void Client_Notification_FormClosed(object sender, FormClosedEventArgs e)
        {
            Client_Dashboard ad = new Client_Dashboard();
            ad.Show();
            this.Hide();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            My_rentals r = new My_rentals();
            r.Show();
            this.Hide();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Client_Dashboard cd = new Client_Dashboard();
            cd.Show();
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
            PB.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Client_Account ca = new Client_Account();
            this.Hide();
            ca.ShowDialog();
            this.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                       "Do you want to log out?", "Logout Confirmation",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                MessageBox.Show("Logged out successfully!");

                Login loginForm = new Login();
                this.Hide();
                loginForm.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Logout cancelled.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Client_Notification_Load(object sender, EventArgs e)
        {
            long clientId = SessionData.LoggedInClientId;

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                string sql = @"SELECT message, created_at, is_read
                       FROM notifications
                       WHERE client_id = @cid
                       ORDER BY created_at DESC";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cid", clientId);

                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvNotifications.DataSource = dt;
                        dgvNotifications.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dgvNotifications.RowHeadersVisible = false;

                    }
                }
            }
        }

        private void btnNotification_Click(object sender, EventArgs e)
        {

        }

        private void btnMap_Click(object sender, EventArgs e)
        {
            Client_Map cm = new Client_Map();
            cm.Show();
            this.Hide();
        }
    }
}
