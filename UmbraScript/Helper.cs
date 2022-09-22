using System;
using System.IO;
using System.Text;

namespace UmbraScript
{
    public class Helper
    {
        /* Reads a file as a string */
        public static string ReadFileS(string File)
        {
            try
            {
                StreamReader m = new StreamReader(File);
                string s = m.ReadToEnd();
                m.Close();
                return s;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /* Reads a file as a byte[] array */
        public static byte[] ReadFileB(string file)
        {
            try
            {
                BinaryReader r = new BinaryReader(File.OpenRead(file));

                byte[] fs = r.ReadBytes((int)r.BaseStream.Length);
                return fs;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
                return null;
            }
        }
        /* Writes a string to file */
        public static bool WriteFileS(string file, string contents)
        {
            try
            {
                StreamWriter m = new StreamWriter(File.OpenWrite(file), Encoding.UTF8);
                m.Write(contents);
                m.Flush();
                m.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
