using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Compra
    {
        public int ObtenerCorrelativo()
        {
            int Idcorrelativo = 0;
            using (SqlConnection conn = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select count(*)+1 from COMPRA");
                    SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    Idcorrelativo = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch
                {
                    Idcorrelativo = 0;
                }
            }
            return Idcorrelativo;
        }

        public bool Registrar(Compra obj,DataTable tabla, out string mensaje)
        {
            bool Resultado = false;
            using (SqlConnection conn = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarCompra", conn);
                    cmd.Parameters.AddWithValue("IdUsuario",obj.oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("IdProveedor",obj.oProveedor.IdProveedor);
                    cmd.Parameters.AddWithValue("TipoDocumento",obj.TipoDocumento);
                    cmd.Parameters.AddWithValue("NumeroDocumento",obj.NumeroDocumento);
                    cmd.Parameters.AddWithValue("MontoTotal",obj.MontoTotal);
                    cmd.Parameters.AddWithValue("DetalleCompra",tabla);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    cmd.ExecuteNonQuery();
                    Resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    Resultado = false;
                }
            }
            return Resultado;
        }

        public Compra ObtenerCompra(string numero)
        {
            Compra compra = new Compra();
            using (SqlConnection conn = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select c.IdCompra,u.NombreCompleto,pr.Documento, pr.RazonSocial,");
                    query.AppendLine("c.TipoDocumento,c.NumeroDocumento,c.MontoTotal,convert(char(10),c.FechaRegistro,103)[FechaRegistro]");
                    query.AppendLine("from Compra c inner join USUARIO u on c.IdUsuario = u.IdUsuario");
                    query.AppendLine("inner join PROVEEDOR pr on pr.IdProveedor = c.IdProveedor");
                    query.AppendLine("where c.NumeroDocumento = @numero");
                    SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            compra = new Compra()
                            {
                                IdCompra = Convert.ToInt32(dr["IdCompra"]),
                                oUsuario = new Usuario() { NombreCompleto = dr["NombreCompleto"].ToString() },
                                oProveedor = new Proveedor() { Documento = dr["Documento"].ToString(), RazonSocial = dr["RazonSocial"].ToString() },
                                TipoDocumento = dr["TipoDocumento"].ToString(),
                                NumeroDocumento = dr["NumeroDocumento"].ToString(),
                                MontoTotal = Convert.ToDecimal(dr["MontoTotal"].ToString()),
                                FechaRegistro = dr["FechaRegistro"].ToString()
                            };
                            
                        }
                    }

                }
                catch
                {
                    compra = new Compra();
                }
            }
            return compra;
        }

        public List<Detalle_Compra> ObtenerDetalleCompra(int idcompra)
        {
            List<Detalle_Compra> lista = new List<Detalle_Compra>();
            using (SqlConnection conn = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select p.Nombre,dc.PrecioCompra,dc.Cantidad,dc.MontoTotal");
                    query.AppendLine("from DETALLE_COMPRA dc");
                    query.AppendLine("inner join PRODUCTO p on p.IdProducto = dc.IdProducto");
                    query.AppendLine("where dc.IdCompra = @idcompra");
                    SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                    cmd.Parameters.AddWithValue("@idcompra", idcompra);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Detalle_Compra()
                            {
                                oProducto = new Producto() { Nombre = dr["Nombre"].ToString() },
                                PrecioCompra = Convert.ToDecimal(dr["PrecioCompra"].ToString()),
                                Cantidad = Convert.ToInt32(dr["Cantidad"].ToString()),
                                MontoTotal = Convert.ToDecimal(dr["MontoTotal"].ToString())
                            });
                        }
                    }

                }
                catch
                {
                    lista = new List<Detalle_Compra>();
                }
            }
            return lista;
        }
    }
}
