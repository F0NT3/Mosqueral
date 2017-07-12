﻿using BiomasaEUPT.Clases;
using BiomasaEUPT.Domain;
using BiomasaEUPT.Modelos;
using BiomasaEUPT.Modelos.Tablas;
using BiomasaEUPT.Vistas.ControlesUsuario;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace BiomasaEUPT.Vistas.GestionClientes
{
    public class TabClientesViewModel : ViewModelBase
    {
        public ObservableCollection<Cliente> Clientes { get; set; }
        public CollectionView ClientesView { get; private set; }
        public ObservableCollection<TipoCliente> TiposClientes { get; set; }
        public ObservableCollection<GrupoCliente> GruposClientes { get; set; }
        public IList<Cliente> ClientesSeleccionados { get; set; }
        public Cliente ClienteSeleccionado { get; set; }
        public FiltroTablaViewModel FiltroTablaViewModel { get; set; }
        public bool ObservacionesEnEdicion { get; set; }

        private ICommand _anadirComando;
        private ICommand _modificarComando;
        private ICommand _borrarComando;
        private ICommand _dgClientes_CellEditEndingComando;
        private ICommand _dgClientes_RowEditEndingComando;
        private ICommand _modificarObservacionesComando;


        public TabClientesViewModel()
        {
            FiltroTablaViewModel = new FiltroTablaViewModel()
            {
                ViewModel = this
            };
        }


        public override void Inicializar()
        {
            CargarClientes();
            FiltroTablaViewModel.CargarFiltro();
        }

        public void CargarClientes()
        {
            using (new CursorEspera())
            {
                using (var context = new BiomasaEUPTContext())
                {
                    Clientes = new ObservableCollection<Cliente>(context.Clientes.Include(c => c.Municipio.Provincia.Comunidad.Pais).ToList());
                    ClientesView = (CollectionView)CollectionViewSource.GetDefaultView(Clientes);
                    // ClientesView.Filter = OnFilterMovie;
                    TiposClientes = new ObservableCollection<TipoCliente>(context.TiposClientes.ToList());
                    GruposClientes = new ObservableCollection<GrupoCliente>(context.GruposClientes.ToList());
                    //(ucContador as Contador).Actualizar();

                    // Por defecto no está seleccionada ninguna fila del datagrid clientes 
                    ClienteSeleccionado = null;
                }
            }
        }


        // Asigna el valor de ClientesSeleccionados ya que no se puede crear un Binding de SelectedItems desde el XAML
        public ICommand DGClientes_SelectionChangedComando => new RelayCommand2<IList<object>>(param => ClientesSeleccionados = param.Cast<Cliente>().ToList());

        #region Editar Celda
        public ICommand DGClientes_CellEditEndingComando => _dgClientes_CellEditEndingComando ??
            (_dgClientes_CellEditEndingComando = new RelayCommand2<DataGridCellEditEndingEventArgs>(
                 param => EditarCeldaCliente(param)
            ));

        private void EditarCeldaCliente(DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var clienteSeleccionado = e.Row.DataContext as Cliente;
                using (var context = new BiomasaEUPTContext())
                {
                    var cliente = context.Clientes.Single(u => u.ClienteId == clienteSeleccionado.ClienteId);

                    cliente.RazonSocial = clienteSeleccionado.RazonSocial;
                    cliente.Nif = clienteSeleccionado.Nif;
                    cliente.Email = clienteSeleccionado.Email;
                    cliente.TipoId = clienteSeleccionado.TipoCliente.TipoClienteId;
                    cliente.GrupoId = clienteSeleccionado.GrupoCliente.GrupoClienteId;
                    cliente.Calle = clienteSeleccionado.Calle;
                    cliente.MunicipioId = clienteSeleccionado.Municipio.MunicipioId;
                    // cliente.Observaciones = clienteSeleccionado.Observaciones;
                    Console.WriteLine(cliente.MunicipioId);
                    context.SaveChanges();
                }

                if (e.Column.DisplayIndex == 3) // 3 = Posición tipo cliente
                {
                    //(ucContador as Contador).Actualizar();
                }
            }
        }
        #endregion

        #region Editar Fila
        public ICommand DGClientes_RowEditEndingComando => _dgClientes_RowEditEndingComando ??
            (_dgClientes_RowEditEndingComando = new RelayCommand2<DataGridRowEditEndingEventArgs>(
                 param => EditarFilaCliente(param)
            ));

        private void EditarFilaCliente(DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var clienteSeleccionado = e.Row.DataContext as Cliente;
                using (var context = new BiomasaEUPTContext())
                {
                    var cliente = context.Clientes.Single(u => u.ClienteId == clienteSeleccionado.ClienteId);

                    cliente.RazonSocial = clienteSeleccionado.RazonSocial;
                    cliente.Nif = clienteSeleccionado.Nif;
                    cliente.Email = clienteSeleccionado.Email;
                    cliente.TipoId = clienteSeleccionado.TipoCliente.TipoClienteId;
                    cliente.GrupoId = clienteSeleccionado.GrupoCliente.GrupoClienteId;
                    cliente.Calle = clienteSeleccionado.Calle;
                    cliente.MunicipioId = clienteSeleccionado.Municipio.MunicipioId;
                    // cliente.Observaciones = clienteSeleccionado.Observaciones;
                    Console.WriteLine(cliente.MunicipioId);
                    context.SaveChanges();
                }

                /* if (e.Column.DisplayIndex == 3) // 3 = Posición tipo cliente
                 {
                     //(ucContador as Contador).Actualizar();
                 }*/
            }
        }
        #endregion      

        #region Añadir
        public ICommand AnadirComando => _anadirComando ??
            (_anadirComando = new RelayComando(
                param => AnadirCliente()
            ));

        private async void AnadirCliente()
        {
            var formCliente = new FormCliente();

            if ((bool)await DialogHost.Show(formCliente, "RootDialog"))
            {
                using (var context = new BiomasaEUPTContext())
                {
                    context.Clientes.Add(new Cliente()
                    {
                        RazonSocial = formCliente.RazonSocial,
                        Nif = formCliente.Nif,
                        Email = formCliente.Email,
                        Calle = formCliente.Calle,
                        TipoId = (formCliente.cbTiposClientes.SelectedItem as TipoCliente).TipoClienteId,
                        GrupoId = (formCliente.cbGruposClientes.SelectedItem as GrupoCliente).GrupoClienteId,
                        MunicipioId = (formCliente.cbMunicipios.SelectedItem as Municipio).MunicipioId,
                        Observaciones = formCliente.Observaciones
                    });
                    context.SaveChanges();
                }
                //CargarClientes();
            }
        }
        #endregion

        #region Borrar     
        public ICommand BorrarComando => _borrarComando ??
            (_borrarComando = new RelayCommand2<IList<object>>(
                param => BorrarCliente(),
                param => ClienteSeleccionado != null
            ));

        private async void BorrarCliente()
        {
            string pregunta = ClientesSeleccionados.Count == 1
               ? "¿Está seguro de que desea borrar al cliente " + ClienteSeleccionado.RazonSocial + "?"
               : "¿Está seguro de que desea borrar los clientes seleccionados?";

            var resultado = (bool)await DialogHost.Show(new MensajeConfirmacion(pregunta), "RootDialog");

            if (resultado)
            {
                var clientesABorrar = new List<Cliente>();
                // var clientesSeleccionados = ucTablaClientes.dgClientes.SelectedItems.Cast<Cliente>().ToList();

                using (var context = new BiomasaEUPTContext())
                {
                    foreach (var cliente in ClientesSeleccionados)
                    {
                        if (!context.PedidosCabeceras.Any(pc => pc.ClienteId == cliente.ClienteId))
                        {
                            clientesABorrar.Add(cliente);
                        }
                    }
                    context.Clientes.RemoveRange(clientesABorrar);
                    context.SaveChanges();
                    CargarClientes();
                }
                if (ClientesSeleccionados.Count != clientesABorrar.Count)
                {
                    string mensaje = ClientesSeleccionados.Count == 1
                           ? "No se ha podido borrar el cliente seleccionado."
                           : "No se han podido borrar todos los clientes seleccionados.";
                    mensaje += "\n\nAsegurese de no que no exista ningún pedido asociado a dicho cliente.";
                    await DialogHost.Show(new MensajeInformacion(mensaje) { Width = 380 }, "RootDialog");
                }
            }
        }
        #endregion

        #region Editar
        public ICommand ModificarComando => _modificarComando ??
            (_modificarComando = new RelayComando(
                param => ModificarCliente(),
                param => ClienteSeleccionado != null
             ));

        private async void ModificarCliente()
        {
            var formCliente = new FormCliente(ClienteSeleccionado);
            if ((bool)await DialogHost.Show(formCliente, "RootDialog"))
            {
                ClienteSeleccionado.RazonSocial = formCliente.RazonSocial;
                ClienteSeleccionado.Nif = formCliente.Nif;
                ClienteSeleccionado.Email = formCliente.Email;
                ClienteSeleccionado.TipoId = (formCliente.cbTiposClientes.SelectedItem as TipoCliente).TipoClienteId;
                ClienteSeleccionado.GrupoId = (formCliente.cbGruposClientes.SelectedItem as GrupoCliente).GrupoClienteId;
                ClienteSeleccionado.MunicipioId = (formCliente.cbMunicipios.SelectedItem as Municipio).MunicipioId;
                ClienteSeleccionado.Calle = formCliente.Calle;
                ClienteSeleccionado.Observaciones = formCliente.Observaciones;
                using (var context = new BiomasaEUPTContext())
                {
                    context.SaveChanges();
                }
            }
        }
        #endregion

        #region Modificar Observaciones Cliente
        public ICommand ModificarObservacionesComando => _modificarObservacionesComando ??
            (_modificarObservacionesComando = new RelayComando(
                param => ModificarObservacionesCliente(),
                param => ClienteSeleccionado != null
             ));

        private void ModificarObservacionesCliente()
        {
            Console.WriteLine("aaaaaaaaaaaaaaaaaaaaa");
            using (var context = new BiomasaEUPTContext())
            {
                var cliente = context.Clientes.Single(u => u.ClienteId == ClienteSeleccionado.ClienteId);

                cliente.Observaciones = ClienteSeleccionado.Observaciones;
                context.SaveChanges();
            }
            ObservacionesEnEdicion = false;
        }
        #endregion
    }
}
