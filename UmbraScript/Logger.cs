using System;
using System.IO;

namespace UmbraScript
{
    public class Logger
    {
        private static StreamWriter m_log = new StreamWriter(new FileStream(DateTime.Now.ToString("MM-dd-yyyy_HH-mm-ss") + ".log", FileMode.OpenOrCreate, FileAccess.Write));

        public static void Write(string message)
        {
            m_log.Write(message);
            m_log.Flush();
        }

        public static void WriteLine(string message)
        {
            m_log.WriteLine(message);
            m_log.Flush();
        }

    }
}
