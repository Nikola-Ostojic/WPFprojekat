using PZ3_NetworkService.Model;
using PZ3_NetworkService.PomocneKlasee;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;

namespace PZ3_NetworkService.ViewModel
{
    public class DataChartViewModel : BindableBase
    {
        public static ObservableCollection<Reaktor> Reaktori3 { get; set; }

        public static ObservableCollection<Reaktor> UcitaniReaktori { get; set; }       //za ucitavanje iz log fajla

        public ObservableCollection<GraphObjectModel> Items { get; set; }

        private ObservableCollection<StanjeModel> promeneStanja;

        public string comboBoxReaktorSelected { get; set; }

        public List<string> vrste_reaktora { get; set; }

        public MyICommand IscrtajGrafikon { get; set; }


        public DataChartViewModel()
        {
            Reaktori3 = new ObservableCollection<Reaktor>();

            vrste_reaktora = new List<string>();

            Items = new ObservableCollection<GraphObjectModel>();

            foreach (Reaktor r in MainWindowViewModel.Reaktori_glavni)
            {
                Reaktori3.Add(r);

                vrste_reaktora.Add(r.Name);
            }

            

            IscrtajGrafikon = new MyICommand(iscrtajGrafikon);

            comboBoxReaktorSelected = "Reaktor 1";

            iscrtajGrafikon();

        }



        public ObservableCollection<StanjeModel> PromeneStanja
        {
            get { return promeneStanja; }
            set
            {
                if (promeneStanja != value)
                {
                    promeneStanja = value;
                }
            }
        }


        



        private void iscrtajGrafikon()
        {
            

            if (comboBoxReaktorSelected != null)
            {
                ObservableCollection<StanjeModel> kolekcija2 = LoadStanjeModel(comboBoxReaktorSelected);

                ObservableCollection<StanjeModel> kolekcija = new ObservableCollection<StanjeModel>();

                if (kolekcija2.Count >= 5)
                {
                    for (int i = kolekcija2.Count - 5; i < kolekcija2.Count; i++)
                    {
                        kolekcija.Add(kolekcija2[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < kolekcija2.Count; i++)
                    {
                        kolekcija.Add(kolekcija2[i]);
                    }
                }

                Grafik.IscrtajGrafik(Items, kolekcija);
            }
            else
            {
                MessageBox.Show("Morate izabrati tip reaktora");
            }
           
        }

        

        static string fileName = "Log.txt";



        public static ObservableCollection<StanjeModel> LoadStanjeModel( string comboBoxIzabraniReaktor)
        {

            ObservableCollection<StanjeModel> collection = new ObservableCollection<StanjeModel>();

            if (File.Exists(fileName))
            {
                using (var reader = new StreamReader(fileName))
                {
                    string str;
                    while ((str = reader.ReadLine()) != null)
                    {
                        
                        
                        string[] podeljen1 = str.Split(' ');

                        if ((podeljen1[4] + " " + podeljen1[5]) == comboBoxIzabraniReaktor)
                        {
                            collection.Add(new StanjeModel() { ID = Convert.ToInt32(podeljen1[0]), Vreme = Convert.ToDateTime(podeljen1[1] + " " + podeljen1[2] + " " + podeljen1[3]), Naziv_Reaktora = (podeljen1[4] + podeljen1[5]), Temperatura = Convert.ToInt32(podeljen1[6]) });
                        }

                      
                           
                    }
                }
            }

            return collection;
        }


    }
}
