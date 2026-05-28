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
    public partial class Fuel_Cost_Management : Form
    {
        public Fuel_Cost_Management()
        {
            InitializeComponent();
            this.FormClosed += Fuel_Cost_Management_FormClosed;
        }
        private void Fuel_Cost_Management_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Fuel_Cost_Management_Load(object sender, EventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT 
                          motorcycle_id AS motorcycle_id,
                          model_name AS model_name,
                          brand AS brand,
                          plate_num AS plate_num,
                          COALESCE(fuel_level, 0) AS fuel_level
                       FROM motorcycle_management";

                using (var da = new NpgsqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvFuel.DataSource = dt;
                }
            }

            // ✅ Lock down columns except fuel_level
            foreach (DataGridViewColumn col in dgvFuel.Columns)
            {
                col.ReadOnly = true;
            }
            dgvFuel.Columns["fuel_level"].ReadOnly = false;

            dgvFuel.SelectionChanged += dgvFuel_SelectionChanged;

            btnUpdateFuel.Enabled = false;
            btnNotifyClient.Enabled = false;
            btnViewHistory.Enabled = false;
        }


        private void dgvFuel_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvFuel.SelectedRows.Count > 0)
            {
                var row = dgvFuel.SelectedRows[0];

                string model = row.Cells["model_name"].Value.ToString();
                string plate = row.Cells["plate_num"].Value.ToString();
                string brand = row.Cells["brand"].Value.ToString();
                int fuelLevel = Convert.ToInt32(row.Cells["fuel_level"].Value);

                lblMotorInfo.Text = $"{model} ({plate}) - {brand}";
                fuelBar.Minimum = 0;
                fuelBar.Maximum = 100;
                fuelBar.Value = fuelLevel;

                btnUpdateFuel.Enabled = true;
                btnNotifyClient.Enabled = true;   // always enabled when a row is selected
                btnViewHistory.Enabled = true;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Admin_DashBoard ad = new Admin_DashBoard();
            ad.Show();
            this.Hide();
        }


        private void button11_Click(object sender, EventArgs e)
        {
            Motorcycle_Management mm = new Motorcycle_Management();
            mm.Show();
            this.Hide();
        }

        private void button9_Click(object sender, EventArgs e)
        {
       
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Payment_Billing pb = new Payment_Billing();
            pb.Show();
            this.Hide();
        }


        private void button7_Click(object sender, EventArgs e)
        {
            Admin_Accounts aa = new Admin_Accounts();
            aa.Show();  
            this.Hide();
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
                this.Close();

                Login loginForm = new Login();
                loginForm.ShowDialog();
                this.Hide();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
              "Do you want to log out?",
              "Logout Confirmation",
              MessageBoxButtons.YesNo,
              MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                MessageBox.Show("Logged out successfully!");
                this.Close();

                Login loginForm = new Login();
                loginForm.ShowDialog();
                this.Hide();
            }
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            Admin_DashBoard aa = new Admin_DashBoard();
            aa.Show();
            this.Hide();
        }

        private void button14_Click(object sender, EventArgs e)
        {
           
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            Motorcycle_Management mm = new Motorcycle_Management();
            mm.Show();
            this.Hide();
        }

        private void button10_Click_1(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            BookingReq bq = new BookingReq();
            bq.Show();
            this.Hide();
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            Payment_Billing pb = new Payment_Billing();
            pb.Show();
            this.Hide();
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
          Admin_Notifications an = new Admin_Notifications();
            an.Show();
            this.Hide();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            Admin_Accounts aa = new Admin_Accounts();
            aa.Show(); 
            this.Hide();
        }

        private void button8_Click_1(object sender, EventArgs e)
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

        private void btnNotifyClient_Click(object sender, EventArgs e)
        {
            if (dgvFuel.SelectedRows.Count > 0)
            {
                var row = dgvFuel.SelectedRows[0];
                long motorcycleId = Convert.ToInt64(row.Cells["motorcycle_id"].Value);
                string model = row.Cells["model_name"].Value.ToString();
                int fuelLevel = Convert.ToInt32(row.Cells["fuel_level"].Value);

                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    string sql = @"INSERT INTO notifications (client_id, message)
                           SELECT r.client_id, @msg
                           FROM rentals r
                           WHERE r.motorcycle_id = @mid 
                             AND r.status IN ('Approved','Active')";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@mid", motorcycleId);
                        cmd.Parameters.AddWithValue("@msg",
                            $"Reminder: Your {model} has low fuel ({fuelLevel}%). Please refuel.");
                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Client notified about low fuel!", "Notification",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No active rentals found for this motorcycle.", "Notification",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }

        private void btnUpdateFuel_Click(object sender, EventArgs e)
        {
            if (dgvFuel.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvFuel.SelectedRows[0];
                long motorcycleId = Convert.ToInt64(row.Cells["motorcycle_id"].Value);

                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "UPDATE motorcycle_management SET fuel_level = @fuel WHERE motorcycle_id = @id";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@fuel", fuelBar.Value);
                        cmd.Parameters.AddWithValue("@id", motorcycleId);
                        cmd.ExecuteNonQuery();
                    }

                    // Log the update
                    string logSql = "INSERT INTO fuel_logs (motorcycle_id, fuel_level) VALUES (@id, @fuel)";
                    using (var logCmd = new NpgsqlCommand(logSql, conn))
                    {
                        logCmd.Parameters.AddWithValue("@id", motorcycleId);
                        logCmd.Parameters.AddWithValue("@fuel", fuelBar.Value);
                        logCmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Fuel level updated and logged successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh grid
                Fuel_Cost_Management_Load(null, null);
            }
        }

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            if (dgvFuel.SelectedRows.Count > 0)
            {
                var row = dgvFuel.SelectedRows[0];
                long motorcycleId = Convert.ToInt64(row.Cells["motorcycle_id"].Value);

                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"SELECT log_date, fuel_level 
                           FROM fuel_logs 
                           WHERE motorcycle_id = @id 
                           ORDER BY log_date DESC";

                    using (var da = new NpgsqlDataAdapter(sql, conn))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@id", motorcycleId);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvFuelHistory.DataSource = dt;
                    }
                }
            }
        }
    }
}
