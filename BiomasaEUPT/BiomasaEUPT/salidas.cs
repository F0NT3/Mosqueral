//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BiomasaEUPT
{
    using System;
    using System.Collections.Generic;
    
    public partial class salidas
    {
        public int id_salida { get; set; }
        public System.DateTime fecha { get; set; }
        public System.DateTime fecha_entrega { get; set; }
        public string pedido { get; set; }
        public int estado_id { get; set; }
        public string estado_nombre { get; set; }
        public int cliente_id { get; set; }
        public string cliente_razon_social { get; set; }
        public string destino { get; set; }
    }
}