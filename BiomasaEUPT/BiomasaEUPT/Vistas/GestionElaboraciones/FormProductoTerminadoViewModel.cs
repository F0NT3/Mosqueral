﻿using BiomasaEUPT.Modelos.Tablas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiomasaEUPT.Vistas.GestionElaboraciones
{
    class FormProductoTerminadoViewModel : INotifyPropertyChanged
    {
        public TipoMateriaPrima TipoMateriaPrima { get; set; }
        public TipoProductoTerminado TipoProductoTerminado { get; set; }
        public ObservableCollection<HistorialHuecoRecepcion> HistorialHuecosRecepcionesDisponibles { get; set; }
        public ObservableCollection<ProductoTerminadoComposicion> ProductosTerminadosComposiciones { get; set; }
        public ObservableCollection<HuecoAlmacenaje> HuecosAlmacenajesDisponibles { get; set; }
        public ObservableCollection<HistorialHuecoAlmacenaje> HistorialHuecosAlmacenajes { get; set; }
        public int? Unidades { get; set; }
        public double? Volumen { get; set; }
        public string CantidadHint { get; set; }
        public double Cantidad { get; set; }
        private string _observaciones;
        public string Observaciones
        {
            get => _observaciones;
            set
            {
                // Si las observaciones es cadena vacía hay que asignarle el valor null
                _observaciones = value == "" ? null : value;
            }
        }
        public DateTime? FechaBaja { get; set; }
        public DateTime? HoraBaja { get; set; }
        public bool QuedaCantidadPorAlmacenar { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        public FormProductoTerminadoViewModel()
        {
            HistorialHuecosRecepcionesDisponibles = new ObservableCollection<HistorialHuecoRecepcion>();
            ProductosTerminadosComposiciones = new ObservableCollection<ProductoTerminadoComposicion>();
            HuecosAlmacenajesDisponibles = new ObservableCollection<HuecoAlmacenaje>();
            HistorialHuecosAlmacenajes = new ObservableCollection<HistorialHuecoAlmacenaje>();
        }
    }
}
