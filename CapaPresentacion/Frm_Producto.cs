using CapaEntidad;
using CapaNegocio;
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
using ClosedXML;
using ClosedXML.Excel;

namespace CapaPresentacion
{
    public partial class Frm_Producto : Form
    {
        public Frm_Producto()
        {
            InitializeComponent();
        }

        private void Frm_Producto_Load(object sender, EventArgs e)
        {
            cboEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cboEstado.DisplayMember = "Texto";
            cboEstado.ValueMember = "Valor";
            cboEstado.SelectedIndex = 0;

            //Llenar el combobox con los Categoriaes
            List<Categoria> ListaCategoria = new CN_Categoria().Listar();

            foreach (Categoria Categorias in ListaCategoria)
            {
                cboCategoria.Items.Add(new OpcionCombo() { Valor = Categorias.IdCategoria, Texto = Categorias.Descripcion });
            }
            cboCategoria.DisplayMember = "Texto";
            cboCategoria.ValueMember = "Valor";
            cboCategoria.SelectedIndex = 0;

            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                if (columna.Visible == true && columna.Name != "btnseleccionar")
                {
                    cboBusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }
            cboBusqueda.DisplayMember = "Texto";
            cboBusqueda.ValueMember = "Valor";
            cboBusqueda.SelectedIndex = 0;

            //Mostrar Productos en datagrid
            List<Producto> ListaProducto = new CN_Producto().Listar();

            foreach (Producto Productos in ListaProducto)
            {
                dgvdata.Rows.Add(new object[] { "", Productos.IdProducto, Productos.Codigo, Productos.Nombre,Productos.Descripcion, 
               Productos.oCategoria.IdCategoria,
               Productos.oCategoria.Descripcion,
               Productos.Lote,
               Productos.Invima,
               Productos.Stock,
               Productos.PrecioCompra,
               Productos.PrecioVenta,
               Productos.Estado == true ? 1 : 0,
               Productos.Estado == true ? "Activo" : "No Activo",
               Productos.FechaVencimiento

               });
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            Producto objProducto = new Producto()
            {
                IdProducto = Convert.ToInt32(txtId.Text),
                Codigo = txtCodigo.Text,
                Nombre = txtNombre.Text,
                Descripcion = txtDescripcion.Text,
                Lote = txtLote.Text,
                Invima = txtInvima.Text,
                oCategoria = new Categoria() { IdCategoria = Convert.ToInt32(((OpcionCombo)cboCategoria.SelectedItem).Valor) },
                Estado = Convert.ToInt32(((OpcionCombo)cboEstado.SelectedItem).Valor) == 1 ? true : false,
                FechaVencimiento = txtFechaVencimiento.Value.ToString(),
            };
            if (objProducto.IdProducto == 0)
            {
                int idProductogenerado = new CN_Producto().Registrar(objProducto, out mensaje);
                if (idProductogenerado != 0)
                {
                    dgvdata.Rows.Add(new object[] { "", idProductogenerado, txtCodigo.Text, txtNombre.Text, txtDescripcion.Text,
                    ((OpcionCombo)cboCategoria.SelectedItem).Valor.ToString(),
                    ((OpcionCombo)cboCategoria.SelectedItem).Texto.ToString(),
                    txtLote.Text,
                    txtInvima.Text,
                    "0",
                    "0.00",
                    "0.00",
                    ((OpcionCombo)cboEstado.SelectedItem).Valor.ToString(),
                    ((OpcionCombo)cboEstado.SelectedItem).Texto.ToString(),
                    txtFechaVencimiento.Value.ToString()
                    }) ;
                    Limpiar();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }
            else
            {
                bool resultado = new CN_Producto().Editar(objProducto,txtLoteAntiguo.Text, out mensaje);
                if (resultado)
                {
                    DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtIndice.Text)];
                    row.Cells["IdProducto"].Value = txtId.Text;
                    row.Cells["Codigo"].Value = txtCodigo.Text;
                    row.Cells["Nombre"].Value = txtNombre.Text;
                    row.Cells["Descripcion"].Value = txtDescripcion.Text;           
                    row.Cells["IdCategoria"].Value = ((OpcionCombo)cboCategoria.SelectedItem).Valor.ToString();
                    row.Cells["Categoria"].Value = ((OpcionCombo)cboCategoria.SelectedItem).Texto.ToString();
                    row.Cells["Lote"].Value = txtLote.Text;
                    row.Cells["Invima"].Value = txtInvima.Text;
                    row.Cells["EstadoValor"].Value = ((OpcionCombo)cboEstado.SelectedItem).Valor.ToString();
                    row.Cells["Estado"].Value = ((OpcionCombo)cboEstado.SelectedItem).Texto.ToString();
                    row.Cells["FechaVencimiento"].Value = txtFechaVencimiento.Value.ToString();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }
                Limpiar();
            }
        }

        public void Limpiar()
        {
            txtIndice.Text = "-1";
            txtId.Text = "0";
            txtCodigo.Text = "";
            txtDescripcion.Text = "";
            txtNombre.Text = "";
            txtInvima.Text = "";
            txtLote.Text = "";
            cboEstado.SelectedIndex = 0;
            cboCategoria.SelectedIndex = 0;

            txtCodigo.Select();
        }
        //Pintar la seleccion de las filas del datagrid
        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.check.Width - 5;
                var h = Properties.Resources.check.Height - 5;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.check, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }
        //Pasa la informacion del datagrid a los campos de texto

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "btnseleccionar")
            {
                int indice = e.RowIndex;

                if (indice >= 0)
                {
                    txtIndice.Text = indice.ToString();
                    txtId.Text = dgvdata.Rows[indice].Cells["IdProducto"].Value.ToString();
                    txtCodigo.Text = dgvdata.Rows[indice].Cells["Codigo"].Value.ToString();
                    txtNombre.Text = dgvdata.Rows[indice].Cells["Nombre"].Value.ToString();
                    txtDescripcion.Text = dgvdata.Rows[indice].Cells["Descripcion"].Value.ToString();
                    txtInvima.Text = dgvdata.Rows[indice].Cells["Invima"].Value.ToString();
                    txtLote.Text = dgvdata.Rows[indice].Cells["Lote"].Value.ToString();
                    txtLoteAntiguo.Text = txtLote.Text;
                    //cambia la Categoria del combobox segun el Producto seleccionado
                    foreach (OpcionCombo oc in cboCategoria.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["IdCategoria"].Value.ToString()))
                        {
                            int ind_combo = cboCategoria.Items.IndexOf(oc);
                            cboCategoria.SelectedIndex = ind_combo;
                            break;
                        }
                    }

                    //cambia el estado del combobox segun el Producto seleccionado
                    foreach (OpcionCombo oc in cboEstado.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["EstadoValor"].Value.ToString()))
                        {
                            int ind_combo = cboEstado.Items.IndexOf(oc);
                            cboEstado.SelectedIndex = ind_combo;
                            break;
                        }
                    }
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtId.Text) != 0)
            {
                if (MessageBox.Show("Desea eliminar el Producto", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje = string.Empty;
                    Producto objProducto = new Producto()
                    {
                        IdProducto = Convert.ToInt32(txtId.Text),
                    };
                    bool respuesta = new CN_Producto().Eliminar(objProducto, out mensaje);

                    if (respuesta)
                    {
                        dgvdata.Rows.RemoveAt(Convert.ToInt32(txtIndice.Text));
                        Limpiar();
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cboBusqueda.SelectedItem).Valor.ToString();
            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow Row in dgvdata.Rows)
                {
                    if (Row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtBusqueda.Text.Trim().ToUpper()) == true)
                    {
                        Row.Visible = true;
                    }
                    else
                        Row.Visible = false;
                }
            }
        }

        private void btnLimpiarBuscador_Click(object sender, EventArgs e)
        {
            txtBusqueda.Text = "";
            foreach (DataGridViewRow Row in dgvdata.Rows)
            {
                Row.Visible = true;
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if(dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("No hay datos para exportar","Mensaje",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
            else
            {
                DataTable tabla = new DataTable();
                foreach(DataGridViewColumn columna in dgvdata.Columns)
                {
                    if(columna.HeaderText != "" && columna.Visible)
                    {
                        tabla.Columns.Add(columna.HeaderText, typeof(string));
                    }
                    
                }
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    if (row.Visible)
                    {
                        tabla.Rows.Add(new object[] { 
                            row.Cells[2].Value.ToString(),
                            row.Cells[3].Value.ToString(),
                            row.Cells[4].Value.ToString(),
                            row.Cells[6].Value.ToString(),
                            row.Cells[7].Value.ToString(),
                            row.Cells[8].Value.ToString(),
                            row.Cells[9].Value.ToString(),
                            row.Cells[11].Value.ToString(),

                        });
                    }

                }
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.FileName = string.Format("ReporteProducto_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
                saveFile.Filter = "Excel Files | *.xlsx";
                if(saveFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        XLWorkbook wb = new XLWorkbook();
                        var hoja = wb.Worksheets.Add(tabla, "Informe");
                        hoja.ColumnsUsed().AdjustToContents();
                        wb.SaveAs(saveFile.FileName);
                        MessageBox.Show("Reporte generado","Mensaje",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show("No se pudo generar el reporte", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

    }
}
