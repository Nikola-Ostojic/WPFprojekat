﻿using PZ3_NetworkService.PomocneKlasee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PZ3_NetworkService.Model
{
    public class Line : GraphObjectModel
    {
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
        public double StrokeThickness { get; set; }
        public Brush Stroke { get; set; }

    }
}
