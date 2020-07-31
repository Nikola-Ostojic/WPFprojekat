using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3_NetworkService.Model
{
    public class StanjeModel
    {
        public int ID { get; set; }
        public DateTime Vreme { get; set; }
       // public int IDPuta { get; set; }
        public string Naziv_Reaktora { get; set; }
        public int Temperatura { get; set; }
    }
}
