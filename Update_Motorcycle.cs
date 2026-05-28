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
using System.IO;

namespace Fleet_Management_Rental
{
    public partial class Update_Motorcycle : Form
    {
        private long motorcycleId;
        private string uploadedIdPath;

        public Update_Motorcycle(long mid)
        {
            InitializeComponent();

            motorcycleId = mid;

            // Load the motorcycle details using the ID
            this.Load += Update_Motorcycle_Load;
            this.FormClosed += Update_Motorcycle_FormClosed;
        }
         private void Update_Motorcycle_Load(object sender, EventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                string sql = @"SELECT model_name, plate_num, brand, price_per_day, image_path 
                       FROM motorcycle_management WHERE motorcycle_id = @id";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", motorcycleId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string model = reader.GetString(0);
                            string plate = reader.GetString(1);
                            string brand = reader.GetString(2);
                            decimal price = reader.GetDecimal(3);
                            string imagePath = reader.GetString(4);

                            // Set textbox values
                            txtModel.Text = model;
                            txtPlate.Text = plate;
                            txtBrand.Text = brand;
                            txtPrice.Text = price.ToString("0.00");

                            // Set label values (under upload button)
                            lblModel.Text = "Model: " + model;
                            lblPlate.Text = "Plate: " + plate;
                            lblBrand.Text = "Brand: " + brand;
                            lblPrice.Text = "₱" + price.ToString("0.00") + "/Day";

                            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                                picMotorcycle.Image = Image.FromFile(imagePath);
                        }
                    }
                }
            }
        }

        private void Update_Motorcycle_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
        private void btnUpdate_Click(object sender, EventArgs e) // upload pic
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    uploadedIdPath = ofd.FileName;
                    picMotorcycle.Image = Image.FromFile(uploadedIdPath);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtModel.Text) ||
                string.IsNullOrWhiteSpace(txtPlate.Text) ||
                string.IsNullOrWhiteSpace(txtBrand.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Please fill in all fields before saving.");
                return;
            }

            decimal priceValue;
            if (!decimal.TryParse(txtPrice.Text, out priceValue))
            {
                MessageBox.Show("Invalid price format. Please enter a number like 650.00");
                return;
            }

            if (priceValue > 1000)
            {
                MessageBox.Show("Price cannot exceed ₱1000. Please enter a lower value.");
                return;
            }

            if (!txtPrice.Text.Contains("."))
            {
                MessageBox.Show("Please enter a decimal amount (e.g. 650.00). Whole numbers are not allowed.");
                return;
            }

            txtPrice.Text = priceValue.ToString("0.00");
            label4.Text = "Price: " + priceValue.ToString("0.00");

            string finalImagePath;

            if (!string.IsNullOrEmpty(uploadedIdPath))
            {
                // ✅ User uploaded a new image
                string destFolder = Path.Combine(Application.StartupPath, "Images");
                Directory.CreateDirectory(destFolder);

                string destPath = Path.Combine(destFolder, Path.GetFileName(uploadedIdPath));
                File.Copy(uploadedIdPath, destPath, true);

                finalImagePath = destPath;
            }
            else
            {
                // ✅ Keep the old image path from database
                finalImagePath = GetCurrentImagePath(motorcycleId);
            }

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                string sql = @"UPDATE motorcycle_management
                       SET model_name = @model,
                           plate_num = @plate,
                           brand = @brand,
                           price_per_day = @price,
                           image_path = @imagePath
                       WHERE motorcycle_id = @id";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@model", txtModel.Text);
                    cmd.Parameters.AddWithValue("@plate", txtPlate.Text);
                    cmd.Parameters.AddWithValue("@brand", txtBrand.Text);
                    cmd.Parameters.AddWithValue("@price", priceValue);
                    cmd.Parameters.AddWithValue("@imagePath", finalImagePath);
                    cmd.Parameters.AddWithValue("@id", motorcycleId);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Motorcycle updated successfully!", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Helper method to fetch current image path
        private string GetCurrentImagePath(long id)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT image_path FROM motorcycle_management WHERE motorcycle_id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var result = cmd.ExecuteScalar();
                    return result == DBNull.Value ? Path.Combine(Application.StartupPath, "Images", "no_photo.png") : (string)result;
                }
            }
        }




        private void lblBrand_Click(object sender, EventArgs e)
        {
           
        }
    }
 }

