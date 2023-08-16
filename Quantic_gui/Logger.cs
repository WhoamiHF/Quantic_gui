using Quantic_console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantic_gui
{
    /**
     * Class responsible for logging moves played by user and computer moves with considered moves with their evaluation
     * also logs which player has won. Logging is realized into file 
     */
    internal class Logger
    {
        readonly string filename;
        readonly Form1 form1;
        public Logger(Form1 form1)
        {
            DateTime currentTime = DateTime.Now;
            filename = "logs/" + currentTime.ToString("yyyy_MM_dd_HH_mm_ss") + ".txt";
            this.form1 = form1;
        }

        /**
         * Logs played or considered move
         */
        public void Log(Move move)
        {
            form1.Invoke(() =>
            {
                Log(move.ToString());
            });
        }

        /**
         * Logs custom message
         */
        public void Log(string message)
        {
            form1.Invoke(() =>
            {
                if (!File.Exists("logs"))
                {
                    Directory.CreateDirectory("logs");
                }
                using (StreamWriter streamWriter = new StreamWriter(filename, true))
                {
                    streamWriter.WriteLine(message);
                    streamWriter.Close();
                }
            });
            
        }

    }
}
