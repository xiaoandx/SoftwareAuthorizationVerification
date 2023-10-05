using SAVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LicenseVerificationExample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                License license1 = GetLicense(@".\license.lic");
                License license = License.GetLicense();
            } catch (System.Exception ex)
            {
                Environment.Exit(0);
            }
        }

        public static License GetLicense(string licenseFile)
        {
            License result;
            try
            {
                if (!System.IO.File.Exists(licenseFile))
                {
                    throw new LicenseInvalidException("没有找到程序授权信息,请联系开发者!");
                }
                Security security = new Security();
                string encodedString = System.IO.File.ReadAllText(licenseFile);
                string s = security.DecodeString(encodedString);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(License));
                License license = (License)xmlSerializer.Deserialize(new System.IO.StringReader(s));
                if (!VerifyLicense(license))
                {
                    throw new LicenseInvalidException("许可无效,License Invalid!");
                }
                result = license;
            } catch
            {
                throw new LicenseInvalidException("许可无效,License Invalid!");
            }
            return result;
        }
        private static bool VerifyLicense(License lic)
        {
            Security security = new Security();
            string hash = security.GetHash(lic.GetHashString());
            bool flag = security.SignatureDeformatter(hash, lic.Signature);
            return flag;
        }
    }
}
