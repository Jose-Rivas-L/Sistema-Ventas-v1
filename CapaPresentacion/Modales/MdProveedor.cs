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
    public partial class MdProveedor : Form
    {

        public Proveedor _Proveedor { get; set; }
        public MdProveedor()
        {
            InitializeComponent();
        }

        private void MdProveedor_Load(object sender, EventArgs e)
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
            List<Proveedor> ListaProveedor = new CN_Proveedor().Listar();

            foreach (Proveedor Proveedors in ListaProveedor)
            {
                dgvdata.Rows.Add(new object[] {Proveedors.IdProveedor, Proveedors.Documento, Proveedors.RazonSocial
               });
            }
        }

        private void dgvdata_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int iRow = e.RowIndex;
            int column = e.ColumnIndex;
            if(iRow >= 0 && column > 0) 
            {
                _Proveedor = new Proveedor()
                {
                    IdProveedor = Convert.ToInt32(dgvdata.Rows[iRow].Cells["Id"].Value.ToString()),
                    Documento = dgvdata.Rows[iRow].Cells["Documento"].Value.ToString(),
                    RazonSocial = dgvdata.Rows[iRow].Cells["RazonSocial"].Value.ToString()
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
