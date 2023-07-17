using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Cliente
    {
        private CD_Cliente objcd_Cliente = new CD_Cliente();

        public List<Cliente> Listar()
        {
            return objcd_Cliente.Listar();
        }

        public int Registrar(Cliente obj, out string mensaje)
        {
            mensaje = string.Empty;
            if (obj.NombreCompleto == "")
            {
                mensaje += "Es necesario el nombre del Cliente\n";
            }
            if (obj.Documento == "")
            {
                mensaje += "Es necesario el documento del Cliente\n";
            }
            if (obj.Correo == "")
            {
                mensaje += "Es necesario el correo del Cliente\n";
            }
            if (mensaje != string.Empty)
            {
                return 0;
            }
            else
            {
                return objcd_Cliente.Registrar(obj, out mensaje);
            }

        }

        public bool Editar(Cliente obj, out string mensaje)
        {
            mensaje = string.Empty;
            if (obj.NombreCompleto == "")
            {
                mensaje += "Es necesario el nombre del Cliente\n";
            }
            if (obj.Documento == "")
            {
                mensaje += "Es necesario el documento del Cliente\n";
            }
            if (obj.Correo == "")
            {
                mensaje += "Es necesario el correo del Cliente\n";
            }

            if (mensaje != string.Empty)
            {
                return false;
            }
            else
            {
                return objcd_Cliente.Editar(obj, out mensaje);
            }

        }

        public bool Eliminar(Cliente obj, out string mensaje)
        {

            return objcd_Cliente.Eliminar(obj, out mensaje);
        }
    }
}
