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
    public partial class Admin_DashBoard : Form
    {
        public Admin_DashBoard()
        {
            InitializeComponent();
        }

        private void Admin_DashBoard_FormClosed(object sender, FormClosedEventArgs e)
        {

        }



        private void Admin_DashBoard_Load(object sender, EventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                // total clients
                
                string sqlClients = @"SELECT COALESCE(COUNT(*), 0) 
                      FROM clientprofile 
                      WHERE role = 'Client'";

                int totalClients = 0;

                using (var cmdClients = new NpgsqlCommand(sqlClients, conn))
                {
                    using (var reader = cmdClients.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            totalClients = Convert.ToInt32(reader[0]);
                        }
                    }
                    lblTotalClients.Text = totalClients.ToString();
                }


                // rented motorcycles 
               
                string sqlRented = @"SELECT COALESCE(COUNT(*), 0) 
                     FROM motorcycle_management 
                     WHERE status = 'Rented' OR status = 'Ongoing'";

                int rentedMotorcycles = 0;

                using (var cmdRented = new NpgsqlCommand(sqlRented, conn))
                {
                    using (var reader = cmdRented.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            rentedMotorcycles = Convert.ToInt32(reader[0]);
                        }
                    }
                    lblRented.Text = rentedMotorcycles.ToString();
                }


                // available motorcycles

                string sqlAvailable = @"SELECT COALESCE(COUNT(*), 0) 
                        FROM motorcycle_management 
                        WHERE status = 'Available'";

                int availableMotorcycles = 0;

                using (var cmdAvailable = new NpgsqlCommand(sqlAvailable, conn))
                {
                    using (var reader = cmdAvailable.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            availableMotorcycles = Convert.ToInt32(reader[0]);
                        }
                    }
                    lblAvailable.Text = availableMotorcycles.ToString();
                }
                //bookings per month chart
                string sqlBookings = @"SELECT DATE_TRUNC('month', start_date) AS month, COUNT(*) AS booking_count
                               FROM rentals
                               GROUP BY month
                               ORDER BY month";

                using (var cmdBookings = new NpgsqlCommand(sqlBookings, conn))
                using (var reader = cmdBookings.ExecuteReader())
                {
                    chart3.Series.Clear();
                    var series = new System.Windows.Forms.DataVisualization.Charting.Series("Bookings");
                    series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;

                    while (reader.Read())
                    {
                        DateTime month = reader.GetDateTime(0);
                        int count = reader.GetInt32(1);

                        series.Points.AddXY(month.ToString("MMM yyyy"), count);
                    }

                    chart3.Series.Add(series);
                }
            }
        }

        

        private void button1_Click_1(object sender, EventArgs e)
        {
            BookingReq bq = new BookingReq();
            bq.Show();
            this.Close();
        }

        private void button10_Click(object sender, EventArgs e)
        {

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

        private void button5_Click_1(object sender, EventArgs e)
        {
            Payment_Billing pb = new Payment_Billing();
            pb.Show();
            this.Close();
        }

        
        private void button7_Click_1(object sender, EventArgs e)
        {
            Admin_Accounts aa = new Admin_Accounts();
            aa.Show();
            this.Close();
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chart3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Admin_Notifications an = new Admin_Notifications();
            an.Show();
            this.Close();
        }

        private void lblAvailable_Click(object sender, EventArgs e)
        {

        }

        private void lblTotalClients_Click(object sender, EventArgs e)
        {

        }
    }
}
