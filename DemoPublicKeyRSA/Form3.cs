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
    public partial class Decode : Form
    {
        private delegate void btnEncryptDecrypt();
        public Decode()
        {
            InitializeComponent();
        }
        private void label13_Click(object sender, EventArgs e)
        {

        }
        private void Decode_Load(object sender, EventArgs e)
        {
            this.tbN.Enabled = false;
            this.tbD.Enabled = false;
            this.tbE.Enabled = false;
        }


        private RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
        private string pathKeysXML = "";
        private bool isEncryptFile = true;

        private void tbPathKeys_TextChanged(object sender, EventArgs e)
        {

        }
        private void enabledOrDisableButtons(bool isEnable)
        {           
            this.tbOutput.Enabled = isEnable;
            this.btnDecrypt.Enabled = isEnable;
            this.btnOpenFileIn.Enabled = isEnable;
            this.btnOpenFileKeys.Enabled = isEnable;
            this.btnOpenFolderIn.Enabled = isEnable;
            this.selectOutput.Enabled = isEnable;
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
                }

                fsCiperText.Close(); //save file
                fsInput.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed: " + ex.Message);
            }
        }

        private void btnDecryptClick()
        {
            MessageBox.Show("Vui lòng chờ!!");
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

                    if (isEncryptFile && Path.GetExtension(inputFileName) != ".lhde")
                    {
                        MessageBox.Show("Tệp tin này không được hỗ trợ đển giải mã!");
                        enabledOrDisableButtons(true);
                        return;
                    }

                    if (isEncryptFile)
                    {
                        outputFileName = tbOutput.Text + "\\" + Path.GetFileName(inputFileName.Substring(0, inputFileName.Length - 5));
                    }

                    RSA = new RSACryptoServiceProvider();
                    RSA.FromXmlString(File.ReadAllText(this.pathKeysXML));

                    if (isEncryptFile)
                        RSA_Algorithm(inputFileName, outputFileName, RSA.ExportParameters(true), false);
                    else
                    {
                        string[] filePaths = Directory.GetFiles(inputFileName, "*.lhde", SearchOption.AllDirectories);
                        if (filePaths.Length == 0 || (filePaths.Length == 1 && (Path.GetFileName(filePaths[0]) == "Thumbs.db")))
                        {
                            MessageBox.Show("Thư mục rỗng!");
                            enabledOrDisableButtons(true);
                            return;
                        }

                        for (int i = 0; i < filePaths.Length; i++)
                            if (Path.GetFileName(filePaths[i]) != "Thumbs.db")
                            {
                                outputFileName = tbOutput.Text + "\\" + Path.GetFileName(filePaths[i].Substring(0, filePaths[i].Length - 5));
                                RSA_Algorithm(filePaths[i], outputFileName, RSA.ExportParameters(true), false);
                            }

                    }
                    enabledOrDisableButtons(true);
                    sw.Stop();
                    double elapsedMs = sw.Elapsed.TotalMilliseconds / 1000;
                    MessageBox.Show("Tổng thời gian thực thi: " + elapsedMs.ToString() + "s");
                }
                else
                {
                    MessageBox.Show("Không đủ điều kiện để giải mã !");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed: " + ex.Message);
            }
           // enabledOrDisableButtons(true);
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            if ((tbOutput.Text.Length == 0) || (tbInput.Text.Length == 0) || (tbPathKeys.Text.Length == 0))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin");
                return;
            }
            btnEncryptDecrypt s = new btnEncryptDecrypt(btnDecryptClick);
            s.BeginInvoke(null, null);
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
    }
}



