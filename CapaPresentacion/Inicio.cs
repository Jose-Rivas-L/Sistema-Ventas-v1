using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;
using FontAwesome.Sharp;

namespace CapaPresentacion
{
    public partial class Inicio : Form
    {
        private static  Usuario UsuarioActual;
        private static IconMenuItem MenuActivo = null;
        private static Form FormActivo = null;
        public Inicio(Usuario objusuario= null)
        {
            UsuarioActual = objusuario;
            InitializeComponent();
        }

        private void Inicio_Load(object sender, EventArgs e)
        {
            List<Permiso> ListaPermisos = new CN_Permiso().Listar(Convert.ToInt32(UsuarioActual.IdUsuario));
            foreach(IconMenuItem iconmenu in menu.Items)
            {
                bool encontrado = ListaPermisos.Any(p => p.NombreMenu == iconmenu.Name);
                if(encontrado == false)
                {
                    iconmenu.Visible = false;
                }

            }
            lblUsuario.Text = UsuarioActual.NombreCompleto.ToString();
        }

        private void AbrirForm(IconMenuItem menu, Form formulario)
        {
            if(MenuActivo != null)
            {
                MenuActivo.BackColor = Color.White;
            }
            menu.BackColor = Color.Silver;
            MenuActivo = menu;

            if (FormActivo != null)
            {
                FormActivo.Close();
            }
            FormActivo = formulario;
            formulario.TopLevel = false;
            formulario.FormBorderStyle = FormBorderStyle.None;
            formulario.Dock = DockStyle.Fill;
            formulario.BackColor = Color.SteelBlue;

            contenedor.Controls.Add(formulario);
            formulario.Show();
        }

        private void menuusuario_Click(object sender, EventArgs e)
        {
            AbrirForm((IconMenuItem)sender,new Frm_Usuarios());
        }

        private void SubMenuCategoria_Click(object sender, EventArgs e)
        {
            AbrirForm(menumantenedor, new Frm_Categoria());
        }

        private void SubMenuProducto_Click(object sender, EventArgs e)
        {
            AbrirForm(menumantenedor, new Frm_Producto());
        }

        private void SubMenuRegistrarVenta_Click(object sender, EventArgs e)
        {
            AbrirForm(menuventas, new Frm_Ventas(UsuarioActual));
        }

        private void SubMenuDetalleVenta_Click(object sender, EventArgs e)
        {
            AbrirForm(menuventas, new Frm_DetalleVenta());
        }

        private void SubMenuRegistrarCompra_Click(object sender, EventArgs e)
        {
            AbrirForm(menucompras, new Frm_Compras(UsuarioActual));
        }

        private void SubMenuVerDetalleCompra_Click(object sender, EventArgs e)
        {
            AbrirForm(menucompras, new Frm_DetalleCompra());
        }

        private void menuclientes_Click(object sender, EventArgs e)
        {
            AbrirForm(menuclientes, new Frm_Clientes());
        }

        private void menuproveedores_Click(object sender, EventArgs e)
        {
            AbrirForm(menuproveedores, new Frm_Proveedores());
        }



        private void SubMenuNegocio_Click(object sender, EventArgs e)
        {
            AbrirForm(menureportes, new Frm_Negocio());
        }

        private void submenureportecompras_Click(object sender, EventArgs e)
        {
            AbrirForm(menureportes, new Frm_ReporteCompras());
        }

        private void submenureporteventas_Click(object sender, EventArgs e)
        {
            AbrirForm(menureportes, new Frm_ReporteVentas());
        }
    }
}
