using PZ3_NetworkService.Models;
using PZ3_NetworkService.Services;
using PZ3_NetworkService.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Net;
using System.Net.Sockets;

namespace PZ3_NetworkService.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public MainWindowViewModel()
        {
            Putevi = PuteviSvc.LoadPutevi();
            PromeneStanja = new ObservableCollection<StanjeModel>();

            OsveziListe();
            
            CreateListener(); //Povezivanje sa serverskom aplikacijom
        }

        private void CreateListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25565);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */

                            var num = 1;
                            if(Putevi.Any(p => p.NadgledaSe))
                                num = Putevi.Where(p => p.NadgledaSe).Max(p => p.ID) + 1;

                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(num.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            //Console.WriteLine(incomming); //Na primer: "Objekat_1:272"

                            //################ IMPLEMENTACIJA ####################

                            var stanje = ParseStanje(incomming);
                            if (stanje != null)
                                OnPromenaStanja(stanje);
                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

        #endregion

        #region Fields

        private ObservableCollection<PutModel> putevi;
        private PutModel puteviSelected;
        private PutModel nadgledaniPuteviSelected;
        private PutModel nenadgledaniPuteviSelected;
        private ObservableCollection<StanjeModel> promeneStanja;

        #endregion

        #region Properties

        public ObservableCollection<PutModel> Putevi
        {
            get { return putevi; }
            set
            {
                if (putevi != value)
                {
                    putevi = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public PutModel PuteviSelected
        {
            get { return puteviSelected; }
            set
            {
                if (puteviSelected != value)
                {
                    puteviSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICollectionView NadgledaniPutevi
        {
            get
            {
                var source = new CollectionViewSource { Source = Putevi }.View;
                source.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
                source.Filter = FilterNadgledani;
                return source;
            }
        }

        public PutModel NadgledaniPuteviSelected
        {
            get { return nadgledaniPuteviSelected; }
            set
            {
                if (nadgledaniPuteviSelected != value)
                {
                    nadgledaniPuteviSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICollectionView NenadgledaniPutevi
        {
            get
            {
                var source = new CollectionViewSource { Source = Putevi }.View;
                source.SortDescriptions.Add(new SortDescription("Broj", ListSortDirection.Ascending));
                source.Filter = FilterNenadgledani;
                return source;
            }
        }

        public PutModel NenadgledaniPuteviSelected
        {
            get { return nenadgledaniPuteviSelected; }
            set
            {
                if (nenadgledaniPuteviSelected != value)
                {
                    nenadgledaniPuteviSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<StanjeModel> PromeneStanja
        {
            get { return promeneStanja; }
            set
            {
                if(promeneStanja != value)
                {
                    promeneStanja = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        private void OsveziListe()
        {
            NotifyPropertyChanged("NadgledaniPutevi");
            NotifyPropertyChanged("NenadgledaniPutevi");
        }

        private bool FilterNadgledani(object item)
        {
            PutModel put = item as PutModel;
            return put.NadgledaSe;
        }

        private bool FilterNenadgledani(object item)
        {
            PutModel put = item as PutModel;
            return !put.NadgledaSe;
        }

        private StanjeModel ParseStanje(string incomming)
        {
            try
            {
                string[] delovi = incomming.Split('_', ':');

                var id = PromeneStanja.Count == 0 ? 1 : PromeneStanja.Max(ps => ps.ID) + 1;
                var idPuta = int.Parse(delovi[1]);
                var brojPuta = 0;
                if(Putevi.Any(p => p.ID == idPuta))
                    brojPuta = Putevi.First(p => p.ID == idPuta).Broj;
                var brojVozila = int.Parse(delovi[2]);

                return new StanjeModel() { ID = id, Vreme = DateTime.Now, IDPuta = idPuta, BrojPuta = brojPuta, BrojVozila = brojVozila };
            }
            catch
            {
                return null;
            }
        }

        public void OnPromenaStanja(StanjeModel stanje)
        {
            PromeneStanja.Add(stanje);
            AzurirajStanjaNadgledanihPuteva(stanje);
            Log.Add(stanje);
        }

        private void AzurirajStanjaNadgledanihPuteva(StanjeModel stanje)
        {
            var nadgledaniPut = Putevi.FirstOrDefault(p => p.NadgledaSe && p.ID == stanje.IDPuta);

            if (nadgledaniPut != null)
            {
                nadgledaniPut.BrojVozila = stanje.BrojVozila;


                System.Diagnostics.Debug.WriteLine(string.Concat(DateTime.Now.ToString("hh:mm:ss.fff "),
                    Environment.NewLine, "    BrojPuta: ", stanje.BrojPuta.ToString(),
                    Environment.NewLine, "    BrojVozila: ", stanje.BrojVozila.ToString(),
                    Environment.NewLine, "    GranicnoStanjeVozila: ", nadgledaniPut.GranicnoStanjeVozila.ToString(),
                    Environment.NewLine, "    StanjeKriticno: ", nadgledaniPut.StanjeKriticno.ToString(),
                    Environment.NewLine, "    Tip.ID: ", nadgledaniPut.Tip.ID.ToString(),
                    Environment.NewLine, "    Tip.Naziv: ", nadgledaniPut.Tip.Naziv));


                OsveziListe();
            }
        }

        #endregion

        #region Commands

        #region AzurirajPuteviCommand

        public ICommand AzurirajPuteviCommand { get { return new RelayCommand(azurirajPuteviCommand, canAzurirajPuteviCommand); } }

        private void azurirajPuteviCommand(object param)
        {
            var win = new Putevi();
            win.ShowDialog();

            OsveziListe();
        }

        private bool canAzurirajPuteviCommand(object param)
        {
            return true;
        }

        #endregion

        #region DodajUNadgledaneCommand

        public ICommand DodajUNadgledaneCommand { get { return new RelayCommand(dodajUNadgledane, canDodajUNadgledane); } }

        private void dodajUNadgledane(object param)
        {
            NenadgledaniPuteviSelected.Index = Putevi.Count(p => p.NadgledaSe);
            NenadgledaniPuteviSelected.NadgledaSe = true;

            PuteviSvc.SavePutevi(Putevi);

            OsveziListe();
        }

        private bool canDodajUNadgledane(object param)
        {
            return NenadgledaniPuteviSelected != null;
        }

        #endregion

        #region DodajUNadgledaneCommand

        public ICommand VratiUNenadgledaneCommand { get { return new RelayCommand(vratiUNenadgledaneCommand, canVratiUNenadgledaneCommand); } }

        private void vratiUNenadgledaneCommand(object param)
        {
            NadgledaniPuteviSelected.Index = 0;
            NadgledaniPuteviSelected.NadgledaSe = false;

            PuteviSvc.SavePutevi(Putevi);

            OsveziListe();
        }

        private bool canVratiUNenadgledaneCommand(object param)
        {
            return NadgledaniPuteviSelected != null;
        }

        #endregion

        #endregion

        #region INotifyPropertyChanged implementacija

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
