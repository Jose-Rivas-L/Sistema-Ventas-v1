using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Producto
    {
        private CD_Producto objcd_Producto = new CD_Producto();

        public List<Producto> Listar()
        {
            return objcd_Producto.Listar();
        }

        public int Registrar(Producto obj, out string mensaje)
        {
            mensaje = string.Empty;
            if (obj.Nombre == "")
            {
                mensaje += "Es necesario el nombre del Producto\n";
            }
            if (obj.Codigo == "")
            {
                mensaje += "Es necesario el codigo del Producto\n";
            }
            if (obj.Descripcion == "")
            {
                mensaje += "Es necesario la descripcion del Producto\n";
            }
            if(obj.Invima == "")
            {
                mensaje += "Es necesario el invima del Producto\n";
            }
            if (obj.Lote == "")
            {
                mensaje += "Es necesario el lote del Producto\n";
            }
            if (obj.FechaVencimiento == "")
            {
                mensaje += "Es necesario la fecha de vencimiento del Producto\n";
            }
            if (mensaje != string.Empty)
            {
                return 0;
            }
            else
            {
                return objcd_Producto.Registrar(obj, out mensaje);
            }

        }

        public bool Editar(Producto obj,string loteAntiguo, out string mensaje)
        {
            mensaje = string.Empty;
            if (obj.Nombre == "")
            {
                mensaje += "Es necesario el nombre del Producto\n";
            }
            if (obj.Descripcion == "")
            {
                mensaje += "Es necesario la descripcion del Producto\n";
            }
            if (obj.Codigo == "")
            {
                mensaje += "Es necesario el codigo del Producto\n";
            }
            if (obj.Invima == "")
            {
                mensaje += "Es necesario el invima del Producto\n";
            }
            if (obj.Lote == "")
            {
                mensaje += "Es necesario el lote del Producto\n";
            }
            if (obj.FechaVencimiento == "")
            {
                mensaje += "Es necesario la fecha de vencimiento del Producto\n";
            }

            if (mensaje != string.Empty)
            {
                return false;
            }
            else
            {
                return objcd_Producto.Editar(obj,loteAntiguo, out mensaje);
            }

        }

        public bool Eliminar(Producto obj, out string mensaje)
        {

            return objcd_Producto.Eliminar(obj, out mensaje);
        }
    }
}
