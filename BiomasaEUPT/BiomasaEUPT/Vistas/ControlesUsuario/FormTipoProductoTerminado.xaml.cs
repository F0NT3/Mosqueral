﻿using BiomasaEUPT.Clases;
using BiomasaEUPT.Vistas.GestionClientes;
using System;
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

namespace BiomasaEUPT.Vistas.ControlesUsuario
{
    /// <summary>
    /// Lógica de interacción para FormTipoProductoTerminado.xaml
    /// </summary>
    public partial class FormTipoProductoTerminado : UserControl
    {
        private string _nombre;
        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; }
        }

        private string _tamano;
        public string Tamano
        {
            get { return _tamano; }
            set { _tamano = value; }
        }

        private double _humedad;
        public double Humedad
        {
            get { return _humedad; }
            set { _humedad = value; }
        }

        public FormTipoProductoTerminado()
        {
            InitializeComponent();
            DataContext = this;
        }


        public FormTipoProductoTerminado(string _titulo) : this()
        {
            gbTitulo.Header = _titulo;
        }

    }
}

