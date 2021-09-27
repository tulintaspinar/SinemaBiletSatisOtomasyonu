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
    public partial class FilmTuru : Form
    {
        public FilmTuru()
        {
            InitializeComponent();
        }
        SqlConnection con;
        private void btnGeri_Click(object sender, EventArgs e)
        {
            AnaSayfa anasayfa = new AnaSayfa();
            anasayfa.Show();
            this.Hide();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            try
            {
                con = new SqlConnection("server=.; Initial Catalog=SinemaBiletSatis;Integrated Security=True");
                string sqlInsert = "INSERT INTO FİLMTÜRLERİ(Film_Türü) values (@filmTürü)";
                SqlCommand cmd = new SqlCommand(sqlInsert, con);
                con.Open();
                cmd.Parameters.AddWithValue("@filmTürü", txtFilmTuru.Text);
                cmd.ExecuteNonQuery();
                con.Close();
                dataGridView();
                MessageBox.Show("Film türü başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FilmTuru_Load(object sender, EventArgs e)
        {
            dataGridView();
        }

        public void dataGridView()
        {
            con = new SqlConnection("server=.; Initial Catalog=SinemaBiletSatis;Integrated Security=True");
            string sqlSelect = "SELECT * FROM FİLMTÜRLERİ";
            con.Open();
            SqlCommand komut = new SqlCommand(sqlSelect, con);
            SqlDataAdapter da = new SqlDataAdapter(komut);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }
    }
}
