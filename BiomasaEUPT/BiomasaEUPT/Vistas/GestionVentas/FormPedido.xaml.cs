﻿using BiomasaEUPT.Modelos;
using BiomasaEUPT.Modelos.Tablas;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Globalization;
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

namespace BiomasaEUPT.Vistas.GestionVentas
{
    /// <summary>
    /// Lógica de interacción para FormPedido.xaml
    /// </summary>
    public partial class FormPedido : UserControl
    {

        private CollectionViewSource pedidosViewSource;
        private CollectionViewSource estadosPedidosViewSource;
        private CollectionViewSource clientesViewSource;

        private FormPedidoViewModel viewModel;

        public DateTime FechaPedido { get; set; }
        public DateTime HoraPedido { get; set; }
        private BiomasaEUPTContext context;

        public FormPedido(BiomasaEUPTContext context)
        {
            InitializeComponent();
            viewModel = new FormPedidoViewModel();
            DataContext = viewModel;
            this.context = context;
            FechaPedido = DateTime.Now;
            HoraPedido = DateTime.Now;

        }

        public FormPedido(BiomasaEUPTContext context, string _titulo) : this(context)
        {
            gbTitulo.Header = _titulo;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            pedidosViewSource = ((CollectionViewSource)(FindResource("pedidosViewSource")));
            estadosPedidosViewSource = ((CollectionViewSource)(FindResource("estadosPedidosViewSource")));
            clientesViewSource = ((CollectionViewSource)(FindResource("clientesViewSource")));


            context.PedidosCabeceras.Load();
            context.EstadosPedidos.Load();
            context.Clientes.Load();

            pedidosViewSource.Source = context.PedidosCabeceras.Local;
            estadosPedidosViewSource.Source = context.EstadosPedidos.Local;
            clientesViewSource.Source = context.Clientes.Local;

            dpFechaPedido.Language = System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name);
        }

        private object GetDataFromListBox(ListBox source, Point point)
        {
            UIElement element = source.InputHitTest(point) as UIElement;
            if (element != null)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);

                    if (data == DependencyProperty.UnsetValue)
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    }

                    if (element == source)
                    {
                        return null;
                    }
                }

                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
            }

            return null;
        }

    }
}
