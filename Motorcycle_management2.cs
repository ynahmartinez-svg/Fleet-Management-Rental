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
    public partial class Motorcycle_management2 : Form
    {
        public Motorcycle_management2()
        {
            InitializeComponent();
            this.FormClosed += Motorcycle_management2_FormClosed;
        }
        private void Motorcycle_management2_FormClosed(object sender, FormClosedEventArgs e)
        {
           
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void Motorcycle_management2_Load(object sender, EventArgs e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                string sql = @"SELECT motorcycle_id,
                              model_name AS model_brand,
                              plate_num,
                              status
                       FROM motorcycle_management
                       ORDER BY motorcycle_id ASC";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt; // bind to DataGridView
                    }

                    // Set column headers for readability
                    dataGridView1.Columns["motorcycle_id"].HeaderText = "Motorcycle ID";
                    dataGridView1.Columns["model_brand"].HeaderText = "Model/ Brand";
                    dataGridView1.Columns["plate_num"].HeaderText = "Plate Number";
                    dataGridView1.Columns["status"].HeaderText = "Status";
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
                    lblAvail.Text = availableMotorcycles.ToString();
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

                //total units
                string sqlTotal = @"SELECT COALESCE(COUNT(*), 0) 
                    FROM motorcycle_management";

                int totalUnits = 0;
                using (var cmdTotal = new NpgsqlCommand(sqlTotal, conn))
                {
                    using (var reader = cmdTotal.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            totalUnits = Convert.ToInt32(reader[0]);
                        }
                    }
                    lblTotalUnits.Text = totalUnits.ToString();
                }

            }
        }

        

        private void button12_Click(object sender, EventArgs e)
        {
            
        }

       

        

        

        private void button6_Click(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
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

        private void pictureBox2_Click(object sender, EventArgs e)
        {

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

        
        private void button11_Click_1(object sender, EventArgs e)
        {
            Motorcycle_Management mm = new Motorcycle_Management();
            mm.Show();
            this.Hide();
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
