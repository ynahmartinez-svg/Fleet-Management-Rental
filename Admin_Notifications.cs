using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fleet_Management_Rental
{
    public partial class Admin_Notifications : Form
    {
        private string connectionString =
           "Host=fmsrental-26507.j77.aws-ap-southeast-1.cockroachlabs.cloud;" +  // ✅ updated host
           "Port=26257;" +
           "Database=fms_rental;" +
           "Username=stephen;" +
           "Password=jQPj8FQl2JF4afGOR37QxQ;" +  // ✅ updated password
           "SSL Mode=VerifyFull;" +
           "Trust Server Certificate=true";
        public Admin_Notifications()
        {
            InitializeComponent();

        
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            Admin_DashBoard ad = new Admin_DashBoard();
            ad.Show();
            this.Close();
        }

        private void button11_Click(object sender, EventArgs e)
        {
           Motorcycle_Management mm = new Motorcycle_Management();
            mm.Show();
            this.Close();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Fuel_Cost_Management fcm = new Fuel_Cost_Management();
            fcm.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BookingReq bq = new BookingReq();
            bq.Show();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Payment_Billing pb = new Payment_Billing();
           pb.Show();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Admin_Accounts aa = new Admin_Accounts();
            aa.Show();
            this.Close();
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

        private void Admin_Notifications_Load(object sender, EventArgs e)
        {
            string sql = @"
                SELECT r.client_id,
                       c.first_name,
                       c.last_name,
                       c.email,
                       r.status,
                       r.motorcycle_id
                FROM rentals r
                INNER JOIN clientprofile c ON r.client_id = c.client_id
                WHERE r.status IN ('Approved','Active')";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var adapter = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
        }
        // helper in terms of inserting one notifcation
        private void InsertNotification(long clientId, string message, NpgsqlConnection conn)
        {
            string insertSql = @"INSERT INTO notifications (client_id, message)
                                 VALUES (@cid, @msg)";
            using (var cmd = new NpgsqlCommand(insertSql, conn))
            {
                cmd.Parameters.AddWithValue("@cid", clientId);
                cmd.Parameters.AddWithValue("@msg", message.Trim());
                cmd.ExecuteNonQuery();
            }
        }
        // Shared logic: notify one or all
        private void NotifyClients(long? selectedClientId, string message, NpgsqlConnection conn)
        {
            if (selectedClientId.HasValue)
            {
                InsertNotification(selectedClientId.Value, message, conn);
            }
            else
            {
                string sqlClients = @"SELECT DISTINCT client_id
                                      FROM rentals
                                      WHERE status IN ('Approved','Active')";
                using (var cmd = new NpgsqlCommand(sqlClients, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    List<long> clientIds = new List<long>();
                    while (reader.Read())
                    {
                        clientIds.Add(reader.GetInt64(0));
                    }
                    reader.Close();

                    foreach (var cid in clientIds)
                    {
                        InsertNotification(cid, message, conn);
                    }
                }
            }
        }
        // notify one client
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                long selectedClientId = Convert.ToInt64(dataGridView1.CurrentRow.Cells["client_id"].Value);
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    NotifyClients(selectedClientId, txtMessage.Text, conn);
                }
                MessageBox.Show("Notification sent to selected client.");
            }
        }

        private void btnNotifyAll_Click(object sender, EventArgs e)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                NotifyClients(null, txtMessage.Text, conn);
            }
            MessageBox.Show("Notification sent to all active renters.");
        }
    }
}
