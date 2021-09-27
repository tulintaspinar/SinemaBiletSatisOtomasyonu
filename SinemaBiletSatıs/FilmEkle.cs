using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SinemaBiletSatıs
{
    public partial class FilmEkle : Form
    {
        public FilmEkle()
        {
            InitializeComponent();
        }
        SqlConnection con = new SqlConnection("server=.; Initial Catalog=SinemaBiletSatis;Integrated Security=True");// Veritabanı bağlantısı
        OpenFileDialog dosya; //Dosya seçimi için nesne oluşturuldu
        string dosyayolu;
        byte[] image = null; 
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                FileStream fs = new FileStream(dosyayolu, FileMode.Open, FileAccess.Read); //Dosyayolu için 
                BinaryReader br = new BinaryReader(fs); // Veritabanına image ekleme işlemini okumak için 
                image = br.ReadBytes((int)fs.Length); 

                con.Open();
                string sqlInsert = "INSERT INTO FİLMLER(Film_Adı,Yönetmen,Film_Türü,Süre,Yapım_Yılı,Tarih,Afiş) values (@FilmAdi,@Yonetmen,@FilmTuru,@Sure,@YapimYili,@Tarih,@Afis)";
                SqlCommand cmd = new SqlCommand(sqlInsert, con);//SQL Sorgusu ve bağlantı command nesnesine eklendi

                cmd.Parameters.AddWithValue("@FilmAdi", txtFilmAdı.Text); //cmd komutuna girilen değerler ekleniyor
                cmd.Parameters.AddWithValue("@Yonetmen", txtYonetmen.Text);
                cmd.Parameters.AddWithValue("@FilmTuru", cmboxFilmTuru.Text);
                cmd.Parameters.AddWithValue("@Sure", Convert.ToDouble(txtSure.Text));
                cmd.Parameters.AddWithValue("@YapimYili", txtYapimYili.Text);
                cmd.Parameters.AddWithValue("@Tarih", dtPicker.Text);
                cmd.Parameters.AddWithValue("@Afis", image);

                cmd.ExecuteNonQuery(); // Veritabanına kayıt işlemi yapılıyor
                con.Close();//Bağlantı kapatılıyor.

                MessageBox.Show("Film Kayıt İşlemi Başarılı");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu. " + ex.ToString());
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Butona basıldığında image seçimi için dosya açılıyor.
            dosya = new OpenFileDialog();
            dosya.Filter = "Resim Dosyası |*.jpg;*.jfif;*.nef;*.png |  Tüm Dosyalar |*.*";
            dosya.ShowDialog();
            dosyayolu = dosya.FileName;
            pictureBox1.ImageLocation = dosyayolu; //Seçilen image picturebox içine ekleniyor
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void FilmEkle_Load(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT * FROM FİLMTÜRLERİ";
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr; //Datagridview listeleme için nesne oluşturuldu
            con.Open();

            dr = cmd.ExecuteReader(); //komut okundu ve dr içine yazıldı
            while (dr.Read()) // dr içinde değer var ise döngü ile combobox içine ekleniyor.
            {
                cmboxFilmTuru.Items.Add(dr["Film_Türü"]);
            }

            con.Close();
        }

        private void btnGeri_Click(object sender, EventArgs e)
        {
            //Butona basılınca anasayfaya dönülecek 
            AnaSayfa anasayfa = new AnaSayfa();
            anasayfa.Show();
            this.Hide();
        }

        private void dtPicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Today;
            dtPicker.MinDate = dt;
        }
    }
}
