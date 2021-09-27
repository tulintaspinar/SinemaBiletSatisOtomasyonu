using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SinemaBiletSatıs
{
    public partial class Satıslar : Form
    {
        public Satıslar()
        {
            InitializeComponent();
        }
        SqlConnection con = new SqlConnection("server=.; Initial Catalog=SinemaBiletSatis;Integrated Security=True");
        private void dtPicker_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                string sqlSelectSatislar = "SELECT * FROM SATILANBİLETLERR WHERE FilmTarihi='" + dtPicker.Text.ToString() + "'";
                SqlCommand cmd = new SqlCommand(sqlSelectSatislar, con);
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double toplamUcret = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if(dataGridView1.Rows[i].Cells["Ücret"].Value != null)
                {
                    string deger = dataGridView1.Rows[i].Cells["Ücret"].Value.ToString();
                    toplamUcret += Convert.ToDouble(deger);
                }
                
                
            }
            label1.Text = Convert.ToString(toplamUcret);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AnaSayfa anasayfa = new AnaSayfa();
            anasayfa.Show();
            this.Hide();
        }
    }
}
