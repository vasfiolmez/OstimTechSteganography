using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace OstimTechSteganography
{
    public partial class resimSifrele : Form
    {
        public resimSifrele()
        {
            InitializeComponent();
        }

        //global değişkenler
        byte[] data;
        Bitmap mainImage;
        Bitmap EncryptedImage;
        OpenFileDialog ofd = new OpenFileDialog();
        string[] type = new string[3];
        private byte getByte(byte[] bits)
        {
            String bitString = "";
            for (int i = 0; i < 8; i++)
                bitString += bits[i];
            byte newpix = Convert.ToByte(bitString, 2);
            int dePix = (int)newpix;
            return (byte)dePix;
        }

        private byte[] getStringBits(string data)
        {
            byte[] text = System.Text.ASCIIEncoding.ASCII.GetBytes(data);
            BitArray bits = new BitArray(text);
            bool[] boolarray = new bool[bits.Count];
            bits.CopyTo(boolarray, 0);
            byte[] bitsArray = boolarray.Select(bit => (byte)(bit ? 1 : 0)).ToArray();
            Array.Reverse(bitsArray);
            return bitsArray;
        }
        private byte[] getBits(byte simplepixel)
        {
            int pixel = 0;
            pixel = (int)simplepixel;
            BitArray bits = new BitArray(new byte[] { (byte)pixel });
            bool[] boolarray = new bool[bits.Count];
            bits.CopyTo(boolarray, 0);
            byte[] bitsArray = boolarray.Select(bit => (byte)(bit ? 1 : 0)).ToArray();
            Array.Reverse(bitsArray);
            return bitsArray;
        }
        public void binRead()
        {
            DialogResult orb = openFileDialog1.ShowDialog();

            if (orb == DialogResult.OK)
            {
                string path = openFileDialog1.FileName;

                try
                {
                    using (FileStream fs = File.Open(path, FileMode.Open))
                    {
                        data = new BinaryReader(fs).ReadBytes((int)fs.Length);
                    }
                }
                catch (IOException ioe)
                {
                    MessageBox.Show("Error while opening file!" + ioe.Message);
                }
            }
        }
        public Color embed(Color pixel, byte[] bits)
        {

            byte[] RedBits = getBits((byte)pixel.R);
            byte[] GreenBits = getBits((byte)pixel.G);
            byte[] BlueBits = getBits((byte)pixel.B);

            /*LSB substition of RGB values is done as following:
            Red: Last 3 bits, Green: Last 3 bits, Blue: Last 2 bits
            This process lets us embed 1 byte in each pixel*/

           
            RedBits[5] = bits[0];
            GreenBits[5] = bits[1];
            RedBits[6] = bits[2];
            RedBits[7] = bits[3];
            GreenBits[6] = bits[4];
            GreenBits[7] = bits[5];
            BlueBits[6] = bits[6];
            BlueBits[7] = bits[7];

            byte newRed = getByte(RedBits);
            byte newGreen = getByte(GreenBits);
            byte newBlue = getByte(BlueBits);

            return Color.FromArgb(newRed, newGreen, newBlue);

        }

        public byte extract(Color pixel)
        {
            byte[] RedBits = getBits((byte)pixel.R);
            byte[] GreenBits = getBits((byte)pixel.G);
            byte[] BlueBits = getBits((byte)pixel.B);
            byte[] BitsToDecrypt = new byte[8];

            BitsToDecrypt[0] = RedBits[5];
            BitsToDecrypt[1] = GreenBits[5];
            BitsToDecrypt[2] = RedBits[6];
            BitsToDecrypt[3] = RedBits[7];
            BitsToDecrypt[4] = GreenBits[6];
            BitsToDecrypt[5] = GreenBits[7];
            BitsToDecrypt[6] = BlueBits[6];
            BitsToDecrypt[7] = BlueBits[7];

            return getByte(BitsToDecrypt);
        }


        private void btndosyaAc_Click(object sender, EventArgs e)
        {
            ofd = new OpenFileDialog();
            //ofd.Filter = "All picture files|*.bmp; *.dib;*.jpg;*.jpeg;*.jpe;*.jfif;*.tif;*.tiff;*.gif;*.png|Bitmap files (*.bmp,*.dib)|*.bmp;*.dib|JPEG (*.jpg,*.jpeg,*.jpe,*.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|TIFF (*.tif,*.tiff)|*.tif;*.tiff|GIF (*.gif)|*.gif|PNG (*.png)|*.png";
            ofd.Filter = "Jpg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|Png Image|*.png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pBsifrelecekResim.ImageLocation = ofd.FileName;
                txtSifrelecekResimYolu.Text = ofd.FileName;
                Bitmap img = new Bitmap(ofd.FileName);
                labelImageSize.Text = "Image size: " + Convert.ToString(img.Width) + " x " + Convert.ToString(img.Height);
            }
        }

       

        private void btnSifrele_Click_1(object sender, EventArgs e)
        {

            string textbox = txtMetinSifrele.Text;
            char[] textarray = txtMetinSifrele.Text.ToArray();

            mainImage = new Bitmap(pBsifrelecekResim.Image);
            labelProgress.Text = "";
            label1.Text = "Metin uzunluğu: " + Convert.ToString(textbox.Length) + " karakter";

            /* Encoding process */
            #region Encoding

            //Embed type of data into last 3 pixels.
            #region type_embed
            // "tt1" is the code to define hidden data is a text message. (type:text:1)
            string[] type = new string[] { "t", "t", "1" };

            for (int j = 0; j < 3; j++)
            {
                Color pixel = mainImage.GetPixel(mainImage.Width - j - 1, mainImage.Height - 1);
                pixel = embed(pixel, getStringBits(type[j]));
                mainImage.SetPixel(mainImage.Width - j - 1, mainImage.Height - 1, pixel);
            }

            #endregion

            // Embed length of message into 13 pixels in reverse [3:15]
            #region length_embed

            string a = Convert.ToString((txtMetinSifrele.Text).Length);
            a = a.PadLeft(13, '0'); //Zero-padding
            char[] b = a.ToArray();

            for (int j = 3; j < 16; j++)
            {
                string aString = Convert.ToString(b[j - 3]);
                Color pixel = mainImage.GetPixel(mainImage.Width - j - 1, mainImage.Height - 1);
                pixel = embed(pixel, getStringBits(aString));
                mainImage.SetPixel(mainImage.Width - j - 1, mainImage.Height - 1, pixel);
            }

            #endregion

            int k = 0;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = textbox.Length - 1;

            labelProgress.Text = "İşleniyor...";

            for (int i = 0; i < mainImage.Height; i++)
            {
                for (int j = 0; j < mainImage.Width; j++)
                {
                    if (k < textbox.Length)
                    {
                        string msg = Convert.ToString(textarray[i + j]);
                        Color pixel = mainImage.GetPixel(j, i);
                        pixel = embed(pixel, getStringBits(msg));
                        mainImage.SetPixel(j, i, pixel);
                        progressBar1.Value = k;
                        k++;
                    }

                }
            }

            labelProgress.Text = "tamamlandı!";
            #endregion

        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveEncoded = new SaveFileDialog();
            saveEncoded.Filter = "Bitmap Image|*.bmp|Png Image|*.png";

            string pathEncoded = "";

            if (saveEncoded.ShowDialog() == DialogResult.OK)
            {
                pathEncoded = saveEncoded.FileName;
            }

            try
            {
                mainImage.Save(pathEncoded);

                MessageBox.Show("Resim başarılı bir şekilde aşağıdaki konuma kaydedildi.\n" + pathEncoded);
            }
            catch (IOException ioe)
            {
                MessageBox.Show("Dosya yazılırken hata!" + ioe.Message);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtMetinSifrele_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMetinSifrele.Text) == false)
            {
                btnSifrele.Enabled = true;
            }
        }

        private void btnSifreliDosyasec_Click(object sender, EventArgs e)
        {
            ofd.Filter = "Bitmap files (*.bmp,*.dib)|*.bmp;*.dib|PNG (*.png)|*.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.ImageLocation = ofd.FileName;
                textBox1.Text = ofd.FileName;

                #region type_extract

                EncryptedImage = new Bitmap(ofd.FileName);
                textBox2.Show();
                textBox2.Text = "";

                for (int j = 0; j < 3; j++)
                {
                    Color pixelToDecode = EncryptedImage.GetPixel(EncryptedImage.Width - j - 1, EncryptedImage.Height - 1);
                    byte detype = extract(pixelToDecode);
                    type[j] = Encoding.ASCII.GetString(BitConverter.GetBytes(detype));
                    textBox2.Text += type[j];
                }
                char[] typex = (textBox2.Text).ToCharArray();

                #endregion

                if (typex[0] == 't' && typex[1] == 't' && typex[2] == '1')
                {
                    
                    
                    Bitmap img = new Bitmap(ofd.FileName);
                    label2.Text = "Resim Boyutu: " + Convert.ToString(img.Width) + " x " + Convert.ToString(img.Height);
                }               
                else
                {
                    MessageBox.Show("Seçilen resimde steganoraphy bulunamadı!\n Farklı bir resim deneyiniz!", "Uyarı!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                textBox2.Hide();

                btnSifreCoz.Enabled = true;
            }
        }

        private void btnSifreCoz_Click(object sender, EventArgs e)
        {
            EncryptedImage = new Bitmap(pictureBox1.Image);
            txtGizliYazı.Text = "";

            /* Decoding process */
            #region Decoding

            int k = 0;
            string tlength = "";

            #region length_extract

            for (int j = 3; j < 16; j++)
            {
                Color pixelToDecode = EncryptedImage.GetPixel(EncryptedImage.Width - j - 1, EncryptedImage.Height - 1);
                byte delength = extract(pixelToDecode);
                tlength += Convert.ToInt32(Encoding.ASCII.GetString(BitConverter.GetBytes(delength)));
            }

            int length = Convert.ToInt32(tlength);
            label3.Text = "Metin Uzunluğu: " + Convert.ToString(length) + " karakter";

            #endregion

            k = 0;
            progressBar2.Minimum = 0;
            progressBar2.Maximum = length - 1;

            for (int i = 0; i < EncryptedImage.Height; i++)
            {
                for (int j = 0; j < EncryptedImage.Width; j++)
                {
                    if (k < length)
                    {
                        Color pixelToDecode = EncryptedImage.GetPixel(j, i);
                        byte demsg = extract(pixelToDecode);
                        txtGizliYazı.Text += Encoding.ASCII.GetString(BitConverter.GetBytes(demsg));
                        progressBar2.Value = k;
                        k++;
                    }
                }
            }

            label4.Text = "Tamamlandı!";
            #endregion
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            menu geridon = new menu();
            geridon.Show();
            this.Hide();

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
