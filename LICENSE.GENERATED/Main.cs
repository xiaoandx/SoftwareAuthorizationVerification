using LICENSE.GENERATED.Utils;
using SAVC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace LICENSE.GENERATED
{
    public partial class Main : Form
    {
        private SaveFileDialog sfd;
        private OpenFileDialog ofd;

        private RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        private SaveFileDialog SavePublicKey;
        private SaveFileDialog SavePrivateKey;
        public Main()
        {
            InitializeComponent();
            InitTitle();
            InitAutoType();
        }

        private void butCreateLicense_Click(object sender, EventArgs e)
        {
            string ValidStatus = string.Empty;
            bool status = FormIsValid(out ValidStatus);
            if (!status)
            {
                MessageBox.Show(ValidStatus);
                return;
            }
            //文件对话框初始化
            sfd = new SaveFileDialog();
            sfd.DefaultExt = "lic";
            sfd.FileName = "license.lic";
            sfd.Filter = "授权文件(*.lic)|*.lic|所有文件|*.*";
            sfd.Title = "保存授权文件";
            sfd.ShowDialog();
            try
            {
                SAVC.License lic = new SAVC.License();
                lic.SerialNumber = System.Guid.NewGuid().ToString().ToUpper();
                lic.LicenceTo = textBFactoryName.Text.Trim();
                lic.ProductName = textBProductName.Text.Trim();
                lic.Edition = comBLicenseType.Text.Split('@')[0];
                lic.MajorVersion = Convert.ToInt32(textBMainVersion.Text);
                lic.MinorVersion = Convert.ToInt32(textBSmallVersion.Text);
                DateTime selectTime;
                DateTime.TryParse(dateTimePTime.Text, out selectTime);
                lic.ExpireTo = selectTime;
                if (!string.IsNullOrWhiteSpace(textBHWCode.Text))
                {
                    lic.MachineDesc = textBHWCode.Text.Trim();
                }
                Utils.Security security = new Utils.Security();
                string tag = comBLicenseType.Text.Split('@')[1];
                lic.UserData = getUserData(tag);
                string hash = security.GetHash(lic.GetHashString());
                lic.Signature = security.SignatureFormatter(hash);
                lic.Save(sfd.FileName);
            } catch (Exception)
            {

            }
        }

        private void butCreatePublicKey_Click(object sender, EventArgs e)
        {
            try
            {
                string publicKey = rsa.ToXmlString(false);
                //文件对话框初始化
                SavePublicKey = new SaveFileDialog();
                SavePublicKey.DefaultExt = "txt";
                SavePublicKey.FileName = "PublicKey.txt";
                SavePublicKey.Filter = "密钥文件(*.txt)|*.txt|所有文件|*.*";
                SavePublicKey.Title = "保存公钥文件";
                File.WriteAllText(SavePublicKey.FileName, publicKey);
                textBPublicKey.Text = publicKey;
                MessageBox.Show("Creat Public Key Success！");
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void butCreatePrivateKey_Click(object sender, EventArgs e)
        {
            try
            {
                string privateKey = rsa.ToXmlString(true);
                //文件对话框初始化
                SavePrivateKey = new SaveFileDialog();
                SavePrivateKey.DefaultExt = "txt";
                SavePrivateKey.FileName = "PrivateKey.txt";
                SavePrivateKey.Filter = "密钥文件(*.txt)|*.txt|所有文件|*.*";
                SavePrivateKey.Title = "保存私钥文件";
                File.WriteAllText(SavePrivateKey.FileName, privateKey);
                textBPrivateKey.Text = privateKey;
                MessageBox.Show("Creat Private Key Success！");
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InitAutoType()
        {
            try
            {
                string versions = ConfigurationManager.AppSettings["version"];
                string[] vs = versions.Split(',');
                foreach (var v in vs)
                {
                    comBLicenseType.Items.Add(v);
                }
            } catch (Exception e)
            {
                MessageBox.Show("初始化程序失败，请检查配置文件是否正确!error:" + e.Message);
            }
        }

        private void InitTitle()
        {
            this.Text = "LINCESE生成工具";
        }

        private bool FormIsValid(out string Msg)
        {
            if (comBLicenseType.SelectedItem == null)
            {
                Msg = "请选择License类型";
                return false;
            }

            if ("".Equals(textBFactoryName.Text))
            {
                Msg = "请输入公司名称";
                return false;
            }

            if ("".Equals(textBProductName.Text))
            {
                Msg = "请输入产品名称";
                return false;
            }

            if ("".Equals(textBMainVersion.Text))
            {
                Msg = "主版本号不能为空";
                return false;
            }

            if ("".Equals(textBSmallVersion.Text))
            {
                Msg = "次版本号不能为空";
                return false;
            }

            if ("".Equals(dateTimePTime.Text))
            {
                Msg = "过期时间不能为空";
                return false;
            }

            DateTime selectTime, currentTime;
            string currentString =  DateTime.Now.ToString("yyyy/MM/dd");
            DateTime.TryParse(dateTimePTime.Text, out selectTime);
            DateTime.TryParse(currentString, out currentTime);

            if (!(currentTime < selectTime))
            {
                Msg = "过期时间必须大于当前时间";
                return false;
            }

            if ("".Equals(textBHWCode.Text))
            {
                Msg = "客户PC编码不能为空";
                return false;
            }
            Msg = "参数校验通过";
            return true;
        }

        private string getUserData(string tag)
        {
            return ConfigurationManager.AppSettings["data" + tag];
        }

        private void butOpenFile_Click(object sender, EventArgs e)
        {
            ofd = new OpenFileDialog();
            ofd.DefaultExt = "lic";
            ofd.Filter = "授权文件(*.lic)|*.lic|所有文件|*.*";
            ofd.FileName = "licence.lic";

            ofd.ShowDialog();
            string f = ofd.FileName;
            textBLicenseFilePath.Text = f;
            try
            {
                SAVC.License license = GetLicense(f);
                textBLicenseInfo.Text = license.ToString();
            } catch (System.Exception ex)
            {
                textBLicenseInfo.Text = ex.ToString();
            }
        }

        public SAVC.License GetLicense(string licenseFile)
        {
            SAVC.License result;
            try
            {
                if (!System.IO.File.Exists(licenseFile))
                {
                    throw new LicenseInvalidException("没有找到程序授权信息,请联系授权方！");
                }
                SAVC.Security security = new SAVC.Security();
                string encodedString = System.IO.File.ReadAllText(licenseFile);
                string s = security.DecodeString(encodedString);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SAVC.License));
                SAVC.License license = (SAVC.License)xmlSerializer.Deserialize(new System.IO.StringReader(s));
                if (!this.VerifyLicense(license))
                {
                    throw new LicenseInvalidException("许可无效,License Invalid!");
                }
                result = license;
            } catch
            {
                throw;
            }
            return result;
        }
        private bool VerifyLicense(SAVC.License lic)
        {
            SAVC.Security security = new SAVC.Security();
            string hash = security.GetHash(lic.GetHashString());
            bool flag = security.SignatureDeformatter(hash, lic.Signature);
            return flag;
        }
    }
}
