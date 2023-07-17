using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaPresentacion.Utilidades;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion
{
    public partial class Frm_Usuarios : Form
    {
        public Frm_Usuarios()
        {
            InitializeComponent();
        }

        private void Frm_Usuarios_Load(object sender, EventArgs e)
        {
            cboEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto ="Activo" });
            cboEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cboEstado.DisplayMember = "Texto";
            cboEstado.ValueMember = "Valor";
            cboEstado.SelectedIndex = 0;

            //Llenar el combobox con los roles
            List<Rol> ListaRol = new CN_Rol().Listar();

            foreach (Rol roles in ListaRol)
            {
                cboRol.Items.Add(new OpcionCombo() { Valor = roles.IdRol, Texto = roles.Descripcion});
            }
            cboRol.DisplayMember = "Texto";
            cboRol.ValueMember = "Valor";
            cboRol.SelectedIndex = 0;

            foreach(DataGridViewColumn columna in dgvdata.Columns)
            {
                if (columna.Visible == true && columna.Name != "btnseleccionar");
                {
                    cboBusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }
            cboBusqueda.DisplayMember = "Texto";
            cboBusqueda.ValueMember = "Valor";
            cboBusqueda.SelectedIndex = 0;

            //Mostrar usuarios en datagrid
            List<Usuario> ListaUsuario = new CN_Usuario().Listar();

            foreach (Usuario usuarios in ListaUsuario)
            {
                dgvdata.Rows.Add(new object[] { "", usuarios.IdUsuario, usuarios.Documento, usuarios.NombreCompleto, usuarios.Correo,usuarios.Clave,
               usuarios.oRol.IdRol,
               usuarios.oRol.Descripcion,
               usuarios.Estado == true ? 1 : 0,
               usuarios.Estado == true ? "Activo" : "No Activo"
               });
            }


        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            Usuario objusuario = new Usuario()
            {
                IdUsuario = Convert.ToInt32(txtId.Text),
                Documento = txtDocumento.Text,
                NombreCompleto = txtNombreCompleto.Text,
                Correo = txtCorreo.Text,
                Clave = txtClave.Text,
                oRol = new Rol() { IdRol = Convert.ToInt32(((OpcionCombo)cboRol.SelectedItem).Valor)},
                Estado = Convert.ToInt32(((OpcionCombo)cboEstado.SelectedItem).Valor) == 1 ? true: false
            };
            if(objusuario.IdUsuario == 0)
            {
                int idusuariogenerado = new CN_Usuario().Registrar(objusuario, out mensaje);
                if (idusuariogenerado != 0)
                {
                    dgvdata.Rows.Add(new object[] { "", idusuariogenerado, txtDocumento.Text, txtNombreCompleto.Text, txtCorreo.Text,txtClave.Text,
               ((OpcionCombo)cboRol.SelectedItem).Valor.ToString(),
               ((OpcionCombo)cboRol.SelectedItem).Texto.ToString(),
               ((OpcionCombo)cboEstado.SelectedItem).Valor.ToString(),
               ((OpcionCombo)cboEstado.SelectedItem).Texto.ToString()
                });
                    Limpiar();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }
            else
            {
                bool resultado = new CN_Usuario().Editar(objusuario, out mensaje);
                if (resultado)
                {
                    DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtIndice.Text)];
                    row.Cells["Id"].Value = txtId.Text;
                    row.Cells["Documento"].Value = txtDocumento.Text;
                    row.Cells["NombreCompleto"].Value = txtNombreCompleto.Text;
                    row.Cells["Correo"].Value = txtCorreo.Text;
                    row.Cells["Clave"].Value = txtClave.Text;
                    row.Cells["IdRol"].Value = ((OpcionCombo)cboRol.SelectedItem).Valor.ToString();
                    row.Cells["Rol"].Value = ((OpcionCombo)cboRol.SelectedItem).Texto.ToString();
                    row.Cells["EstadoValor"].Value = ((OpcionCombo)cboEstado.SelectedItem).Valor.ToString();
                    row.Cells["Estado"].Value = ((OpcionCombo)cboEstado.SelectedItem).Texto.ToString();
                    Limpiar();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }
            
        }

        public void Limpiar()
        {
            txtIndice.Text = "-1";
            txtId.Text = "0";
            txtDocumento.Text = "";
            txtNombreCompleto.Text = "";
            txtCorreo.Text = "";
            txtClave.Text = "";
            txtConfirmarClave.Text = "";
            cboEstado.SelectedIndex = 0;
            cboRol.SelectedIndex = 0;

            txtDocumento.Select();
        }


        //Pintar la seleccion de las filas del datagrid
        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if(e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds,DataGridViewPaintParts.All);

                var w = Properties.Resources.check.Width-5;
                var h = Properties.Resources.check.Height-5;
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

                if(indice >= 0)
                {
                    txtIndice.Text = indice.ToString();
                    txtId.Text = dgvdata.Rows[indice].Cells["Id"].Value.ToString();
                    txtDocumento.Text = dgvdata.Rows[indice].Cells["Documento"].Value.ToString();
                    txtNombreCompleto.Text = dgvdata.Rows[indice].Cells["NombreCompleto"].Value.ToString();
                    txtCorreo.Text = dgvdata.Rows[indice].Cells["Correo"].Value.ToString();
                    txtClave.Text = dgvdata.Rows[indice].Cells["Clave"].Value.ToString();
                    txtConfirmarClave.Text = dgvdata.Rows[indice].Cells["Clave"].Value.ToString();
                    //cambia el rol del combobox segun el usuario seleccionado
                    foreach (OpcionCombo oc in cboRol.Items)
                    {
                        if(Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["IdRol"].Value.ToString()))
                        {
                            int ind_combo = cboRol.Items.IndexOf(oc);
                            cboRol.SelectedIndex = ind_combo;
                            break;
                        }
                    }

                    //cambia el estado del combobox segun el usuario seleccionado
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
            if(Convert.ToInt32(txtId.Text) != 0)
            {
                if (MessageBox.Show("Desea eliminar el usuario", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje = string.Empty;
                    Usuario objusuario = new Usuario()
                    {
                        IdUsuario = Convert.ToInt32(txtId.Text),
                    };
                    bool respuesta = new CN_Usuario().Eliminar(objusuario, out mensaje);

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
            if(dgvdata.Rows.Count > 0)
            {
                foreach(DataGridViewRow Row in dgvdata.Rows)
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
            foreach(DataGridViewRow Row in dgvdata.Rows)
            {
                Row.Visible = true;
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }
    }
}
