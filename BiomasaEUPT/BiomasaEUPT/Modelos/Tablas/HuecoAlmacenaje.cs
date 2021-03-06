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
    /// Dentro del sitio de almacenaje, cada una de las partes en las que se encuentra dividido
    /// </summary>
    [Table("HuecosAlmacenajes")]
    public class HuecoAlmacenaje
    {
        [Key]
        public int HuecoAlmacenajeId { get; set; }

        [Required]
        [StringLength(Constantes.LONG_MAX_NOMBRE_HUECO, MinimumLength = Constantes.LONG_MIN_NOMBRE_HUECO)]
        [Index(IsUnique = true)]
        [DisplayName("Nombre"), Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [DisplayName("Volumen total"), Display(Name = "Volumen total")]
        public double VolumenTotal { get; set; }

        [Range(0, 1000)]
        [DisplayName("Unidades totales"), Display(Name = "Unidades totales")]
        public int UnidadesTotales { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DisplayName("Ocupado"), Display(Name = "Ocupado")]
        public bool? Ocupado { get; set; }

        public int SitioId { get; set; }

        [ForeignKey("SitioId")]
        public virtual SitioAlmacenaje SitioAlmacenaje { get; set; }

        public virtual List<HistorialHuecoAlmacenaje> HistorialHuecosAlmacenajes { get; set; }
    }
}
