using PZ3_NetworkService.PomocneKlasee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PZ3_NetworkService.Model
{
    public class CrtajModel : GraphObjectModel
    {
        public string Text { get; set; }
        public Thickness Margin { get; set; }
        public double Width { get; set; }
        public TextAlignment TextAlignment { get; set; }
    }
}
