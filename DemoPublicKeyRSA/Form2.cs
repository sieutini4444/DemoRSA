using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace DemoPublicKeyRSA
{
    public partial class Encode : Form
    {
        private delegate void btnEncryptDecrypt();

        private RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
        private string pathKeysXML = "";
        private bool isEncryptFile = true;
        
        private void enabledOrDisableButtons(bool isEnable)
        {
            this.tbOutput.Enabled = isEnable;
            this.btnEncrypt.Enabled = isEnable;
            this.btnGenerateKey.Enabled = isEnable;
            this.btnOpenFileIn.Enabled = isEnable;
            this.btnOpenFileKeys.Enabled = isEnable;
            this.btnOpenFolderIn.Enabled = isEnable;
            this.selectOutput.Enabled = isEnable;
        }
        //private bool isEncryptFile = true;

        public Encode()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e); // let the base class do it's OnLoad
            this.comboBoxLengKey.Items.Add("512 bits");
            this.comboBoxLengKey.Items.Add("1024 bits");
            this.comboBoxLengKey.Items.Add("2048 bits");
            this.comboBoxLengKey.Items.Add("4096 bits");
            this.tbN.Enabled = false;
            this.tbD.Enabled = false;
            this.tbE.Enabled = false;
            //lbl_md5.Enabled = false;
            //lbl_sha1.Enabled = false;
            //lbl_sha256.Enabled = false;
            this.comboBoxLengKey.Text = "1024 bits";
            Control.CheckForIllegalCrossThreadCalls = false;

            //Do your own stuff...
        }
        private void label13_Click(object sender, EventArgs e)
        {

        }
        private void comboBoxLengKey_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void tbPathKeys_TextChanged(object sender, EventArgs e)
        {

        }
        private void btnGenerateKey_Click(object sender, EventArgs e)
        {
            // Tạo file chứa key
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.DefaultExt = "xml";
            saveFileDialog1.Filter = "Xml File|*.xml|All File|*.*";
            saveFileDialog1.Title = "Chọn tên file";
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            int lengthKey = 0;
            if (this.comboBoxLengKey.Text == "1024 bits") lengthKey = 1024;
            else if (this.comboBoxLengKey.Text == "512 bits") lengthKey = 512;
            else if (this.comboBoxLengKey.Text == "2048 bits") lengthKey = 2048;
            else if (this.comboBoxLengKey.Text == "4096 bits") lengthKey = 4096;
            saveFileDialog1.RestoreDirectory = true;
            String pathPrivateKey = saveFileDialog1.FileName;
            //tạo key có độ dài lengthKey
            RSA = new RSACryptoServiceProvider(lengthKey); //tạo key có độ dài lengtheKey


            File.WriteAllText(pathPrivateKey, RSA.ToXmlString(true));  // Private Key

            pathKeysXML = pathPrivateKey;
            tbPathKeys.Text = pathPrivateKey; //Hiển thị đường dẫn key

            if (File.Exists(pathKeysXML))
            {
                if (Path.GetExtension(pathKeysXML) == ".xml") //kiểm tra định dạng
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(File.ReadAllText(pathKeysXML)); //đọc RSA Key
                    try
                    {
                        XmlNode xnList = xml.SelectSingleNode("/RSAKeyValue/Modulus");
                        tbN.Text = xnList.InnerText;
                        xnList = xml.SelectSingleNode("/RSAKeyValue/Exponent");
                        tbE.Text = xnList.InnerText;
                        xnList = xml.SelectSingleNode("/RSAKeyValue/D");
                        tbD.Text = xnList.InnerText;
                        tbN.Enabled = true;
                        tbE.Enabled = true;
                        tbD.Enabled = true;
                    }
                    catch (Exception ix)
                    {
                        MessageBox.Show(ix.Message);
                    }
                }
            }
            MessageBox.Show("Tạo key có độ dài " + lengthKey.ToString() + " bits thành công.");
        }

        private void btnOpenFileKeys_Click(object sender, EventArgs e)
        {
            this.tbD.Clear(); this.tbE.Clear(); this.tbN.Clear();
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "Xml files (*.xml)|*.xml|All Files (*.*)|*.*";
            if (op.ShowDialog() == DialogResult.OK)
            {
                pathKeysXML = op.FileName;
                tbPathKeys.Text = op.FileName;
            }

            if (File.Exists(pathKeysXML))
            {

                if (Path.GetExtension(pathKeysXML) == ".xml")
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(File.ReadAllText(pathKeysXML));
                    try
                    {
                        XmlNode xnList = xml.SelectSingleNode("/RSAKeyValue/Modulus");
                        tbN.Text = xnList.InnerText;
                        xnList = xml.SelectSingleNode("/RSAKeyValue/Exponent");
                        tbE.Text = xnList.InnerText;
                        xnList = xml.SelectSingleNode("/RSAKeyValue/D");
                        tbD.Text = xnList.InnerText;
                        tbN.Enabled = true;
                        tbE.Enabled = true;
                        tbD.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed: " + ex.Message);
                    }
                }
            }
        }

        private void btnOpenFileIn_Click(object sender, EventArgs e)
        {
            isEncryptFile = true;
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "All Files (*.*)|*.*";
            if (op.ShowDialog() == DialogResult.OK)
                tbInput.Text = op.FileName;
        }

        private void btnOpenFolderIn_Click(object sender, EventArgs e)
        {
            isEncryptFile = false;
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                this.tbInput.Text = folderBrowserDialog1.SelectedPath;
        }

        private void selectOutput_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog f1 = new FolderBrowserDialog();
            if (f1.ShowDialog() == DialogResult.OK)
            {
                this.tbOutput.Text = f1.SelectedPath;
            }
        }

        private void RSA_Algorithm(string inputFile, string outputFile, RSAParameters RSAKeyInfo, bool isEncrypt)
        {
            try
            {
                FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read); //Đọc file input
                FileStream fsCiperText = new FileStream(outputFile, FileMode.Create, FileAccess.Write); //Tạo file output

                fsCiperText.SetLength(0);
                byte[] bin, encryptedData;
                long rdlen = 0;
                long totlen = fsInput.Length;
                int len;
                this.progressBar1.Minimum = 0;
                this.progressBar1.Maximum = 100;

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.ImportParameters(RSAKeyInfo); //Nhập thông tin khoá RSA (bao gồm khoá riêng)

                int maxBytesCanEncrypted;
                //RSA chỉ có thể mã hóa các khối dữ liệu ngắn hơn độ dài khóa, chia dữ liệu cho một số khối và sau đó mã hóa từng khối và sau đó hợp nhất chúng
                if (isEncrypt)
                    maxBytesCanEncrypted = ((RSA.KeySize - 384) / 8) + 37;// + 7: OAEP - Đệm mã hóa bất đối xứng tối ưu
                else
                    maxBytesCanEncrypted = (RSA.KeySize / 8);
                //Read from the input file, then encrypt and write to the output file.
                while (rdlen < totlen)
                {
                    if (totlen - rdlen < maxBytesCanEncrypted) maxBytesCanEncrypted = (int)(totlen - rdlen);
                    bin = new byte[maxBytesCanEncrypted];
                    len = fsInput.Read(bin, 0, maxBytesCanEncrypted);

                    if (isEncrypt) encryptedData = RSA.Encrypt(bin, false); //Mã Hoá
                    else encryptedData = RSA.Decrypt(bin, false); //Giải mã

                    fsCiperText.Write(encryptedData, 0, encryptedData.Length);
                    rdlen = rdlen + len;

                    this.label1f.Text = "Tên tệp xử lý : " + Path.GetFileName(inputFile) + "\t Thành công: " + ((long)(rdlen * 100) / totlen).ToString() + " %";
                    this.label1f.Update();
                    this.label1f.Refresh();

                    this.progressBar1.Value = (int)((rdlen * 100) / totlen);//thanh tiến trình
                }

                fsCiperText.Close(); //save file
                fsInput.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed: " + ex.Message);
            }
        }

        private void btnEncryptClick()
        {
            enabledOrDisableButtons(false);
            if (this.tbPathKeys.Text.Length == 0 || this.tbN.Text.Length == 0 || this.tbD.Text.Length == 0 || this.tbE.Text.Length == 0)
            {
                MessageBox.Show("Key không hợp lệ!");
                enabledOrDisableButtons(true);
                return;
            }

            try
            {
                if (tbInput.Text.Length != 0 &&
                tbPathKeys.Text.Length != 0 &&
                tbN.Text.Length != 0)
                {

                    //Calculator time ex...
                    Stopwatch sw = Stopwatch.StartNew();
                    sw.Start();
                    string inputFileName = tbInput.Text, outputFileName = "";

                    if (isEncryptFile)
                    {
                        outputFileName = tbOutput.Text + "\\" + Path.GetFileName(tbInput.Text) + ".lhde";
                    }
                    //get Keys.
                    RSA = new RSACryptoServiceProvider();
                    RSA.FromXmlString(File.ReadAllText(this.pathKeysXML));
                    if (isEncryptFile)
                        RSA_Algorithm(inputFileName, outputFileName, RSA.ExportParameters(true), true);
                    else
                    {
                        string[] filePaths = Directory.GetFiles(inputFileName, "*");

                        if (filePaths.Length == 0 || (filePaths.Length == 1 && (Path.GetFileName(filePaths[0]) == "Thumbs.db")))
                        {
                            MessageBox.Show("Thư mục rỗng!");
                            enabledOrDisableButtons(true);
                            return;
                        }



                        // tbt.Text = Path.GetDirectoryName(outputFileName);
                        for (int i = 0; i < filePaths.Length; i++)
                        {
                            outputFileName = tbOutput.Text + "\\" + Path.GetFileName(filePaths[i]);
                            if (Path.GetFileName(filePaths[i]) != "Thumbs.db")
                                RSA_Algorithm(filePaths[i], outputFileName + ".lhde", RSA.ExportParameters(true), true);
                        }
                    }
                    enabledOrDisableButtons(true);
                    sw.Stop();
                    double elapsedMs = sw.Elapsed.TotalMilliseconds / 1000;
                    MessageBox.Show("Thời gian thực thi " + elapsedMs.ToString() + "s");
                }
                else
                {
                    enabledOrDisableButtons(true);
                    MessageBox.Show("Dữ liệu không đủ để mã hóa!");
                }
            }
            catch (Exception ex)
            {
                enabledOrDisableButtons(true);
                MessageBox.Show("Failed: " + ex.Message);
            }
            enabledOrDisableButtons(true);
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (tbOutput.Text.Length == 0)
            {
                MessageBox.Show("Vui lòng chọn đường dẫn đến thư mục Output");
                return;
            }
            if (tbPathKeys.Text.Length == 0)
            {
                MessageBox.Show("Vui lòng chọn đường dẫn đến Key!");
                return;
            }
            btnEncryptDecrypt s = new btnEncryptDecrypt(btnEncryptClick);
            s.BeginInvoke(null, null);
        }
    }
}
