using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3_NetworkService.Model
{


    public class Log
    {

        public static string ime_fajla = "Log.txt";

        public static void Make_Log_File(Reaktor reaktor) 
        {
            using (StreamWriter wr = new StreamWriter(ime_fajla, true))
            {
                wr.WriteLine("{0} {1} {2} {3}", reaktor.ID, DateTime.Now, reaktor.Name, reaktor.Temperatura);
            }
        
        }



    }
}
