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
    public partial class Active_Client : Form
    {
        public Active_Client()
        {
            InitializeComponent();
            this.FormClosed += Active_Client_FormClosed;
        }
        private void Active_Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            Client_Dashboard cd = new Client_Dashboard();
            cd.Show();
            this.Hide();
        }
        private void Active_Client_Load(object sender, EventArgs e)
        {
            long clientId = SessionData.LoggedInClientId;

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                string sqlUpdate = @"UPDATE rentals
                             SET status = 'Active'
                             WHERE client_id = @cid
                               AND status = 'Approved'
                               AND start_date <= CURRENT_DATE";
                using (var cmdUpdate = new NpgsqlCommand(sqlUpdate, conn))
                {
                    cmdUpdate.Parameters.AddWithValue("@cid", clientId);
                    cmdUpdate.ExecuteNonQuery();
                }

                string sql = @"SELECT r.start_date, r.return_date, r.duration_days,
                              m.model_name, m.plate_num, m.image_path, r.status
                       FROM rentals r
                       JOIN motorcycle_management m ON r.motorcycle_id = m.motorcycle_id
                       WHERE r.client_id = @cid AND (r.status = 'Active' OR (r.status = 'Approved' AND r.start_date <= CURRENT_DATE))";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cid", clientId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        flowLayoutPanel2.Controls.Clear();

                        while (reader.Read())
                        {
                            string model = reader.GetString(3);
                            string plate = reader.GetString(4);
                            string imagePath = reader.GetString(5);
                            DateTime startDate = reader.GetDateTime(0);
                            DateTime endDate = reader.GetDateTime(1);
                            long duration = reader.IsDBNull(2) ? 0 : reader.GetInt64(2);
                            string rentalStatus = reader.GetString(6);

                            Panel rentalPanel = new Panel { Width = 410, Height = 150, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(5) };

                            PictureBox pic = new PictureBox { Width = 150, Height = 120, Dock = DockStyle.Left, SizeMode = PictureBoxSizeMode.Zoom };
                            if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                                pic.Image = Image.FromFile(imagePath);

                            Label lblInfo = new Label { Dock = DockStyle.Fill, Padding = new Padding(10), TextAlign = ContentAlignment.MiddleLeft };
                            lblInfo.Text = $"{model} {plate}\nStart: {startDate:d}\nEnd: {endDate:d}\nDuration: {duration} days\nStatus: {rentalStatus}";

                            rentalPanel.Controls.Add(lblInfo);
                            rentalPanel.Controls.Add(pic);

                            flowLayoutPanel2.Controls.Add(rentalPanel);
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


    }
}
