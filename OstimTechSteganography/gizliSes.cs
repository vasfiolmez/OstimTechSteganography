using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OstimTechSteganography
{
    public partial class gizliSes : Form
    {
        public gizliSes()
        {
            InitializeComponent();
        }
        public byte[] getBytes(string text)
        {

            byte[] bytes = Encoding.ASCII.GetBytes(text);
            return bytes;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {


                try
                {
                    int subChunk1Size = 16;
                    short audioFormat = 1;
                    short bitsPerSample = 16; // her sample 2 bayt
                    short numChannels = 2;
                    int sampleRate = 22050;
                    int byteRate = sampleRate * numChannels * (bitsPerSample / 8);
                    int numSamples = 19000;
                    short blockAlign = (short)(numChannels * (bitsPerSample / 8));
                    int subChunk2Size = numSamples * numChannels * (bitsPerSample / 8);
                    int chunkSize = 4 + (8 + subChunk1Size) + (8 + subChunk2Size);

                   

                    File.Delete("test.wav");
                    FileStream f = new FileStream("test.wav",FileMode.Create);
                    BinaryWriter wr = new BinaryWriter(f);
                    wr.Write(getBytes("RIFF"));
                    wr.Write(chunkSize);
                    wr.Write(getBytes("WAVE"));
                    wr.Write(getBytes("fmt"));
                    wr.Write((byte)32);
                    wr.Write(subChunk1Size);
                    wr.Write(audioFormat);
                    wr.Write(numChannels);
                    wr.Write(sampleRate);
                    wr.Write(byteRate);
                    wr.Write(blockAlign);
                    wr.Write(bitsPerSample);
                    wr.Write(getBytes("data"));
                    wr.Write(subChunk2Size);
                    Random RandomByte = new Random();
                    byte[] rbs = { 0, 0, 0, 0 };
                    wr.Write(rbs);
                    textBox1.Text += "/";
                    for (int i = 0; i < numSamples; i++)
                    {
                        wr.Write(getBytes(textBox1.Text));
                    }

                    for (int i = 0; i < numSamples; i++)
                    {
                        wr.Write((byte)RandomByte.Next(255));
                        wr.Write((byte)RandomByte.Next(255));
                    }
                    MessageBox.Show("Şifrelenmiş Halde Ses Dosyanız Oluşturuldu");
                    wr.Close();
                    wr.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Mesaj Yeri Boş Geçilemez");
            }
        }
        struct WavHeader
        {
            public byte[] riffID;
            public uint size;
            public byte[] wavID;
            public byte[] fmtID;
            public uint fmtSize;
            public ushort format;
            public ushort channels;
            public uint sampleRate;
            public uint bytePerSec;
            public ushort blockSize;
            public ushort bit;
            public byte[] dataID;
            public uint dataSize;
            public ulong data;
        }
        private void button2_Click(object sender, EventArgs e)
        {

            label2.Text = "";
            string dosya_yolu = string.Empty;
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                dosya_yolu = fd.FileName.ToString();
            }
            WavHeader Header = new WavHeader();
            List<short> lDataList = new List<short>();
            List<short> rDataList = new List<short>();
            char[] abs = new char[400];
            string mesaj = "";
            using (FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                try
                {

                    Header.riffID = br.ReadBytes(4);
                    Header.size = br.ReadUInt32();
                    Header.wavID = br.ReadBytes(4);
                    Header.fmtID = br.ReadBytes(4);
                    Header.fmtSize = br.ReadUInt32();
                    Header.format = br.ReadUInt16();
                    Header.channels = br.ReadUInt16();
                    Header.sampleRate = br.ReadUInt32();
                    Header.bytePerSec = br.ReadUInt32();
                    Header.blockSize = br.ReadUInt16();
                    Header.bit = br.ReadUInt16();
                    Header.dataID = br.ReadBytes(4);
                    Header.dataSize = br.ReadUInt32();


                    for (int i = 0; i < 400; i++)
                    {

                        char character = (char)(br.ReadByte());
                        if (character == '/')
                        {
                            break;
                        }
                        abs[i] = character;

                    }
                    for (int i = 0; i < 400; i++)
                    {
                        listBox1.Items.Add(abs[i]);
                        label2.Text += abs[i];
                    }
                }
                finally
                {
                    listBox1.Refresh();
                    if (br != null)
                    {
                        br.Close();
                    }
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }

            }
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
