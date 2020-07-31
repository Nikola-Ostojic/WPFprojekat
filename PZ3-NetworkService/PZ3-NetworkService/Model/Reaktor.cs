using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3_NetworkService.Model
{
    public class Reaktor : INotifyPropertyChanged 
    {
        public static int id_brojac = 3;

        private string ime;

        private int id;

        private double temperatura;

        private Tip_Reaktora tip;

        public int ID 
        { 
            get { return id; } 
            
            set
            {
                if (id != value)
                {
                    id = value;
                    
                }
            } 
        }

        public string Name
        {
            get { return ime; }

            set
            {
                if (ime != value)
                {
                    ime = value;
                    
                }
            }
        }

        public Tip_Reaktora Tip
        {
            get
            {
                return tip;
            }
            set
            {
                if (tip != value)
                {
                    tip = value;
                    RaisePropertyChanged("Tip");
                }
            }
        }

        public double Temperatura
        {
            get
            { 
                return temperatura; 
            } 
            set 
            {
                if (temperatura != value)
                {
                    temperatura = value;
                    RaisePropertyChanged("Temperatura");
                }
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        
    }
}
