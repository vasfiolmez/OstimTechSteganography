using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OstimTechSteganography
{
    public partial class MetinSifreleCoz : Form
    {
        public MetinSifreleCoz()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        AesSifreleveCoz aes = new AesSifreleveCoz();
        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = aes.AesSifrele(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = aes.AesSifre_coz(textBox2.Text);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            menu menu = new menu();
            menu.Show();
            this.Hide();
        }
    }
}
