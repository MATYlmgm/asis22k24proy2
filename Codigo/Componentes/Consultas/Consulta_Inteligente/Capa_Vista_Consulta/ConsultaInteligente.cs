﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Capa_Controlador_Consulta;
using System.Data.Odbc;
using System.Data;

namespace Capa_Vista_Consulta
{
    public partial class ConsultaInteligente : Form
    {
        consultaControlador csControlador = new consultaControlador();
        string tablabusqueda;
        private string[] datos;
        private string[] datosComplejo;
        private string[] tipos;
        private string consultaSeleccionada;


        public ConsultaInteligente()
        {
            InitializeComponent();
            string BD = "consultasBD";
            tipos = new string[] { "nombre_consulta", "tipo_consulta", "consulta_SQLE", "consulta_estatus" };
            csControlador.CargarTablas(cboTabla, BD);
            csControlador.CargarTablas(cboEditarTabla, BD);
            cboTabla.SelectedIndexChanged += new EventHandler(cboTabla_SelectedIndexChanged);
            cboEditarTabla.SelectedIndexChanged += new EventHandler(cboTablaEditar_SelectedIndexChanged);
            chbCondiciones.CheckedChanged += chbCondiciones_CheckedChanged;
            gbCondiciones.Enabled = false;
            gbOrdenar.Enabled = false;
            gbListadoConsultas.Enabled = true;
            gbEditarLogica.Enabled = false;
            gbEditarOrden.Enabled = false;
            csControlador.obtenerNombresConsultas(cboQuery1);
            cboQuery1.SelectedIndexChanged += new EventHandler(cboConsultas_SelectedIndexChanged);
            csControlador.obtenerNombresConsultas(cboQuery3);
            cboQuery3.SelectedIndexChanged += new EventHandler(cboConsultas_SelectedIndexChanged);
            csControlador.obtenerNombresConsultas(txtEditarNombreConsulta);
            txtEditarNombreConsulta.SelectedIndexChanged += new EventHandler(cboConsultas_SelectedIndexChanged);
            llenarComboLogico(cboLogico);
            llenarComboComparador(cboComparador);
            llenarComboOrden(cboOrdenar);
        }
        string consulta = "";
        string tabla = "tbl_consultaInteligente";
        private void cboTabla_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Verifica si se ha seleccionado una tabla
            if (!string.IsNullOrEmpty(cboTabla.Text))
            {
                // Llama al método para llenar el segundo ComboBox con las columnas de la tabla seleccionada
                csControlador.obtenerColumbasPorTabla(cboCampos, cboTabla.Text);
                csControlador.obtenerColumbasPorTabla(cboComparadorCampo, cboTabla.Text);
                csControlador.obtenerColumbasPorTabla(cboLogicoCampo, cboTabla.Text);
                csControlador.obtenerColumbasPorTabla(cboOrdenarCampo, cboTabla.Text);
            }
        }
        private void llenarComboLogico(ComboBox comboBox1)
        {
            comboBox1.Items.Add("Seleccionar"); comboBox1.Items.Add("OR");
            comboBox1.Items.Add("AND"); comboBox1.Items.Add("NOT"); comboBox1.SelectedIndex = 0;
        }
        private void llenarComboComparador(ComboBox comboBox1)
        {
            comboBox1.Items.Add("Seleccionar"); comboBox1.Items.Add("WHERE");comboBox1.SelectedIndex = 0;
        }
        private void llenarComboOrden(ComboBox comboBox1)
        {
            comboBox1.Items.Add("Seleccionar"); comboBox1.Items.Add("Ordenar"); comboBox1.Items.Add("Agrupar"); comboBox1.SelectedIndex = 0;
        }
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            //boton agregar, consulta simple
            // Datos que se van a procesar
            datos = new string[] { txtNombreConsulta.Text, "0", cboTabla.Text, chbTodosCampos.Text, txtQueryFinal.Text, "1" };

            // Inicializamos la variable para los campos que se mostrarán en el query
            string camposSeleccionados;

            // Verificamos si el CheckBox de 'Todos los campos' está marcado
            if (chbTodosCampos.Checked)
            {
                // Si está marcado, mostramos "Todos los campos"
                camposSeleccionados = "*";
            }
            else
            {
                // Si no está marcado, verificamos si hay un campo seleccionado en el ComboBox
                if (!string.IsNullOrEmpty(cboCampos.Text))
                {
                    // Si hay un campo seleccionado, lo mostramos
                    camposSeleccionados = cboCampos.Text;
                }
                else
                {
                    // Si no hay campo seleccionado, dejamos un valor vacío o un mensaje de advertencia
                    MessageBox.Show("Debe seleccionar o un campo o todos", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (string.IsNullOrEmpty(txtQuery.Text))
            {
                // Si txtQuery está vacío, asignamos el valor inicial
                txtQuery.Text = camposSeleccionados + " + " + txtAlias.Text + " + ";
                datos[4] = txtQuery.Text;
            }
            else
            {
                // Si txtQuery ya tiene texto, agregamos el nuevo valor sin repetir el nombre de la consulta y el tipo
                txtQuery.Text += Environment.NewLine + camposSeleccionados + " + " + txtAlias.Text + " + ";
                datos[4] += txtQuery.Text;
            }

            // Procesar los datos en la tabla correspondiente
            csControlador.ingresar(tipos, datos, "tbl_consultaInteligente");
        }

        private void chbCondiciones_CheckedChanged(object sender, EventArgs e)
        {
            bool habilitarControles = chbCondiciones.Checked;
            gbCondiciones.Enabled = habilitarControles;
            gbOrdenar.Enabled = habilitarControles;
            gbListadoConsultas.Enabled = habilitarControles;
            gbEditarLogica.Enabled = habilitarControles;
            gbEditarOrden.Enabled = habilitarControles;
            if (datos != null && datos.Length > 0)
            {
                datos[1] = "1";
                Console.WriteLine("Se habilitó la consulta Compleja.");
            }
            else
            {
                datos[1] = "0";
                Console.WriteLine("Se habilitó la consulta Compleja.");
            }
        }

        private void btnAgregarComparacion_Click(object sender, EventArgs e)
        {
            datosComplejo = new string[] { cboComparador.Text, cboComparadorCampo.Text, txtValorComparador.Text, txtQueryFinal.Text };
            csControlador.ingresar(tipos, datosComplejo, "tbl_consultaInteligente");
            string queryGenerado = csControlador.GenerarQueryComplejo(datosComplejo, datos);
            txtQueryFinal.Clear(); txtQueryFinal.Text = queryGenerado; datosComplejo = new string[0];
            string eliminar;
        }

        private void brnAgregarLogica_Click(object sender, EventArgs e)
        {
            datosComplejo = new string[] { cboLogico.Text, cboLogicoCampo.Text, txtValorLogico.Text, txtQueryFinal.Text};
            csControlador.ingresar(tipos, datosComplejo, "tbl_consultaInteligente");
            string queryGenerado = csControlador.GenerarQueryComplejo(datosComplejo, datos);
            txtQueryFinal.Clear();txtQueryFinal.Text = queryGenerado;datosComplejo = new string[0];
            string eliminar;
        }

        private void btnAgregarConsultaSimple_Click(object sender, EventArgs e)
        {
            string queryGenerado = csControlador.GenerarQuerySimple(datos);
            txtQueryFinal.Text = queryGenerado;
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            // Procesar los datos en la tabla correspondiente
            csControlador.InsertarDatos(tipos, datos, tabla);
        }

        private void btnAgregarOrden_Click(object sender, EventArgs e)
        {
            datosComplejo = new string[] { cboOrdenar.Text, cboOrdenarCampo.Text, "0", txtQueryFinal.Text };
            if (cboOrdenar.Text == "Ordenar") {
                if (chbOrdenAscendente.Checked) {
                    datosComplejo[2] = "ASC";
                } else if (chbOrdenDescendente.Checked) {
                    datosComplejo[2] = "DESC";
                } else {
                    MessageBox.Show("Si se desa agregar un ordenamiento, debe seleccionar el tipo.","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
            } else if (cboOrdenar.Text == "Agrupar") {
                datosComplejo[2] = "0";
            } else {
                MessageBox.Show("Antes de agregar debe asignar valores.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            csControlador.ingresar(tipos, datosComplejo, "tbl_consultaInteligente");
            string queryGenerado = csControlador.GenerarQueryComplejo(datosComplejo, datos);
            txtQueryFinal.Clear(); txtQueryFinal.Text = queryGenerado; datosComplejo = new string[0];
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {

        }

        private void ConsultaInteligente_Load(object sender, EventArgs e)
        {

        }

        private void creciónToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }

        private void button17_Click(object sender, EventArgs e)
        {

        }

        private void chbEditarDescendente_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnEditarLogico_Click(object sender, EventArgs e)
        {

        }

        private void btnCancelarSimple_Click(object sender, EventArgs e)
        {
            datos = new string[0];
            txtNombreConsulta.Clear();
            cboTabla.ResetText();
            cboCampos.ResetText();
            txtAlias.Clear();
            chbTodosCampos.Checked = false;
            txtQuery.Clear();
        }

        private void btnCancelarLogica_Click(object sender, EventArgs e)
        {
            datos = new string[0];
            datosComplejo = new string[0];
            cboLogico.ResetText();
            cboLogicoCampo.ResetText();
            txtValorLogico.Clear();
        }

        private void btnCancelarComparacion_Click(object sender, EventArgs e)
        {
            datos = new string[0];
            datosComplejo = new string[0];
            cboComparador.ResetText();
            cboComparadorCampo.ResetText();
            txtValorComparador.Clear();
        }

        private void btnCancelarOrden_Click(object sender, EventArgs e)
        {
            datos = new string[0];
            datosComplejo = new string[0];
            cboOrdenar.ResetText();
            cboOrdenarCampo.ResetText();
            chbOrdenAscendente.Checked = false;
            chbOrdenDescendente.Checked = false;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            datos = new string[0];
            datosComplejo = new string[0];
            txtNombreConsulta.Clear();
            cboTabla.ResetText();
            cboCampos.ResetText();
            txtAlias.Clear();
            chbTodosCampos.Checked = false;
            cboLogico.ResetText();
            cboLogicoCampo.ResetText();
            txtValorLogico.Clear();
            cboComparador.ResetText();
            cboComparadorCampo.ResetText();
            txtValorComparador.Clear();
            cboOrdenar.ResetText();
            cboOrdenarCampo.ResetText();
            chbOrdenAscendente.Checked = false;
            chbOrdenDescendente.Checked = false;
            txtQuery.Clear();
            txtQueryFinal.Clear();
        }



        //SALVADOR MARTÍNEZ // PESTAÑA DE EDITAR

        private void cboTablaEditar_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Verifica si se ha seleccionado una tabla
            if (!string.IsNullOrEmpty(cboEditarTabla.Text))
            {
                // Llama al método para llenar el segundo ComboBox con las columnas de la tabla seleccionada
                csControlador.obtenerColumbasPorTabla(cboEditarCampo, cboEditarTabla.Text);
            }
        }




        private void btnEditar_Click_1(object sender, EventArgs e)
        {
            // Asegúrate de que consultaSeleccionada tiene un valor válido
            if (string.IsNullOrEmpty(consultaSeleccionada))
            {
                MessageBox.Show("Debe seleccionar una consulta para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Actualiza datos con la consulta seleccionada
            datos[0] = consultaSeleccionada; // Suponiendo que datos[0] es el nombre de la consulta

            // Procesar los datos en la tabla correspondiente
            csControlador.actualizarQuery(tipos, datos, tabla);




    }

        private void cboConsultas_SelectedIndexChanged(object sender, EventArgs e)
        {
            /// Captura el nombre de la consulta seleccionada desde el ComboBox
            consultaSeleccionada = txtEditarNombreConsulta.SelectedItem.ToString();

            // Captura el nombre de la consulta seleccionada
            string nombreConsulta = cboQuery1.Text;
            string nombreConsulta1 = cboQuery3.Text;

            // Guardar el nombre de la consulta en una variable de instancia
            consultaSeleccionada = nombreConsulta;
            consultaSeleccionada = nombreConsulta1;

        }


        private void btnEditarSimple_Click(object sender, EventArgs e)
        {
            //boton agregar, consulta simple
            // Datos que se van a procesar
            datos = new string[] { txtEditarNombreConsulta.Text, "0", cboEditarTabla.Text, chbTodosCampos.Text, txtQueryEditadoFinal.Text, "1" };

            // Inicializamos la variable para los campos que se mostrarán en el query
            string camposSeleccionados;

            // Verificamos si el CheckBox de 'Todos los campos' está marcado
            if (chbEditarTodosCampos.Checked)
            {
                // Si está marcado, mostramos "Todos los campos"
                camposSeleccionados = "*";
            }
            else
            {
                // Si no está marcado, verificamos si hay un campo seleccionado en el ComboBox
                if (!string.IsNullOrEmpty(cboEditarCampo.Text))
                {
                    // Si hay un campo seleccionado, lo mostramos
                    camposSeleccionados = cboEditarCampo.Text;
                }
                else
                {
                    // Si no hay campo seleccionado, dejamos un valor vacío o un mensaje de advertencia
                    MessageBox.Show("Debe seleccionar o un campo o todos", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (string.IsNullOrEmpty(txtQueryEditado.Text))
            {
                // Si txtQuery está vacío, asignamos el valor inicial
                txtQueryEditado.Text = camposSeleccionados + " + " + txtEditarAlias.Text + " + ";
                datos[4] = txtQueryEditado.Text;
            }
            else
            {
                // Si txtQuery ya tiene texto, agregamos el nuevo valor sin repetir el nombre de la consulta y el tipo
                txtQueryEditado.Text += Environment.NewLine + camposSeleccionados + " + " + txtEditarAlias.Text + " + ";
                datos[4] += txtQueryEditado.Text;
            }

            // Procesar los datos en la tabla correspondiente
            csControlador.ingresar(tipos, datos, "tbl_consultaInteligente");
        }





















































































































































































        //Cambios por Sebastian Luna 
        
        private void cboQuery3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboTabla_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void cboQuery2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Captura el nombre de la consulta seleccionada
            string nombreConsulta = cboQuery1.Text;
            string nombreConsulta1 = cboQuery3.Text;

            // Guardar el nombre de la consulta en una variable de instancia
            consultaSeleccionada = nombreConsulta;
            consultaSeleccionada = nombreConsulta1;
        }

        private void btnEditarCampoSimple_Click(object sender, EventArgs e)
        {

        }

        private void btnBuscarQuery1_Click(object sender, EventArgs e)
        {
            consultaControlador controlador = new consultaControlador();
            string querySeleccionado = cboQuery1.SelectedItem.ToString();
            controlador.BuscarQuerySeleccionado(querySeleccionado, dgvConsultas, txtQuery11);
        }

        private void cboQuery1_SelectedIndexChanged(object sender, EventArgs e)
        {
            consultaControlador controlador = new consultaControlador();
            controlador.CargarQueryEnTextBox(cboQuery1, txtQuery11);
        }
        //Fin participacion sebastian Luna
    }
}
    

