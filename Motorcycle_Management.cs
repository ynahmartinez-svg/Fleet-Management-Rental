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
    public partial class Motorcycle_Management : Form
    {
        private long selectedMotorcycleId = -1;
        private string selectedStatus = "";
        private Panel currentlySelectedCard = null;
        public Motorcycle_Management()
        {
            InitializeComponent();

        }
        private void Motorcycle_Management_FormClosed(object sender, FormClosedEventArgs e)
        {
           
        }

        private void Motorcycle_Management_Load(object sender, EventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                // ✅ Auto-reset motorcycles that are marked as Rented but have no active rentals
                string sqlUpdate = @"UPDATE motorcycle_management m
                             SET status = 'Available'
                             WHERE m.status = 'Rented'
                               AND NOT EXISTS (
                                   SELECT 1 FROM rentals r
                                   WHERE r.motorcycle_id = m.motorcycle_id
                                     AND r.status IN ('Active','Approved')
                               );";

                using (var cmdUpdate = new NpgsqlCommand(sqlUpdate, conn))
                {
                    cmdUpdate.ExecuteNonQuery();
                }
            }

            LoadMotorcycles();
        }
        private void LoadMotorcycles()
        {
            flowLayoutPanel1.Controls.Clear();

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT DISTINCT motorcycle_id, model_name, plate_num, brand, price_per_day, image_path, status 
                   FROM motorcycle_management";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        long id = reader.GetInt64(0);
                        string model = reader.GetString(1);
                        string plate = reader.GetString(2);
                        string brand = reader.GetString(3);
                        decimal price = reader.GetDecimal(4);
                        string imagePath = reader.GetString(5);
                        string status = reader.GetString(6);

                        Panel card = new Panel
                        {
                            Size = new Size(220, 280),
                            Margin = new Padding(10),
                            BorderStyle = BorderStyle.FixedSingle,
                            Tag = new Tuple<long, string>(id, status),
                            BackColor = Color.White
                        };

                        PictureBox pic = new PictureBox
                        {
                            Size = new Size(200, 120),
                            Location = new Point(10, 10),
                            SizeMode = PictureBoxSizeMode.StretchImage
                        };

                        if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                            pic.Image = Image.FromFile(imagePath);
                        else
                            pic.BackColor = Color.LightGray;

                        Label lblModel = new Label { Text = "Model: " + model, Top = 140, Left = 10, Width = 200 };
                        Label lblPlate = new Label { Text = "Plate: " + plate, Top = 165, Left = 10, Width = 200 };
                        Label lblBrand = new Label { Text = "Brand: " + brand, Top = 190, Left = 10, Width = 200 };
                        Label lblPrice = new Label { Text = "₱" + price.ToString("0.00") + "/Day", Top = 215, Left = 10, Width = 200 };
                        Label lblStatus = new Label { Text = "Status: " + status, Top = 240, Left = 10, Width = 200 };

                        card.Controls.Add(pic);
                        card.Controls.Add(lblModel);
                        card.Controls.Add(lblPlate);
                        card.Controls.Add(lblBrand);
                        card.Controls.Add(lblPrice);
                        card.Controls.Add(lblStatus);

                        // Shared click handler
                        EventHandler selectHandler = (s, e) =>
                        {
                            var tuple = (Tuple<long, string>)card.Tag;

                            // Toggle selection
                            if (currentlySelectedCard == card)
                            {
                                // Deselect
                                card.BackColor = Color.White;
                                currentlySelectedCard = null;
                                selectedMotorcycleId = -1;
                                selectedStatus = "";
                            }
                            else
                            {
                                // Clear previous selection
                                foreach (Panel p in flowLayoutPanel1.Controls)
                                    p.BackColor = Color.White;

                                // Select new card
                                card.BackColor = Color.LightBlue;
                                currentlySelectedCard = card;
                                selectedMotorcycleId = tuple.Item1;
                                selectedStatus = tuple.Item2;

                                if (selectedStatus == "Rented")
                                {
                                    MessageBox.Show("You cannot update this vehicle! It is still rented!");
                                }
                            }
                        };

                        // Attach handler to card and all children
                        card.Click += selectHandler;
                        pic.Click += selectHandler;
                        lblModel.Click += selectHandler;
                        lblPlate.Click += selectHandler;
                        lblBrand.Click += selectHandler;
                        lblPrice.Click += selectHandler;
                        lblStatus.Click += selectHandler;

                        flowLayoutPanel1.Controls.Add(card);
                    }
                }
            }
        }
     
        
        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            Admin_DashBoard ad = new Admin_DashBoard();
            ad.Show();
            this.Hide();
        }

        
        private void button14_Click(object sender, EventArgs e)
        {
            Fuel_Cost_Management fcm = new Fuel_Cost_Management();
            fcm.Show();
            this.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            BookingReq bq = new BookingReq();
            bq.Show();
            this.Hide();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Payment_Billing pb = new Payment_Billing();
            pb.Show();
            this.Hide();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Admin_Accounts aa = new Admin_Accounts();
            aa.Show();
            this.Hide();
        }

        private void button9_Click(object sender, EventArgs e)
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

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            Admin_Notifications an = new Admin_Notifications();
            an.Show();
            this.Hide();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            Add_Motorcycle addForm = new Add_Motorcycle();
            addForm.ShowDialog();
            LoadMotorcycles();
        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            if (selectedMotorcycleId == -1)
            {
                MessageBox.Show("Please select a motorcycle first.");
                return;
            }

            if (selectedStatus == "Rented")
            {
                MessageBox.Show("You cannot update this vehicle! It is still rented!");
                return;
            }

            Update_Motorcycle updateForm = new Update_Motorcycle(selectedMotorcycleId);
            updateForm.ShowDialog();
            LoadMotorcycles();
        }

        private void btnRemove_Click_1(object sender, EventArgs e)
        {
            if (selectedMotorcycleId == -1)
            {
                MessageBox.Show("Please select a motorcycle first.");
                return;
            }
            // Prevent deletion if motorcycle is rented
            if (selectedStatus == "Rented")
            {
                MessageBox.Show("You cannot delete this vehicle because it is currently rented.");
                return;
            }

            var result = MessageBox.Show(
                "Do you want to delete this vehicle permanently from the database?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    // Check if motorcycle has rental history
                    string checkSql = "SELECT COUNT(*) FROM rentals WHERE motorcycle_id = @id";
                    using (var checkCmd = new NpgsqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@id", selectedMotorcycleId);
                        int rentalCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (rentalCount > 0)
                        {
                            MessageBox.Show("This motorcycle cannot be deleted because it has rental records.",
                                "Delete Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Safe delete
                    string sql = "DELETE FROM motorcycle_management WHERE motorcycle_id = @id";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedMotorcycleId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Vehicle deleted successfully.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //  Reset selection so you don’t accidentally reuse old ID
                selectedMotorcycleId = -1;
                selectedStatus = "";
                currentlySelectedCard = null;

                //  Refresh the list
                LoadMotorcycles();
            }
        }

        private void btnNxtPage_Click(object sender, EventArgs e)
        {
            Motorcycle_management2   mm2 = new Motorcycle_management2();
            mm2.Show();
            this.Hide();
        }
    }
 }
