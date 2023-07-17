using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public  class CN_Negocio
    {
        private CD_Negocio objcd_Negocio = new CD_Negocio();

        public Negocio ObtenerDatos()
        {
            return objcd_Negocio.ObtenerDatos();
        }

        public bool GuardarDatos(Negocio obj, out string mensaje)
        {
            mensaje = string.Empty;
            if (obj.Nombre == "")
            {
                mensaje += "Es necesario el nombre del Negocio\n";
            }
            if (obj.RUC == "")
            {
                mensaje += "Es necesario el RUC del Negocio\n";
            }
            if (obj.Direccion == "")
            {
                mensaje += "Es necesario la direccion del Negocio\n";
            }
            if (mensaje != string.Empty)
            {
                return false;
            }
            else
            {
                return objcd_Negocio.GuardarDatos(obj, out mensaje);
            }

        }

        public byte[] ObtenerLogo(out bool obtenido)
        {
            return objcd_Negocio.ObtenerLogo(out obtenido);
        }

        public bool ActualizarLogo(byte[] imagen, out string mensaje)
        {
            return objcd_Negocio.ActualizarLogo(imagen,out mensaje);
        }
    }
}
