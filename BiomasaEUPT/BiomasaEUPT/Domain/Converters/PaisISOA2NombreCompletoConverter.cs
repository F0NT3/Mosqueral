﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BiomasaEUPT.Domain.Converters
{
    public class PaisISOA2NombreCompletoConverter : IValueConverter
    {
        // https://es.wikipedia.org/wiki/ISO_3166-1
        // Listado de paises: http://www.worldatlas.com/aatlas/ctycodes.htm
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                RegionInfo regionInfo = new RegionInfo(value.ToString());
                return regionInfo.DisplayName;
            }
            catch (Exception ex)
            {
                if (ex is CultureNotFoundException || ex is ArgumentException)
                {
                    return value.ToString();
                }

                throw;
            }

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
