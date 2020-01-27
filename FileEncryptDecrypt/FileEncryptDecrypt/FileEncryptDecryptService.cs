/*
|-------------------------------------------------------------------------------|
|	This code was written by Srijon Chakraborty								    |
|	Main source code link on https://github.com/srijonchakro			        |
|	All my source codes are available on http://srijon.softallybd.com           |
|	C# File Encrypt	Decrypt                                                     |
|	LinkedIn https://bd.linkedin.com/in/srijon-chakraborty-0ab7aba7				|
|-------------------------------------------------------------------------------|
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Configuration;

namespace FileEncryptDecrypt
{
    [Serializable]
    public class FileObject
    {
        public string Extension;
        public byte[] bytes;
    }
    public class FileEncryptDecryptService
    {
        public static event Action<bool> ActionEncryptDone;
        public static event Action<Exception> ActionEncryptException;
        public static event Action<bool> ActionDecryptDone;
        public static event Action<Exception> ActionDecryptException;
        public async static Task FileEncryptAsync(string pathInput,string outPutPath)
        {
           
            Stream stream = null;
            try
            {
                string key = ConfigurationManager.AppSettings["keyCode"];
                var itm= System.Convert.FromBase64String(key);
                var itm2 = System.Text.ASCIIEncoding.UTF8.GetString(itm);
                itm = System.Convert.FromBase64String(itm2);
                itm2 = System.Text.ASCIIEncoding.UTF8.GetString(itm).Substring(0, 16);//Make the key 128 bit
                string extention = EncryptStringToBytesAes(Path.GetExtension(pathInput), Encoding.ASCII.GetBytes(itm2), Encoding.ASCII.GetBytes(itm2));

                FileObject mc = new FileObject();
                mc.Extension = extention;
                mc.bytes = File.ReadAllBytes(@""+ pathInput);
                mc.bytes = mc.bytes.Reverse().ToArray();
                stream = File.Open(@""+ outPutPath, FileMode.Create);
                Console.WriteLine("Writing Information");
                BinaryFormatter bformatter=new BinaryFormatter();
                bformatter.Serialize(stream, mc);
                stream.Close();
                ActionEncryptDone?.Invoke(true);
            }
            catch (Exception ex)
            {
                stream?.Close();
                ActionEncryptException?.Invoke(ex);
                throw new Exception(ex.Message);
            }
        }
        public async static Task FileDecryptAsync(string pathInput, string outPutPath)
        {
            Stream stream = null;
            FileStream fileStream = null;
            try
            {
                string key = ConfigurationManager.AppSettings["keyCode"];
                var itm = System.Convert.FromBase64String(key);
                var itm2 = System.Text.ASCIIEncoding.UTF8.GetString(itm);
                itm = System.Convert.FromBase64String(itm2);
                itm2 = System.Text.ASCIIEncoding.UTF8.GetString(itm).Substring(0, 16);//Make the key 128 bit

                stream = File.Open(@""+ pathInput, FileMode.Open);
                BinaryFormatter bformatter = new BinaryFormatter();
                Console.WriteLine("Reading Information");
                FileObject mc = (FileObject)bformatter.Deserialize(stream);
                mc.bytes = mc.bytes.Reverse().ToArray();
                stream.Close();
                
                string extension = DecryptStringFromBytesAes(mc.Extension, Encoding.ASCII.GetBytes(itm2), Encoding.ASCII.GetBytes(itm2));
                MemoryStream ms = new MemoryStream(mc.bytes);
                string finalPath = Path.ChangeExtension(outPutPath, extension);
                fileStream = File.OpenWrite(finalPath);
                ms.WriteTo(fileStream);
                fileStream.Flush();
                fileStream.Close();
                ActionDecryptDone?.Invoke(true);
            }
            catch (Exception ex)
            {
                fileStream?.Close();
                ActionDecryptException?.Invoke(ex);
                throw new Exception(ex.Message);
            }
        }
        private static string EncryptStringToBytesAes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            byte[] encrypted;

            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt =
                            new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }
        private static string DecryptStringFromBytesAes(string cipherTextString, byte[] Key, byte[] IV)
        {
            byte[] cipherText = Convert.FromBase64String(cipherTextString);

            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt =
                            new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}
