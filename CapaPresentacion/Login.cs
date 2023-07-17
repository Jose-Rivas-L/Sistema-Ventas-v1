using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaNegocio;
using CapaEntidad;

namespace CapaPresentacion
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            List<Usuario> users = new CN_Usuario().Listar();
            Usuario oUsuario = new CN_Usuario().Listar().Where(u => u.Documento == txtDocumento.Text && u.Clave == txtClave.Text).FirstOrDefault();

            if(oUsuario != null)
            {
                Inicio form = new Inicio(oUsuario);
                form.Show();
                this.Hide();
                form.FormClosing += frm_closing;
            }
            else
            {
                MessageBox.Show("No se ha encontrado el usuario, intente de nuevo","Error",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                Limpiar();
            }
           
        }

        private void Limpiar()
        {
            txtClave.Clear();
            txtDocumento.Clear();
        }
        private void frm_closing(object sender, FormClosingEventArgs e)
        {
            txtClave.Clear();
            txtDocumento.Clear();
            this.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
