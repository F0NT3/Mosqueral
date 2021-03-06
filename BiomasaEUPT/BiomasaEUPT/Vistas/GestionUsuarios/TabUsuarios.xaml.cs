﻿using BiomasaEUPT.Clases;
using BiomasaEUPT.Domain;
using BiomasaEUPT.Modelos;
using BiomasaEUPT.Modelos.Tablas;
using BiomasaEUPT.Vistas.ControlesUsuario;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
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
using System.Windows.Threading;

namespace BiomasaEUPT.Vistas.GestionUsuarios
{
    /// <summary>
    /// Lógica de interacción para TabUsuarios.xaml
    /// </summary>
    public partial class TabUsuarios : UserControl
    {
        public TabUsuarios()
        {
            InitializeComponent();

            /*   Style style = new Style(typeof(CheckBox));
               style.Setters.Add(new EventSetter(CheckBox.CheckedEvent, new RoutedEventHandler(BaneadoColumna_Checked)));
               style.Setters.Add(new EventSetter(CheckBox.UncheckedEvent, new RoutedEventHandler(BaneadoColumna_Checked)));
               ucTablaUsuarios.baneadoColumna.CellStyle = style;*/
            IsVisibleChanged += new DependencyPropertyChangedEventHandler(MyControl_IsVisibleChanged);
        }

        private void MyControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Se añade el foco del teclado al UserControl para que funcionen los atajos de teclado
            if (!(bool)(e.NewValue))
                return;
            Focusable = true;
            Keyboard.Focus(this);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Focusable = true;
            // Keyboard.Focus(this);
            // Focus();
        }
    }
}
