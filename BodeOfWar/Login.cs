﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BodeOfWarServer;

namespace BodeOfWar
{
    public partial class Login : Form
    {
        private string idPartida;
        public Login(String id)
        {
            this.idPartida = id;
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            this.Location = MousePosition;
        }

        private void btnLogar_Click(object sender, EventArgs e)
        {
            string senha = txtSenha.Text;
            string nome = txtNome.Text;
            int id = Int32.Parse(idPartida);

            string idJogador = Jogo.EntrarPartida(id, nome, senha);
            if (!(Erro(idJogador)))
            {
                string[] iten = idJogador.Split(',');

                Bode bode = new Bode(iten[0], iten[1], Int32.Parse(this.idPartida));
                bode.Show();
                this.Close();
            }
            
        }

        private bool Erro(string erro)
        {
            if (erro.Contains("ERRO:"))
            {
                MessageBox.Show(erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            return false;
        }
    }
}
