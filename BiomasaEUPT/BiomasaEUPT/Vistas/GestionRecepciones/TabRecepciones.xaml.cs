﻿using BiomasaEUPT.Clases;
using BiomasaEUPT.Modelos;
using BiomasaEUPT.Modelos.Tablas;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace BiomasaEUPT.Vistas.GestionRecepciones
{
    /// <summary>
    /// Lógica de interacción para TabRecepciones.xaml
    /// </summary>
    public partial class TabRecepciones : UserControl
    {
        private BiomasaEUPTContext context;
        private CollectionViewSource materiasPrimasViewSource;
        private CollectionViewSource tiposMateriasPrimasViewSource;
        private CollectionViewSource gruposMateriasPrimasViewSource;
        //private CollectionViewSource sitiosRecepcionesViewSource;
        private CollectionViewSource procedenciasViewSource;
        private CollectionViewSource recepcionesViewSource;
        private CollectionViewSource proveedoresViewSource;
        private CollectionViewSource estadosRecepcionesViewSource;

        public TabRecepciones()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            using (new CursorEspera())
            {
                context = new BiomasaEUPTContext();
                materiasPrimasViewSource = ((CollectionViewSource)(ucTablaMateriasPrimas.FindResource("materiasPrimasViewSource")));
                tiposMateriasPrimasViewSource = ((CollectionViewSource)(ucTablaMateriasPrimas.FindResource("tiposMateriasPrimasViewSource")));
                gruposMateriasPrimasViewSource = ((CollectionViewSource)(ucTablaMateriasPrimas.FindResource("gruposMateriasPrimasViewSource")));
                procedenciasViewSource = ((CollectionViewSource)(ucTablaMateriasPrimas.FindResource("procedenciasViewSource")));
                recepcionesViewSource = ((CollectionViewSource)(ucTablaRecepciones.FindResource("recepcionesViewSource")));
                proveedoresViewSource = ((CollectionViewSource)(ucTablaRecepciones.FindResource("proveedoresPrimasViewSource")));
                estadosRecepcionesViewSource = ((CollectionViewSource)(ucTablaRecepciones.FindResource("estadosRecepcionesViewSource")));

                context.TiposMateriasPrimas.Load();
                context.GruposMateriasPrimas.Load();
                context.Procedencias.Load();
                context.Recepciones.Load();
                context.Proveedores.Load();
                context.EstadosRecepciones.Load();

                tiposMateriasPrimasViewSource.Source = context.TiposMateriasPrimas.Local;
                gruposMateriasPrimasViewSource.Source = context.GruposMateriasPrimas.Local;
                procedenciasViewSource.Source = context.Procedencias.Local;
                recepcionesViewSource.Source = context.Recepciones.Local;
                proveedoresViewSource.Source = context.Proveedores.Local;
                estadosRecepcionesViewSource.Source = context.EstadosRecepciones.Local;

                ucTablaRecepciones.dgRecepciones.SelectionChanged += DgRecepciones_SelectionChanged;
                ucTablaRecepciones.cbNumeroAlbaran.Checked += (s, e1) => { FiltrarTablaRecepciones(); };
                ucTablaRecepciones.cbNumeroAlbaran.Unchecked += (s, e1) => { FiltrarTablaRecepciones(); };
                ucTablaRecepciones.cbFechaRecepcion.Checked += (s, e1) => { FiltrarTablaRecepciones(); };
                ucTablaRecepciones.cbFechaRecepcion.Unchecked += (s, e1) => { FiltrarTablaRecepciones(); };
                ucTablaRecepciones.cbEstado.Checked += (s, e1) => { FiltrarTablaRecepciones(); };
                ucTablaRecepciones.cbEstado.Unchecked += (s, e1) => { FiltrarTablaRecepciones(); };
                ucTablaRecepciones.cbProveedor.Checked += (s, e1) => { FiltrarTablaRecepciones(); };
                ucTablaRecepciones.cbProveedor.Unchecked += (s, e1) => { FiltrarTablaRecepciones(); };

                ucTablaMateriasPrimas.cbTipo.Checked += (s, e1) => { FiltrarTablaMateriasPrimas(); };
                ucTablaMateriasPrimas.cbTipo.Unchecked += (s, e1) => { FiltrarTablaMateriasPrimas(); };
                ucTablaMateriasPrimas.cbGrupo.Checked += (s, e1) => { FiltrarTablaMateriasPrimas(); };
                ucTablaMateriasPrimas.cbGrupo.Unchecked += (s, e1) => { FiltrarTablaMateriasPrimas(); };
                ucTablaMateriasPrimas.cbVolUni.Checked += (s, e1) => { FiltrarTablaMateriasPrimas(); };
                ucTablaMateriasPrimas.cbVolUni.Unchecked += (s, e1) => { FiltrarTablaMateriasPrimas(); };
                ucTablaMateriasPrimas.cbProcedencia.Checked += (s, e1) => { FiltrarTablaMateriasPrimas(); };
                ucTablaMateriasPrimas.cbProcedencia.Unchecked += (s, e1) => { FiltrarTablaMateriasPrimas(); };
                ucTablaMateriasPrimas.cbFechaBaja.Checked += (s, e1) => { FiltrarTablaMateriasPrimas(); };
                ucTablaMateriasPrimas.cbFechaBaja.Unchecked += (s, e1) => { FiltrarTablaMateriasPrimas(); };

                ucTablaRecepciones.bAnadirRecepcion.Click += BAnadirRecepcion_Click;
                ucTablaMateriasPrimas.bAnadirMateriaPrima.Click += BAnadirMateriaPrima_Click;

                Style rowStyle = new Style(typeof(DataGridRow));
                rowStyle.Setters.Add(new EventSetter(MouseDoubleClickEvent, new MouseButtonEventHandler(RowRecepciones_DoubleClick)));
                ucTablaRecepciones.dgRecepciones.RowStyle = rowStyle;
                ucTablaRecepciones.dgRecepciones.SelectedIndex = -1;
            }
        }

        private void DgRecepciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var recepcion = (sender as DataGrid).SelectedItem as Recepcion;
            if (recepcion != null)
            {
                materiasPrimasViewSource.Source = context.MateriasPrimas.Where(mp => mp.RecepcionId == recepcion.RecepcionId).ToList();
            }
        }

        private async void BAnadirRecepcion_Click(object sender, RoutedEventArgs e)
        {
            var formRecepcion = new FormRecepcion(context);

            if ((bool)await DialogHost.Show(formRecepcion, "RootDialog"))
            {
                context.Recepciones.Add(new Recepcion()
                {
                    NumeroAlbaran = formRecepcion.NumeroAlbaran,
                    FechaRecepcion = new DateTime(formRecepcion.Fecha.Year, formRecepcion.Fecha.Month, formRecepcion.Fecha.Day, formRecepcion.Hora.Hour, formRecepcion.Hora.Minute, formRecepcion.Hora.Second),
                    ProveedorId = (formRecepcion.cbProveedores.SelectedItem as Proveedor).ProveedorId,
                    EstadoId = (formRecepcion.cbEstadosRecepciones.SelectedItem as EstadoRecepcion).EstadoRecepcionId
                });
                context.SaveChanges();
            }
        }

        private async void BAnadirMateriaPrima_Click(object sender, RoutedEventArgs e)
        {
            var formMateriaPrima = new FormMateriaPrima(context);

            if ((bool)await DialogHost.Show(formMateriaPrima, "RootDialog"))
            {
                var materiaPrima = new MateriaPrima()
                {
                    RecepcionId = (ucTablaRecepciones.dgRecepciones.SelectedItem as Recepcion).RecepcionId,
                    Observaciones = formMateriaPrima.Observaciones,
                    Codigo = formMateriaPrima.Codigo,
                    ProcedenciaId = (formMateriaPrima.cbProcedencias.SelectedItem as Procedencia).ProcedenciaId,
                    TipoId = (formMateriaPrima.cbTiposMateriasPrimas.SelectedItem as TipoMateriaPrima).TipoMateriaPrimaId,
                    Volumen = formMateriaPrima.Volumen,
                    Unidades = formMateriaPrima.Unidades
                };
                if (formMateriaPrima.FechaBaja != null)
                    materiaPrima.FechaBaja = new DateTime(formMateriaPrima.FechaBaja.Value.Year, formMateriaPrima.FechaBaja.Value.Month, formMateriaPrima.FechaBaja.Value.Day, formMateriaPrima.HoraBaja.Value.Hour, formMateriaPrima.HoraBaja.Value.Minute, formMateriaPrima.HoraBaja.Value.Second);

                context.MateriasPrimas.Add(materiaPrima);
                List<HuecoMateriaPrima> huecosMateriasPrimas = new List<HuecoMateriaPrima>();
                foreach (var hmp in formMateriaPrima.HuecosMateriasPrimas)
                {
                    var a = hmp.HuecoRecepcion.HuecoRecepcionId;
                    if (hmp.Unidades != 0 && hmp.Volumen != 0)
                    {
                        hmp.HuecoRecepcion = null;
                        hmp.HuecoRecepcionId = a;
                        hmp.MateriaPrima = materiaPrima;
                        huecosMateriasPrimas.Add(hmp);
                    }
                }
                context.HuecosMateriasPrimas.AddRange(huecosMateriasPrimas);
                context.SaveChanges();
            }
        }

        public void FiltrarTablaRecepciones()
        {
            recepcionesViewSource.Filter += new FilterEventHandler(FiltroTablaRecepciones);
        }

        private void FiltroTablaRecepciones(object sender, FilterEventArgs e)
        {
            string textoBuscado = ucTablaRecepciones.tbBuscar.Text.ToLower();
            var recepcion = e.Item as Recepcion;
            string fechaRecepcion = recepcion.FechaRecepcion.ToString();
            string numeroAlbaran = recepcion.NumeroAlbaran.ToLower();
            string proveedor = recepcion.Proveedor.RazonSocial.ToLower();
            string estado = recepcion.EstadoRecepcion.Nombre.ToLower();

            e.Accepted = (ucTablaRecepciones.cbFechaRecepcion.IsChecked == true ? fechaRecepcion.Contains(textoBuscado) : false) ||
                         (ucTablaRecepciones.cbNumeroAlbaran.IsChecked == true ? numeroAlbaran.Contains(textoBuscado) : false) ||
                         (ucTablaRecepciones.cbProveedor.IsChecked == true ? proveedor.Contains(textoBuscado) : false) ||
                         (ucTablaRecepciones.cbEstado.IsChecked == true ? estado.Contains(textoBuscado) : false);

        }

        public void FiltrarTablaMateriasPrimas()
        {
            materiasPrimasViewSource.Filter += new FilterEventHandler(FiltroTablaMateriasPrimas);
        }

        private void FiltroTablaMateriasPrimas(object sender, FilterEventArgs e)
        {
            string textoBuscado = ucTablaMateriasPrimas.tbBuscar.Text.ToLower();
            var materiaPrima = e.Item as MateriaPrima;
            string tipo = materiaPrima.TipoMateriaPrima.Nombre.ToLower();
            string grupo = materiaPrima.TipoMateriaPrima.GrupoMateriaPrima.Nombre.ToLower();
            string volumen = materiaPrima.Volumen.ToString();
            string unidades = materiaPrima.Unidades.ToString();
            string procedencia = materiaPrima.Procedencia.Nombre.ToLower();
            string fechaBaja = materiaPrima.FechaBaja.ToString();

            e.Accepted = (ucTablaMateriasPrimas.cbFechaBaja.IsChecked == true ? fechaBaja.Contains(textoBuscado) : false) ||
                         (ucTablaMateriasPrimas.cbTipo.IsChecked == true ? tipo.Contains(textoBuscado) : false) ||
                         (ucTablaMateriasPrimas.cbGrupo.IsChecked == true ? grupo.Contains(textoBuscado) : false) ||
                         (ucTablaMateriasPrimas.cbVolUni.IsChecked == true ? (volumen.Contains(textoBuscado) || unidades.Contains(textoBuscado)) : false) ||
                         (ucTablaMateriasPrimas.cbProcedencia.IsChecked == true ? procedencia.Contains(textoBuscado) : false);

        }

        #region BorrarRecepcion
        private ICommand _borrarRecepcionComando;

        public ICommand BorrarRecepcionComando
        {
            get
            {
                if (_borrarRecepcionComando == null)
                {
                    _borrarRecepcionComando = new RelayComando(
                        param => BorrarRecepcion(),
                        param => CanBorrarRecepcion()
                    );
                }
                return _borrarRecepcionComando;
            }
        }

        private bool CanBorrarRecepcion()
        {
            if (ucTablaRecepciones.dgRecepciones.SelectedIndex != -1)
            {
                Recepcion recepcionSeleccionada = ucTablaRecepciones.dgRecepciones.SelectedItem as Recepcion;
                return recepcionSeleccionada != null;
            }
            return false;
        }

        private async void BorrarRecepcion()
        {
            string pregunta = ucTablaRecepciones.dgRecepciones.SelectedItems.Count == 1
                ? "¿Está seguro de que desea borrar la recepción " + (ucTablaRecepciones.dgRecepciones.SelectedItem as Recepcion).NumeroAlbaran + "?"
                : "¿Está seguro de que desea borrar las recepciones seleccionadas?";

            if ((bool)await DialogHost.Show(new MensajeConfirmacion(pregunta), "RootDialog"))
            {
                context.Recepciones.RemoveRange(ucTablaRecepciones.dgRecepciones.SelectedItems.Cast<Recepcion>().ToList());
                context.SaveChanges();
            }
        }
        #endregion

        #region BorrarMateriaPrima
        private ICommand _borrarMateriaPrimaComando;

        public ICommand BorrarMateriaPrimaComando
        {
            get
            {
                if (_borrarMateriaPrimaComando == null)
                {
                    _borrarMateriaPrimaComando = new RelayComando(
                        param => BorrarMateriaPrima(),
                        param => CanBorrarMateriaPrima()
                    );
                }
                return _borrarMateriaPrimaComando;
            }
        }

        private bool CanBorrarMateriaPrima()
        {
            if (ucTablaRecepciones.dgRecepciones.SelectedIndex != -1)
            {
                Recepcion recepcionSeleccionada = ucTablaRecepciones.dgRecepciones.SelectedItem as Recepcion;
                return recepcionSeleccionada != null;
            }
            return false;
        }

        private async void BorrarMateriaPrima()
        {
            string pregunta = ucTablaRecepciones.dgRecepciones.SelectedItems.Count == 1
                ? "¿Está seguro de que desea borrar la materia prima " + (ucTablaMateriasPrimas.dgMateriasPrimas.SelectedItem as MateriaPrima).Codigo + "?"
                : "¿Está seguro de que desea borrar las materias primas seleccionadas?";

            if ((bool)await DialogHost.Show(new MensajeConfirmacion(pregunta), "RootDialog"))
            {
                context.MateriasPrimas.RemoveRange(ucTablaMateriasPrimas.dgMateriasPrimas.SelectedItems.Cast<MateriaPrima>().ToList());
                context.SaveChanges();
                ucTablaMateriasPrimas.dgMateriasPrimas.Items.Refresh();
            }
        }
        #endregion

        private async void RowRecepciones_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var fila = sender as DataGridRow;
            var recepcionSeleccionada = ucTablaRecepciones.dgRecepciones.SelectedItem as Recepcion;
            var formRecepcion = new FormRecepcion(context, "Editar Recepción")
            {
                NumeroAlbaran = recepcionSeleccionada.NumeroAlbaran,
                Fecha = recepcionSeleccionada.FechaRecepcion,
                Hora = recepcionSeleccionada.FechaRecepcion
            };
            var albaranViejo = formRecepcion.NumeroAlbaran;
            formRecepcion.vAlbaranUnico.NombreActual = recepcionSeleccionada.NumeroAlbaran;
            formRecepcion.cbEstadosRecepciones.SelectedValue = recepcionSeleccionada.EstadoRecepcion.EstadoRecepcionId;
            formRecepcion.cbProveedores.SelectedValue = recepcionSeleccionada.Proveedor.ProveedorId;
            if ((bool)await DialogHost.Show(formRecepcion, "RootDialog"))
            {
                recepcionSeleccionada.NumeroAlbaran = formRecepcion.NumeroAlbaran;
                recepcionSeleccionada.FechaRecepcion = new DateTime(formRecepcion.Fecha.Year, formRecepcion.Fecha.Month, formRecepcion.Fecha.Day, formRecepcion.Hora.Hour, formRecepcion.Hora.Minute, formRecepcion.Hora.Second);
                recepcionSeleccionada.ProveedorId = (formRecepcion.cbProveedores.SelectedItem as Proveedor).ProveedorId;
                recepcionSeleccionada.EstadoId = (formRecepcion.cbEstadosRecepciones.SelectedItem as EstadoRecepcion).EstadoRecepcionId;
                recepcionesViewSource.View.Refresh();
                context.SaveChanges();
                /* using (var context = new BiomasaEUPTContext())
                 {
                     var recepcion = context.Recepciones.Single(tc => tc.NumeroAlbaran == albaranViejo);
                     recepcion.NumeroAlbaran = formTipo.Nombre;
                     tipoCliente.Descripcion = formTipo.Descripcion;
                     context.SaveChanges();
                 }*/
            }
        }

    }
}
