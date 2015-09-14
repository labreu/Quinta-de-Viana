using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;   
using System.Windows.Forms;    

namespace Quinta_de_Viana
{
    public partial class Form1 : Form
    {
        private static string conexao = "Data Source=Banco.db";
        private static string nomebanco = "Banco.db";
        private static int IDregistro;
        private int id = 1;
        int impressoraConectada = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

            if (!File.Exists(nomebanco))   
            {
                SQLiteConnection.CreateFile(nomebanco);
                SQLiteConnection conn = new SQLiteConnection(conexao);
                conn.Open();
                
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("CREATE TABLE IF NOT EXISTS PRODUTOS ([CODIGO] INTEGER PRIMARY KEY,[NOME] VARCHAR(50),[PRECO] REAL,[DESCRICAO] VARCHAR(255))");

                SQLiteCommand cmd = new SQLiteCommand(sql.ToString(), conn);
                try
                {
                    cmd.ExecuteNonQuery();
               
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao criar banco: " + ex.Message);
                    return;
                }

                StringBuilder sql2 = new StringBuilder();
                sql2.AppendLine("CREATE TABLE IF NOT EXISTS HISTORICO_PRODUTOS ([ID] INTEGER PRIMARY KEY AUTOINCREMENT, [CODIGO] INTEGER ,[NOME] VARCHAR(50),[PRECO] REAL,[DESCRICAO] VARCHAR(255),[DATA] VARCHAR(50))");

                SQLiteCommand cmd2 = new SQLiteCommand(sql2.ToString(), conn);
                try
                {
                    cmd2.ExecuteNonQuery();
            
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao criar banco: " + ex.Message);
                    return;
                }

                StringBuilder sql3 = new StringBuilder();
                sql3.AppendLine("CREATE TABLE IF NOT EXISTS CONTAS ([ID] INTEGER PRIMARY KEY AUTOINCREMENT, [CODIGO_CONTA] INTEGER, [CODIGO] INTEGER, [NOME] VARCHAR(50),[PRECO] REAL,[DATA] VARCHAR(50))");

                SQLiteCommand cmd3 = new SQLiteCommand(sql3.ToString(), conn);
                try
                {
                    cmd3.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao criar banco: " + ex.Message);
                    return;
                }
            }
            
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.button2, "Deleta a linha selecionada abaixo.");

            System.Windows.Forms.ToolTip ToolTip2 = new System.Windows.Forms.ToolTip();
            ToolTip2.SetToolTip(this.button3, "Não é possível alterar um código. Utilize o Cadastro.");

            System.Windows.Forms.ToolTip ToolTip3 = new System.Windows.Forms.ToolTip();
            ToolTip3.SetToolTip(this.button6, "Limpa os campos acima.");

            System.Windows.Forms.ToolTip ToolTip4 = new System.Windows.Forms.ToolTip();
            ToolTip4.SetToolTip(this.button5, "Limpa a conta ao lado.");

            System.Windows.Forms.ToolTip ToolTip5 = new System.Windows.Forms.ToolTip();
            ToolTip5.SetToolTip(this.cadastrarButton, "Não é possível cadastrar dois produtos com mesmo código.");

            System.Windows.Forms.ToolTip ToolTip6 = new System.Windows.Forms.ToolTip();
            ToolTip6.SetToolTip(this.button4, "Anular uma conta que foi impressa errada para não alterar o balanço final.\nO número da conta está na nota.");

            System.Windows.Forms.ToolTip ToolTip7 = new System.Windows.Forms.ToolTip();
            ToolTip7.SetToolTip(this.button1, "Imprime a conta e registra dados para balanço.");
            string path = "C:\\Users\\Quinta de Viana\\OneDrive\\Documentos\\";
            File.Delete(path + "Banco.db");
            File.Copy("Banco.db", path + "Banco.db");
            listar();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (nomeTextBox.Text == "" || precoTextBox.Text == ""
                || codTextBox.Text == "")
            {
                MessageBox.Show("Preencha todos os campos");
                return;
            }

            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("INSERT INTO PRODUTOS(CODIGO, NOME, PRECO, DESCRICAO) VALUES (@CODIGO, @NOME, @PRECO, @DESCRICAO)", conn);
            cmd.Parameters.AddWithValue("@CODIGO", Convert.ToInt32(codTextBox.Text.Trim()));
            cmd.Parameters.AddWithValue("@NOME", nomeTextBox.Text.Trim());
            cmd.Parameters.AddWithValue("@PRECO", Convert.ToDouble(precoTextBox.Text.Trim()));
            cmd.Parameters.AddWithValue("@DESCRICAO", descricaoTextBox.Text.Trim());
            
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                
                MessageBox.Show("Erro ao cadastrar: Verifique se o código já existe\n"+ex.Message);
                return;
            }
            DateTime saveNow = DateTime.Now;

            SQLiteCommand cmd2 = new SQLiteCommand("INSERT INTO HISTORICO_PRODUTOS(CODIGO, NOME, PRECO, DESCRICAO, DATA) VALUES (@CODIGO, @NOME, @PRECO, @DESCRICAO, @DATA)", conn);
            cmd2.Parameters.AddWithValue("@CODIGO", Convert.ToInt32(codTextBox.Text.Trim()));
            cmd2.Parameters.AddWithValue("@NOME", nomeTextBox.Text.Trim());
            cmd2.Parameters.AddWithValue("@PRECO", Convert.ToDouble(precoTextBox.Text.Trim()));
            cmd2.Parameters.AddWithValue("@DESCRICAO", "*Cadastro*"+descricaoTextBox.Text.Trim());
            cmd2.Parameters.AddWithValue("@DATA", saveNow);

            try
            {
                cmd2.ExecuteNonQuery();
                MessageBox.Show("Registro salvo");
                nomeTextBox.Text = string.Empty;
                precoTextBox.Text = string.Empty;
                descricaoTextBox.Text = string.Empty;
                codTextBox.Text = string.Empty;
                listar();
            }
            catch (Exception ex)
            {
                listar();
                MessageBox.Show("Erro ao cadastrar: Cheque se o código já existe\n" + ex.Message);
            }
        }

        private void listar()
        {
            func();
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM PRODUTOS", conn);
            SQLiteDataReader dr = cmd.ExecuteReader();
            List<Produto> lista = new List<Produto>();
            while (dr.Read())
            {
                lista.Add(new Produto{ Código = Convert.ToInt32(dr["CODIGO"]),
                              Nome = dr["NOME"].ToString()
                            , Descrição = dr["DESCRICAO"].ToString()
                            , Preço = Convert.ToDouble(dr["PRECO"])  });
            }
            dataGridView1.DataSource = lista;
            dataGridView2.DataSource = lista;

            dataGridView1.Columns[1].Width = 300;
            dataGridView1.Columns[3].Width = 800;
            dataGridView2.Columns[1].Width = 150;
            dataGridView3.Columns[1].Width = 200;

            textBoxAjuda.Text = "Qual papel devo utilizar para a impressão?\r\n  Deve ser utilizada uma bobina de papel térmico, com diâmetro externo de até 80 mm (comprar 80mm de preferência para manter o padrão). Para maior vida útil da guilhotina, utilizar papel de 56 g / m².";
            textBoxAjuda.Text = textBoxAjuda.Text + "\r\n\r\nComo troco a bobina de papel da impressora?\r\n  Puxe para frente a alavanca da tampa e abra a tampa da impressora. Tire o tubete da bobina de papel usada. Insira a nova bobina da maneira ilustrada no manual, ou seja, com o papel saindo embaixo do rolo.Deixe um pouco de papel para fora. Feche a tampa e destaque a sobra de papel contra a serrilha.";
            textBoxAjuda.Text = textBoxAjuda.Text + "\r\n\r\nO papel atolou, como devo proceder para voltar a operar?\r\n  Em caso de atolamento de papel, abra a tampa da impressora puxando a alavanca.Ao abrir tampa a guilotina destravará e a impressora voltará a operação. Feche a tampa da impressora lembrando de posicionar a bobina de forma correta dentro do receptáculo da impressora. Impressões em andamento serão perdidas devendo ser re - enviadas.";
            textBoxAjuda.Text = textBoxAjuda.Text + "\r\n\r\nQuando é necessário fazer a limpeza da cabeça de impressão?\r\n  Durante a operação normal, algumas partículas de tinta de papel ficarão aderidas à superfície da cabeça térmica de impressão. Portanto, recomenda - se limpar a cabeça depois de 10 Km de comprimento de papel impresso ou se a qualidade de impressão estiver degradada.";
            textBoxAjuda.Text = textBoxAjuda.Text + "\r\n\r\nComo limpar a cabeça de impressão?\r\n  Desligue a impressora, e, caso ela estivesse em operação, aguarde alguns minutos até que ela esfrie.Limpe a cabeça de impressão com um pedaço de algodão ou cotonete embebido em álcool isopropílico (etanol ou isopropanol). Não limpe o cabeçote de impressão com objetos duros, abrasivos ou com os dedos uma vez que isso pode causar danos à superfície delicada.Limpe o rolete do cilindro(rolete de borracha) com o algodão para remover partículas de pó.Quando a limpeza estiver concluída, aguarde algum tempo para voltar a operar, de modo que qualquer eventual excesso de álcool possa evaporar.";
            textBoxAjuda.Text = textBoxAjuda.Text + "\r\n\r\n\r\nFonte: http://www.bematech.com.br/suporte/equipamento/mp-4200-th";


        }

        private void button2_Click(object sender, EventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("DELETE FROM PRODUTOS WHERE CODIGO = @CODIGO", conn);
            cmd.Parameters.AddWithValue("CODIGO", Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value));
            
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Registro excluido");
                listar();
            }
            catch (Exception ex) { MessageBox.Show("error" + ex.Message); }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            func();
            Produto p = new Produto
            {
                Código = Convert.ToInt32(dataGridView2.CurrentRow.Cells[0].Value),
                Nome = dataGridView2.CurrentRow.Cells[1].Value.ToString(),
                Descrição = dataGridView2.CurrentRow.Cells[3].Value.ToString(),
                Preço = Convert.ToDouble(dataGridView2.CurrentRow.Cells[2].Value)
            };

            dataGridView3.Rows.Add(p.Código, p.Nome, p.Preço);

            atualizaTotal();

        }

        private void atualizaTotal()
        {

            double sum = 0;

            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                sum = sum + Convert.ToDouble(dataGridView3.Rows[i].Cells[2].Value);
            }
            //MessageBox.Show(sum.ToString());
            string yourValue = (((double)sum / 100)*100).ToString("C");
            labelTotal.Text = "Total: " + yourValue;
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            IDregistro = 0;
            IDregistro = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
            nomeTextBox.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            codTextBox.Text = IDregistro.ToString();
            descricaoTextBox.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            precoTextBox.Text = Convert.ToDouble(dataGridView1.CurrentRow.Cells[2].Value).ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            nomeTextBox.Text = string.Empty;
            precoTextBox.Text = string.Empty;
            descricaoTextBox.Text = string.Empty;
            codTextBox.Text = string.Empty;
            listar();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (nomeTextBox.Text == "" || precoTextBox.Text == ""
                || codTextBox.Text == "")
            {
                MessageBox.Show("Preencha todos os campos");
                return;
            }
            IDregistro = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
            if (IDregistro > 0)
            {
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                SQLiteCommand cmd = new SQLiteCommand("UPDATE PRODUTOS SET NOME = @NOME, DESCRICAO = @DESCRICAO, PRECO = @PRECO WHERE CODIGO = @CODIGO", conn);
                cmd.Parameters.AddWithValue("@DESCRICAO", descricaoTextBox.Text);
                cmd.Parameters.AddWithValue("@CODIGO", Convert.ToInt32(codTextBox.Text));
                cmd.Parameters.AddWithValue("@NOME", nomeTextBox.Text);
                cmd.Parameters.AddWithValue("@PRECO", Convert.ToDouble(precoTextBox.Text));
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Registro Atualizado");
                    listar();
                }
                catch (Exception ex) {
                    MessageBox.Show("Error: " + ex.Message);
                    return;
                }
                listar();
                DateTime saveNow = DateTime.Now;
                SQLiteCommand cmd2 = new SQLiteCommand("INSERT INTO HISTORICO_PRODUTOS(CODIGO, NOME, PRECO, DESCRICAO, DATA) VALUES (@CODIGO, @NOME, @PRECO, @DESCRICAO, @DATA)", conn);
                cmd2.Parameters.AddWithValue("@CODIGO", Convert.ToInt32(codTextBox.Text.Trim()));
                cmd2.Parameters.AddWithValue("@NOME", nomeTextBox.Text.Trim());
                cmd2.Parameters.AddWithValue("@PRECO", Convert.ToDouble(precoTextBox.Text.Trim()));
                cmd2.Parameters.AddWithValue("@DESCRICAO", "*Atualização*"+descricaoTextBox.Text.Trim());
                cmd2.Parameters.AddWithValue("@DATA", saveNow);

                try
                {
                    cmd2.ExecuteNonQuery();
                    //MessageBox.Show("Registro salvo");
                    nomeTextBox.Text = string.Empty;
                    precoTextBox.Text = string.Empty;
                    descricaoTextBox.Text = string.Empty;
                    codTextBox.Text = string.Empty;
                    listar();
                }
                catch (Exception ex)
                {
                    listar();
                    MessageBox.Show("Erro ao cadastrar: Cheque se o código já existe\n" + ex.Message);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView3.DataSource = null;
            dataGridView3.Rows.Clear();
            atualizaTotal();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
        }
        private void func() {

            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("SELECT MAX(CODIGO_CONTA) FROM CONTAS", conn);

            try
            {
                SQLiteDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    try
                    {
                        id = dr.GetInt32(0) + 1;
                        //MessageBox.Show(id + "");  //proxima conta
                    }
                    catch (Exception ex)
                    {
                        id = 1;
                        //MessageBox.Show(ex.Message + "\n ID = "+id);
                    }
                    dr.Close();
                    conn.Close();
                }
                label3.Text = "Conta: " + id;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            func();
            if (dataGridView3.Rows.Count==0)
            {
                MessageBox.Show("Insira itens na conta clicando duas vezes sobre eles no menu à esquerda.");
                return;
            }
            

            DateTime saveNow = DateTime.Now;
            
            try
            {
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                for (int k = 0; k < dataGridView3.Rows.Count; k++)
                {

                    SQLiteCommand cmd2 = new SQLiteCommand("INSERT INTO CONTAS(CODIGO_CONTA, CODIGO, NOME, PRECO, DATA) VALUES (@CODIGO_CONTA, @CODIGO, @NOME, @PRECO, @DATA)", conn);
                    cmd2.Parameters.AddWithValue("@CODIGO_CONTA", Convert.ToInt32(id));
                    cmd2.Parameters.AddWithValue("@CODIGO", Convert.ToInt32(dataGridView3.Rows[k].Cells[0].Value));
                    cmd2.Parameters.AddWithValue("@NOME", dataGridView3.Rows[k].Cells[1].Value.ToString());
                    cmd2.Parameters.AddWithValue("@PRECO", Convert.ToDouble(dataGridView3.Rows[k].Cells[2].Value));
                    cmd2.Parameters.AddWithValue("@DATA", saveNow.ToString());

                    using (cmd2)
                    {
                        cmd2.ExecuteNonQuery();
                    }
                }
                
                listar();
                dataGridView3.DataSource = null;
                dataGridView3.Rows.Clear();
                atualizaTotal();

                MessageBox.Show("Conta impressa");//colocar gaveta para abrir
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao fechar conta:\n" + ex.Message);
                return;
            }


            string path = "C:\\Users\\Quinta de Viana\\OneDrive\\Documentos\\";
            File.Delete(path + "Banco.db");
            File.Copy("Banco.db", path + "Banco.db");

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("UPDATE CONTAS SET NOME = @NOME, DATA = @DATA WHERE CODIGO_CONTA = @CODIGO_CONTA", conn);
            cmd.Parameters.AddWithValue("@NOME", "Excluido");
            cmd.Parameters.AddWithValue("@DATA", DateTime.Now.ToString());
            cmd.Parameters.AddWithValue("@CODIGO_CONTA", Convert.ToInt32(textBox1.Text));
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Registro Anulado");
                listar();
                textBox1.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            string path = "C:\\Users\\Quinta de Viana\\OneDrive\\Documentos\\";
            File.Delete(path + "Banco.db");
            File.Copy("Banco.db", path + "Banco.db");

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            try {

                int iRetorno = MP2032.ConfiguraModeloImpressora(Convert.ToInt32(modeloImpressoraTextField.Text.ToString()));
                if (iRetorno == 0)
                {
                    impressoraConectada = iRetorno;
                }
                else
                {
                    impressoraConectada = iRetorno;
                }
                iRetorno = MP2032.IniciaPorta(portaTextField.Text);
                if (iRetorno == 0)
                {
                    MessageBox.Show("Impressora MP4200 TH nao conectada.");
                    impressoraConectada = iRetorno;
                }
                else
                {
                    MessageBox.Show("Impressora MP4200 TH conectada.");
                    impressoraConectada = iRetorno;
                    button9.Enabled = true;
                }
                
                
            }
            catch(Exception ex)
            {
                impressoraConectada = 0;
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MP2032.FechaPorta();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                int iRetorno;
                iRetorno = MP2032.FormataTX(textArea.Text, 3, 0, 0, 1, 0);

                iRetorno = MP2032.AcionaGuilhotina(1);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int iRetorno;
            int charCode = 27;
            int charCode2 = 118;
            int charCode3 = 140;

            char specialChar = Convert.ToChar(charCode);
            char specialChar2 = Convert.ToChar(charCode2);
            char specialChar3 = Convert.ToChar(charCode3);

            string s_cmdTX = "" + specialChar + specialChar2 + specialChar3;

            try
            {
                iRetorno = MP2032.ComandoTX(s_cmdTX, s_cmdTX.Length);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            atualizaTotal();
            if (impressoraConectada == 0)
                button7.Enabled = false;
            else
                button7.Enabled = true;//desabilita e habilita botao de abrir gaveta
        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {
            if (impressoraConectada == 0)
                button9.Enabled = false;
            else
                button9.Enabled = true;
        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {
            DateTime saveNow = DateTime.Now;

            diaTextBox.Text = saveNow.Day.ToString();
            mesTextBox.Text = saveNow.Month.ToString();
            anoTextBox.Text = saveNow.Year.ToString();

        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void Tabs_Enter(object sender, EventArgs e)
        {

        }
    }
}