﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BodeOfWarServer;


namespace BodeOfWar
{

    public partial class Bode : Form
    {
        private Timer TimerChecarVez;

        private string estadoJogo;

        private int valorBode;
        private string[] cartasMao;
        private int idJogador;
        private string senha;
        private int idPartida;

        private bool checarQtdBode = true;
        private bool jogando = true;

        public void InitTimer()
        {
            TimerChecarVez = new Timer();
            TimerChecarVez.Tick += new EventHandler(update);
            TimerChecarVez.Interval = 5000; //5s 
            TimerChecarVez.Start();
        }

        public Bode(string idJogador, string senha, int idPartida)
        {
            this.idJogador = Int32.Parse(idJogador);
            this.senha = senha;
            this.idPartida = idPartida;
            InitializeComponent();
        }

        private void Bode_Load(object sender, EventArgs e)
        {
            InitTimer();
            txtHistorico.Text = Jogo.ExibirNarracao(this.idPartida);
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            var retorno = Jogo.IniciarPartida(idJogador, this.senha);
            update(sender, e);
        }

        private void update(object sender, EventArgs e)
        {

            string verificarVez = Jogo.VerificarVez(this.idPartida);
            string[] iten = verificarVez.Split(',');

            txtHistorico.Text = Jogo.ExibirNarracao(this.idPartida);

            string retorno = Jogo.ListarPartidas("E");
            retorno = retorno.Replace("\r", "");
            retorno = retorno.Substring(0, retorno.Length - 1);
            string[] partidas = retorno.Split('\n');

            if (iten[0].Contains('J'))
            {
                int qtdBode = Int32.Parse(lblQtdBodes.Text);

                lblJogadorVez.Text = iten[1];
                string temp = iten[3].Replace('\r', ' ');
                temp = temp.Replace('\n', ' ');
                estadoJogo = temp.Trim();

                string valores = Jogo.VerificarIlha(idJogador, senha);

                if (valores.Contains("ERRO:") == false)
                {

                    lblEscolherIlha.Text = valores;
                }
                string mesaIlha = Jogo.VerificarMesa(idPartida);
                if (mesaIlha[0].Equals('I'))
                {
                    lblValorIlha.Text = mesaIlha.Replace(mesaIlha[0], ' ');//tira o l inicial

                }
                if (estadoJogo.Contains('I'))
                {
                    if (checarQtdBode)
                    {
                        QtsBode(qtdBode, mesaIlha);
                    }
                    this.checarQtdBode = false;

                }
                else
                {
                    this.checarQtdBode = true;
                }

            }
            if (false == jogando)
            {
                return;
            }
            foreach (string partida in partidas)
            {
                string[] item = partida.Split(',');
                if (idPartida == Int32.Parse(item[0]))
                {
                    string historico = Jogo.ExibirNarracao(idPartida).Replace('\r', ' ').Trim();
                    string[] fimJogo = historico.Split('\n');
                    MessageBox.Show("Partida Finalizada \n" + fimJogo[0] + '\n' + fimJogo[1]);
                    jogando = false;
                    break;
                }
            }
            /*
            else if (iten[0].Contains('E')) 
            {
                if (iten[0].Contains("ERRO")) return;
                
                
                
            }
            */
            
        }

        private void QtsBode(int quantidadeBode, string mesaIlha)
        {


            mesaIlha = mesaIlha.Replace('\r', ' ');
            string[] bodes = mesaIlha.Split('\n');
            for (int i = 1; i < bodes.Length-1; i++)
            {
                string[] bodeJogado = bodes[i].Split(',');
                if (Int32.Parse(bodeJogado[1]) >this.valorBode)
                {
                    return;
                }

                string[] carta = EncontreCarta(bodeJogado,1);

                quantidadeBode += Int32.Parse( carta[1]);
            }
            lblQtdBodes.Text = quantidadeBode.ToString();
        }

        private void btnImg_Click(object sender, EventArgs e)
        {
            string nomeFont = "Microsoft Sans Serif";
            int tamanhoFont = 10;

            string mao = Jogo.VerificarMao(this.idJogador, this.senha);//volta o que tem na mao
            mao = mao.Replace('\r', ' ');
            cartasMao = mao.Split('\n');

            this.pnlMao.Controls.Clear();

            int x = 20;
            int y = 20;
            int alturaMax = -1;

            for (int i = 0; i < cartasMao.Length - 1; i++)
            {
                PictureBox img = new PictureBox();

                Label lblValorCarta = new Label();
                Label lblQuantidadeBode = new Label();

                img.Size = new Size(115, 165);

                //pega a img correta da carta

                string[] carta = EncontreCarta(cartasMao[i].Split(','),0);

                if (carta != null)
                {
                    img.Image = (Image)Properties.Resources.ResourceManager.GetObject("b" + carta[2].Trim());

                    lblValorCarta.Text = carta[0];
                    lblQuantidadeBode.Text = carta[1];

                    img.Location = new Point(x, y);
                    lblValorCarta.Location = new Point(x + 20, y + 10);
                    lblValorCarta.AutoSize = true;
                    lblValorCarta.Font = new Font(nomeFont, tamanhoFont);
                    lblValorCarta.ForeColor = Color.Black;
                    //lblValorCarta.BackColor = Color.FromArgb(1, 1, 1,1);

                    lblQuantidadeBode.Location = new Point(x + 20, img.Height - 10);
                    lblQuantidadeBode.AutoSize = true;
                    lblQuantidadeBode.Font = new Font(nomeFont, tamanhoFont);
                    lblQuantidadeBode.ForeColor = Color.Black;
                    //lblQuantidadeBode.BackColor = Color.FromArgb(1,5,13,18);

                    img.SizeMode = PictureBoxSizeMode.StretchImage;

                    x += img.Width + 10;
                    alturaMax = img.Height;
                    if (x > pnlMao.Width - 100)
                    {
                        x = 20;
                        y += alturaMax + 10;
                    }
                    this.pnlMao.Controls.Add(lblQuantidadeBode);
                    this.pnlMao.Controls.Add(lblValorCarta);
                    this.pnlMao.Controls.Add(img);
                }
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            switch (estadoJogo)
            {
                case "B":
                    EscolhaBode(txtEscolha.Text,  sender, e);
                    break;
                case "I":
                    EscolhaIlha(txtEscolha.Text);
                    break;
                default:
                    break;
            }
            txtEscolha.Clear();
        }

        private void EscolhaBode(string valorBode, object sender, EventArgs e)
        {
            int valorBod = 0;// = Int32.Parse(valorBode);
            if (false == int.TryParse(valorBode,out valorBod))
            {
                MessageBox.Show("ERRO: Valor invalido");
                return;
            }
            for (int i = 0; i < cartasMao.Length-1; i++)
            {
                int valor = Int32.Parse(cartasMao[i]);
                
                if (valor == valorBod)
                {
                    string[] carta = EncontreCarta(cartasMao[i].Split(','), 0);
                    string mensagem = Jogo.Jogar(idJogador, senha, Int32.Parse(carta[0]));
                    this.valorBode = valorBod;
                    if (mensagem.Contains("ERRO"))
                    {
                        MessageBox.Show(mensagem);
                        return;
                    }
                    
                    btnImg_Click( sender, e);
                    return;
                }
            }
            MessageBox.Show("ERRO: Valor não encontrado");
        }

        private void EscolhaIlha(string valorIlha)
        {
            string valores = Jogo.VerificarIlha(idJogador, senha);
            valores = valores.Replace('\r', ' ');
            valores = valores.Replace('\n', ' ');
            valores = valores.Trim();
            string[] valoresIlhas = valores.Split(',');

            int valorIlha1, valorIlha2,entradaValorIlha;

            if (false == Int32.TryParse(valoresIlhas[0], out valorIlha1))
            {
                return;
            }
            if (false == Int32.TryParse(valoresIlhas[1], out valorIlha2))
            {
                return;
            }
            if (false == Int32.TryParse(valorIlha, out entradaValorIlha))
            {
                return;
            }

            if (valorIlha1 == entradaValorIlha ||
                valorIlha2 == entradaValorIlha)
            {
                Jogo.DefinirIlha(idJogador, senha, Int32.Parse(valorIlha));
                string mesaIlha = Jogo.VerificarMesa(idPartida);
                lblValorIlha.Text = mesaIlha.Replace(mesaIlha[0], ' ');//tira o l inicial
                lblEscolherIlha.Text = " ";

                return;
            }
            MessageBox.Show("Valor de Ilha Invalido");
        }

        private string[] EncontreCarta(string[] cartaMao, int idChecagem)
        {

            string cartas = Jogo.ListarCartas();//todas as cartas do jogo valor, quantidade bode, idImagem
            cartas = cartas.Replace('\r', ' ');
            string[] cartasValores = cartas.Split('\n');

            if (cartaMao.Length <= 0 || cartaMao[0].Contains("ERRO:"))
            {
                return null;
            }
            for (int i = 0; i < cartasValores.Length-1; i++)
            {
                string[] aux = cartasValores[i].Split(',');
                int valorCarta = Int32.Parse(aux[0]);
                int valorMao = Int32.Parse(cartaMao[idChecagem]);

                if (valorCarta == valorMao)
                {
                    return aux;
                }
                
            }
            return null;
        }
    }
}
