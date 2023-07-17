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

namespace CapaPresentacion.Modales
{
    public partial class MdProducto : Form
    {
        public Producto _Producto { get; set; }
        public MdProducto()
        {
            InitializeComponent();
        }

        private void MdProducto_Load(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                if (columna.Visible == true)
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
                dgvdata.Rows.Add(new object[] {Productos.IdProducto, Productos.Codigo, Productos.Nombre,
               Productos.oCategoria.Descripcion,
               Productos.Stock,
               Productos.PrecioCompra,
               Productos.PrecioVenta
               });
            }
        }

        private void dgvdata_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int iRow = e.RowIndex;
            int column = e.ColumnIndex;
            if (iRow >= 0 && column > 0)
            {
                _Producto = new Producto()
                {
                    IdProducto = Convert.ToInt32(dgvdata.Rows[iRow].Cells["IdProducto"].Value.ToString()),
                    Codigo = dgvdata.Rows[iRow].Cells["Codigo"].Value.ToString(),
                    Nombre = dgvdata.Rows[iRow].Cells["Nombre"].Value.ToString(),
                    Stock = Convert.ToInt32(dgvdata.Rows[iRow].Cells["Stock"].Value.ToString()),
                    PrecioCompra = Convert.ToDecimal(dgvdata.Rows[iRow].Cells["PrecioCompra"].Value.ToString()),
                    PrecioVenta = Convert.ToDecimal(dgvdata.Rows[iRow].Cells["PrecioVenta"].Value.ToString()),
                };
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnBuscar1_Click(object sender, EventArgs e)
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
    }
}
