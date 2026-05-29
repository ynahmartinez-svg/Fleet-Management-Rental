using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fleet_Management_Rental
{
    public partial class booking : Form
    {
        public booking()
        {
            InitializeComponent();
            this.FormClosed += booking_FormClosed;
            this.Load += booking_Load;
        }
        private void booking_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }





        private void button7_Click(object sender, EventArgs e)
        {
            Client_Account ca = new Client_Account();
            ca.Show();
            this.Hide();
        }

        private bool IsClientProfileComplete(long clientId)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT first_name, last_name, date_of_birth, phone_no, valid_id,
                       street, city, postal_code
                FROM clientprofile
                WHERE client_id = @cid";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cid", clientId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Check if any required field is empty/null
                            return !(reader.IsDBNull(0) || reader.IsDBNull(1) || reader.IsDBNull(2) ||
                                     reader.IsDBNull(3) || reader.IsDBNull(4) ||
                                     reader.IsDBNull(5) || reader.IsDBNull(6) || reader.IsDBNull(7));
                        }
                    }
                }
            }
            return false;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            long clientId = SessionData.LoggedInClientId;

            if (!IsClientProfileComplete(clientId))
            {
                MessageBox.Show("Please complete your profile information before renting a motorcycle.",
                                "Profile Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

            }

            if (((Button)sender).Tag != null)
            {
                long motorcycleId = Convert.ToInt64(((Button)sender).Tag);

                Booking_Details bd = new Booking_Details(motorcycleId, clientId);
                bd.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("No motorcycle available for booking.",
                                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            Client_Dashboard cd = new Client_Dashboard();
            cd.Show();
            this.Hide();
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            My_rentals r = new My_rentals();
            r.Show();
            this.Hide();
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            Payments_and_Billing pAB = new Payments_and_Billing();
            pAB.Show();
            this.Hide();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            Client_Account ca = new Client_Account();
            ca.Show();
            this.Hide();
        }

        private void button8_Click_1(object sender, EventArgs e)
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

        private void booking_Load(object sender, EventArgs e)
        {
            flowLayoutPanel2.Controls.Clear();

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT motorcycle_id, model_name, plate_num, brand, price_per_day, image_path, status 
                       FROM motorcycle_management";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        long id = reader.GetInt64(0);
                        string model = reader.GetString(1);
                        string plate = reader.GetString(2);
                        string brand = reader.GetString(3);
                        decimal price = reader.GetDecimal(4);
                        string imagePath = reader.GetString(5);
                        string status = reader.GetString(6);

                        AddMotorcycleCard(id, model, plate, brand, price, imagePath, status);
                        count++;
                    }

                    if (count == 0)
                        MessageBox.Show("No motorcycles found.");
                }
            }
        }

        private void AddMotorcycleCard(long id, string model, string plate, string brand, decimal price, string imagePath, string status)
        {
            Panel card = new Panel
            {
                Size = new Size(220, 300),
                Margin = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };

            PictureBox pic = new PictureBox
            {
                Size = new Size(200, 120),
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            if (!string.IsNullOrEmpty(imagePath))
            {
                string fullPath = Path.Combine(Application.StartupPath, imagePath);
                if (File.Exists(fullPath))
                    pic.Image = Image.FromFile(fullPath);
                else
                    pic.BackColor = Color.LightGray;
            }
            else
            {
                pic.BackColor = Color.LightGray;
            }

                Label lblModel = new Label { Text = "Model: " + model, Top = 130, Left = 10, Width = 200 };
                Label lblPlate = new Label { Text = "Plate: " + plate, Top = 155, Left = 10, Width = 200 };
                Label lblBrand = new Label { Text = "Brand: " + brand, Top = 180, Left = 10, Width = 200 };
                Label lblPrice = new Label { Text = "₱" + price.ToString("0.00") + "/Day", Top = 205, Left = 10, Width = 200 };
                Label lblStatus = new Label { Text = "Status: " + status, Top = 230, Left = 10, Width = 200 };
                Button btnAction = new Button { Top = 260, Left = 10, Width = 120, Tag = id };

            // ✅ Change button text depending on status
            if (status == "Available")
            {
                btnAction.Text = "Rent Now";
                btnAction.Enabled = true;
                btnAction.Click += button15_Click;
            }
            else if (status == "Rented")
            {
                btnAction.Text = "Currently Rented";
                btnAction.Enabled = false;
            }
            else if (status == "Extended")
            {
                btnAction.Text = "Extended Rental";
                btnAction.Enabled = false;
            }
            else
            {
                btnAction.Text = "Unavailable";
                btnAction.Enabled = false;
            }

            card.Controls.Add(pic);
            card.Controls.Add(lblModel);
            card.Controls.Add(lblPlate);
            card.Controls.Add(lblBrand);
            card.Controls.Add(lblPrice);
            card.Controls.Add(lblStatus);
            card.Controls.Add(btnAction);

            flowLayoutPanel2.Controls.Add(card);
        }


        private void button2_Click(object sender, EventArgs e)
        {

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

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
