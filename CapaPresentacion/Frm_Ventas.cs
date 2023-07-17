using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
using CapaPresentacion.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class Frm_Ventas : Form
    {
        private Usuario usuario;
        public Frm_Ventas(Usuario objusuario = null)
        {
            this.usuario = objusuario;
            InitializeComponent();
        }

        private void Frm_Ventas_Load(object sender, EventArgs e)
        {
            cboTipoDocumento.Items.Add(new OpcionCombo() { Valor = "Boleta", Texto = "Boleta" });
            cboTipoDocumento.Items.Add(new OpcionCombo() { Valor = "Factura", Texto = "Factura" });
            cboTipoDocumento.DisplayMember = "Texto";
            cboTipoDocumento.ValueMember = "Valor";
            cboTipoDocumento.SelectedIndex = 0;

            txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

            txtIdProducto.Text = "0";
        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            using (var modal = new MdCliente())
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK)
                {
                    /*txtIdCliente.Text = modal._Cliente.IdCliente.ToString();*/
                    txtDocCliente.Text = modal._Cliente.Documento.ToString();
                    txtNombreCliente.Text = modal._Cliente.NombreCompleto.ToString();
                    txtCodProducto.Select();
                }
                else
                {
                    txtDocCliente.Select();
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
                    txtPrecio.Text = modal._Producto.PrecioVenta.ToString();
                    txtStock.Text = modal._Producto.Stock.ToString();
                    txtCantidad.Select();
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
                    txtPrecio.Text = oProducto.PrecioVenta.ToString();
                    txtStock.Text = oProducto.Stock.ToString();
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
            decimal precio = 0;
            bool producto_existe = false;

            if (int.Parse(txtIdProducto.Text) == 0)
            {
                MessageBox.Show("Debe seleccionar un producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!decimal.TryParse(txtPrecio.Text, out precio))
            {
                MessageBox.Show("Precio Venta - Formato de moneda incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPrecio.Select();
                return;
            }
            if (txtCantidad.Value > Convert.ToInt32(txtStock.Text))
            {
                MessageBox.Show("La cantidad sobrepasa las existencias del producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPrecio.Select();
                return;
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
                precio.ToString("0.00"),
                txtCantidad.Value.ToString(),
                (txtCantidad.Value * precio).ToString("0.00")
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
            txtPrecio.Text = "";
            txtStock.Text = "";
            txtCantidad.Value = 1;
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

            if (e.ColumnIndex == 5)
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
                if (indice >= 0)
                {
                    dgvdata.Rows.RemoveAt(indice);
                    CalcularTotal();
                }

            }
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtPrecio.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
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

        private void txtPago_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtPago.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
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

        private void CalcularCambio()
        {
            if(txtTotal.Text.Trim() == "")
            {
                MessageBox.Show("No existen productos en la venta","Mensaje",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                return;
            }

            decimal pagacon;
            decimal total = Convert.ToDecimal(txtTotal.Text);

            if(txtPago.Text.Trim() == "")
            {
                txtPago.Text = "0";
            }

            if(Decimal.TryParse(txtPago.Text.Trim(), out pagacon))
            {
                if(pagacon < total)
                {
                    txtCambio.Text = "0.00";
                }
                else
                {
                    decimal cambio = pagacon - total;
                    txtCambio.Text = cambio.ToString("0.00");
                }
            }
        }

        private void txtPago_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                CalcularCambio();
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if ((txtDocCliente.Text) == "")
            {
                MessageBox.Show("Debe ingresar el documento del cliente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if ((txtNombreCliente.Text) == "")
            {
                MessageBox.Show("Debe ingresar el nombre del cliente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("Debe registrar productos en la venta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            DataTable DetalleVenta = new DataTable();
            DetalleVenta.Columns.Add("IdProducto", typeof(int));
            DetalleVenta.Columns.Add("PrecioVenta", typeof(decimal));
            DetalleVenta.Columns.Add("Cantidad", typeof(int));
            DetalleVenta.Columns.Add("SubTotal", typeof(decimal));

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                DetalleVenta.Rows.Add(new object[]
                {
                    Convert.ToInt32(row.Cells["Id"].Value.ToString()),
                    Convert.ToDecimal(row.Cells["Precio"].Value.ToString()),
                    Convert.ToInt32(row.Cells["Cantidad"].Value.ToString()),
                    Convert.ToDecimal(row.Cells["SubTotal"].Value.ToString())
                });
            }
            int idcorrelativo = new CN_Venta().ObtenerCorrelativo();
            string numerodoc = string.Format("{0:00000}", idcorrelativo);
            CalcularCambio();
            Venta venta = new Venta()
            {
                oUsuario = usuario,
                TipoDocumento = ((OpcionCombo)cboTipoDocumento.SelectedItem).Texto,
                NumeroDocumento = numerodoc,
                DocumentoCliente = txtDocCliente.Text,
                NombreCliente = txtNombreCliente.Text,
                MontoPago = Convert.ToDecimal(txtPago.Text),
                MontoCambio = Convert.ToDecimal(txtCambio.Text),
                MontoTotal = Convert.ToDecimal(txtTotal.Text),
            };

            string mensaje = string.Empty;
            bool respuesta = new CN_Venta().Registrar(venta, DetalleVenta, out mensaje);
            if (respuesta)
            {
                var result = MessageBox.Show("Numero de venta generado\n" + numerodoc + "\n\n¿Desea copiar al portapapeles?", mensaje, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                    Clipboard.SetText(numerodoc);

                txtDocCliente.Text = "";
                txtNombreCliente.Text = "";
                txtCambio.Text = "";       
                dgvdata.Rows.Clear();
                LimpiarProducto();
                CalcularTotal();
                CalcularCambio();
                
            }
            else
            {
                MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
