﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiomasaEUPT.Modelos.Tablas
{
    /// <summary>
    /// Producto final listo para venta
    /// </summary>
    [Table("ProductosEnvasados")]
    public class ProductoEnvasado
    {
        [Key]
        public int ProductoEnvasadoId { get; set; }

        [DisplayName("Volumen"), Display(Name = "Volumen")]
        public double? Volumen { get; set; }

        [DisplayName("Unidades"), Display(Name = "Unidades")]
        public double? Unidades { get; set; }

        [StringLength(Constantes.LONG_MAX_OBSERVACIONES, MinimumLength = Constantes.LONG_MIN_OBSERVACIONES)]
        [DisplayName("Observaciones"), Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        [Required]
        [StringLength(Constantes.LONG_MAX_CODIGO, MinimumLength = Constantes.LONG_MIN_CODIGO)]
        [DisplayName("Código"), Display(Name = "Código")]
        public string Codigo { get; set; }       

        public int PickingId { get; set; }

        public int PedidoDetalleId { get; set; }

        public int TipoProductoEnvasadoId { get; set; }

        [ForeignKey("PickingId")]
        public virtual Picking Picking { get; set; }

        [ForeignKey("PedidoDetalleId")]
        public virtual PedidoDetalle PedidoDetalle { get; set; }

        [ForeignKey("TipoProductoEnvasadoId")]
        public virtual TipoProductoEnvasado TipoProductoEnvasado { get; set; }

        public virtual List<ProductoEnvasadoComposicion> ProductoEnvasadoComposiciones { get; set; }

    }
}
