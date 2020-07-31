using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3_NetworkService.Model
{
    public class Tip_Reaktora
    {
      
        private string ime_tipa;
        public string Ime_tipa
        {
            get
            {
                return ime_tipa;
            }
            set
            {
                if (ime_tipa != value)
                {
                    ime_tipa = value;
                   
                }
            }
        }

        private string slika;
        public string Slika
        {
            get
            {
                return slika;
            }
            set
            {
                if (slika != value)
                {
                    slika = value;
                    
                }
            }
        }

        public Tip_Reaktora(string ime_tipa, string slikaUri)
        {
            Ime_tipa = ime_tipa;

            slika = slikaUri;
        }
    }
}
