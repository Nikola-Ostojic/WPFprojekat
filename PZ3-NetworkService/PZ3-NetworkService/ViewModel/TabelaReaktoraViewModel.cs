using PZ3_NetworkService.Model;
using PZ3_NetworkService.PomocneKlasee;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PZ3_NetworkService.ViewModel
{
    public class TabelaReaktoraViewModel : BindableBase
    {
        public static ObservableCollection<Reaktor> Reaktori { get; set; }



        public static ObservableCollection<Reaktor> SearchReaktori_GlavniPrikaz { get; set; }


        public MyICommand DeleteCommand { get; set; }

        public MyICommand AddCommand {get; set;}

        public MyICommand OdustaniButtonCommand { get; set; }

        public List<string> vrste_reaktora { get; set; }

        private Reaktor selectedReaktor;

        public string tipReaktora { get; set; }



        string slika_osnova = @"C:\Users\\user\\Desktop\\3. projekat\\PZ3-NetworkService\\Reaktori_Slike";


        private bool combobox1, combobox2;

        private string searchtextbox;

        public MyICommand SearchButtonCommand { get; set; }

        public TabelaReaktoraViewModel()
        {
            Reaktori = new ObservableCollection<Reaktor>();



            SearchReaktori_GlavniPrikaz = new ObservableCollection<Reaktor>();


            foreach (Reaktor r in MainWindowViewModel.Reaktori_glavni)
            {
                Reaktori.Add(r);
            }

            DeleteCommand = new MyICommand(OnDelete, CanDelete);



            AddCommand = new MyICommand(OnAdd);

            SearchButtonCommand = new MyICommand(OnSearch);

            OdustaniButtonCommand = new MyICommand(OnOdustani);

            vrste_reaktora = new List<string>() { "Terminalni", "Fuzioni", "Hibridni" };
        }





        public Reaktor SelectedReaktor
        {
            get { return selectedReaktor; }

            set {
                
                    selectedReaktor = value;

                    DeleteCommand.RaiseCanExecuteChanged();

                } 
        }

        



        public bool ComboBox1
        {
            get { return combobox1; }

            set
            {
                if (combobox2 != true)
                      combobox1 = value;               
            }
        }

        public bool ComboBox2
        {
            get { return combobox2; }

            set
            {
                if (combobox1 != true)
                     combobox2 = value;              
            }
        }

        public string SearchTextBox
        {
            get { return searchtextbox; }

            set
            {
                searchtextbox = value;
            }
        }



        public bool CanDelete()
        {
            return selectedReaktor != null;
        }

        public void OnDelete()
        {
            Reaktor r = selectedReaktor;

            Reaktori.Remove(r);

            NetworkView_ViewModel.Reaktori2.Remove(r);

            MainWindowViewModel.Reaktori_glavni.Remove(r);

            
        }


        private void OnAdd()
        {

            if (tipReaktora != null)
            {
                

                    Reaktori.Add(new Reaktor { ID = ++(Reaktor.id_brojac), Name = "Reaktor " + Convert.ToString(Reaktor.id_brojac), Tip = new Tip_Reaktora(tipReaktora, slika_osnova + @"\" + tipReaktora + ".jpg"), Temperatura = 0 });

                    MainWindowViewModel.Reaktori_glavni.Add(new Reaktor { ID = Reaktor.id_brojac, Name = "Reaktor " + Convert.ToString(Reaktor.id_brojac), Tip = new Tip_Reaktora(tipReaktora, slika_osnova + @"\" + tipReaktora + ".jpg"), Temperatura = 0 });
                
            }
            else
            {
                MessageBox.Show("Morate izabrati tip");
            }

        }


        public void OnSearch()
        {
            if (Reaktori.Count != 0)
            {
                if (SearchTextBox != null)
                {
                   if (ComboBox1 == true && ComboBox2 == false)
                   {
                            
                         foreach(Reaktor r in Reaktori.ToList())
                         {

                            if (r.Name != SearchTextBox)
                            {
                                SearchReaktori_GlavniPrikaz.Add(r);
                                Reaktori.Remove(r);
                            }

                         }

                        if (SearchReaktori_GlavniPrikaz.Count == 0)
                        {
                            MessageBox.Show("Nema Reaktora sa tim tipom.");
                        }

                    }

                   else if (ComboBox2 == true && ComboBox1 == false)
                        {
                            foreach (Reaktor r in Reaktori.ToList())
                            {

                                if (r.Tip.Ime_tipa != SearchTextBox)
                                {
                                    SearchReaktori_GlavniPrikaz.Add(r);
                                    Reaktori.Remove(r);
                                }


                            }

                            if (SearchReaktori_GlavniPrikaz.Count == 0)
                            {
                                MessageBox.Show("Nema Reaktora sa tim imenom.");
                            }
                        }
                   else
                        {
                            MessageBox.Show("Morate izabrati Search po imenu ili po tipu.");
                        }

                }
                else
                {
                    MessageBox.Show("Niste uneli tekst u Search polje.");
                }

            }
            else
            {
                MessageBox.Show("Nema Reaktora u tabeli.");
            }
        }


        /*public bool CanOdustani()
        {
            return SearchReaktori_GlavniPrikaz.Count != 0;
        }*/


        public void OnOdustani()
        {
            if (SearchReaktori_GlavniPrikaz.Count != 0)
            {
                foreach (Reaktor r in SearchReaktori_GlavniPrikaz.ToList())
                {
                    Reaktori.Add(r);

                    SearchReaktori_GlavniPrikaz.Remove(r);
                }
            }
            else
                MessageBox.Show("Svi Reaktori su prikazani u tabeli.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        

    }
}
