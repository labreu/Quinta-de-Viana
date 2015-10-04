using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Quinta_de_Viana
{
    public partial class Form1 : Form
    {
        private static string conexao = "Data Source=Banco.db";
        private static string nomebanco = "Banco.db";
        private static int IDregistro;
        private int id = 1;
        int impressoraConectada = 0;
        string total;
        

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

                StringBuilder sql4 = new StringBuilder();
                sql4.AppendLine("CREATE TABLE IF NOT EXISTS CATEGORIAS ([CODIGO] INTEGER PRIMARY KEY,[NOME] VARCHAR(50),[PRECO] REAL,[CATEGORIA] VARCHAR(255))");

                SQLiteCommand cmd4 = new SQLiteCommand(sql4.ToString(), conn);
                try
                {
                    cmd4.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao criar banco: " + ex.Message);
                    return;
                }

                StringBuilder sql5 = new StringBuilder();
                sql5.AppendLine("CREATE TABLE IF NOT EXISTS H_GAVETA([ID] INTEGER PRIMARY KEY AUTOINCREMENT, [CODIGO_CONTA] INTEGER, [DATA] VARCHAR(255))");

                SQLiteCommand cmd5 = new SQLiteCommand(sql5.ToString(), conn);
                try
                {
                    cmd5.ExecuteNonQuery();

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
            ToolTip3.SetToolTip(this.button6, "Limpa os campos ao lado.");

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


            //inicia impressora
            try
            {
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
            catch (Exception ex)
            {
                impressoraConectada = 0;
                MessageBox.Show(ex.Message);
            }

            comboBox1.Items.Add("Bebida");
            comboBox1.Items.Add("Petisco");
            comboBox1.Items.Add("À la carte");
            comboBox1.Items.Add("Prato executivo");
            comboBox1.Items.Add("Sobremesa");
            comboBox1.Items.Add("Produto caseiro");
            comboBox1.Items.Add("Café da manhã");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (nomeTextBox.Text == "" || precoTextBox.Text == ""
                || codTextBox.Text == "" || comboBox1.Text == "")
            {
                MessageBox.Show("Preencha todos os campos. Apenas a descrição é opcional.");
                return;
            }

            if(precoTextBox.Text.ToString().Contains("."))
            { 
                MessageBox.Show("Utilize vírgula para colocar o preço.");
                return;
            }

            try {
                Convert.ToInt32(codTextBox.Text.Trim());
                Convert.ToDouble(precoTextBox.Text.Trim());
            }catch(Exception ex) {
                MessageBox.Show("Utilize apenas números para o código e preço. Utilize a vírgula para colocar preços como por exemplo: 3,50.");
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
                
                MessageBox.Show("Erro ao cadastrar: Verifique se o código já existe. Se já existir, utilize o botão alterar\n"+ex.Message);
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
            }
            catch (Exception ex)
            {
                listar();
                MessageBox.Show("Erro ao cadastrar: Cheque se o código já existe\n" + ex.Message);
            }

            SQLiteCommand cmd3 = new SQLiteCommand("INSERT INTO CATEGORIAS(CODIGO, NOME, PRECO, CATEGORIA) VALUES (@CODIGO, @NOME, @PRECO, @CATEGORIA)", conn);
            cmd3.Parameters.AddWithValue("@CODIGO", Convert.ToInt32(codTextBox.Text.Trim()));
            cmd3.Parameters.AddWithValue("@NOME", nomeTextBox.Text.Trim());
            cmd3.Parameters.AddWithValue("@PRECO", Convert.ToDouble(precoTextBox.Text.Trim()));
            cmd3.Parameters.AddWithValue("@CATEGORIA", "*Cadastro*" + comboBox1.Text.Trim());
            

            try
            {
                cmd3.ExecuteNonQuery();
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

        private double atualizaTotal()
        {

            double sum = 0;

            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                sum = sum + Convert.ToDouble(dataGridView3.Rows[i].Cells[2].Value);
            }
            //MessageBox.Show(sum.ToString());
            total = (((double)sum / 100)*100).ToString("C");
            labelTotal.Text = "Total: " + total;

            return sum;
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            IDregistro = 0;
            IDregistro = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
            nomeTextBox.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            codTextBox.Text = IDregistro.ToString();
            descricaoTextBox.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            precoTextBox.Text = Convert.ToDouble(dataGridView1.CurrentRow.Cells[2].Value).ToString();
            MessageBox.Show("Lembre-se de preencher novamente a categoria");
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
                || codTextBox.Text == "" || comboBox1.Text == "")
            {
                MessageBox.Show("Preencha todos os campos");
                return;
            }
            if (precoTextBox.Text.ToString().Contains("."))
            {
                MessageBox.Show("Utilize vírgula para colocar o preço.");
                return;
            }
            try
            {
                Convert.ToInt32(codTextBox.Text.Trim());
                Convert.ToDouble(precoTextBox.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Utilize apenas números para o código e preço. Utilize a vírgula para colocar preços como por exemplo: 3,50.");
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
                   
                }
                catch (Exception ex)
                {
                    listar();
                    MessageBox.Show("Erro ao cadastrar: Cheque se o código já existe\n" + ex.Message);
                }
                                                        
                SQLiteCommand cmd3 = new SQLiteCommand("UPDATE CATEGORIAS SET NOME=@NOME, PRECO=@PRECO, CATEGORIA=@CATEGORIA WHERE CODIGO=@CODIGO", conn);
                cmd3.Parameters.AddWithValue("@CODIGO", Convert.ToInt32(codTextBox.Text.Trim()));
                cmd3.Parameters.AddWithValue("@NOME", nomeTextBox.Text.Trim());
                cmd3.Parameters.AddWithValue("@PRECO", Convert.ToDouble(precoTextBox.Text.Trim()));
                cmd3.Parameters.AddWithValue("@CATEGORIA", "*Atualização*" + comboBox1.Text.Trim());
                

                try
                {
                    cmd3.ExecuteNonQuery();
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
            textBox2.Text = "";
            textBox3.Text = "";


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
            string cupom = "";
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
                    //cupom = cupom + "\r\n" + Convert.ToInt32(dataGridView3.Rows[k].Cells[0].Value).ToString();
                    cupom = cupom + "" + dataGridView3.Rows[k].Cells[1].Value.ToString();

                    int row = dataGridView3.Rows[k].Cells[1].Value.ToString().Length+1;

                    string yourValue = (((double)Convert.ToDouble(dataGridView3.Rows[k].Cells[2].Value) / 100) * 100).ToString("C");
                    row = row + yourValue.Length;
                    row = 33 - row;
                    string spaces="";
                    for(int n = 0; n < row; n++)
                    {
                        spaces = spaces + " ";
                    }
                    cupom = cupom +spaces+ yourValue+"\r\n";
                    
                    using (cmd2)
                    {
                        cmd2.ExecuteNonQuery();
                    }
                }
                atualizaTotal();
                cupom = cupom + "---------------------------------\r\nTOTAL: " + total;
                listar();
                dataGridView3.DataSource = null;
                dataGridView3.Rows.Clear();
                atualizaTotal();
                try
                {
                    int iRetorno;
                    iRetorno = MP2032.FormataTX(Cabecalho(0) + cupom + finalConta(), 1, 0, 0, 1, 0);

                    iRetorno = MP2032.AcionaGuilhotina(1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show("Conta impressa");
                }
                
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
                iRetorno = MP2032.FormataTX(Cabecalho(1)+textArea.Text + finalConta(), 1, 0, 0, 1, 0);

                iRetorno = MP2032.AcionaGuilhotina(1);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            func();
            int iRetorno;
            int charCode = 27;
            int charCode2 = 118;
            int charCode3 = 140;

            char specialChar = Convert.ToChar(charCode);
            char specialChar2 = Convert.ToChar(charCode2);
            char specialChar3 = Convert.ToChar(charCode3);

            string s_cmdTX = "" + specialChar + specialChar2 + specialChar3;
            DateTime saveNow = DateTime.Now;
            try
            {
                iRetorno = MP2032.ComandoTX(s_cmdTX, s_cmdTX.Length);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("INSERT INTO H_GAVETA(CODIGO_CONTA, DATA) VALUES (@CODIGO_CONTA, @DATA)", conn);
            cmd.Parameters.AddWithValue("@CODIGO_CONTA", id);
            cmd.Parameters.AddWithValue("@DATA", saveNow.ToString());
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {


                return;
            }

        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            atualizaTotal();
            
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

            //diaTextBox.Text = saveNow.Day.ToString();
            //mesTextBox.Text = saveNow.Month.ToString();
            //anoTextBox.Text = saveNow.Year.ToString();

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

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void Tabs_KeyPress(object sender, KeyPressEventArgs e)
        {
          
        }

        private string Cabecalho(int k) {

            DateTime saveNow = DateTime.Now;
            func();
            string c="";
            if(k==0)
                c = "   RESTAURANTE QUINTA DE VIANA\r\n  RUA ASPAZIA VAREJÃO DIAS, 224\r\n       CENTRO - VIANA - ES\r\nCNPJ:21.600.905/0001-43\r\nIE:083.07828-2\r\n---------------------------------\r\n"+saveNow+ "\r\nCódigo da Conta: " + (id-1) + "\r\n              CUPOM\r\n";
            if(k==1)
                c = "   RESTAURANTE QUINTA DE VIANA\r\n  RUA ASPAZIA VAREJÃO DIAS, 224\r\n       CENTRO - VIANA - ES\r\nCNPJ:21.600.905/0001-43\r\nIE:083.07828-2\r\n---------------------------------\r\n" + saveNow + "\r\n";
            return c;
        }

        private string finalConta()
        {

            string c = "\r\n---------------------------------\r\n    OBRIGADO PELA PREFERÊNCIA\r\n    www.quintadeviana.com.br\r\n          (27)3255-1153\r\n";

            return c;
        }

        private void Tabs_KeyDown(object sender, KeyEventArgs e)
        {
            if( e.KeyCode == Keys.F1)
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                DateTime saveNow = DateTime.Now;
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                SQLiteCommand cmd = new SQLiteCommand("INSERT INTO H_GAVETA(CODIGO_CONTA, DATA) VALUES (@CODIGO_CONTA, @DATA)", conn);
                cmd.Parameters.AddWithValue("@CODIGO_CONTA", id);
                cmd.Parameters.AddWithValue("@DATA", saveNow.ToString());
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {


                    return;
                }
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
                    }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void precoTextBox_TextChanged(object sender, EventArgs e)
        {
            if (precoTextBox.Text.ToString().Contains("."))
                precoTextBox.Text = "Utilize vírgula.";
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                string s = row.Cells[0].Value.ToString();
                if (!s.StartsWith(textBox2.Text, true, null))
                {
                    CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[dataGridView2.DataSource];
                    currencyManager1.SuspendBinding();
                    row.Visible = false;
                    currencyManager1.ResumeBinding();
                }
                else
                    row.Visible = true;
            }
            
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            double total = 0;
            
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("SELECT SUM(PRECO) FROM CONTAS WHERE DATA LIKE @DATA", conn);
            cmd.Parameters.AddWithValue("@DATA", "%" + fechaDataText.Text + "%");
            try
            {
                SQLiteDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    try
                    {

                        total = dr.GetDouble(0);
                        //MessageBox.Show(id + "");  //proxima conta
                    }
                    catch (Exception ex)
                    {

                        //MessageBox.Show(ex.Message + "\n ID = "+id);
                    }
                    dr.Close();
                    conn.Close();
                }
                fechaValorText.Text = "Total do dia: " + total;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            

            SQLiteConnection conn2 = new SQLiteConnection(conexao);
            if (conn2.State == ConnectionState.Closed)
                conn2.Open();
            SQLiteCommand cmd2 = new SQLiteCommand("SELECT * FROM CONTAS WHERE DATA LIKE @DATA", conn2);
            
            cmd2.Parameters.AddWithValue("@DATA", "%" + fechaDataText.Text + "%");
            List<Relatorio> lista = new List<Relatorio>();
            string tudo = "";
            tudo = "Total do dia: " + total+ "\r\n";
            try
            {
                SQLiteDataReader dr = cmd2.ExecuteReader();
                while (dr.Read())
                {
                    int qtdade, cod, cod_conta;
                    double preco;
                    string data, nome;
                    try
                    {
                        lista.Add(new Relatorio
                        {
                            cod = Convert.ToInt32(dr["CODIGO"]),
                            nome = dr["NOME"].ToString()
                            ,
                            data = dr["DATA"].ToString()
                            ,
                            preco = Convert.ToDouble(dr["PRECO"]),
                            codConta = Convert.ToInt32(dr["CODIGO_CONTA"]),
                            qtdade = Convert.ToInt32(dr["ID"])
                        });


                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message );
                    }
                    
                }
                dr.Close();
                conn2.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            
            for (int i = 0; i < lista.Count; i++)
            {
                tudo = tudo + lista[i].formata();
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Users\\Quinta de Viana\\OneDrive\\Documentos\\" + "relatorio" +".txt");
            file.WriteLine(tudo);

            file.Close();
        }

        private void tabPage3_Enter_1(object sender, EventArgs e)
        {
            DateTime saveNow = DateTime.Now;
            fechaDataText.Text = System.DateTime.Today.ToShortDateString();
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                string s = row.Cells[1].Value.ToString();
                if (!s.StartsWith(textBox3.Text, true, null))
                {
                    CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[dataGridView2.DataSource];
                    currencyManager1.SuspendBinding();
                    row.Visible = false;
                    currencyManager1.ResumeBinding();
                }
                else
                    row.Visible = true;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (this.dataGridView3.SelectedRows.Count > 0)
            {
                dataGridView3.Rows.RemoveAt(this.dataGridView3.SelectedRows[0].Index);
            }
            atualizaTotal();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string s = row.Cells[0].Value.ToString();
                if (!s.StartsWith(textBox5.Text, true, null))
                {
                    CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[dataGridView1.DataSource];
                    currencyManager1.SuspendBinding();
                    row.Visible = false;
                    currencyManager1.ResumeBinding();
                }
                else
                    row.Visible = true;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string s = row.Cells[1].Value.ToString();
                if (!s.StartsWith(textBox4.Text, true, null))
                {
                    CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[dataGridView1.DataSource];
                    currencyManager1.SuspendBinding();
                    row.Visible = false;
                    currencyManager1.ResumeBinding();
                }
                else
                    row.Visible = true;
            }
        }
    }
}