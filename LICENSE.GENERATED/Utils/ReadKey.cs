using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LICENSE.GENERATED.Utils
{
    public class ReadKey
    {
        public static string Name(string FileName)
        {
            string content = string.Empty;
            try
            {
                content = File.ReadAllText(FileName);
            } catch (Exception ex)
            {
                content = "";
            }
            return content;
        }

        public static string Public()
        {
            string content = string.Empty;
            try
            {
                content = File.ReadAllText("PublicKey.txt");
            } catch (Exception ex)
            {
                content = "";
            }
            return content;
        }

        public static string Private()
        {
            string content = string.Empty;
            try
            {
                content = File.ReadAllText("PrivateKey.txt");
            } catch (Exception ex)
            {
                content = "";
            }
            return content;
        }
    }
}
