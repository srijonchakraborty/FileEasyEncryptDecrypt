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
                FileObject mc = new FileObject(); 
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
                stream = File.Open(@""+ pathInput, FileMode.Open);
                BinaryFormatter bformatter = new BinaryFormatter();
                Console.WriteLine("Reading Information");
                FileObject mc = (FileObject)bformatter.Deserialize(stream);
                mc.bytes = mc.bytes.Reverse().ToArray();
                stream.Close();
                MemoryStream ms = new MemoryStream(mc.bytes);
                fileStream = File.OpenWrite(outPutPath);
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
    }
}
