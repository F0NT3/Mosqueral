﻿using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SeedCodigosPostales
{
    public class SeedCP
    {
        string URL_DESCARGA = "http://download.geonames.org/export/zip/{0}.zip";

        public SeedCP()
        {

        }

        public void Generar()
        {
            string[] paises = { "ES", "FR" };
            var datosCP = new List<string>();

            foreach (var p in paises)
            {
                datosCP = datosCP.Concat(ObtenerDatosCP(p)).ToList();
            }

            SeedPaises(datosCP);
        }

        private List<string> ObtenerDatosCP(string codigoPais)
        {
            var lineas = new List<string>();
            using (ZipFile zip = ZipFile.Read(new MemoryStream(new WebClient().DownloadData(string.Format(URL_DESCARGA, codigoPais)))))
            {
                var memoryStream = new MemoryStream();
                zip[codigoPais + ".txt"].Extract(memoryStream);
                //string datosPais = Encoding.UTF8.GetString(memoryStream.ToArray());

                memoryStream.Position = 0;
                using (var reader = new StreamReader(memoryStream, Encoding.UTF8))
                {
                    while (!reader.EndOfStream)
                    {
                        var linea = reader.ReadLine();
                        lineas.Add(linea);
                    }
                }

                return lineas;
            }
        }

        private void SeedPaises(List<string> datosCP)
        {
            var lineas = new List<string>();
            //var datos = from linea in lineas select (linea.Split('\t')).ToArray();
            //Console.WriteLine(datos.Select(d => d[1]).Count());
            var codigosPostales = datosCP.Select(l => new
            {
                CodigoPais = l.Split('\t').ElementAt(0),
                CodidoPostal = l.Split('\t').ElementAt(1),
                Municipio = l.Split('\t').ElementAt(2),
                Comunidad = l.Split('\t').ElementAt(3),
                CodigoComunidad = l.Split('\t').ElementAt(4),
                Provincia = l.Split('\t').ElementAt(5),
                CodigoProvincia = l.Split('\t').ElementAt(6),
                Latitud = l.Split('\t').ElementAt(9),
                Longitud = l.Split('\t').ElementAt(10)
            });

            lineas.Add("Codigo;Nombre");
            var paises = codigosPostales.Select(c => new { c.CodigoPais }).Distinct().OrderBy(c => c.CodigoPais).ToList();
            for (int i = 0; i < paises.Count(); i++)
            {
                lineas.Add(paises[i].CodigoPais + ";" + new RegionInfo(paises[i].CodigoPais).DisplayName);
                Console.Write("\rParseando Países {0,3}%", i * 100 / paises.Count());
            }
            Console.WriteLine("\rParseando Países 100%");

            File.WriteAllLines("SeedPaises.csv", lineas);
            lineas.Clear();

            lineas.Add("Codigo;Nombre");
            var comunidades = codigosPostales.Select(c => new { c.CodigoPais, c.CodigoComunidad, c.Comunidad }).Distinct().OrderBy(c => c.Comunidad).ToList();
            for (int i = 0; i < comunidades.Count(); i++)
            {
                if (comunidades[i].CodigoComunidad != "") // El fichero de Francia no está bien
                {
                    lineas.Add(comunidades[i].CodigoPais + "-" + comunidades[i].CodigoComunidad + ";" + comunidades[i].Comunidad);
                }
                Console.Write("\rParseando Comunidades {0,3}%", i * 100 / comunidades.Count());
            }
            Console.WriteLine("\rParseando Comunidades 100%");
            File.WriteAllLines("SeedComunidades.csv", lineas);
            lineas.Clear();

            lineas.Add("Codigo;Nombre;CodigoComunidad");
            var provincias = codigosPostales.Select(c => new { c.CodigoPais, c.CodigoComunidad, c.CodigoProvincia, c.Provincia }).Distinct().OrderBy(c => c.Provincia).ToList();
            for (int i = 0; i < provincias.Count(); i++)
            {
                if (provincias[i].CodigoProvincia != "") // El fichero de Francia no está bien
                {
                    lineas.Add(provincias[i].CodigoPais + "-" + provincias[i].CodigoProvincia + ";" + provincias[i].Provincia + ";" + provincias[i].CodigoPais + "-" + provincias[i].CodigoComunidad);
                }
                Console.Write("\rParseando Provincias {0,3}%", i + 1 * 100 / provincias.Count());
            }
            Console.WriteLine("\rParseando Provincias 100%");
            File.WriteAllLines("SeedProvincias.csv", lineas);
            lineas.Clear();

            lineas.Add("CodigoPostal;Nombre;Latitud;Longitud;CodigoProvincia");
            var municipios = codigosPostales.Select(c => new { c.CodidoPostal, c.CodigoPais, c.CodigoProvincia, c.Municipio, c.Latitud, c.Longitud }).Distinct().OrderBy(c => c.Municipio).ToList();
            for (int i = 0; i < municipios.Count(); i++)
            {
                if (municipios[i].CodigoProvincia != "") // El fichero de Francia no está bien
                {
                    lineas.Add(municipios[i].CodidoPostal + ";" + municipios[i].Municipio.Replace(";", ",") + ";" + municipios[i].Latitud + ";" + municipios[i].Longitud + ";" + municipios[i].CodigoPais + "-" + municipios[i].CodigoProvincia);
                }
                Console.Write("\rParseando Municipios {0,3}%", i * 100 / municipios.Count());
            }
            Console.WriteLine("\rParseando Municipios 100%");
            File.WriteAllLines("SeedMunicipios.csv", lineas);
            lineas.Clear();
            Console.WriteLine("\nFicheros CSV generados correctamente.");
        }
    }
}
