using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class Frm_Negocio : Form
    {
        public Frm_Negocio()
        {
            InitializeComponent();
        }

        public Image ByteToImage(byte[] imageBytes)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(imageBytes, 0, imageBytes.Length);

            Image imagen = new Bitmap(ms);
            return imagen;
        }

        private void Frm_Negocio_Load(object sender, EventArgs e)
        {
            bool obtenido = true;
            byte[] imagen = new CN_Negocio().ObtenerLogo(out obtenido);

            if (obtenido)
                picLogo.Image = ByteToImage(imagen);

            Negocio datos = new CN_Negocio().ObtenerDatos();

            txtNombre.Text = datos.Nombre;
            txtRuc.Text = datos.RUC;
            txtDireccion.Text = datos.Direccion;
        }

        private void btnSubir_Click(object sender, EventArgs e)
        {
            string mensaje = String.Empty;
            OpenFileDialog oOpenFile = new OpenFileDialog();
            oOpenFile.FileName = "Files |*.jpg;*.jpeg;*.png";
            if(oOpenFile.ShowDialog() == DialogResult.OK)
            {
                byte[] bytesImage = File.ReadAllBytes(oOpenFile.FileName);
                bool respuesta = new CN_Negocio().ActualizarLogo(bytesImage,out mensaje);
                if (respuesta)
                {
                    picLogo.Image = ByteToImage(bytesImage);
                }
                else
                {
                    MessageBox.Show(mensaje,"Mensaje",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = String.Empty;

            Negocio obj = new Negocio()
            {
                Nombre = txtNombre.Text,
                RUC = txtRuc.Text,
                Direccion = txtDireccion.Text
            };
            bool respuesta = new CN_Negocio().GuardarDatos(obj, out mensaje);
            if (respuesta)
            {
                MessageBox.Show("Los cambios fueron guardados", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Los cambios no fueron guardados","Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
