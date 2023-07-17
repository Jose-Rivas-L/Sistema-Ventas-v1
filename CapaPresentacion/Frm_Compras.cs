using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
using CapaPresentacion.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class Frm_Compras : Form
    {
        private Usuario usuario;
        public Frm_Compras(Usuario objusuario = null)
        {
            this.usuario = objusuario;
            InitializeComponent();
        }

        private void Frm_Compras_Load(object sender, EventArgs e)
        {
            cboTipoDocumento.Items.Add(new OpcionCombo() { Valor = "Boleta", Texto = "Boleta" });
            cboTipoDocumento.Items.Add(new OpcionCombo() { Valor = "Factura", Texto = "Factura" });
            cboTipoDocumento.DisplayMember = "Texto";
            cboTipoDocumento.ValueMember = "Valor";
            cboTipoDocumento.SelectedIndex = 0;

            txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

            txtIdProveedor.Text = "0";
            txtIdProducto.Text = "0";
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            using (var modal = new MdProveedor())
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK)
                {
                    txtIdProveedor.Text = modal._Proveedor.IdProveedor.ToString();
                    txtDocumentoProv.Text = modal._Proveedor.Documento.ToString();
                    txtNombreProv.Text = modal._Proveedor.RazonSocial.ToString();
                }
                else
                {
                    txtDocumentoProv.Select();
                }
            }
        }

        private void btnBuscarProd_Click(object sender, EventArgs e)
        {
            using (var modal = new MdProducto())
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK)
                {
                    txtIdProducto.Text = modal._Producto.IdProducto.ToString();
                    txtCodProducto.Text = modal._Producto.Codigo.ToString();
                    txtNombreProducto.Text = modal._Producto.Nombre.ToString();
                    txtPrecioCompra.Text = modal._Producto.PrecioCompra.ToString();
                    txtPrecioCompra.Select();
                    txtPrecioVenta.Text = (modal._Producto.PrecioCompra + ((modal._Producto.PrecioCompra * 35) / 100)).ToString();
                }
                else
                {
                    txtCodProducto.Select();
                }
            }
        }

        private void txtCodProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                Producto oProducto = new CN_Producto().Listar().Where(P => P.Codigo == txtCodProducto.Text && P.Estado == true).FirstOrDefault();
                if (oProducto != null)
                {
                    txtCodProducto.BackColor = Color.Honeydew;
                    txtIdProducto.Text = oProducto.IdProducto.ToString();
                    txtNombreProducto.Text = oProducto.Nombre;
                    txtPrecioCompra.Select();
                    txtPrecioVenta.Text = (oProducto.PrecioCompra + ((oProducto.PrecioCompra * 35)/100)).ToString(); 
                }
                else
                {
                    txtCodProducto.BackColor = Color.MistyRose;
                    txtIdProducto.Text = "0";
                    txtNombreProducto.Text = "";
                }
            }


        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            decimal precioCompra = 0, precioVenta = 0;
            bool producto_existe = false;
            string formato = "dd/MM/yyyy";

            DateTime fecha;
            bool formatoValido = DateTime.TryParseExact(txtFechaVencimiento.Text, formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha);

            if (int.Parse(txtIdProducto.Text) == 0)
            {
                MessageBox.Show("Debe seleccionar un producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!decimal.TryParse(txtPrecioCompra.Text, out precioCompra))
            {
                MessageBox.Show("Precio Compra - Formato de moneda incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPrecioCompra.Select();
                return;
            }

            if (!decimal.TryParse(txtPrecioVenta.Text, out precioVenta))
            {
                MessageBox.Show("Precio Venta - Formato de moneda incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPrecioVenta.Select();
                return;
            }
            if(txtLote.Text == "")
            {
                MessageBox.Show("Debe introducir un lote válido del producto","Mensaje",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtLote.Select();
                return;
            }
            if (!formatoValido)
            {
                MessageBox.Show("Debe introducir un formato de fecha válida","Mensaje",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return ;
            }

            foreach (DataGridViewRow fila in dgvdata.Rows)
            {
                if (fila.Cells["Id"].Value.ToString() == txtIdProducto.Text)
                {
                    producto_existe = true;
                    break;
                }
            }

            if (!producto_existe)
            {
                dgvdata.Rows.Add(new object[] { txtIdProducto.Text,
                txtNombreProducto.Text,
                precioCompra.ToString("0.00"),
                precioVenta.ToString("0.00"),
                txtCantidad.Value.ToString(),
                (txtCantidad.Value * precioCompra).ToString("0.00"),
                txtLote.Text,
                txtFechaVencimiento.Text
                });
                CalcularTotal();
                LimpiarProducto();
                txtCodProducto.Select();
            }
        }


        private void LimpiarProducto()
        {
            txtCodProducto.BackColor = Color.White;
            txtCodProducto.Text = "";
            txtIdProducto.Text = "0";
            txtNombreProducto.Text = "";
            txtPrecioCompra.Text = "";
            txtPrecioVenta.Text = "";
            txtCantidad.Value = 1;
            txtLote.Text = "";
            txtFechaVencimiento.Text = "";
        }

        private void CalcularTotal()
        {
            decimal total = 0;
            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow fila in dgvdata.Rows)
                {
                    total += Convert.ToDecimal(fila.Cells["Subtotal"].Value.ToString());
                }             
            }
            txtTotal.Text = total.ToString("0.00");
        }

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 8)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.check.Width - 5;
                var h = Properties.Resources.check.Height - 5;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.delete, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "btneliminar")
            {
                int indice = e.RowIndex;
                if(indice >= 0)
                {
                    dgvdata.Rows.RemoveAt(indice);
                    CalcularTotal();
                }
               
            }
        }

        private void txtPrecioCompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if(txtPrecioCompra.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    if(Char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void txtPrecioVenta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtPrecioVenta.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    if (Char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if(Convert.ToInt32(txtIdProveedor.Text ) == 0)
            {
                MessageBox.Show("Debe seleccionar un proveedor","Mensaje",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                return;
            }

            if(dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("Debe ingresar productos en la compra", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if(txtNumeroFactura.Text == "")
            {
                MessageBox.Show("Debe ingresar el numero de la factura","Mensaje",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                return;
            }

            DataTable DetalleCompra = new DataTable();
            DetalleCompra.Columns.Add("IdProducto", typeof(int));
            DetalleCompra.Columns.Add("PrecioCompra", typeof(decimal));
            DetalleCompra.Columns.Add("PrecioVenta", typeof(decimal));
            DetalleCompra.Columns.Add("Cantidad", typeof(int));
            DetalleCompra.Columns.Add("MontoTotal", typeof(decimal));
            DetalleCompra.Columns.Add("Lote", typeof(string));
            DetalleCompra.Columns.Add("FechaVencimiento",typeof(string));

            foreach(DataGridViewRow row in dgvdata.Rows)
            {
                DetalleCompra.Rows.Add(new object[]
                {
                    Convert.ToInt32(row.Cells["Id"].Value.ToString()),
                    Convert.ToDecimal(row.Cells["PrecioCompra"].Value.ToString()),
                    Convert.ToDecimal(row.Cells["PrecioVenta"].Value.ToString()),
                    Convert.ToInt32(row.Cells["Cantidad"].Value.ToString()),
                    Convert.ToDecimal(row.Cells["SubTotal"].Value.ToString()),
                    row.Cells["Lote"].Value.ToString(),
                    row.Cells["FechaVencimiento"].Value.ToString()
                });
            }

            Compra compra = new Compra()
            {
                oUsuario = usuario,
                oProveedor = new Proveedor() { IdProveedor = Convert.ToInt32(txtIdProveedor.Text)},
                TipoDocumento = ((OpcionCombo)cboTipoDocumento.SelectedItem).Texto,
                NumeroDocumento = txtNumeroFactura.Text,
                MontoTotal = Convert.ToDecimal(txtTotal.Text),
            };

            string mensaje = String.Empty;
            bool respuesta = new CN_Compra().Registrar(compra,DetalleCompra, out mensaje);
            if (respuesta)
            {
                var result = MessageBox.Show("Numero de compra generado\n"+txtNumeroFactura.Text+"\n\n¿Desea copiar al portapapeles?",mensaje,MessageBoxButtons.YesNo,MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                    Clipboard.SetText(txtNumeroFactura.Text);

                txtIdProveedor.Text = "0";
                txtDocumentoProv.Text = "";
                txtNombreProv.Text = "";
                txtNumeroFactura.Text = "";
                dgvdata.Rows.Clear();
                CalcularTotal();

            }
            else
            {
                MessageBox.Show(mensaje,"Mensaje",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
        }




    }
}
