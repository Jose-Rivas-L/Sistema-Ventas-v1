using CapaEntidad;
using CapaNegocio;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
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
    public partial class Frm_DetalleCompra : Form
    {
        public Frm_DetalleCompra()
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Compra compra = new CN_Compra().ObtenerCompra(txtDocCompra.Text);
            if(compra.IdCompra != 0)
            {
                txtFecha.Text = compra.FechaRegistro;
                txtTipoDoc.Text = compra.TipoDocumento;
                txtUsuario.Text = compra.oUsuario.NombreCompleto;
                txtDocumentoProv.Text = compra.oProveedor.Documento;
                txtNombreProv.Text = compra.oProveedor.RazonSocial;
                txtNumDocumento.Text = txtDocCompra.Text;
                txtTotal.Text = compra.MontoTotal.ToString();
                dgvdata.Rows.Clear();
                foreach(Detalle_Compra detalle in compra.oDetalleCompra)
                {
                    dgvdata.Rows.Add(new object[] { detalle.oProducto.Nombre, detalle.PrecioCompra, 
                        detalle.Cantidad, detalle.MontoTotal });
                }
                

            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtDocCompra.Text = "";
            txtFecha.Text = "";
            txtTipoDoc.Text = "";
            txtUsuario.Text = "";
            txtDocumentoProv.Text = "";
            txtNombreProv.Text = "";
            txtNumDocumento.Text = "";
            txtTotal.Text = "0.00";
            dgvdata.Rows.Clear();
        }

        private void btnGenerarPdf_Click(object sender, EventArgs e)
        {
            if (txtTipoDoc.Text == "")
            {
                MessageBox.Show("No hay datos para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            string texto_Html = Properties.Resources.PlantillaCompra.ToString();
            Negocio oDatos = new CN_Negocio().ObtenerDatos();
            texto_Html = texto_Html.Replace("@nombrenegocio", oDatos.Nombre.ToUpper());
            texto_Html = texto_Html.Replace("@docnegocio", oDatos.RUC);
            texto_Html = texto_Html.Replace("@direcnegocio", oDatos.Direccion);

            texto_Html = texto_Html.Replace("@tipodocumento", txtTipoDoc.Text.ToUpper());
            texto_Html = texto_Html.Replace("@numerodocumento", txtNumDocumento.Text);


            texto_Html = texto_Html.Replace("@docproveedor", txtDocumentoProv.Text);
            texto_Html = texto_Html.Replace("@nombreproveedor", txtNombreProv.Text);
            texto_Html = texto_Html.Replace("@fecharegistro", txtFecha.Text);
            texto_Html = texto_Html.Replace("@usuarioregistro", txtUsuario.Text);
            string filas = string.Empty;
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + row.Cells["Producto"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["PrecioCompra"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Subtotal"].Value.ToString() + "</td>";
                filas += "</tr>";
            }
            texto_Html = texto_Html.Replace("@filas", filas);
            texto_Html = texto_Html.Replace("@montototal", txtTotal.Text);

            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("Venta_{0}.pdf", txtNumDocumento.Text);
            savefile.Filter = "pdf files| *.pdf";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(savefile.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);

                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();

                    bool Obtenido = true;
                    byte[] imagen = new CN_Negocio().ObtenerLogo(out Obtenido);
                    if (Obtenido)
                    {
                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imagen);
                        img.ScaleToFit(60, 60);
                        img.Alignment = iTextSharp.text.Image.UNDERLYING;
                        img.SetAbsolutePosition(pdfDoc.Left, pdfDoc.GetTop(51));
                        pdfDoc.Add(img);
                    }

                    using (StringReader sr = new StringReader(texto_Html))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    }
                    pdfDoc.Close();
                    stream.Close();
                    MessageBox.Show("Documento Generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

    }
}
