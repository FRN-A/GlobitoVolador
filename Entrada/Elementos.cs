﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Diagnostics;

using System.Windows.Threading;

namespace Entrada
{
    public class Elementos 
    {
        public Image Imagen { get; set; }
        public double Left { get; set; }
        public bool punto { get; set; }
        
        public Elementos(Image imagen, double left)
        {
            this.Imagen = imagen;
            Left = left;
            punto = true;
        }
    }
}
