using PZ3_NetworkService.Models;
using PZ3_NetworkService.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PZ3_NetworkService.ViewModels
{
    public class PuteviViewModel: INotifyPropertyChanged
    {
        #region Constructor

        public PuteviViewModel()
        {
            Putevi = PuteviSvc.LoadPutevi();
            TipoviPuta = PuteviSvc.LoadTipoviPuta();
            TipoviPutaSelected = TipoviPuta.FirstOrDefault();
            ConfirmingMessageVisibility = Visibility.Collapsed;
        }

        #endregion

        #region Fields

        private ObservableCollection<PutModel> putevi;
        private PutModel puteviSelected;
        private int broj;
        private ObservableCollection<TipPuta> tipoviPuta;
        private TipPuta tipoviPutaSelected;
        private Visibility confirmingMessageVisibility;

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
                    PodesiVrednostiKontrola(value);
                }
            }
        }

        public int Broj
        {
            get { return broj; }
            set
            {
                if (broj != value)
                {
                    broj = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<TipPuta> TipoviPuta
        {
            get { return tipoviPuta; }
            set
            {
                if (tipoviPuta != value)
                {
                    tipoviPuta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public TipPuta TipoviPutaSelected
        {
            get { return tipoviPutaSelected; }
            set
            {
                if (tipoviPutaSelected != value)
                {
                    tipoviPutaSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Visibility ConfirmingMessageVisibility
        {
            get { return confirmingMessageVisibility; }
            set
            {
                if (confirmingMessageVisibility != value)
                {
                    confirmingMessageVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        private int NewId()
        {
            /* Nov Id je prvi slobodan integer.
             * Ako je lista prazna, uzimamo vrednost 1,
             * a ako nije, tražimo prvi slobodan integer */
            int newId = 1;
            if(Putevi.Count > 0)
            {
                var maxId = Putevi.Max(p => p.ID);
                for (int i = 1; i <= (maxId + 1); i++)
                {
                    if (!Putevi.Any(p => p.ID == i))
                    {
                        newId = i;
                        break;
                    }
                }
            }
            return newId;
        }

        private void PodesiVrednostiKontrola(PutModel put)
        {
            if (put == null)
            {
                Broj = 0;
                TipoviPutaSelected = TipoviPuta.FirstOrDefault();
            }
            else
            {
                Broj = put.Broj;
                TipoviPutaSelected = TipoviPuta.FirstOrDefault(tp => tp.ID == put.Tip.ID);
            }
        }

        private void Snimi()
        {
            PuteviSvc.SavePutevi(Putevi);
            Putevi = PuteviSvc.LoadPutevi();
        }

        private void Osvezi()
        {
            PuteviSelected = null;
            PodesiVrednostiKontrola(null);
        }

        #endregion

        #region Commands

        #region Dodaj

        public ICommand Dodaj { get { return new RelayCommand(dodaj, canDodaj); } }

        private void dodaj(object param)
        {
            Putevi.Add(new PutModel { ID = NewId(), Broj = Broj, Tip = TipoviPutaSelected });

            Snimi();
            Osvezi();
        }

        private bool canDodaj(object param)
        {
            return PuteviSelected == null
                && Broj > 0
                && !Putevi.Any(p => p.Broj == Broj)
                && TipoviPutaSelected != null;
        }

        #endregion

        #region Izmeni

        public ICommand Izmeni { get { return new RelayCommand(izmeni, canIzmeni); } }

        private void izmeni(object param)
        {
            PuteviSelected.Broj = Broj;
            PuteviSelected.Tip = TipoviPutaSelected;

            Snimi();
        }

        private bool canIzmeni(object param)
        {
            return PuteviSelected != null
                && Broj > 0
                && !Putevi.Any(p => p.ID != PuteviSelected.ID && p.Broj == Broj)
                && TipoviPutaSelected != null;
        }

        #endregion

        #region Izbrisi

        public ICommand Izbrisi { get { return new RelayCommand(izbrisi, canIzbrisi); } }

        private void izbrisi(object param)
        {
            ConfirmingMessageVisibility = Visibility.Visible;
        }

        private bool canIzbrisi(object param)
        {
            return PuteviSelected != null;
        }

        #endregion

        #region Odustani

        public ICommand Odustani { get { return new RelayCommand(odustani, canOdustani); } }

        private void odustani(object param)
        {
            Osvezi();
        }

        private bool canOdustani(object param)
        {
            return true;
        }

        #endregion

        #region BrisanjeOdustani

        public ICommand BrisanjeOdustani { get { return new RelayCommand(brisanjeOdustani, canBrisanjeOdustani); } }

        private void brisanjeOdustani(object param)
        {
            ConfirmingMessageVisibility = Visibility.Collapsed;
        }

        private bool canBrisanjeOdustani(object param)
        {
            return true;
        }

        #endregion

        #region BrisanjePotvrdi

        public ICommand BrisanjePotvrdi { get { return new RelayCommand(brisanjePotvrdi, canBrisanjePotvrdi); } }

        private void brisanjePotvrdi(object param)
        {
            Putevi.Remove(PuteviSelected);

            Snimi();
            Osvezi();

            ConfirmingMessageVisibility = Visibility.Collapsed;
        }

        private bool canBrisanjePotvrdi(object param)
        {
            return true;
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
