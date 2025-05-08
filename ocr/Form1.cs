using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ocr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        NeuralNetwork neuralNetwork = new NeuralNetwork();
        int[] butonGirisDegerleri = new int[35]
        {
          0,0,0,0,0,
          0,0,0,0,0,
          0,0,0,0,0,
          0,0,0,0,0,
          0,0,0,0,0,
          0,0,0,0,0,
          0,0,0,0,0
        };

        private void btnEgit_Click(object sender, EventArgs e)
        {
            // Eğitim öncesi parametreleri ayarla
            double errorThreshold = 0.01; // Varsayılan değer

            // Eğer hata eşik değeri textbox'ı doldurulmuşsa, kullan
            if (!string.IsNullOrEmpty(txtErrorThreshold.Text))
            {
                if (double.TryParse(txtErrorThreshold.Text, out double threshold))
                {
                    errorThreshold = threshold;
                }
                else
                {
                    MessageBox.Show("Hata eşik değeri geçerli bir sayı değil. Varsayılan değer (0.01) kullanılacak.");
                }
            }

            // Butonları devre dışı bırak
            btnEgit.Enabled = false;
            button36.Enabled = false;

            // Eğitim durumunu bildir
            lblHataOrani.Text = "Hata Oranı: Eğitim sürüyor...";

            // Eğitimi ayrı bir thread'de başlat
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, args) =>
            {
                Veriler veriler = new Veriler();
                bool durum = neuralNetwork.Training(veriler.veriler, veriler.beklenenler);
                args.Result = durum;
            };

            worker.RunWorkerCompleted += (s, args) =>
            {
                bool durum = (bool)args.Result;
                if (durum)
                {
                    btnEgit.Enabled = true;
                    button36.Enabled = true;
                    lblHataOrani.Text = $"Hata Oranı: {neuralNetwork.hataOrani:F6}";
                    MessageBox.Show("Eğitim başarıyla tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    btnEgit.Enabled = true;
                    button36.Enabled = true;
                    lblHataOrani.Text = $"Hata Oranı: {neuralNetwork.hataOrani:F6}";
                    MessageBox.Show("Eğitim tamamlandı, ancak hedef hata oranına ulaşılamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            worker.RunWorkerAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int deger = int.Parse(btn.Name.Substring(6));
            if (btn.BackColor == Color.White)
            {
                butonGirisDegerleri[deger - 1] = 1;
                btn.BackColor = Color.Black;
            }
            else
            {
                butonGirisDegerleri[deger - 1] = 0;
                btn.BackColor = Color.White;
            }
            YazdirMatrisi();
        }

        private void button36_Click(object sender, EventArgs e)
        {
            neuralNetwork.tahmin(butonGirisDegerleri);
            lblASonuc.Text = neuralNetwork.sonuc[0].ToString("F6");
            lblBSonuc.Text = neuralNetwork.sonuc[1].ToString("F6");
            lblCSonuc.Text = neuralNetwork.sonuc[2].ToString("F6");
            lblDSonuc.Text = neuralNetwork.sonuc[3].ToString("F6");
            lblESonuc.Text = neuralNetwork.sonuc[4].ToString("F6");

            // En yüksek değere sahip harfi tespit et
            int tahminIndex = neuralNetwork.GetPredictedLetter();
            string tahminHarfi = "";

            // İndekse göre harfi belirle
            switch (tahminIndex)
            {
                case 0:
                    tahminHarfi = "A";
                    break;
                case 1:
                    tahminHarfi = "B";
                    break;
                case 2:
                    tahminHarfi = "C";
                    break;
                case 3:
                    tahminHarfi = "D";
                    break;
                case 4:
                    tahminHarfi = "E";
                    break;
            }

            // Tahmin etiketini güncelle
            lblTahmin.Text = "Tahmin: " + tahminHarfi;

            // Test sırasında oluşan hatayı da güncelle
            lblHataOrani.Text = $"Hata Oranı: {neuralNetwork.hataOrani:F6}";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Form yüklendiğinde varsayılan değerleri ayarla
            txtErrorThreshold.Text = "0.01";
            lblHataOrani.Text = "Hata Oranı: -";
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {
        }

        private void button37_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 35; i++)
            {
                // Button adları: button1, button2, ..., button35
                Control[] controls = this.Controls.Find("button" + (i + 1).ToString(), true);
                if (controls.Length > 0 && controls[0] is Button btn)
                {
                    btn.BackColor = Color.White;
                }
                // Giriş verisini de sıfırla
                butonGirisDegerleri[i] = 0;
            }

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void YazdirMatrisi()
        {
            StringBuilder sb = new StringBuilder();
            for (int satir = 0; satir < 7; satir++)
            {
                for (int sutun = 0; sutun < 5; sutun++)
                {
                    int index = satir * 5 + sutun;
                    sb.Append(butonGirisDegerleri[index]);
                }
                sb.AppendLine(); // Yeni satıra geç
            }
            richTextBox1.Text = sb.ToString();
        }

        private void label6_Click(object sender, EventArgs e)
        {
        }

        private void label7_Click(object sender, EventArgs e)
        {
        }

        private void lblTahmin_Click(object sender, EventArgs e)
        {

        }
    }
}