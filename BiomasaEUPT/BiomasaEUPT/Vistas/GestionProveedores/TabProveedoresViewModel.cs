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

namespace BiomasaEUPT.Vistas.GestionProveedores
{
    public class TabProveedoresViewModel : ViewModelBase
    {
        public ObservableCollection<Proveedor> Proveedores { get; set; }
        public CollectionView ProveedoresView { get; private set; }
        public ObservableCollection<TipoProveedor> TiposProveedores { get; set; }
        public IList<Proveedor> ProveedoresSeleccionados { get; set; }
        public Proveedor ProveedorSeleccionado { get; set; }
        public bool ObservacionesEnEdicion { get; set; }
        public FiltroViewModel<TipoProveedor> FiltroTiposViewModel { get; set; }
        public ContadorViewModel<TipoProveedor> ContadorViewModel { get; set; }

        // Checkbox Filtro Proveedores
        public bool RazonSocialSeleccionada { get; set; } = true;
        public bool NifSeleccionado { get; set; } = true;
        public bool EmailSeleccionado { get; set; } = false;
        public bool CalleSeleccionada { get; set; } = false;
        public bool CodigoPostalSeleccionado { get; set; } = false;
        public bool MunicipioSeleccionado { get; set; } = false;

        private string _textoFiltroProveedores = "";
        public string TextoFiltroProveedores
        {
            get { return _textoFiltroProveedores; }
            set
            {
                _textoFiltroProveedores = value.ToLower();
                FiltrarProveedores();
            }
        }

        private ICommand _anadirProveedorComando;
        private ICommand _modificarProveedorComando;
        private ICommand _borrarProveedorComando;
        private ICommand _refrescarProveedoresComando;
        private ICommand _filtrarProveedoresComando;
        private ICommand _dgProveedores_CellEditEndingComando;
        private ICommand _dgProveedores_RowEditEndingComando;
        private ICommand _modificarObservacionesProveedorComando;

        private ICommand _anadirTipoComando;
        private ICommand _modificarTipoComando;
        private ICommand _borrarTipoComando;

        public BiomasaEUPTContext Context { get; set; }


        public TabProveedoresViewModel()
        {
            FiltroTiposViewModel = new FiltroViewModel<TipoProveedor>()
            {
                FiltrarItems = FiltrarProveedores,
                AnadirComando = AnadirTipoComando,
                ModificarComando = ModificarTipoComando,
                BorrarComando = BorrarTipoComando
            };
            ContadorViewModel = new ContadorViewModel<TipoProveedor>();
        }

        public override void Inicializar()
        {
            Context = new BiomasaEUPTContext();
            CargarProveedores();
        }

        public void CargarProveedores()
        {
            using (new CursorEspera())
            {
                Proveedores = new ObservableCollection<Proveedor>(Context.Proveedores.Include(p => p.Municipio.Provincia.Comunidad.Pais).ToList());
                ProveedoresView = (CollectionView)CollectionViewSource.GetDefaultView(Proveedores);
                TiposProveedores = new ObservableCollection<TipoProveedor>(Context.TiposProveedores.ToList());
                ContadorViewModel.Tipos = TiposProveedores;
                FiltroTiposViewModel.Items = TiposProveedores;

                // Por defecto no está seleccionada ninguna fila del datagrid proveedores
                ProveedorSeleccionado = null;
            }
        }

        // Asigna el valor de ProveedoresSeleccionados ya que no se puede crear un Binding de SelectedItems desde el XAML
        public ICommand DGProveedores_SelectionChangedComando => new RelayCommandGenerico<IList<object>>(param => ProveedoresSeleccionados = param.Cast<Proveedor>().ToList());


        #region Editar Celda
        public ICommand DGProveedores_CellEditEndingComando => _dgProveedores_CellEditEndingComando ??
            (_dgProveedores_CellEditEndingComando = new RelayCommandGenerico<DataGridCellEditEndingEventArgs>(
                 param => EditarCeldaProveedor(param)
            ));

        private void EditarCeldaProveedor(DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var proveedorSeleccionado = e.Row.DataContext as Proveedor;

                /*var proveedor = context.Proveedores.Single(u => u.ProveedorId == proveedorSeleccionado.ProveedorId);

                proveedor.RazonSocial = proveedorSeleccionado.RazonSocial;
                proveedor.Nif = proveedorSeleccionado.Nif;
                proveedor.Email = proveedorSeleccionado.Email;
                proveedor.TipoId = proveedorSeleccionado.TipoProveedor.TipoProveedorId;
                proveedor.Calle = proveedorSeleccionado.Calle;
                proveedor.MunicipioId = proveedorSeleccionado.Municipio.MunicipioId;
                // Proveedor.Observaciones = ProveedorSeleccionado.Observaciones;*/
                Context.SaveChanges();

                if (e.Column.DisplayIndex == 3) // 3 = Posición tipo proveedor
                {
                    ContadorViewModel.Tipos = new ObservableCollection<TipoProveedor>(Context.TiposProveedores.ToList());
                }
            }
        }
        #endregion


        #region Editar Fila
        public ICommand DGProveedores_RowEditEndingComando => _dgProveedores_RowEditEndingComando ??
            (_dgProveedores_RowEditEndingComando = new RelayCommandGenerico<DataGridRowEditEndingEventArgs>(
                 param => EditarFilaProveedor(param)
            ));

        private void EditarFilaProveedor(DataGridRowEditEndingEventArgs e)
        {
            /*if (e.EditAction == DataGridEditAction.Commit)
            {
                var proveedorSeleccionado = e.Row.DataContext as Proveedor;
                using (var context = new BiomasaEUPTContext())
                {
                    var proveedor = context.Proveedores.Single(u => u.ProveedorId == proveedorSeleccionado.ProveedorId);

                    proveedor.RazonSocial = proveedorSeleccionado.RazonSocial;
                    proveedor.Nif = proveedorSeleccionado.Nif;
                    proveedor.Email = proveedorSeleccionado.Email;
                    proveedor.TipoId = proveedorSeleccionado.TipoProveedor.TipoProveedorId;
                    proveedor.Calle = proveedorSeleccionado.Calle;
                    proveedor.MunicipioId = proveedorSeleccionado.Municipio.MunicipioId;
                    Console.WriteLine(proveedor.MunicipioId);
                    context.SaveChanges();
                }

                 if (e.Column.DisplayIndex == 3) // 3 = Posición tipo proveedor
                 {
                     //(ucContador as Contador).Actualizar();
                 }
            }*/
        }
        #endregion


        #region Añadir Proveedor
        public ICommand AnadirProveedorComando => _anadirProveedorComando ??
            (_anadirProveedorComando = new RelayCommand(
                param => AnadirProveedor()
            ));

        private async void AnadirProveedor()
        {
            var formProveedor = new FormProveedor();

            if ((bool)await DialogHost.Show(formProveedor, "RootDialog"))
            {
                var formProveedorViewModel = formProveedor.DataContext as FormProveedorViewModel;

                Context.Proveedores.Add(new Proveedor()
                {
                    RazonSocial = formProveedorViewModel.RazonSocial,
                    Nif = formProveedorViewModel.Nif,
                    Email = formProveedorViewModel.Email,
                    Calle = formProveedorViewModel.Calle,
                    TipoId = formProveedorViewModel.TipoProveedorSeleccionado.TipoProveedorId,
                    MunicipioId = formProveedorViewModel.MunicipioSeleccionado.MunicipioId,
                    Observaciones = formProveedorViewModel.Observaciones
                });
                Context.SaveChanges();
                CargarProveedores();
            }
        }
        #endregion


        #region Borrar Proveedor
        public ICommand BorrarProveedorComando => _borrarProveedorComando ??
            (_borrarProveedorComando = new RelayCommandGenerico<IList<object>>(
                param => BorrarProveedor(),
                param => ProveedorSeleccionado != null
            ));

        private async void BorrarProveedor()
        {
            string pregunta = ProveedoresSeleccionados.Count == 1
               ? "¿Está seguro de que desea borrar al proveedor " + ProveedorSeleccionado.RazonSocial + "?"
               : "¿Está seguro de que desea borrar los proveedores seleccionados?";

            var resultado = (bool)await DialogHost.Show(new MensajeConfirmacion(pregunta), "RootDialog");

            if (resultado)
            {
                var proveedoresABorrar = new List<Proveedor>();

                foreach (var proveedor in ProveedoresSeleccionados)
                {
                    if (!Context.Recepciones.Any(pc => pc.ProveedorId == proveedor.ProveedorId))
                    {
                        proveedoresABorrar.Add(proveedor);
                    }
                }
                Context.Proveedores.RemoveRange(proveedoresABorrar);
                Context.SaveChanges();
                CargarProveedores();

                if (ProveedoresSeleccionados.Count != proveedoresABorrar.Count)
                {
                    string mensaje = ProveedoresSeleccionados.Count == 1
                           ? "No se ha podido borrar el proveedor seleccionado."
                           : "No se han podido borrar todos los proveedores seleccionados.";
                    mensaje += "\n\nAsegurese de no que no exista ninguna recepción asociada a dicho proveedor.";
                    await DialogHost.Show(new MensajeInformacion(mensaje) { Width = 380 }, "RootDialog");
                }
            }
        }
        #endregion


        #region Modificar Proveedor
        public ICommand ModificarProveedorComando => _modificarProveedorComando ??
            (_modificarProveedorComando = new RelayCommand(
                param => ModificarProveedor(),
                param => ProveedorSeleccionado != null
             ));

        private async void ModificarProveedor()
        {
            var formProveedor = new FormProveedor(ProveedorSeleccionado);
            if ((bool)await DialogHost.Show(formProveedor, "RootDialog"))
            {
                var formProveedorViewModel = formProveedor.DataContext as FormProveedorViewModel;

                ProveedorSeleccionado.RazonSocial = formProveedorViewModel.RazonSocial;
                ProveedorSeleccionado.Nif = formProveedorViewModel.Nif;
                ProveedorSeleccionado.Email = formProveedorViewModel.Email;
                ProveedorSeleccionado.TipoId = formProveedorViewModel.TipoProveedorSeleccionado.TipoProveedorId;
                ProveedorSeleccionado.MunicipioId = formProveedorViewModel.MunicipioSeleccionado.MunicipioId;
                ProveedorSeleccionado.Calle = formProveedorViewModel.Calle;
                ProveedorSeleccionado.Observaciones = formProveedorViewModel.Observaciones;

                Context.SaveChanges();
                CargarProveedores();
            }
        }
        #endregion


        #region Refrescar Proveedores
        public ICommand RefrescarProveedoresComando => _refrescarProveedoresComando ??
            (_refrescarProveedoresComando = new RelayCommand(
                param => RefrescarProveedores()
             ));

        private void RefrescarProveedores()
        {
            // Hay que volver a instanciar un nuevo context ya que sino no se pueden refrescar los datos
            // debido a que se guardardan en una cache
            Context.Dispose();
            Context = new BiomasaEUPTContext();
            CargarProveedores();
        }
        #endregion


        #region Modificar Observaciones Proveedor
        public ICommand ModificarObservacionesProveedorComando => _modificarObservacionesProveedorComando ??
            (_modificarObservacionesProveedorComando = new RelayCommand(
                param => ModificarObservacionesProveedor(),
                param => ProveedorSeleccionado != null
             ));

        private void ModificarObservacionesProveedor()
        {
            /*var proveedor = context.Proveedores.Single(u => u.ProveedorId == ProveedorSeleccionado.ProveedorId);

            proveedor.Observaciones = ProveedorSeleccionado.Observaciones;*/
            Context.SaveChanges();

            ObservacionesEnEdicion = false;
        }
        #endregion


        #region Filtro Proveedores
        public ICommand FiltrarProveedoresComando => _filtrarProveedoresComando ??
           (_filtrarProveedoresComando = new RelayCommand(
                param => FiltrarProveedores()
           ));

        public void FiltrarProveedores()
        {
            ProveedoresView.Filter = FiltroProveedores;
            ProveedoresView.Refresh();
        }

        private bool FiltroProveedores(object item)
        {
            var proveedor = item as Proveedor;
            string razonSocial = proveedor.RazonSocial.ToLower();
            string nif = proveedor.Nif.ToLower();
            string email = proveedor.Email.ToLower();
            string calle = proveedor.Calle.ToLower();
            string codigoPostal = proveedor.Municipio.CodigoPostal.ToLower();
            string municipio = proveedor.Municipio.Nombre.ToLower();
            string tipo = proveedor.TipoProveedor.Nombre.ToLower();
            var itemAceptado = true;

            var condicion = (RazonSocialSeleccionada == true ? razonSocial.Contains(TextoFiltroProveedores) : false)
                || (NifSeleccionado == true ? nif.Contains(TextoFiltroProveedores) : false)
                || (EmailSeleccionado == true ? email.Contains(TextoFiltroProveedores) : false)
                || (CalleSeleccionada == true ? calle.Contains(TextoFiltroProveedores) : false)
                || (CodigoPostalSeleccionado == true ? codigoPostal.Contains(TextoFiltroProveedores) : false)
                || (MunicipioSeleccionado == true ? municipio.Contains(TextoFiltroProveedores) : false);

            // Filtra Tipos Proveedores
            if (FiltroTiposViewModel.ItemsSeleccionados == null || FiltroTiposViewModel.ItemsSeleccionados.Count == 0)
            {
                itemAceptado = condicion;
            }
            else
            {
                foreach (TipoProveedor tipoproveedor in FiltroTiposViewModel.ItemsSeleccionados)
                {
                    if (tipoproveedor.Nombre.ToLower().Equals(tipo))
                    {
                        // Si lo encuentra no hace falta que siga haciendo el foreach
                        itemAceptado = condicion;
                        break;
                    }
                    else { itemAceptado = false; }
                }
            }
            return itemAceptado;
        }
        #endregion


        #region Añadir Tipo
        public ICommand AnadirTipoComando => _anadirTipoComando ??
           (_anadirTipoComando = new RelayCommand(
               param => AnadirTipo()
           ));

        private async void AnadirTipo()
        {
            var formTipo = new FormTipo();
            formTipo.vNombreUnico.Atributo = "Nombre";
            formTipo.vNombreUnico.Tipo = "TipoProveedor";

            if ((bool)await DialogHost.Show(formTipo, "RootDialog"))
            {
                Context.TiposProveedores.Add(new TipoProveedor()
                {
                    Nombre = formTipo.Nombre,
                    Descripcion = formTipo.Descripcion
                });
                Context.SaveChanges();
                CargarProveedores();
            }
        }
        #endregion


        #region Borrar Tipo
        public ICommand BorrarTipoComando => _borrarTipoComando ??
          (_borrarTipoComando = new RelayCommand(
              param => BorrarTipo(),
              param => FiltroTiposViewModel.ItemSeleccionado != null
          ));

        private async void BorrarTipo()
        {
            var mensajeConf = new MensajeConfirmacion()
            {
                Mensaje = "¿Está seguro de que desea borrar el tipo " + FiltroTiposViewModel.ItemSeleccionado.Nombre + "?"
            };
            if ((bool)await DialogHost.Show(mensajeConf, "RootDialog"))
            {
                if (!Context.Proveedores.Any(t => t.TipoId == FiltroTiposViewModel.ItemSeleccionado.TipoProveedorId))
                {
                    Context.TiposProveedores.Remove(FiltroTiposViewModel.ItemSeleccionado);
                    Context.SaveChanges();
                    CargarProveedores();
                }
                else
                {
                    await DialogHost.Show(new MensajeInformacion("No puede borrar el tipo debido a que está en uso"), "RootDialog");
                }
            }
        }
        #endregion


        #region Modificar Tipo
        public ICommand ModificarTipoComando
        {
            get
            {
                if (_modificarTipoComando == null)
                {
                    _modificarTipoComando = new RelayCommand(
                        param => ModificarTipo(),
                        param => FiltroTiposViewModel.ItemSeleccionado != null
                    );
                }
                return _modificarTipoComando;
            }
        }

        private async void ModificarTipo()
        {
            var formTipo = new FormTipo("Editar Tipo");
            formTipo.Nombre = FiltroTiposViewModel.ItemSeleccionado.Nombre;
            formTipo.Descripcion = FiltroTiposViewModel.ItemSeleccionado.Descripcion;
            formTipo.vNombreUnico.Atributo = "Nombre";
            formTipo.vNombreUnico.Tipo = "TipoProveedor";
            formTipo.vNombreUnico.NombreActual = FiltroTiposViewModel.ItemSeleccionado.Nombre;
            if ((bool)await DialogHost.Show(formTipo, "RootDialog"))
            {
                FiltroTiposViewModel.ItemSeleccionado.Nombre = formTipo.Nombre;
                FiltroTiposViewModel.ItemSeleccionado.Descripcion = formTipo.Descripcion;
                Context.SaveChanges();
                CargarProveedores();
            }
        }
        #endregion
    }
}
