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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Fleet_Management_Rental
{
    public partial class My_rentals : Form
    {

        public My_rentals()
        {
            InitializeComponent();

        }
        private void My_rentals_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void My_rentals_Load(object sender, EventArgs e)
        {
            long clientId = SessionData.LoggedInClientId;

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                // ✅ Auto-update expired rentals to Completed
                string sqlUpdate = @"UPDATE rentals
                             SET status = 'Completed'
                             WHERE client_id = @cid
                               AND status IN ('Active','Approved')
                               AND return_date < CURRENT_DATE;

                            UPDATE motorcycle_management
                            SET status = 'Available'
                            WHERE motorcycle_id IN(
                            SELECT motorcycle_id
                            FROM rentals
                            WHERE client_id = @cid
                           AND status = 'Completed'
                           AND return_date < CURRENT_DATE
                     ); ";

                using (var cmdUpdate = new NpgsqlCommand(sqlUpdate, conn))
                {
                    cmdUpdate.Parameters.AddWithValue("@cid", clientId);
                    cmdUpdate.ExecuteNonQuery();
                }

                // ✅ Now fetch rentals with corrected statuses
                string sql = @"SELECT m.motorcycle_id, m.model_name, m.plate_num, m.image_path,
                              r.rental_id, r.start_date, r.return_date, r.duration_days, r.status AS rental_status
                       FROM rentals r
                       JOIN motorcycle_management m ON r.motorcycle_id = m.motorcycle_id
                       WHERE r.client_id = @cid
                       ORDER BY r.start_date DESC";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cid", clientId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        flowLayoutPanel2.Controls.Clear();

                        while (reader.Read())
                        {
                            long motorcycleId = reader.GetInt64(0);
                            string model = reader.GetString(1);
                            string plate = reader.GetString(2);
                            string imagePath = reader.GetString(3);

                            long rentalId = reader.IsDBNull(4) ? 0 : reader.GetInt64(4);
                            string startDate = reader.IsDBNull(5) ? "N/A" : reader.GetDateTime(5).ToShortDateString();
                            string endDate = reader.IsDBNull(6) ? "N/A" : reader.GetDateTime(6).ToShortDateString();
                            string duration = reader.IsDBNull(7) ? "0" : reader.GetInt64(7).ToString();
                            string rentalStatus = reader.IsDBNull(8) ? "Not Rented" : reader.GetString(8);

                            // Panel for each rental
                            Panel motorPanel = new Panel
                            {
                                Width = 420,
                                Height = 160,
                                BorderStyle = BorderStyle.FixedSingle,
                                Margin = new Padding(5)
                            };

                            PictureBox pic = new PictureBox
                            {
                                Width = 150,
                                Height = 120,
                                Dock = DockStyle.Left,
                                SizeMode = PictureBoxSizeMode.Zoom
                            };
                            if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                                pic.Image = Image.FromFile(imagePath);
                            else
                                pic.BackColor = Color.LightGray;

                            Label lblInfo = new Label
                            {
                                Dock = DockStyle.Fill,
                                TextAlign = ContentAlignment.MiddleLeft,
                                Padding = new Padding(10),
                                AutoSize = false,
                                Text = $"{model} ({plate})\n" +
                                       $"Start: {startDate}\n" +
                                       $"End: {endDate}\n" +
                                       $"Duration: {duration} days\n" +
                                       $"Status: {rentalStatus}"
                            };

                            Button btnAction = new Button
                            {
                                Text = rentalId > 0 ? "Extend Rental" : "Rent Now",
                                Dock = DockStyle.Bottom,
                                Enabled = rentalStatus == "Active" || rentalStatus == "Approved"
                            };
                            btnAction.Tag = new { MotorcycleId = motorcycleId, RentalId = rentalId };
                            btnAction.Click += BtnExtend_Click_1;

                            motorPanel.Controls.Add(lblInfo);
                            motorPanel.Controls.Add(pic);
                            motorPanel.Controls.Add(btnAction);

                            flowLayoutPanel2.Controls.Add(motorPanel);
                        }
                    }
                }
            }
        }



        private void button12_Click(object sender, EventArgs e)
        {
            My_rentals r = new My_rentals();
            r.Show();
            this.Hide();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            Completed_Client cc = new Completed_Client();
            cc.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Upcoming_Client uc = new Upcoming_Client();
            uc.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Active_Client ac = new Active_Client();
            ac.Show();
            this.Hide();
        }


        private void button10_Click_1(object sender, EventArgs e)
        {
            Client_Dashboard cd = new Client_Dashboard();
            cd.Show();
            this.Hide();
        }

        private void button12_Click_1(object sender, EventArgs e)
        {

        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            booking b = new booking();
            b.Show();
            this.Hide();
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            Payments_and_Billing pAB = new Payments_and_Billing();
            pAB.Show();
            this.Hide();
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            Client_Account ca = new Client_Account();
            ca.Show();
            this.Hide();
        }

        private void button9_Click(object sender, EventArgs e)
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



        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void BtnExtend_Click_1(object sender, EventArgs e)
        {
            dynamic tag = ((Button)sender).Tag;
            long motorcycleId = tag.MotorcycleId;
            long rentalId = tag.RentalId;
            long clientId = SessionData.LoggedInClientId;

            Booking_Details bd = new Booking_Details(motorcycleId, clientId, rentalId, true);
            bd.Show();
            this.Hide();

        }



        private void btnNotification_Click(object sender, EventArgs e)
        {
            Client_Notification cn = new Client_Notification();
            cn.Show();
            this.Hide();
        }

        private void btnMap_Click(object sender, EventArgs e)
        {
            Client_Map cm = new Client_Map();
            cm.Show();
            this.Hide();
        }
    }
}
