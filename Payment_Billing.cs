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
    public partial class Payment_Billing : Form
    {
        public Payment_Billing()
        {
            InitializeComponent();
        }
        private void Payment_Billing_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void Payment_Billing_Load(object sender, EventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                // ✅ Show all completed rentals for admin
                string sql = @"
            SELECT r.start_date AS transaction_date,
                   (m.price_per_day * (r.return_date - r.start_date)) AS motorcycle_price,
                   m.model_name AS motorcycle_unit,
                   (r.return_date - r.start_date) AS duration,
                   r.status AS rental_status,
                   (c.first_name || ' ' || c.last_name) AS client_name
            FROM rentals r
            JOIN motorcycle_management m ON r.motorcycle_id = m.motorcycle_id
            JOIN clientprofile c ON r.client_id = c.client_id
            WHERE r.status = 'Completed'
            ORDER BY r.start_date DESC";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var adapter = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvPmt.DataSource = dt;

                    dgvPmt.Columns["transaction_date"].HeaderText = "Transaction Date";
                    dgvPmt.Columns["motorcycle_price"].HeaderText = "Total Price";
                    dgvPmt.Columns["motorcycle_unit"].HeaderText = "Motorcycle Unit";
                    dgvPmt.Columns["duration"].HeaderText = "Duration (Days)";
                    dgvPmt.Columns["rental_status"].HeaderText = "Status";
                    dgvPmt.Columns["client_name"].HeaderText = "Client";
                }

                // --- Annual Revenue ---
                string sqlAnnual = @"
            SELECT COALESCE(SUM(m.price_per_day * (r.return_date - r.start_date)), 0)
            FROM rentals r
            JOIN motorcycle_management m ON r.motorcycle_id = m.motorcycle_id
            WHERE r.status = 'Completed'
              AND EXTRACT(YEAR FROM r.start_date) = EXTRACT(YEAR FROM CURRENT_DATE)";

                decimal annualRevenue = 0;
                using (var cmdAnnual = new NpgsqlCommand(sqlAnnual, conn))
                {
                    annualRevenue = Convert.ToDecimal(cmdAnnual.ExecuteScalar());
                    lblAnnualRev.Text = annualRevenue.ToString("₱#,##0.00");
                }

                // --- Monthly Revenue ---
                string sqlMonthly = @"
            SELECT COALESCE(SUM(m.price_per_day * (r.return_date - r.start_date)), 0)
            FROM rentals r
            JOIN motorcycle_management m ON r.motorcycle_id = m.motorcycle_id
            WHERE r.status = 'Completed'
              AND EXTRACT(YEAR FROM r.start_date) = EXTRACT(YEAR FROM CURRENT_DATE)
              AND EXTRACT(MONTH FROM r.start_date) = EXTRACT(MONTH FROM CURRENT_DATE)";

                decimal monthlyRevenue = 0;
                using (var cmdMonthly = new NpgsqlCommand(sqlMonthly, conn))
                {
                    monthlyRevenue = Convert.ToDecimal(cmdMonthly.ExecuteScalar());
                    lblMonthlyRev.Text = monthlyRevenue.ToString("₱#,##0.00");
                }
            }
        }



        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            Admin_DashBoard ad = new Admin_DashBoard();
            ad.Show();
            this.Hide();
        }

        
        private void button11_Click_1(object sender, EventArgs e)
        {
            Motorcycle_Management mm = new Motorcycle_Management();
            mm.Show();
            this.Hide();
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            Fuel_Cost_Management fcm = new Fuel_Cost_Management();
            fcm.Show();
            this.Hide();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            BookingReq bq = new BookingReq();
            bq.Show();
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

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void dgvPmt_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Admin_Notifications an = new Admin_Notifications();
            an.Show();
            this.Close();
        }

        

        
    }
}
