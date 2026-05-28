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
    public partial class Add_Motorcycle : Form
    {  

        private string uploadedImagePath = null;
        public Add_Motorcycle()
        {
            InitializeComponent();
        }

        private void Add_Motorcycle_Load(object sender, EventArgs e)
        {

        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    uploadedImagePath = ofd.FileName;
                    picMotorcycle.Image = Image.FromFile(uploadedImagePath);

                    // Show current values in labels under the upload button
                    label1.Text = "Model: " + txtModel.Text;
                    label3.Text = "Plate: " + txtPlate.Text;
                    label2.Text = "Brand: " + txtBrand.Text;
                    label4.Text = "Price: " + txtPrice.Text;
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

            // Clean the input first (remove peso sign and spaces)
            string rawInput = txtPrice.Text.Replace("₱", "").Trim();

            if (!decimal.TryParse(rawInput, out priceValue))
            {
                MessageBox.Show("Invalid price format. Please enter a number like 650.00");
                return;
            }

            // ✅ Require user to type with a decimal point
            if (!rawInput.Contains("."))
            {
                MessageBox.Show("Please enter a decimal amount (e.g. 650.00). Whole numbers are not allowed.");
                return;
            }

            // ✅ Maximum price check (₱1000 inclusive)
            if (priceValue > 1000)
            {
                MessageBox.Show("Price cannot exceed ₱1000. Please enter a lower value.");
                return;
            }

            // ✅ Format textbox and label to 0.00 style
            txtPrice.Text = priceValue.ToString("0.00");
            label4.Text = "Price: " + priceValue.ToString("0.00");


            string finalImagePath;

            if (!string.IsNullOrEmpty(uploadedImagePath))
            {
                string destFolder = Path.Combine(Application.StartupPath, "Images");
                Directory.CreateDirectory(destFolder);

                string destPath = Path.Combine(destFolder, Path.GetFileName(uploadedImagePath));
                File.Copy(uploadedImagePath, destPath, true);

                finalImagePath = destPath;
            }
            else
            {
                // Default placeholder image
                finalImagePath = Path.Combine(Application.StartupPath, "Images", "no_photo.png");
                Directory.CreateDirectory(Path.GetDirectoryName(finalImagePath));

                if (!File.Exists(finalImagePath))
                {
                    using (Bitmap bmp = new Bitmap(200, 120))
                    {
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.Clear(Color.LightGray);
                        }
                        bmp.Save(finalImagePath);
                    }
                }
            }

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                string sql = @"INSERT INTO motorcycle_management 
                       (model_name, plate_num, brand, price_per_day, image_path, status) 
                       VALUES (@model, @plate, @brand, @price, @imagePath, 'Available')";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@model", txtModel.Text);
                    cmd.Parameters.AddWithValue("@plate", txtPlate.Text);
                    cmd.Parameters.AddWithValue("@brand", txtBrand.Text);
                    cmd.Parameters.AddWithValue("@price", priceValue);
                    cmd.Parameters.AddWithValue("@imagePath", finalImagePath);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Motorcycle added successfully!", "Add", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }




    }
}

