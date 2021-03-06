using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoPublicKeyRSA
{
    public partial class Check : Form
    {
        public Check()
        {
            InitializeComponent();
        }

        public static string MD5(string path)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
                }
            }
        }

        public static string SHA1(string path)
        {
            using (var cryptoProvider = new SHA1CryptoServiceProvider())
            {
                var stream = File.OpenRead(path);
                string hash = BitConverter
                    .ToString(cryptoProvider.ComputeHash(stream)).Replace("-", "");
                return hash.ToLower();
            }
        }

        public static string SHA256(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                SHA256Managed sha = new SHA256Managed();
                byte[] hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        private void btn_chonfile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txt_input.Text = dialog.FileName;
                }
            }
        }

        private void btn_checksum_Click(object sender, EventArgs e)
        {
            if (txt_input.Text.Length > 0) {
                lbl_md5.Text = MD5(txt_input.Text);
                lbl_sha1.Text = SHA1(txt_input.Text);
                lbl_sha256.Text = SHA256(txt_input.Text);
                lbl_md5.Enabled = true;
                lbl_sha1.Enabled = true;
                lbl_sha256.Enabled = true;
            }           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txt_input2.Text = dialog.FileName;
                }
            }
        }

        private void btn_checksum2_Click(object sender, EventArgs e)
        {
            if (txt_input2.Text.Length >0) {
                lbl_md5_2.Text = MD5(txt_input2.Text);
                lbl_sha1_2.Text = SHA1(txt_input2.Text);
                lbl_sha256_2.Text = SHA256(txt_input2.Text);
                lbl_md5_2.Enabled = true;
                lbl_sha1_2.Enabled = true;
                lbl_sha256_2.Enabled = true;
            }         
        }

        private void Check_Load(object sender, EventArgs e)
        {
            this.lbl_md5.Enabled = false;
            this.lbl_md5_2.Enabled = false;
            this.lbl_sha1.Enabled = false;
            this.lbl_sha1_2.Enabled = false;
            this.lbl_sha256.Enabled = false;
            this.lbl_sha256_2.Enabled = false;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if ((lbl_md5.Text != null && lbl_sha1.Text != null && lbl_sha256.Text != null))
                {
                if (lbl_md5.Text == lbl_md5_2.Text && lbl_sha1.Text == lbl_sha1_2.Text && lbl_sha256.Text == lbl_sha256_2.Text)
                {
                    this.labelkq.ForeColor = System.Drawing.Color.Green;
                    this.labelkq.Text = "2 file giống nhau";
                }
                else
                {
                    this.labelkq.ForeColor = System.Drawing.Color.Red;
                    this.labelkq.Text = "2 file khác nhau";
                }
            }
        }
    }
}
