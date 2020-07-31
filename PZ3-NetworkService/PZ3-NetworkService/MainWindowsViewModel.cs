using PZ3_NetworkService.Model;
using PZ3_NetworkService.PomocneKlasee;
using PZ3_NetworkService.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace PZ3_NetworkService
{
    public class MainWindowViewModel : BindableBase
    {
        public MyICommand<string> NavCommand { get; private set; }
        private TabelaReaktoraViewModel reaktorViewModel1;
        private NetworkView_ViewModel reaktorViewModel2;
        private DataChartViewModel reaktorViewModel3;
        private BindableBase currentViewModel;

        public static ObservableCollection<Reaktor> Reaktori_glavni { get; set; }

        

        string slika_osnova = @"C:\Users\user\Desktop\PZ3-NetworkService\Reaktori_Slike";


        private int count = 3; // Inicijalna vrednost broja objekata u sistemu
                               // ######### ZAMENITI stvarnim brojem elemenata

        public MainWindowViewModel()
        {
            Reaktori_glavni = new ObservableCollection<Reaktor>();



            reaktorViewModel1 = new TabelaReaktoraViewModel();

            reaktorViewModel2 = new NetworkView_ViewModel();

            reaktorViewModel3 = new DataChartViewModel();

            


            Reaktori_glavni.Add(new Reaktor { ID = 1, Name = "Reaktor 1", Tip = new Tip_Reaktora("Termalni", slika_osnova + @"\" + "Termalni" + @".jpg"), Temperatura = 0 });
            Reaktori_glavni.Add(new Reaktor { ID = 2, Name = "Reaktor 2", Tip = new Tip_Reaktora("Hibridni", slika_osnova + @"\" + "Hibridni" + @".jpg"), Temperatura = 0 });
            Reaktori_glavni.Add(new Reaktor { ID = 3, Name = "Reaktor 3", Tip = new Tip_Reaktora("Fuzioni", slika_osnova + @"\" + "Fuzioni" + @".jpg"), Temperatura = 0 });

            NavCommand = new MyICommand<string>(OnNav);
            CurrentViewModel = reaktorViewModel1;


            createListener(); //Povezivanje sa serverskom aplikacijom

        }

        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }

        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "reaktor1":
                    CurrentViewModel = reaktorViewModel1;
                    break;
                case "reaktor2":
                    CurrentViewModel = reaktorViewModel2;
                    break;
                case "reaktor3":
                    CurrentViewModel = reaktorViewModel3;
                    break;
            }
        }


        private void createListener()
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
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(MainWindowViewModel.Reaktori_glavni.Count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Objekat_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji

                            string[] podeljen1 = incomming.Split('_');

                            string[] podeljen2 = podeljen1[1].Split(':');

                            string[] podeljen3;

                            foreach (Reaktor r in MainWindowViewModel.Reaktori_glavni)
                            {
                                podeljen3 = r.Name.Split(' ');

                                if (podeljen3[1] == Convert.ToString(Convert.ToInt32(podeljen2[0]) + 1))
                                {
                                    foreach(Reaktor r2 in NetworkView_ViewModel.canvas_oslobodi.Values)
                                    {
                                     //   if (String.Equals(r.Name,r2.Name))
                                        if (r == r2)
                                        {
                                            r2.Temperatura = Convert.ToInt32(podeljen2[1]);






                                            foreach (Canvas canvas in NetworkView_ViewModel.canvas_oslobodi.Keys)
                                            {

                                                if (NetworkView_ViewModel.canvas_oslobodi[canvas] == r2)
                                                {

                                                    Application.Current.Dispatcher.Invoke(new Action(() => 
                                                    {


                                                        if (canvas.Resources["taken"] != null)
                                                        {



                                                            if (250 > NetworkView_ViewModel.canvas_oslobodi[canvas].Temperatura || NetworkView_ViewModel.canvas_oslobodi[canvas].Temperatura > 350)
                                                            {
                                                                ((TextBlock)(canvas).Children[0]).Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));

                                                                ((TextBlock)(canvas).Children[0]).FontSize = 14;

                                                                ((TextBlock)(canvas).Children[0]).FontWeight = FontWeights.Bold;

                                                                ((TextBlock)canvas.Children[0]).Text = NetworkView_ViewModel.canvas_oslobodi[canvas].Name + ": " +  NetworkView_ViewModel.canvas_oslobodi[canvas].Temperatura.ToString();

                                                               

                                                            }
                                                            else
                                                            {
                                                                ((TextBlock)(canvas).Children[0]).Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("Green"));

                                                                ((TextBlock)(canvas).Children[0]).FontSize = 14;

                                                                ((TextBlock)(canvas).Children[0]).FontWeight = FontWeights.Bold;

                                                                ((TextBlock)canvas.Children[0]).Text = NetworkView_ViewModel.canvas_oslobodi[canvas].Name + ": " + NetworkView_ViewModel.canvas_oslobodi[canvas].Temperatura.ToString();


                                                            }



                                                        }
                                                       

                                                    }), DispatcherPriority.ContextIdle);

                                                }

                                            }





                                        }
                                    }

                                    r.Temperatura = Convert.ToInt32(podeljen2[1]);

                                    Log.Make_Log_File(r);

                                    foreach (Reaktor r3 in TabelaReaktoraViewModel.SearchReaktori_GlavniPrikaz)
                                    {
                                        if (r == r3)
                                            r3.Temperatura = r.Temperatura;
                                    }

                                    foreach (Reaktor r4 in DataChartViewModel.Reaktori3)
                                    {
                                        if (r == r4)
                                            r4.Temperatura = r.Temperatura;
                                    }

                                }
                            }

                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

    }
}
