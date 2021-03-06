﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Security.Cryptography;
using PagoAgilFrba.AbmCliente; //ACA AGREGO EL FORMULARIO ABMCLIENTE



namespace PagoAgilFrba
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /******************************************************************************/

        private void Form1_Load(object sender, EventArgs e)
        {

            getConexion();
        }

        /******************************************************************************/

        public SqlConnection getConexion()
        {
            SqlConnection conex = null;
            if (conex == null || conex.State == ConnectionState.Closed)
            {
                try
                {
                    String str = "Data Source=localhost\\SQLSERVER2012;Initial Catalog=GD2C2017;Integrated Security=True"; ;
                    //String str = ConfigurationManager.ConnectionStrings["GD1C2017"].ConnectionString; //VER SI FUNCIONA OK
                    conex = new SqlConnection(str);
                    conex.ConnectionString = str;
                    conex.Open();
                    MessageBox.Show("Conexión establecida", "Información", MessageBoxButtons.OK);
                }
                catch (SqlException)
                {
                    MessageBox.Show("Error", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

            return conex;

        }

        /******************************************************************************/


        /******************************************************************************/

        private SqlDataReader logueoCorrecto(String usuario, string password)
        {
            SqlDataReader reader = null;
            SqlCommand cmd = new SqlCommand("SELECT DISTINCT USU_ID, USU_INTENTOS,USU_HABILITADO FROM GOQ.USUARIO WHERE USU_USERNAME = @USUARIO AND USU_PASSWORD = GOQ.F_Hash256(@PASS) AND USU_HABILITADO='1'", getConexion());

            cmd.Parameters.Add("USUARIO", SqlDbType.NVarChar).Value = usuario;
            cmd.Parameters.Add("PASS", SqlDbType.NVarChar).Value = password;

            reader = cmd.ExecuteReader();
            reader.Read();

            return reader;
        }

        /******************************************************************************/

        private void inhabilitarUsuario(String usuario)
        {
            SqlDataReader reader = null;
            SqlCommand cmd = new SqlCommand("UPDATE GOQ.USUARIO SET USU_HABILITADO ='I' WHERE USU_USERNAME = @USUARIO AND USU_HABILITADO='1'", getConexion());

            cmd.Parameters.Add("USUARIO", SqlDbType.NVarChar).Value = txtUsuario.Text;
            reader = cmd.ExecuteReader();
            reader.Read();
            MessageBox.Show("Usuario Inhabilitado", "Información");
            reader.Close();
        }

        /******************************************************************************/

        private SqlDataReader usuarioExiste(String usuario)
        {
            SqlDataReader reader = null;
            SqlCommand cmd = new SqlCommand("SELECT USU_INTENTOS FROM GOQ.USUARIO WHERE USU_USERNAME = @USUARIO AND USU_HABILITADO='1'", getConexion());

            cmd.Parameters.Add("USUARIO", SqlDbType.NVarChar).Value = txtUsuario.Text;
            reader = cmd.ExecuteReader();
            reader.Read();
            return reader;
        }

        /******************************************************************************/

        private void resetearIntentos(int nro)
        {
            SqlDataReader reader = null;
            SqlCommand cmd = new SqlCommand("UPDATE GOQ.USUARIO SET USU_INTENTOS=@NRO WHERE USU_USERNAME = @USUARIO", getConexion());

            cmd.Parameters.Add("USUARIO", SqlDbType.NVarChar).Value = txtUsuario.Text;
            cmd.Parameters.Add("NRO", SqlDbType.NVarChar).Value = nro;
            reader = cmd.ExecuteReader();
            reader.Close();
        }

        /******************************************************************************/

        /******************************************************************************/

        private void abrirPantallaPrincipal()
        {
            this.Hide();
            // FormPrincipal  p = new FormPrincipal ();
            //p.ShowDialog();
        }

        /******************************************************************************/

        private void txtClave_KeyPress(object sender, KeyPressEventArgs e)
        {
            MessageBox.Show("Enter");
            if (e.KeyChar == (char)Keys.Enter)
            {

                btnIngresar.Focus();
            }
        }

        /******************************************************************************/

        private int cargarRoles(string id)
        {
            int cant = 0;
            cbRol.Visible = true;

            SqlDataReader reader = null;
            // SqlCommand cmd = new SqlCommand("SELECT DISTINCT DESCRIPCION FROM BOBBYTABLES.ROL WHERE ID = @ID AND ESTADO='A'", getConexion());
            //  cmd.Parameters.Add("ID", SqlDbType.NVarChar).Value = id;

            // reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    cant += 1;
                    cbRol.Items.Add(reader.GetString(0));
                }
            }
            reader.Close();
            return cant;
        }


        private void btnCancelar_Click(object sender, EventArgs e)
        {
            //DESCOMENTAR THIS.CLOSE ACTUALMENTE ES UNA PRUEBA PARA VER EL FORMULARIO QUE HICE
            //this.Close();
            ABMCliente p = new ABMCliente();
            p.Show();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if ((txtUsuario.Text.Length > 0) && (txtClave.Text.Length > 0))
            {
                if (logueoCorrecto(txtUsuario.Text, txtClave.Text).HasRows)
                {
                    MessageBox.Show("LOGUEO CORRECTO", "Información");
                    /*
                                        if (cbRol.Text.Length == 0)
                                        {

                                            if (cargarRoles(logueoCorrecto(txtUsuario.Text, txtClave.Text).GetString(0)) > 1)
                                            {
                                                MessageBox.Show("Seleccione un Rol...", "Información");
                                                if (logueoCorrecto(txtUsuario.Text, txtClave.Text).GetString(1) != "3")
                                                {
                                                    resetearIntentos(3);
                                                }
                                                logueoCorrecto(txtUsuario.Text, txtClave.Text).Close();
                                            }
                                            else
                                            {
                                                abrirPantallaPrincipal();
                                            }

                                        }
 
                                    }
                                    else
                                    {
                                        MessageBox.Show("Ingreso incorrecto", "Información");
                                        //actualizar intentos descontar x 1
                                        int intento = 0;
                                        if (usuarioExiste(txtUsuario.Text).HasRows)
                                        {
                                            usuarioExiste(txtUsuario.Text).Read();

                                            intento = Convert.ToInt16(usuarioExiste(txtUsuario.Text).GetString(0));

                                            if (intento > 0)
                                            {
                                                resetearIntentos(intento - 1);
                                            }
                                            else
                                            {
                                                inhabilitarUsuario(txtUsuario.Text);
                                            }
                                            txtUsuario.Focus();
                                            usuarioExiste(txtUsuario.Text).Close();
                                        }

                                    }

                                    if (cbRol.Text.Length > 0)
                                    {
                                        abrirPantallaPrincipal();
                                    }
                     */
                }
            }


        }

    }
}
