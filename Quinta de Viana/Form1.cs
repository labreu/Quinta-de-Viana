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
            listar();
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
                }
            }
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
                }
                nomeTextBox.Text = string.Empty;
                precoTextBox.Text = string.Empty;
                descricaoTextBox.Text = string.Empty;
                codTextBox.Text = string.Empty;
                listar();
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
    }
}