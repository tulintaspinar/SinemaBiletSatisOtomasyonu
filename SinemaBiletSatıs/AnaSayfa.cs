using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SinemaBiletSatıs
{
    public partial class AnaSayfa : Form
    {
        SqlConnection con = new SqlConnection("server=.; Initial Catalog=SinemaBiletSatis;Integrated Security=True");// Veritabanı bağlantısı

        public AnaSayfa()
        {
            InitializeComponent();
        }
        private void AnaSayfa_Load(object sender, EventArgs e)
        {
            filmListesi();
            salonListele();
            IptalFilmListeleme();
        }

        private void Film_ekle_Click(object sender, EventArgs e)
        {
            FilmEkle ekle = new FilmEkle();
            ekle.Show();
            this.Hide();
        }

        private void Seans_ekle_Click(object sender, EventArgs e)
        {
            SeansEkle ekle = new SeansEkle();
            ekle.Show();
            this.Hide();
        }
        private void btnFilmTuru_Click(object sender, EventArgs e)
        {
            FilmTuru tur = new FilmTuru();
            tur.Show();
            this.Hide();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void salon_ekle_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("server=.; Initial Catalog=SinemaBiletSatis;Integrated Security=True"); //Veritabanı Bağlantısı

            con.Open();
            int id;
            string sqlSelect = "SELECT salonID FROM SALONLAR ORDER BY salonID DESC";
            SqlCommand cmd = new SqlCommand(sqlSelect, con);
            SqlDataReader dr = cmd.ExecuteReader();//komut içindeki veriler sqldatareader içine alınıyor
            if (dr.Read() == true) // dr içinde veri varsa daha önce eklenmiş salon kaydı mevcuttur ve son id üzerine ekleme yapılacak.
            {
                id = Convert.ToInt32(dr["salonID"].ToString()); //veritabanındaki id değişken içine atanıyor.
                id++; // Değişken değeri 1 arttırıldı.
            }
            else
            {
                id = 0;
                id++;
            }
           
            con.Close(); //İşlem sonu veritabanına bağlantı kapatıldı.

            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                // Bağlantımızı kontrol ediyoruz, eğer kapalıysa açıyoruz.
                string kayit = "insert into SALONLAR(salonAdi) values (@salonadi)";
                // müşteriler tablomuzun ilgili alanlarına kayıt ekleme işlemini gerçekleştirecek sorgumuz.
                cmd = new SqlCommand(kayit, con);
                //Sorgumuzu ve baglantimizi parametre olarak alan bir SqlCommand nesnesi oluşturuyoruz.
                cmd.Parameters.AddWithValue("@salonadi", "SALON" + id);
                //Parametrelerimize Form üzerinde ki kontrollerden girilen verileri aktarıyoruz.
                cmd.ExecuteNonQuery();
                //Veritabanında değişiklik yapacak komut işlemi bu satırda gerçekleşiyor.
                con.Close();
                MessageBox.Show("Salon Kayıt İşlemi Gerçekleşti.");

                //Salon ekleme işlemi gerçekleştirildi.

            }
            catch (Exception ex)
            {
                MessageBox.Show("İşlem Sırasında Hata Oluştu." + ex.Message);
                con.Close();
            }

            salonListeleCombobax();
        }

        private void btnSatislar_Click(object sender, EventArgs e)
        {
            Satıslar satis = new Satıslar();
            satis.Show();
            this.Hide();
        }

        private void seans_listele_Click(object sender, EventArgs e)
        {
            SeansListele seanslistele = new SeansListele();
            seanslistele.Show();
            this.Hide();
        }

        private void bilet_iptal_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                string sqlSelectSalonID = "SELECT salonID FROM SALONLAR WHERE salonAdi='" + cmboxSalonIptal.Text + "'";
                SqlCommand cmdSalon = new SqlCommand(sqlSelectSalonID, con);
                int salonID = (int)cmdSalon.ExecuteScalar();
                con.Close();
                //Seçilmiş olan salona ait ıd veritabanından alındı.

                con.Open();
                string sqlSelectFilmID = "SELECT ID FROM FİLMLER WHERE Film_Adı='" + cmboxFilmİptal.Text + "'";
                SqlCommand cmdFilm = new SqlCommand(sqlSelectFilmID, con);
                int filmID = (int)cmdFilm.ExecuteScalar();
                con.Close();
                //Seçilmiş olan salona ait ıd veritabanından alındı.


                con.Open();
                string sqlDeleteKoltuk = "UPDATE KOLTUKDURUMU SET Durum='BOŞ' WHERE FilmID="+filmID+" and SalonID="+salonID
                    +" and Tarih='"+dtPickerIptal.Text.ToString()+"' and Seans='"+cmboxFilmSeansIptal.Text+"' and KoltukNumarası='"+cmboxKoltukİptal.Text+"'";
                SqlCommand cmd = new SqlCommand(sqlDeleteKoltuk, con);
                cmd.ExecuteNonQuery();
                con.Close();

                con.Open();
                SqlCommand cmdBilet = new SqlCommand("delete from SATILANBİLETLERR where FilmAdı='"+cmboxFilmİptal.Text
                    +"' and SalonAdı='"+cmboxSalonIptal.Text+"' and FilmTarihi='"+dtPickerIptal.Text.ToString()
                    +"' and FilmSeansı='"+cmboxFilmSeansIptal.Text+"' and KoltukNumarası='"+cmboxKoltukİptal.Text+"'", con);
                cmdBilet.ExecuteNonQuery();
                con.Close();

                //Seçilen salona ait koltuk durumu boş yapıldı bilet iptali sağlandı.

                MessageBox.Show("Bilet satışı iptal edildi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        private void bilet_sat_Click(object sender, EventArgs e)
        {
            con.Open();
            string sqlselectFilm = "SELECT ID FROM FİLMLER WHERE Film_Adı ='" + (cmbx_film_adi.Text) + "'";
            SqlCommand cmdFilm = new SqlCommand(sqlselectFilm, con);
            int filmID = (int)cmdFilm.ExecuteScalar(); //Seçilen filmin id'si veritabanından alındı.
            cmdFilm.ExecuteNonQuery();
            con.Close();//Önceki işleme ait bağlantı kapatılıyor.

            con.Open();// Yeni bir işlem için yeni bağlantı açılıyor.
            string sqlSelectSalon = "SELECT salonID FROM SALONLAR WHERE salonAdi ='" + (cmbx_salon_adi.Text) + "'";
            SqlCommand cmdSalon = new SqlCommand(sqlSelectSalon, con);
            int salonID = (int)cmdSalon.ExecuteScalar(); //Seçilen salonun id'si veritabanından alındı.
            cmdSalon.ExecuteNonQuery();
            con.Close();

            con.Open();//Seçim yapılan kriterlere göre koltuk durumu kontrol ediliyor.
            string sqlKontrol = "SELECT Durum FROM KOLTUKDURUMU WHERE KoltukNumarası='" + txt_koltuk_no.Text + "' and FilmID=" + filmID
                + " and SalonID=" + salonID + " and Seans='" + cmbx_film_seans.Text + "'";
            SqlCommand cmdKontrol = new SqlCommand(sqlKontrol, con);
            string durum = (string)cmdKontrol.ExecuteScalar();
            con.Close();

            if (durum == "BOŞ ")
            {
                try //Bilet Satışı Yapılıyor
                {
                    con.Open();
                    //Koltuk durumu boş ise bilet satış işlemi yapılacak eğer dolu ise uyarı verilecek.
                    string sqlInsert = "INSERT INTO SATILANBİLETLERR(FilmAdı,SalonAdı,FilmTarihi,FilmSeansı,KoltukNumarası,Ad,Soyad,Ücret)" +
                                            "VALUES(@fAdı,@sAdı,@fTarih,@fSeans,@kNo,@ad,@soyad,@ucret)";
                    SqlCommand cmd = new SqlCommand(sqlInsert, con);
                    cmd.Parameters.AddWithValue("@fAdı", cmbx_film_adi.Text);
                    cmd.Parameters.AddWithValue("@sAdı", cmbx_salon_adi.Text);
                    cmd.Parameters.AddWithValue("@fTarih", dtPicker.Text.ToString());
                    cmd.Parameters.AddWithValue("@fSeans", cmbx_film_seans.Text.ToString());
                    cmd.Parameters.AddWithValue("@kNo", txt_koltuk_no.Text);
                    cmd.Parameters.AddWithValue("@ad", txtbx_ad.Text);
                    cmd.Parameters.AddWithValue("@soyad", txtbx_soyad.Text);
                    cmd.Parameters.AddWithValue("@ucret", Convert.ToDouble(txtUcret.Text));

                    cmd.ExecuteNonQuery();
                    con.Close();

                    con.Open();
                    string sqlUpdate = " UPDATE KOLTUKDURUMU SET Durum='DOLU' WHERE FilmID=" + filmID + " and SalonID=" + salonID
                        + " and Tarih='" + dtPicker.Text.ToString() + "' and Seans='" + cmbx_film_seans.Text + "' and KoltukNumarası='"+txt_koltuk_no.Text+"'";
                    SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, con);
                    cmdUpdate.ExecuteNonQuery();
                    con.Close();


                    MessageBox.Show("Bilet satışı tamanlandı");
                    koltukDolulukOrani();
                    txtbx_ad.Text = "";
                    txtbx_soyad.Text = "";
                    txtUcret.Text = "";
                    txt_koltuk_no.Text = "";
                    cmbx_film_adi.Text = "";
                    cmbx_salon_adi.Text = "";
                    cmbx_film_seans.Text = "";
                    koltukRengi();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Koltuk dolu bilet satışı yapılamaz");
            }
        }
        private void filmListesi()
        {
            con.Open();
            string sqlSelect = "SELECT Film_Adı FROM FİLMLER";
            SqlCommand cmd = new SqlCommand(sqlSelect, con);
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader(); //Datagridview listeleme için nesne oluşturuldu
            while (dr.Read())// dr içinde değer var ise döngü ile combobox içine ekleniyor.
            {
                cmbx_film_adi.Items.Add(dr["Film_Adı"]);
            }

            con.Close();
        }
        private void salonListele()
        {
            con.Open();
            string sqlSelect = "SELECT salonAdi FROM SALONLAR";
            SqlCommand cmd = new SqlCommand(sqlSelect, con);

            SqlDataReader dr = cmd.ExecuteReader(); //Datagridview listeleme için nesne oluşturuldu
            while (dr.Read())// dr içinde değer var ise döngü ile combobox içine ekleniyor.
            {
                cmbx_salon_adi.Items.Add(dr["salonAdi"]);
            }

            con.Close();
        }
        private void seansListele()
        {
            con.Open();
            string sqlSelectSalon = "SELECT salonID FROM SALONLAR WHERE salonAdi='" + cmbx_salon_adi.Text + "'";
            //Seçim yapılmış salona ait ıd listeleniyor.
            SqlCommand cmdSalon = new SqlCommand(sqlSelectSalon, con);
            int salonID = (int)cmdSalon.ExecuteScalar();
            con.Close();

            con.Open();
            string sqlSelectFilm = "SELECT ID FROM FİLMLER WHERE Film_Adı='" + cmbx_film_adi.Text + "'";
            //Seçim yapılmış filme ait ıd listeleniyor.
            SqlCommand cmdFilm = new SqlCommand(sqlSelectFilm, con);
            int filmID = (int)cmdFilm.ExecuteScalar();
            con.Close();

            con.Open();
            string sqlSelect = "SELECT Seans FROM SEANSEKLEME WHERE FilmID=" + filmID + " and salonID=" + salonID + " and Tarih='" + dtPicker.Text.ToString() + "'";
            // Seçilen film ve salon idleri ve tarihe göre seanslar listeleniyor.
            SqlCommand cmd = new SqlCommand(sqlSelect, con);

            SqlDataReader dr = cmd.ExecuteReader(); //Datagridview listeleme için nesne oluşturuldu
            while (dr.Read())// dr içinde değer var ise döngü ile combobox içine ekleniyor.
            {
                cmbx_film_seans.Items.Add(dr["Seans"]);
            }
            con.Close();
        }
        private void IptalFilmListeleme() 
        {
            string sqlSelectFilm = "SELECT Film_Adı FROM FİLMLER";
            SqlCommand cmdSelectFilm = new SqlCommand(sqlSelectFilm, con);
            con.Open();
            SqlDataReader dr = cmdSelectFilm.ExecuteReader();
            while (dr.Read())
            {
                cmboxFilmİptal.Items.Add(dr["Film_Adı"]);
            }
            con.Close();
        }
        private void koltukDolulukOrani()
        {
            try
            {
                con.Open();
                string sqlselectFilm = "SELECT ID FROM FİLMLER WHERE Film_Adı ='" + (cmbx_film_adi.Text) + "'";
                SqlCommand cmdFilm = new SqlCommand(sqlselectFilm, con);
                int filmID = (int)cmdFilm.ExecuteScalar(); //Seçilen filmin id'si veritabanından alındı.
                cmdFilm.ExecuteNonQuery();
                con.Close();//Önceki işleme ait bağlantı kapatılıyor.

                con.Open();// Yeni bir işlem için yeni bağlantı açılıyor.
                string sqlSelectSalon = "SELECT salonID FROM SALONLAR WHERE salonAdi ='" + (cmbx_salon_adi.Text) + "'";
                SqlCommand cmdSalon = new SqlCommand(sqlSelectSalon, con);
                int salonID = (int)cmdSalon.ExecuteScalar(); //Seçilen salonun id'si veritabanından alındı.
                cmdSalon.ExecuteNonQuery();
                con.Close();

                con.Open();
                //Veritabanından seçilen bilgilere göre koltukların dolu-boş olma durumu getiriliyor ve butonların rengi ayarlanıyor
                string sqlSelect = "SELECT KoltukNumarası,Durum FROM KOLTUKDURUMU WHERE FilmID=" + filmID + " and SalonID=" + salonID +
                    "and Tarih='" + dtPicker.Text.ToString() + "' and Seans='" + cmbx_film_seans.Text + "'";
                SqlCommand cmd = new SqlCommand(sqlSelect, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read() == true)
                {
                    string durum = dr["Durum"].ToString();
                    string bos = "BOŞ ",dolu = "DOLU";
                    string koltuk = dr["KoltukNumarası"].ToString();

                    if (durum.Equals(bos) && koltuk.Equals("A1")) A1.BackColor = Color.White;
                    if (durum.Equals(bos) && koltuk.Equals("A2")) A2.BackColor = Color.White;
                    if (durum.Equals(bos) && koltuk.Equals("A3")) A3.BackColor = Color.White;
                    if (durum.Equals(bos) && koltuk.Equals("A4")) A4.BackColor = Color.White;
                    if (durum.Equals(bos) && koltuk.Equals("A5")) A5.BackColor = Color.White;

                    if (durum.Equals(dolu) && koltuk.Equals("A1")) A1.BackColor = Color.Red;
                    if (durum.Equals(dolu) && koltuk.Equals("A2")) A2.BackColor = Color.Red;
                    if (durum.Equals(dolu) && koltuk.Equals("A3")) A3.BackColor = Color.Red;
                    if (durum.Equals(dolu) && koltuk.Equals("A4")) A4.BackColor = Color.Red;
                    if (durum.Equals(dolu) && koltuk.Equals("A5")) A5.BackColor = Color.Red;

                    if (durum.Equals(bos) && koltuk.Equals("B1")) B1.BackColor = Color.White;
                    if (durum.Equals(bos) && koltuk.Equals("B2")) B2.BackColor = Color.White;
                    if (durum.Equals(bos) && koltuk.Equals("B3")) B3.BackColor = Color.White;
                    if (durum.Equals(bos) && koltuk.Equals("B4")) B4.BackColor = Color.White;
                    if (durum.Equals(bos) && koltuk.Equals("B5")) B5.BackColor = Color.White;

                    if (durum.Equals(dolu) && koltuk.Equals("B1")) B1.BackColor = Color.Red;
                    if (durum.Equals(dolu) && koltuk.Equals("B2")) B2.BackColor = Color.Red;
                    if (durum.Equals(dolu) && koltuk.Equals("B3")) B3.BackColor = Color.Red;
                    if (durum.Equals(dolu) && koltuk.Equals("B4")) B4.BackColor = Color.Red;
                    if (durum.Equals(dolu) && koltuk.Equals("B5")) B5.BackColor = Color.Red;

                    if (durum.Equals(bos) && koltuk.Equals("C1")) C1.BackColor = Color.White;
                    if (durum.Equals(bos) && koltuk.Equals("C2")) C2.BackColor = Color.White;
                    if (durum.Equals(bos) && koltuk.Equals("C3")) C3.BackColor = Color.White;
                    if (durum.Equals(bos) && koltuk.Equals("C4")) C4.BackColor = Color.White;
                    if (durum.Equals(bos) && koltuk.Equals("C5")) C5.BackColor = Color.White;

                    if (durum.Equals(dolu) && koltuk.Equals("C1")) C1.BackColor = Color.Red;
                    if (durum.Equals(dolu) && koltuk.Equals("C2")) C2.BackColor = Color.Red;
                    if (durum.Equals(dolu) && koltuk.Equals("C3")) C3.BackColor = Color.Red;
                    if (durum.Equals(dolu) && koltuk.Equals("C4")) C4.BackColor = Color.Red;
                    if (durum.Equals(dolu) && koltuk.Equals("C5")) C5.BackColor = Color.Red;

                    if (durum.Equals(bos) && koltuk.Equals("D1")) D1.BackColor = Color.White;
                    if (durum.Equals(bos) && koltuk.Equals("D2")) D2.BackColor = Color.White;
                    if (durum.Equals(bos) && koltuk.Equals("D3")) D3.BackColor = Color.White;
                    if (durum.Equals(bos) && koltuk.Equals("D4")) D4.BackColor = Color.White;
                    if (durum.Equals(bos) && koltuk.Equals("D5")) D5.BackColor = Color.White;

                    if (durum.Equals(dolu) && koltuk.Equals("D1")) D1.BackColor = Color.Red;
                    if (durum.Equals(dolu) && koltuk.Equals("D2")) D2.BackColor = Color.Red;
                    if (durum.Equals(dolu) && koltuk.Equals("D3")) D3.BackColor = Color.Red;
                    if (durum.Equals(dolu) && koltuk.Equals("D4")) D4.BackColor = Color.Red;
                    if (durum.Equals(dolu) && koltuk.Equals("D5")) D5.BackColor = Color.Red;

                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        private void koltukRengi()
        {
            A1.BackColor = Button.DefaultBackColor;
            A2.BackColor = Button.DefaultBackColor; 
            A3.BackColor = Button.DefaultBackColor; 
            A4.BackColor = Button.DefaultBackColor;
            A5.BackColor = Button.DefaultBackColor;

            B1.BackColor = Button.DefaultBackColor;
            B2.BackColor = Button.DefaultBackColor;
            B3.BackColor = Button.DefaultBackColor;
            B4.BackColor = Button.DefaultBackColor;
            B5.BackColor = Button.DefaultBackColor;

            C1.BackColor = Button.DefaultBackColor;
            C2.BackColor = Button.DefaultBackColor;
            C3.BackColor = Button.DefaultBackColor;
            C4.BackColor = Button.DefaultBackColor;
            C5.BackColor = Button.DefaultBackColor;

            D1.BackColor = Button.DefaultBackColor;
            D2.BackColor = Button.DefaultBackColor;
            D3.BackColor = Button.DefaultBackColor;
            D4.BackColor = Button.DefaultBackColor;
            D5.BackColor = Button.DefaultBackColor;
        }
        private void salonListeleCombobax()
        {
            cmbx_salon_adi.Items.Clear();
            con.Open();
            string sqlSelect = "SELECT salonAdi FROM SALONLAR";
            SqlCommand cmd = new SqlCommand(sqlSelect, con);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read() == true)
                {
                    cmbx_salon_adi.Items.Add(rdr["salonAdi"].ToString());
                }
            }
            con.Close();
            rdr.Close();
        }
        private void dtPicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Today;
            dtPicker.MinDate = dt;
            cmbx_film_seans.Items.Clear(); //combobox içi temizleniyor.
            seansListele(); //Fonksiyon çalıştırılıyor.
        }
        private void cmbx_film_adi_SelectedIndexChanged(object sender, EventArgs e)
        {
            con.Open();
            string sqlSelect = "SELECT Afiş FROM FİLMLER WHERE Film_Adı='" + cmbx_film_adi.Text + "'"; //Film adına göre afişini getiren sorgu
            SqlCommand cmd = new SqlCommand(sqlSelect, con);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read() == true)
                {
                    byte[] RESİM = (byte[])rdr[0]; //resim değişkeni içine veritabanından gelen afiş ekleniyor

                    MemoryStream MİS = new MemoryStream();
                    MİS.Write(RESİM, 0, RESİM.Length); // Bellekte yeni bir resim oluşturuluyor

                    picturebox_film_afisi.SizeMode = PictureBoxSizeMode.StretchImage; //Afişi picturebox içine tam sığdırma işlemi
                    picturebox_film_afisi.Image = Image.FromStream(MİS); // Değer dönüşümü yapılıp picturebox içine afiş ekleniyor.
                }
            }
            con.Close();
            rdr.Close();
        }
        private void cmbx_film_seans_SelectedValueChanged(object sender, EventArgs e)
        {
            koltukDolulukOrani();
        }
        private void dtPickerIptal_ValueChanged(object sender, EventArgs e)
        {
            //Film ve tarihe göre salon listeleniyor.
            string salonAdı = "";
            cmboxSalonIptal.Items.Clear();
            string sqlSelectSalon = "SELECT SalonAdı FROM SATILANBİLETLERR WHERE FilmAdı='" + cmboxFilmİptal.Text
                + "' and FilmTarihi='" + dtPickerIptal.Text.ToString() +"'";
            SqlCommand cmdSalonListesi = new SqlCommand(sqlSelectSalon, con);
            con.Open();
            SqlDataReader dr = cmdSalonListesi.ExecuteReader();
            while (dr.Read())
            {
                if (dr["SalonAdı"].ToString() != salonAdı)
                {
                    salonAdı = dr["SalonAdı"].ToString();
                    cmboxSalonIptal.Items.Add(salonAdı);
                }
                
            }
            con.Close();
        }
        private void cmboxSalonIptal_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Salon,tarih ve filmID'ye göre seans listeleniyor
            string seansAdı = "";
            cmboxFilmSeansIptal.Items.Clear();
            string sqlSelectSeans = "SELECT FilmSeansı FROM SATILANBİLETLERR WHERE FilmAdı='" + cmboxFilmİptal.Text
                + "' and FilmTarihi='" + dtPickerIptal.Text.ToString() + "'";
            SqlCommand sqlSelectSeans_ = new SqlCommand(sqlSelectSeans, con);
            con.Open();
            SqlDataReader dr2 = sqlSelectSeans_.ExecuteReader();
            while (dr2.Read())
            {
                if (dr2["FilmSeansı"].ToString() != seansAdı) //Her koltuk kaydı için seans kaydı tutuluyor.
                                                              //İf ile aynı değerlere sahip değerlerden bir tanesi listelenecek
                {
                    seansAdı = dr2["FilmSeansı"].ToString();
                    cmboxFilmSeansIptal.Items.Add(seansAdı);
                }

            }
            con.Close();
        }
        private void cmboxFilmSeansIptal_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Salon,tarih ve filmID'ye göre koltuklar listeleniyor
            string koltuk = "";
            string sqlSelectSeans = "SELECT KoltukNumarası FROM SATILANBİLETLERR WHERE FilmAdı='" + cmboxFilmİptal.Text
                + "' and FilmTarihi='" + dtPickerIptal.Text.ToString() + "'";
            SqlCommand sqlSelectSeans_ = new SqlCommand(sqlSelectSeans, con);
            con.Open();
            SqlDataReader dr2 = sqlSelectSeans_.ExecuteReader();
            while (dr2.Read())
            {
                if (dr2["KoltukNumarası"].ToString() != koltuk) //Her koltuk kaydı için seans kaydı tutuluyor.
                                                                //İf ile aynı değerlere sahip değerlerden bir tanesi listelenecek
                {
                    koltuk = dr2["KoltukNumarası"].ToString();
                    cmboxKoltukİptal.Items.Add(koltuk);
                }

            }
            con.Close();
        }

        //Koltukların adları yazan butonlara basıldığı an Koltuk Numarası textboxına basılan koltuğun adı yazılacak
        private void A1_Click(object sender, EventArgs e)
        {
            if (A1.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "A1";
        }
        private void A2_Click(object sender, EventArgs e)
        {
            if (A2.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "A2";

        }
        private void A3_Click(object sender, EventArgs e)
        {
            if (A3.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "A3";
        }
        private void A4_Click(object sender, EventArgs e)
        {
            if (A4.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "A4";
        }
        private void A5_Click(object sender, EventArgs e)
        {
            if (A5.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "A5";
        }
        private void B1_Click(object sender, EventArgs e)
        {
            if (B1.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "B1";
        }
        private void B2_Click(object sender, EventArgs e)
        {
            if (B2.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "B2";
        }
        private void B3_Click(object sender, EventArgs e)
        {
            if (B3.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "B3";
        }
        private void B4_Click(object sender, EventArgs e)
        {
            if (B4.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "B4";
        }
        private void B5_Click(object sender, EventArgs e)
        {
            if (B5.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "B5";
        }
        private void C1_Click(object sender, EventArgs e)
        {
            if (C1.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "C1";
        }
        private void C2_Click(object sender, EventArgs e)
        {
            if (C2.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "C2";
        }
        private void C3_Click(object sender, EventArgs e)
        {
            if (C3.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "C3";
        }
        private void C4_Click(object sender, EventArgs e)
        {
            if (C4.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "C4";
        }
        private void C5_Click(object sender, EventArgs e)
        {
            if (C5.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "C5";
        }
        private void D1_Click(object sender, EventArgs e)
        {
            if (D1.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "D1";
        }
        private void D2_Click(object sender, EventArgs e)
        {
            if (D2.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "D2";
        }
        private void D3_Click(object sender, EventArgs e)
        {
            if (D3.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "D3";
        }
        private void D4_Click(object sender, EventArgs e)
        {
            if (D4.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "D4";
        }
        private void D5_Click(object sender, EventArgs e)
        {
            if (D5.BackColor == Color.Red) MessageBox.Show("Koltuk Seçimi Yapılamaz. DOLU.");
            else txt_koltuk_no.Text = "D5";
        }      
    }
}
