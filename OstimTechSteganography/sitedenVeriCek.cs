using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.IO;
using System.Net;
using HtmlDocument = System.Windows.Forms.HtmlDocument;
using System.Text.RegularExpressions;
using System.Collections;
using OstimTechSteganography;

namespace bilgitoplama1
{
    public partial class sitedenVeriCek : Form
    {


        public sitedenVeriCek()
        {
            InitializeComponent();
            ToolTip toolTip = new ToolTip();
            toolTip.Active = true;
            toolTip.ToolTipTitle = "Açıklama";
            toolTip.ToolTipIcon = ToolTipIcon.Info;
            toolTip.UseFading = true;
            toolTip.UseAnimation = true;
            toolTip.IsBalloon = true;
            toolTip.ShowAlways = true;
            toolTip.AutoPopDelay = 7000;
            toolTip.ReshowDelay = 500;
            toolTip.InitialDelay = 100;
            toolTip.SetToolTip(textBox_hedefsite, "Aramak istediğiniz hedef sitenin linkini arama motorundan kopyalayıp ilgili alana yapıştırınız."+ Environment.NewLine + "Aksi takdirde programda hata ile karşılaşabilirsiniz.");
        }

        private void search_Click(object sender, EventArgs e)
        {
            listBox4.Items.Add(textBox_hedefsite.Text);
            //string kaynakKod = kaynakkodcek("http://www.usluer.net");
            richTextBox1.Clear();
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
          
            linklericek();
            sorgu();
           if (richTextBox1.Text == "")
            {
                MessageBox.Show("Girdiğiniz adreste bir site bulunamamıştır . Lütfen girdiğiniz adresi kontrol ediniz!", "Hata; ", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                textBox_hedefsite.Clear();

            }
           
           //listboxların içi boş kalırsa ilgili sitenin sosyal medya hesapları bulunamamıştır
            if (listBox1.Items.Count < 1)
            {
                listBox1.Items.Add("Aradığınız site içinde facebook adresi bulunamamıştır.");
            }
            if (listBox2.Items.Count < 1)
            {
                listBox2.Items.Add("Aradığınız site içinde instagram adresi bulunamamıştır.");
            }
            if (listBox3.Items.Count < 1)
            {
                listBox3.Items.Add("Aradığınız site içinde twitter adresi bulunamamıştır.");
            }


        }
        private void sorgu() 
        {
            if (textBox_hedefsite.Text == "")
            {
                MessageBox.Show("Hedef site adresini boş bırakamazsınız!", "Hata; ", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

            else
            {
                string hedefSite = textBox_hedefsite.Text.Replace("http://", "");
                hedefSite = hedefSite.Replace("https://", "");
                hedefSite = hedefSite.Replace("www.", "");


                Uri url = new Uri(textbox_kaynaksite.Text + hedefSite); // url oluştruduk
                WebClient client = new WebClient(); // siteye erişim için client tanımladık
                client.Encoding = System.Text.Encoding.UTF8;// veriyi çekerken yaşadığımız türkçe karakter sorununu ortadan kaldıran kod.            
                string html = client.DownloadString(url); //sitenin html lini indirdik
                HtmlAgilityPack.HtmlDocument dokuman = new HtmlAgilityPack.HtmlDocument(); //burada HtmlAgilityPack Kütüphanesini kullandık
                dokuman.LoadHtml(html); // indirdiğimiz sitenin html lini oluşturduğumuz dokumana dolduruyoruz
                try
                {
                    try
                    {

                        var veri = dokuman.DocumentNode.SelectNodes("//*[@id='registryData']");// siteden aldığımız xpath i buraya yazıp kaynak kısmını seçiyoruz
                        if (veri != null)
                        {
                            richTextBox1.Text = veri[0].InnerText;
                        }

                    }
                    catch
                    {

                    }
                    try
                    {
                        var veri1 = dokuman.DocumentNode.SelectNodes("//*[@id='registrarData']");// siteden aldığımız xpath i buraya yazıp kaynak kısmını seçiyoruz
                        if (veri1 != null)
                        {
                            richTextBox1.Text = veri1[0].InnerText;
                        }
                    }
                    catch
                    {
                    }
                    MessageBox.Show("Bilgi toplama işlemi tamamlandı.", "Bilgi; ", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    textBox_hedefsite.Clear();

                }
                catch
                {
                }


            }
          

        }



        //private string kaynakkodcek(string kaynak)
        //{


        //    HttpWebRequest istek = (HttpWebRequest)WebRequest.Create(adres);
        //    HttpWebResponse cevap = (HttpWebResponse)istek.GetResponse();
        //    using (StreamReader okuyucu = new StreamReader(cevap.GetResponseStream(), Encoding.UTF8))
        //    {
        //        return okuyucu.ReadToEnd();
        //    }
        //}

        private void linklericek()
        {
            try
            {
                Uri url = new Uri(textBox_hedefsite.Text);
                WebClient client = new WebClient();
                string html = client.DownloadString(url);
                HtmlAgilityPack.HtmlDocument dokuman = new HtmlAgilityPack.HtmlDocument();
                dokuman.LoadHtml(html);

                ArrayList aList = new ArrayList();// liste olusturduk



                foreach (HtmlNode link in dokuman.DocumentNode.SelectNodes("//a[@href]"))
                {
                    HtmlAttribute linkler = link.Attributes["href"];
                    aList.Add(linkler.Value);// listeye sitede bulunan linkleri atadık

                }

                string facebook = "https://www.facebook.com/";
                string insta = "https://www.instagram.com/";
                string twitter = "https://twitter.com/";


                foreach (string ara in aList)
                {
                    for (int i = 0; i < ara.Length - facebook.Length + 1; i++)
                    {

                        if (ara.Substring(i, facebook.Length).ToString() == facebook)
                        {
                            listBox2.Items.Add(ara);

                        }


                    }

                }
                foreach (string ara in aList)
                {
                    for (int i = 0; i < ara.Length - insta.Length + 1; i++)
                    {

                        if (ara.Substring(i, insta.Length).ToString() == insta)
                        {
                            listBox1.Items.Add(ara);
                        }

                    }

                }

                foreach (string ara in aList)
                {
                    for (int i = 0; i < ara.Length - twitter.Length + 1; i++)
                    {

                        if (ara.Substring(i, twitter.Length).ToString() == twitter)
                        {
                            listBox3.Items.Add(ara);
                        }


                    }

                }
            }
            catch 
            { 
            }
            
            }

       

        private void buttonKaydet_Click_1(object sender, EventArgs e)
        {
            
            saveFileDialog1.Title = "Kayıt edilecek yeri seçiniz...";
            saveFileDialog1.Filter = "Text Dosyalari|*.txt";
            saveFileDialog1.FileName = "";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter yaz = new StreamWriter(saveFileDialog1.OpenFile());

                yaz.Write(richTextBox1.Text + Environment.NewLine + " İnstagram Adresi" + Environment.NewLine + listBox1.Items[0].ToString() + Environment.NewLine
                    + " Facebook Adresi" + Environment.NewLine + listBox2.Items[0].ToString() + Environment.NewLine
                    + " Twitter Adresi" + Environment.NewLine + listBox3.Items[0].ToString());

                yaz.Close();
                
                MessageBox.Show("Kaydetme işlemini başarıyla tamamladınız. Bilgiler silinecektir.", "Bilgi; ", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
               
                        richTextBox1.Clear();
                        listBox1.Items.Clear();
                        listBox2.Items.Clear();
                        listBox3.Items.Clear();

                

            }
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
         
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            menu geridon = new menu();
            geridon.Show();
            this.Hide();
        }
    }
}
