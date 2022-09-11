using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bilgitoplama1;

namespace OstimTechSteganography
{
    public partial class menu : Form
    {
        public menu()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            resimSifrele resim = new resimSifrele();
            resim.Show();
            this.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ToolTip Aciklama = new ToolTip();
            Aciklama.SetToolTip(pictureBox2, "Ses dosyası şifrele");
            gizliSes sesolustur = new gizliSes();
            sesolustur.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            sitedenVeriCek site = new sitedenVeriCek();
            site.Show();
            this.Hide();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            MetinSifreleCoz sifrele = new MetinSifreleCoz();
            sifrele.Show();
            this.Hide();
        }

        private void yardımMenu_Click(object sender, EventArgs e)
        {
            yardımMenu yardim = new yardımMenu();
            yardim.Show();
            this.Hide();
        }
    }
}
