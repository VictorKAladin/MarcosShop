using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PrjMercado
{
    public partial class carrinho : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensagem.Visible = false;
            if (!IsPostBack)
            {
                string nome = Convert.ToString(Session["UsuarioNome"]);
                string primeiroNome = ObterPrimeiroNome(nome);
                if (primeiroNome.Length > 0)
                {
                    btnLogin.Text = primeiroNome;
                    btnLogin.ToolTip = "Acessar o perfil de " + primeiroNome;
                    imgPerfil.AlternateText = "Acessar o perfil de " + primeiroNome;
                    imgPerfil.ToolTip = "Acessar o perfil de " + primeiroNome;
                }
                else
                {
                    btnLogin.Text = "Acesse sua conta";
                    btnLogin.ToolTip = "Acessar sua conta";
                    imgPerfil.AlternateText = "Acessar sua conta";
                    imgPerfil.ToolTip = "Acessar sua conta";
                }

                CarregarCarrinho();
            }
        }

        private static string ObterPrimeiroNome(string nomeCompleto)
        {
            if (String.IsNullOrWhiteSpace(nomeCompleto)) return String.Empty;
            string[] partes = nomeCompleto.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return partes.Length == 0 ? String.Empty : partes[0];
        }


        protected void btnHomePage_Click(object sender, EventArgs e)
        {
            Response.Redirect("index.aspx");
        }

        protected void btnProdutos_Click(object sender, EventArgs e)
        {
            Response.Redirect("produtos.aspx");
        }

        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            string busca = (txtPesquisa.Text ?? string.Empty).Trim();
            Response.Redirect("produtos.aspx?busca=" + Server.UrlEncode(busca));
        }

        protected void btnSobreNos_Click(object sender, EventArgs e)
        {
            Response.Redirect("sobreNos.aspx");
        }

        protected void btnContato_Click(object sender, EventArgs e)
        {
            Response.Redirect("faleConosco.aspx");
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (Session["UsuarioId"] == null)
            {
                Response.Redirect("login.aspx");
                return;
            }

            string tipo = Convert.ToString(Session["UsuarioTipo"]);
            if (String.Equals(tipo, "Admin", StringComparison.OrdinalIgnoreCase))
                Response.Redirect("perfilAdmin.aspx");
            else if (String.Equals(tipo, "Cliente", StringComparison.OrdinalIgnoreCase))
                Response.Redirect("perfil.aspx");
            else
                Response.Redirect("login.aspx");
        }

        protected void btnCarrinho_Click(object sender, EventArgs e)
        {
            Response.Redirect("carrinho.aspx");
        }

        protected void rptCarrinho_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Remover") return;

            int idProduto;
            if (!Int32.TryParse(Convert.ToString(e.CommandArgument), out idProduto)) return;

            Dictionary<int, int> carrinho = ObterCarrinho();
            carrinho.Remove(idProduto);
            Session["CarrinhoProdutos"] = carrinho;
            CarregarCarrinho();
        }

        protected void btnLimparCarrinho_Click(object sender, EventArgs e)
        {
            Session.Remove("CarrinhoProdutos");
            CarregarCarrinho();
        }

        private Dictionary<int, int> ObterCarrinho()
        {
            Dictionary<int, int> carrinho = Session["CarrinhoProdutos"] as Dictionary<int, int>;
            return carrinho ?? new Dictionary<int, int>();
        }

        private void CarregarCarrinho()
        {
            Dictionary<int, int> carrinho = ObterCarrinho();
            if (carrinho.Count == 0)
            {
                ExibirCarrinhoVazio();
                return;
            }

            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarErro("A conexão MercadoOnline não está configurada.");
                return;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT P.idProduto, P.descricao, P.preco, P.estoque, C.descricao AS categoria ");
            sql.Append("FROM Produto P INNER JOIN Categoria C ON P.idCategoria = C.idCategoria ");
            sql.Append("WHERE P.idProduto IN (");

            int indice = 0;
            foreach (int idProduto in carrinho.Keys)
            {
                if (indice > 0) sql.Append(", ");
                sql.Append("@produto").Append(indice++);
            }
            sql.Append(") ORDER BY C.descricao, P.descricao");

            try
            {
                using (SqlConnection conexao = conexaoBanco)
                using (SqlCommand comando = new SqlCommand(sql.ToString(), conexao))
                using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                {
                    indice = 0;
                    foreach (int idProduto in carrinho.Keys)
                        comando.Parameters.Add("@produto" + indice++, SqlDbType.Int).Value = idProduto;

                    DataTable produtos = new DataTable();
                    adaptador.Fill(produtos);
                    produtos.Columns.Add("quantidade", typeof(int));
                    produtos.Columns.Add("subtotal", typeof(decimal));

                    decimal total = 0m;
                    HashSet<int> produtosEncontrados = new HashSet<int>();
                    List<DataRow> linhasIndisponiveis = new List<DataRow>();
                    foreach (DataRow produto in produtos.Rows)
                    {
                        int idProduto = Convert.ToInt32(produto["idProduto"]);
                        int quantidade = carrinho[idProduto];
                        int estoque = Convert.ToInt32(produto["estoque"]);

                        produtosEncontrados.Add(idProduto);
                        if (estoque <= 0)
                        {
                            carrinho.Remove(idProduto);
                            linhasIndisponiveis.Add(produto);
                            continue;
                        }

                        if (quantidade > estoque)
                        {
                            quantidade = estoque;
                            carrinho[idProduto] = quantidade;
                        }

                        decimal subtotal = Convert.ToDecimal(produto["preco"], CultureInfo.InvariantCulture) * quantidade;
                        produto["quantidade"] = quantidade;
                        produto["subtotal"] = subtotal;
                        total += subtotal;
                    }
                    foreach (DataRow linha in linhasIndisponiveis)
                        produtos.Rows.Remove(linha);

                    List<int> produtosRemovidos = new List<int>();
                    foreach (int idProduto in carrinho.Keys)
                        if (!produtosEncontrados.Contains(idProduto) || carrinho[idProduto] <= 0)
                            produtosRemovidos.Add(idProduto);
                    foreach (int idProduto in produtosRemovidos)
                        carrinho.Remove(idProduto);

                    Session["CarrinhoProdutos"] = carrinho;

                    if (produtos.Rows.Count == 0)
                    {
                        ExibirCarrinhoVazio();
                        return;
                    }

                    rptCarrinho.DataSource = produtos;
                    rptCarrinho.DataBind();
                    lblTotal.Text = total.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));
                    pnlCarrinhoVazio.Visible = false;
                    pnlItensCarrinho.Visible = true;
                }
            }
            catch (SqlException)
            {
                pnlCarrinhoVazio.Visible = false;
                pnlItensCarrinho.Visible = false;
                MostrarErro("Não foi possível carregar o carrinho do banco MercadoOnline.");
            }
        }

        private void ExibirCarrinhoVazio()
        {
            rptCarrinho.DataSource = null;
            rptCarrinho.DataBind();
            pnlItensCarrinho.Visible = false;
            pnlCarrinhoVazio.Visible = true;
        }

        private void MostrarErro(string mensagem)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.Visible = true;
            pnlCarrinhoVazio.Visible = false;
            pnlItensCarrinho.Visible = false;
        }
    }
}
