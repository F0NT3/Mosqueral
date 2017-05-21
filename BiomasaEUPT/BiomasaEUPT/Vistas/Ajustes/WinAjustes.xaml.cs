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
using System.Windows.Shapes;

namespace BiomasaEUPT.Vistas.Ajustes
{
    /// <summary>
    /// Lógica de interacción para WinAjustes.xaml
    /// </summary>
    public partial class WinAjustes : Window
    {
        public WinAjustes()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ucTabApariencia.cbVentanaMaximizada.IsChecked = Properties.Settings.Default.VentanaMaximizada;
        }
    }
}
