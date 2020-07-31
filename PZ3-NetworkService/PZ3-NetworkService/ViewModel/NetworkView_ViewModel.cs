using PZ3_NetworkService.Model;
using PZ3_NetworkService.PomocneKlasee;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PZ3_NetworkService.ViewModel
{
    public class NetworkView_ViewModel : BindableBase
    {
        public static ObservableCollection<Reaktor> Reaktori2 { get; set; }

        private Reaktor selectedReaktor;

        public MyICommand<ListView> SelectionChanged { get; set; }

        public MyICommand<Canvas> drop { get; set; }

        public MyICommand<Canvas> dragOver { get; set; }

        public MyICommand<Canvas> oslobodi { get; set; }

        public static Dictionary<Canvas, Reaktor> canvas_oslobodi { get; set; }


        public MyICommand<Canvas> ZauzmiSve { get; set; }

        public MyICommand<Canvas> OslobodiSve { get; set; }

        public NetworkView_ViewModel()
        {


           Reaktori2 = new ObservableCollection<Reaktor>();

           canvas_oslobodi = new Dictionary<Canvas, Reaktor>();

            foreach (Reaktor r in MainWindowViewModel.Reaktori_glavni)
            {


                        Reaktori2.Add(r);
                    
                

              
            }

           /* foreach (Canvas canvas in MainWindowViewModel.canvas_oslobodi.Keys)
            {

                
                

               // canvas.Resources.Remove("taken");

                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri(MainWindowViewModel.canvas_oslobodi[canvas].Tip.Slika);
                logo.EndInit();

                canvas.Background = new ImageBrush(logo);

                ((TextBlock)(canvas).Children[0]).Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDAFF00"));
                ((TextBlock)(canvas).Children[0]).Text = "Mesto zauzeto";

                //canvas.Resources.Add("taken", true);

                // (canvas).Resources.Add("taken", true);

            }*/




            SelectionChanged = new MyICommand<ListView>(OnDrag, Ondelete);

            drop = new MyICommand<Canvas>(Ondrop);

            dragOver = new MyICommand<Canvas>(OndragOver);

            oslobodi = new MyICommand<Canvas>(Onoslobodi);

            ZauzmiSve = new MyICommand<Canvas>(OnZauzmiSve, CanZauzmiSve);

            //OslobodiSve = new MyICommand<Canvas>(OnOslobodiSve, CanOslobodiSve);

            OslobodiSve = new MyICommand<Canvas>(OnOslobodiSve);

        }




        public Reaktor SelectedReaktor
        {
            get { return selectedReaktor; }

            set
            {

                selectedReaktor = value;



            }
        }




        private void OnZauzmiSve(Canvas mainCanvas)
        {
            
            foreach(Reaktor r in Reaktori2.ToList())
            {

               

                    for (int j = 0; j < 4; j++)
                    {


                            for (int k = 0; k < 4; k++)
                            {



                                // ((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k])

                                    if (Reaktori2.Contains(r))
                                    {

                                            if (((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k]).Resources["taken"] == null)
                                            {
                                                BitmapImage logo = new BitmapImage();

                                                logo.BeginInit();

                                                logo.UriSource = new Uri(r.Tip.Slika);

                                                logo.EndInit();

                                                ((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k]).Background = new ImageBrush(logo);

                                                ((TextBlock)((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k]).Children[0]).Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDAFF00"));

                                                ((TextBlock)((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k]).Children[0]).Text = r.Name;

                                                ((TextBlock)((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k]).Children[0]).FontWeight = FontWeights.Bold;

                                                ((TextBlock)((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k]).Children[0]).FontSize = 15;

                                                ((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k]).Resources.Add("taken", true);


                                                canvas_oslobodi.Add(((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k]), r);

                                                Reaktori2.Remove(r);
                                            }
                                    }
                        
                            }

                    }
                    
                
            }
        }



        private bool CanZauzmiSve(Canvas mainCanvas)
        {
            return Reaktori2.Count != 0;
        }



        private void OnOslobodiSve(Canvas mainCanvas)
        {

            for (int j = 0; j < 4; j++)
            {


                for (int k = 0; k < 4; k++)
                {



                    // ((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k])



                    if (((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k]).Resources["taken"] != null)
                        {

                            Reaktori2.Add(canvas_oslobodi[((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k])]);


                            canvas_oslobodi.Remove(((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k]));


                            ((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k]).Background = Brushes.GhostWhite;

                            ((TextBlock)((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k]).Children[0]).Foreground = Brushes.Black;

                            ((TextBlock)((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k]).Children[0]).Text = "Slobodno mesto";

                            ((TextBlock)((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k]).Children[0]).FontWeight = FontWeights.Normal;

                            ((TextBlock)((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k]).Children[0]).FontSize = 14;
                         

                             ((Canvas)((StackPanel)(((StackPanel)mainCanvas.Children[0]).Children[j])).Children[k]).Resources.Remove("taken");

                       
                        }
                    

                }

            }

            MessageBox.Show("Svi Reaktori su oslobodjeni", "", MessageBoxButton.OK, MessageBoxImage.Information);

        }

     


        private void OnDrag(ListView listView)
        {
            

            DragDrop.DoDragDrop(listView, selectedReaktor, DragDropEffects.Copy | DragDropEffects.Move);

           // Reaktori2.Remove(selectedReaktor);


        }

        private bool Ondelete(ListView listView)
        {


            return selectedReaktor != null;

        }




        private void OndragOver(Canvas canvas)
        {

            if (canvas.Resources["taken"] != null)
            {
                canvas.AllowDrop = false;
            }
            else
            {
                canvas.AllowDrop = true;
            }
           
        }



        private void Ondrop(Canvas canvas)
        {

                if (selectedReaktor != null)
                {
                    if ((canvas).Resources["taken"] == null)
                    {



                        BitmapImage logo = new BitmapImage();
                        logo.BeginInit();
                        logo.UriSource = new Uri(selectedReaktor.Tip.Slika);
                        logo.EndInit();

                        (canvas).Background = new ImageBrush(logo);

                        ((TextBlock)(canvas).Children[0]).Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDAFF00"));
                        ((TextBlock)(canvas).Children[0]).Text = selectedReaktor.Name;

                    ((TextBlock)(canvas).Children[0]).FontWeight = FontWeights.Bold;

                    ((TextBlock)(canvas).Children[0]).FontSize = 15;

                    (canvas).Resources.Add("taken", true);


                        canvas_oslobodi.Add(canvas, selectedReaktor);
                        Reaktori2.Remove(selectedReaktor);

                    }
                }

                

           // canvas_oslobodi.Add(canvas, selectedReaktor);

            //Reaktori2.Remove(selectedReaktor);

        }


         private void Onoslobodi(Canvas canvas)
         {
             if (canvas.Resources["taken"] != null)
             {
                 Reaktori2.Add(canvas_oslobodi[canvas]);

                 canvas_oslobodi.Remove(canvas);

                 canvas.Background = Brushes.GhostWhite;

                 ((TextBlock)canvas.Children[0]).Foreground = Brushes.Black;

                 ((TextBlock)canvas.Children[0]).Text = "Slobodno mesto";

               

                ((TextBlock)(canvas).Children[0]).FontSize = 12;

                ((TextBlock)(canvas).Children[0]).FontWeight = FontWeights.Normal;

               

                canvas.Resources.Remove("taken");
             }
         }



       



    }
}
