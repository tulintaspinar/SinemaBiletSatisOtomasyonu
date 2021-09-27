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
    public partial class SeansEkle : Form
    {
        public SeansEkle()
        {
            InitializeComponent();
        }

        SqlConnection con;
        string seans = "";
        private void btnEKLE_Click(object sender, EventArgs e)
        {
            con = new SqlConnection("server=.//SQLEXPRESS; Initial Catalog=SinemaBiletSatis;Integrated Security=True");
            
            con.Open();
            string sqlselectFilm = "SELECT ID FROM FİLMLER WHERE Film_Adı ='" + (cmboxFilm.Text) + "'";
            SqlCommand cmdFilm = new SqlCommand(sqlselectFilm, con);
            int filmID = (int)cmdFilm.ExecuteScalar(); //Seçilen filmin id'si veritabanından alındı.
            cmdFilm.ExecuteNonQuery();
            con.Close();//Önceki işleme ait bağlantı kapatılıyor.

            con.Open();// Yeni bir işlem için yeni bağlantı açılıyor.
            string sqlSelectSalon = "SELECT salonID FROM SALONLAR WHERE salonAdi ='" + (cmboxSalon.Text) + "'";
            SqlCommand cmdSeans = new SqlCommand(sqlSelectSalon, con);
            int salonID = (int)cmdSeans.ExecuteScalar(); //Seçilen salonun id'si veritabanından alındı.
            cmdSeans.ExecuteNonQuery();
            con.Close();

            try
            {
                con.Open();
                string select = "SELECT * FROM SEANSEKLEME WHERE Seans ='"+seans+"' and Tarih='"+dtPicker.Text.ToString()+"' and SalonID="+salonID+" and FilmID="+filmID;
                SqlCommand kontrol = new SqlCommand(select,con);
                SqlDataReader dr = kontrol.ExecuteReader(); // Seçilen salon, tarih ve seansa ait kayıt olup olmadığı kontrol edildi.
                
                if (dr.Read()==false) //Eğer kayıt yok ise daha önce eklenmiş filme ait seans yoktur ve o seansa kayıt yapılabilir.
                {
                    con.Close();
                    con.Open(); 
                    string sqlInsert = "INSERT INTO SEANSEKLEME (FilmID,SalonID,Tarih,Seans) VALUES (@filmID,@salonID,@tarih,@seans) ";
                    SqlCommand cmd = new SqlCommand(sqlInsert, con);

                    cmd.Parameters.AddWithValue("@filmID", filmID);
                    cmd.Parameters.AddWithValue("@salonID", salonID);
                    cmd.Parameters.AddWithValue("@tarih", dtPicker.Text);
                    cmd.Parameters.AddWithValue("@seans", seans);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Seans eklendi.");
                    seansDurumu();
                    
                }
                else
                {
                    MessageBox.Show("Seçilen tarih seans bulunmaktadır.");
                    
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            try
            {
                con.Open();
                // Salona ait koltukların eklenmesi yapılıyor.
                string sqlInsertKoltuk = "INSERT INTO KOLTUKDURUMU(FilmID ,SalonID,Tarih,Seans,KoltukNumarası,Durum) " +
                    "VALUES(@filmID, @salonID,@tarih,@seansID,@kNo,@durum)";

                for (int i = 1; i <= 20; i++) // A ve B numaralı koltuklardan 5'er tane eklemek için döngü oluşturuldu.
                {
                    SqlCommand cmdInsert = new SqlCommand(sqlInsertKoltuk, con);
                    if (i <= 5)//i'nin değeri 5'den küçük ise A harfli koltuklar ekleniyor
                    {
                        cmdInsert.Parameters.AddWithValue("@filmID", filmID);
                        cmdInsert.Parameters.AddWithValue("@salonID", salonID);
                        cmdInsert.Parameters.AddWithValue("@tarih", dtPicker.Text.ToString());
                        cmdInsert.Parameters.AddWithValue("@seansID", seans);
                        cmdInsert.Parameters.AddWithValue("@kNo", ("A" + i));
                        cmdInsert.Parameters.AddWithValue("@durum", "BOŞ");
                        cmdInsert.ExecuteNonQuery();//Veritabanında değişikliği sağlayan komut.
                    }
                    else if (i >= 6 && i <= 10)
                    {
                        cmdInsert.Parameters.AddWithValue("@filmID", filmID);
                        cmdInsert.Parameters.AddWithValue("@salonID", salonID);
                        cmdInsert.Parameters.AddWithValue("@tarih", dtPicker.Text.ToString());
                        cmdInsert.Parameters.AddWithValue("@seansID", seans);
                        cmdInsert.Parameters.AddWithValue("@kNo", ("B" + (i-5)));//B1,B2 olması için i değeri azaltıldı.
                                                                             //(Burada i değeri 6'dan başlayacak)
                        cmdInsert.Parameters.AddWithValue("@durum", "BOŞ");

                        cmdInsert.ExecuteNonQuery();//Veritabanında değişikliği sağlayan komut.
                    }
                    else if (i >= 11 && i <= 15)
                    {
                        cmdInsert.Parameters.AddWithValue("@filmID", filmID);
                        cmdInsert.Parameters.AddWithValue("@salonID", salonID);
                        cmdInsert.Parameters.AddWithValue("@tarih", dtPicker.Text.ToString());
                        cmdInsert.Parameters.AddWithValue("@seansID", seans);
                        cmdInsert.Parameters.AddWithValue("@kNo", ("C" + (i - 10)));
                        cmdInsert.Parameters.AddWithValue("@durum", "BOŞ");
                        cmdInsert.ExecuteNonQuery();
                    }
                    else
                    {
                        cmdInsert.Parameters.AddWithValue("@filmID", filmID);
                        cmdInsert.Parameters.AddWithValue("@salonID", salonID);
                        cmdInsert.Parameters.AddWithValue("@tarih", dtPicker.Text.ToString());
                        cmdInsert.Parameters.AddWithValue("@seansID", seans);
                        cmdInsert.Parameters.AddWithValue("@kNo", ("D" + (i - 15)));
                        cmdInsert.Parameters.AddWithValue("@durum", "BOŞ");
                        cmdInsert.ExecuteNonQuery();
                    }
                }
                con.Close(); //Veritabanında değişikliği sağlayan komut.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                con.Close();
            }
        }
        private void radio_10_CheckedChanged(object sender, EventArgs e)
        {
            seans = "10:00";
        }

        private void radio_11_CheckedChanged(object sender, EventArgs e)
        {
            seans = "11:00";
        }

        private void radio_12_CheckedChanged(object sender, EventArgs e)
        {
            seans = "12:00";
        }

        private void radio_13_CheckedChanged(object sender, EventArgs e)
        {
            seans = "13:00";
        }

        private void radio_14_CheckedChanged(object sender, EventArgs e)
        {
            seans = "14:00";
        }

        private void radio_15_CheckedChanged(object sender, EventArgs e)
        {
            seans = "15:00";
        }

        private void radio_16_CheckedChanged(object sender, EventArgs e)
        {
            seans = "16:00";
        }

        private void radio_17_CheckedChanged(object sender, EventArgs e)
        {
            seans = "17:00";
        }

        private void radio_18_CheckedChanged(object sender, EventArgs e)
        {
            seans = "18:00";
        }

        private void radio_19_CheckedChanged(object sender, EventArgs e)
        {
            seans = "19:00";
        }

        private void radio_20_CheckedChanged(object sender, EventArgs e)
        {
            seans = "20:00";
        }

        private void radio_21_CheckedChanged(object sender, EventArgs e)
        {
            seans = "21:00";
        }

        private void SeansEkle_Load(object sender, EventArgs e)
        {
            try
            {
                con = new SqlConnection("server=.//SQLEXPRESS; Initial Catalog=SinemaBiletSatis;Integrated Security=True");
                SqlCommand cmd = new SqlCommand();
                con.Open();
                cmd.CommandText = "SELECT * FROM FİLMLER";
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    cmboxFilm.Items.Add(dr["Film_Adı"]);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu " + ex.ToString());
            }
            try
            {
                con = new SqlConnection("server=.//SQLEXPRESS; Initial Catalog=SinemaBiletSatis;Integrated Security=True");
                SqlCommand cmd2 = new SqlCommand();
                con.Open();
                cmd2.CommandText = "SELECT * FROM SALONLAR";
                cmd2.Connection = con;
                cmd2.CommandType = CommandType.Text;

                SqlDataReader dr2;
                dr2 = cmd2.ExecuteReader();
                while (dr2.Read())
                {
                    cmboxSalon.Items.Add(dr2["salonAdi"]);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu " + ex.ToString());
            }
        }

        private void btnGeri_Click(object sender, EventArgs e)
        {
            AnaSayfa anaSayfa = new AnaSayfa();
            anaSayfa.Show();
            this.Hide();
        }

        private void dtPicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Today;
            dtPicker.MinDate = dt;
            seansDurumu();
        }
        private void seansDurumu()
        {
            radio_10.Enabled = true;
            radio_11.Enabled = true;
            radio_12.Enabled = true;
            radio_13.Enabled = true;
            radio_14.Enabled = true;
            radio_15.Enabled = true;
            radio_16.Enabled = true;
            radio_17.Enabled = true;
            radio_18.Enabled = true;
            radio_19.Enabled = true;
            radio_20.Enabled = true;
            radio_21.Enabled = true;
            con = new SqlConnection("server=.//SQLEXPRESS; Initial Catalog=SinemaBiletSatis;Integrated Security=True");
            con.Open();
            string sqlselectFilm = "SELECT ID FROM FİLMLER WHERE Film_Adı ='" + (cmboxFilm.Text) + "'";
            SqlCommand cmdFilm = new SqlCommand(sqlselectFilm, con);
            int filmID = (int)cmdFilm.ExecuteScalar(); //Seçilen filmin id'si veritabanından alındı.
            con.Close();//Önceki işleme ait bağlantı kapatılıyor.

            con.Open();
            string filmSure = "select Süre from FİLMLER where ID=" + filmID;
            SqlCommand cmdSure = new SqlCommand(filmSure, con);
            double sure = (double)cmdSure.ExecuteScalar();
            con.Close();//Seçilen filmin FİLMLER tablosundan süresi veritabanından alındı. 

            con.Open();// Yeni bir işlem için yeni bağlantı açılıyor.
            string sqlSelectSalon = "SELECT salonID FROM SALONLAR WHERE salonAdi ='" + (cmboxSalon.Text) + "'";
            SqlCommand cmdSeans = new SqlCommand(sqlSelectSalon, con);
            int salonID = (int)cmdSeans.ExecuteScalar(); //Seçilen salonun id'si veritabanından alındı.
            con.Close();

            con.Open();
            string seanslar = " select Seans from SEANSEKLEME where FilmID=" + filmID + " and SalonID=" + salonID + " and Tarih='" + dtPicker.Text + "'";
            SqlCommand cmd = new SqlCommand(seanslar, con);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if (dr["Seans"].ToString().Trim(' ') == "10:00")
                {
                    radio_10.Enabled = false;
                    if (sure == 2)
                    {
                        radio_11.Enabled = false;
                        radio_12.Enabled = false;
                    }
                    else if (sure == 3)
                    {
                        radio_11.Enabled = false;
                        radio_12.Enabled = false;
                        radio_13.Enabled = false;
                    }
                }
                if (dr["Seans"].ToString().Trim(' ') == "11:00")
                {
                    radio_11.Enabled = false;
                    if (sure == 2)
                    {
                        radio_12.Enabled = false;
                        radio_13.Enabled = false;
                    }
                    else if (sure == 3)
                    {
                        radio_12.Enabled = false;
                        radio_13.Enabled = false;
                        radio_14.Enabled = false;
                    }
                }
                if (dr["Seans"].ToString().Trim(' ') == "12:00")
                {
                    radio_12.Enabled = false;
                    if (sure == 2)
                    {
                        radio_13.Enabled = false;
                        radio_14.Enabled = false;
                    }
                    else if (sure == 3)
                    {
                        radio_13.Enabled = false;
                        radio_14.Enabled = false;
                        radio_15.Enabled = false;
                    }
                }
                if (dr["Seans"].ToString().Trim(' ') == "13:00")
                {
                    radio_13.Enabled = false;
                    if (sure == 2)
                    {
                        radio_14.Enabled = false;
                        radio_15.Enabled = false;
                    }
                    else if (sure == 3)
                    {
                        radio_14.Enabled = false;
                        radio_15.Enabled = false;
                        radio_16.Enabled = false;
                    }
                }
                if (dr["Seans"].ToString().Trim(' ') == "14:00")
                {
                    radio_14.Enabled = false;
                    if (sure == 2)
                    {
                        radio_15.Enabled = false;
                        radio_16.Enabled = false;
                    }
                    else if (sure == 3)
                    {
                        radio_15.Enabled = false;
                        radio_16.Enabled = false;
                        radio_17.Enabled = false;
                    }
                }
                if (dr["Seans"].ToString().Trim(' ') == "15:00")
                {
                    radio_15.Enabled = false;
                    if (sure == 2)
                    {
                        radio_16.Enabled = false;
                        radio_17.Enabled = false;
                    }
                    else if (sure == 3)
                    {
                        radio_16.Enabled = false;
                        radio_17.Enabled = false;
                        radio_18.Enabled = false;
                    }
                }
                if (dr["Seans"].ToString().Trim(' ') == "16:00")
                {
                    radio_16.Enabled = false;
                    if (sure == 2)
                    {
                        radio_17.Enabled = false;
                        radio_18.Enabled = false;
                    }
                    else if (sure == 3)
                    {
                        radio_17.Enabled = false;
                        radio_18.Enabled = false;
                        radio_19.Enabled = false;
                    }
                }
                if (dr["Seans"].ToString().Trim(' ') == "17:00")
                {
                    radio_17.Enabled = false;
                    if (sure == 2)
                    {
                        radio_18.Enabled = false;
                        radio_19.Enabled = false;
                    }
                    else if (sure == 3)
                    {
                        radio_18.Enabled = false;
                        radio_19.Enabled = false;
                        radio_20.Enabled = false;
                    }
                }
                if (dr["Seans"].ToString().Trim(' ') == "18:00")
                {
                    radio_18.Enabled = false;
                    if (sure == 2)
                    {
                        radio_19.Enabled = false;
                        radio_20.Enabled = false;
                    }
                    else if (sure == 3)
                    {
                        radio_19.Enabled = false;
                        radio_20.Enabled = false;
                        radio_21.Enabled = false;
                    }
                }
                if (dr["Seans"].ToString().Trim(' ') == "19:00")
                {
                    radio_19.Enabled = false;
                    if (sure == 2)
                    {
                        radio_20.Enabled = false;
                        radio_21.Enabled = false;
                    }
                    else if (sure == 3)
                    {
                        radio_20.Enabled = false;
                        radio_21.Enabled = false;
                    }
                }
                if (dr["Seans"].ToString().Trim(' ') == "20:00")
                {
                    radio_20.Enabled = false;
                    if (sure == 2)
                    {
                        radio_21.Enabled = false;
                    }
                    else if (sure == 3)
                    {
                        radio_21.Enabled = false;
                    }
                }
                if (dr["Seans"].ToString().Trim(' ') == "21:00")
                {
                    radio_21.Enabled = false;
                }
            }
            con.Close();
        }
    }
}
