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
    public partial class SeansListele : Form
    {
        public SeansListele()
        {
            InitializeComponent();
        }
        SqlConnection con = new SqlConnection("server=.; Initial Catalog=SinemaBiletSatis;Integrated Security=True"); // Veritabanı bağlantısı

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            AnaSayfa anasayfa = new AnaSayfa();
            anasayfa.Show();
            this.Hide();
        }

        private void btnListele_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                string sqlSelect = "SELECT FİLMLER.Film_Adı AS [Film Adı],SALONLAR.salonAdi AS [Salon Adı],SEANSEKLEME.Tarih,SEANSEKLEME.Seans " + //AS [] ifadesi sutün adını değiştirmemizi sağlıyor 
                    "FROM SEANSEKLEME JOIN FİLMLER ON SEANSEKLEME.FilmID=FİLMLER.ID " + //İnner Join ile seansekleme tablsou ile filmler tablsou ıd'ler ile bağlandı filmAdı alındı
                    "JOIN SALONLAR ON SEANSEKLEME.SalonID=SALONLAR.salonID " +//İnner Join ile seansekleme tablsou ile salonlar tablsou ıd'ler ile bağlandı salonadı alındı
                    "WHERE SEANSEKLEME.Tarih='" +dtPicker.Text+"'";
                SqlCommand cmd = new SqlCommand(sqlSelect, con); //SQL Sorgusu ve bağlantı command nesnesine eklendixt;
                SqlDataAdapter daSelect = new SqlDataAdapter(cmd);  //SqlDataAdapter sınıfı verilerin databaseden aktarılması işlemini gerçekleştirir.
                DataTable dtSelect = new DataTable();
                daSelect.Fill(dtSelect);
                //Bir DataTable oluşturarak DataAdapter ile getirilen verileri tablo içerisine dolduruyoruz.
                dataGridView1.DataSource = dtSelect;
                con.Close();
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            con.Open();
            string sqlSelect = "SELECT FİLMLER.Film_Adı AS [Film Adı],SALONLAR.salonAdi AS [Salon Adı],SEANSEKLEME.Tarih,SEANSEKLEME.Seans " + //AS [] ifadesi sutün adını değiştirmemizi sağlıyor 
                "FROM SEANSEKLEME JOIN FİLMLER ON SEANSEKLEME.FilmID=FİLMLER.ID " + //İnner Join ile seansekleme tablsou ile filmler tablsou ıd'ler ile bağlandı filmAdı alındı
                "JOIN SALONLAR ON SEANSEKLEME.SalonID=SALONLAR.salonID";//İnner Join ile seansekleme tablsou ile salonlar tablsou ıd'ler ile bağlandı salonadı alındı
            SqlCommand cmd = new SqlCommand(sqlSelect, con); //SQL Sorgusu ve bağlantı command nesnesine eklendi;
            SqlDataAdapter daSelect = new SqlDataAdapter(cmd);  //SqlDataAdapter sınıfı verilerin databaseden aktarılması işlemini gerçekleştirir.
            DataTable dtSelect = new DataTable();
            daSelect.Fill(dtSelect);
            //Bir DataTable oluşturarak DataAdapter ile getirilen verileri tablo içerisine dolduruyoruz.
            dataGridView1.DataSource = dtSelect;
            con.Close();
        }
    }
}
