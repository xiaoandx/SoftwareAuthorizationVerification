using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GUID
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"=============================== Start ===============================");
            Console.WriteLine($"Get the GUID of the current computer, please wait ......");
            Thread.Sleep(2000);
            Console.WriteLine($"GUID obtained successfully! Please copy and save");
            Console.WriteLine();
            Console.WriteLine($"GUID:{GetHWInfo()}");
            Console.WriteLine();
            Console.WriteLine($"=============================== Colse ===============================");
            Console.ReadLine();
        }

        public static string GetMachineHash()
        {
            Security security = new Security();
            return security.MD5(GetHWInfo());
        }
        public static string GetHWInfo()
        {
            Security security = new Security();
            string plainString = string.Format("{0}\r\n{1}\r\n{2}", GetCpuID(), GetHddSerial(), GetMacAddress());
            plainString = security.EncodeString(plainString);
            return security.EncodeString(plainString);
        }
        private static string GetMacAddress()
        {
            string result;
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection instances = managementClass.GetInstances();
                using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ManagementObject managementObject = (ManagementObject)enumerator.Current;
                        if ((bool)managementObject["IPEnabled"])
                        {
                            if (stringBuilder.Length > 0)
                            {
                                stringBuilder.Append(";");
                            }
                            stringBuilder.AppendFormat("{0}", managementObject["MacAddress"]);
                        }
                    }
                }
                result = stringBuilder.ToString();
            } catch
            {
                result = string.Empty;
            }
            return result;
        }
        private static string GetCpuID()
        {
            return GetWMIProperties("Win32_Processor", "Caption", "ProcessorId");
        }
        private static string GetHddSerial()
        {
            return GetWMIProperties("Win32_DiskDrive", "Caption", "Model");
        }
        private static string GetWMIProperties(string className, string caption, string property)
        {
            string result;
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                ManagementClass managementClass = new ManagementClass(className);
                ManagementObjectCollection instances = managementClass.GetInstances();
                using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ManagementObject managementObject = (ManagementObject)enumerator.Current;
                        PropertyData propertyData = managementObject.Properties[property];
                        if (propertyData != null && propertyData.Value != null)
                        {
                            if (stringBuilder.Length > 0)
                            {
                                stringBuilder.Append(";");
                            }
                            stringBuilder.AppendFormat("{0}:{1}", managementObject[caption], propertyData.Value.ToString().Trim());
                        }
                    }
                }
                result = stringBuilder.ToString();
            } catch
            {
                result = string.Empty;
            }
            return result;
        }
    }

    public class Security
    {
        private string p_strKeyPublic = "<RSAKeyValue><Modulus>qTUlxhj40BFCJ93ub8TdUAywKVc5otivYfUeFWxTTOdFLjmc0Ek61UXgWYm4qYL6t+RrG9sSKsJen/NUZtiGJicZYgdhq5mD2Im/Xd05smCx8a05FUDrviXalaJOoba0WIWZG4NM9YlV/z5ZMiJFmhUvrWip7DOyPZ6qp5wIRFk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        public string GetHash(string m_strSource)
        {
            HashAlgorithm hashAlgorithm = HashAlgorithm.Create("MD5");
            byte[] bytes = Encoding.UTF8.GetBytes(m_strSource);
            byte[] inArray = hashAlgorithm.ComputeHash(bytes);
            return Convert.ToBase64String(inArray);
        }
        public string MD5(string source)
        {
            HashAlgorithm hashAlgorithm = HashAlgorithm.Create("MD5");
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            byte[] value = hashAlgorithm.ComputeHash(bytes);
            return BitConverter.ToString(value);
        }
        public bool SignatureDeformatter(string p_strHashbyteDeformatter, string p_strDeformatterData)
        {
            bool result;
            try
            {
                //hash
                byte[] rgbHash = Convert.FromBase64String(p_strHashbyteDeformatter);
                byte[] rgbSignature = Convert.FromBase64String(p_strDeformatterData);

                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
                rSACryptoServiceProvider.FromXmlString(this.p_strKeyPublic);
                RSAPKCS1SignatureDeformatter rSAPKCS1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(rSACryptoServiceProvider);
                rSAPKCS1SignatureDeformatter.SetHashAlgorithm("MD5");

                if (rSAPKCS1SignatureDeformatter.VerifySignature(rgbHash, rgbSignature))
                {
                    result = true;
                } else
                {
                    result = false;
                }
            } catch
            {
                result = false;
            }
            return result;
        }
        public string EncodeString(string plainString)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(plainString);
            return Convert.ToBase64String(bytes);
        }
        public string DecodeString(string encodedString)
        {
            byte[] bytes = Convert.FromBase64String(encodedString);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
