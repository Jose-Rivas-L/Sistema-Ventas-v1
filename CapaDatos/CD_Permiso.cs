using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Permiso
    {
        public List<Permiso> Listar(int IdUsuario)
        {
            List<Permiso> lista = new List<Permiso>();
            using (SqlConnection conn = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder Query = new StringBuilder();
                    Query.AppendLine("select p.IdRol,NombreMenu from PERMISO p");
                    Query.AppendLine("inner join ROL r on r.IdRol = p.IdRol");
                    Query.AppendLine("inner join USUARIO u on u.IdRol = r.IdRol");
                    Query.AppendLine("where u.IdUsuario = @IdUsuario");

                    SqlCommand cmd = new SqlCommand(Query.ToString(), conn);
                    cmd.Parameters.Add("@IdUsuario", IdUsuario);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Permiso()
                            {
                                pRol = new Rol(){IdRol = Convert.ToInt32(dr["IdRol"]) },
                                NombreMenu = dr["NombreMenu"].ToString()
                            });
                        }
                    }

                }
                catch (Exception ex)
                {
                    lista = new List<Permiso>();
                }
            }
            return lista;
        }
    }
}
