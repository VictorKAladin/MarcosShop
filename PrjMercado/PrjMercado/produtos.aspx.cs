using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace PrjMercado
{
    public partial class produtos : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
            }

            lblMensagem.Visible = false;
            if (!IsPostBack)
            {
                string buscaInicial = Request.QueryString["busca"] ?? String.Empty;
                txtPesquisa.Text = buscaInicial;
                CarregarCategorias();
                SelecionarCategoriasDaUrl(Request.QueryString["categorias"]);
                CarregarProdutos();
            }
        }

        private void SelecionarCategoriasDaUrl(string valor)
        {
            if (String.IsNullOrWhiteSpace(valor)) return;

            string[] categoriasSolicitadas = valor.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (ListItem item in cblCategorias.Items)
            {
                foreach (string categoria in categoriasSolicitadas)
                {
                    if (String.Equals(item.Text.Trim(), categoria.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        item.Selected = true;
                        break;
                    }
                }
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
        protected void cblCategorias_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarProdutos();
        }

        

        protected void rptProdutos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            DataRowView produto = e.Item.DataItem as DataRowView;
            if (produto == null) return;

            Image imgImagem = e.Item.FindControl("imgImagemProduto") as Image;
            HtmlGenericControl pnlIcone = e.Item.FindControl("pnlIconeProduto") as HtmlGenericControl;
            byte[] imagem = produto["imagem"] == DBNull.Value ? null : produto["imagem"] as byte[];
            string tipoImagem = produto["tipoImagem"] == DBNull.Value
                ? String.Empty
                : Convert.ToString(produto["tipoImagem"]);
            if (imgImagem != null && imagem != null && imagem.Length > 0 &&
                (tipoImagem == "image/png" || tipoImagem == "image/jpeg" || tipoImagem == "image/webp" || tipoImagem == "image/gif"))
            {
                imgImagem.ImageUrl = "data:" + tipoImagem + ";base64," + Convert.ToBase64String(imagem);
                imgImagem.AlternateText = Convert.ToString(produto["descricao"]);
                imgImagem.Visible = true;
                if (pnlIcone != null) pnlIcone.Visible = false;
            }

            int estoque = Convert.ToInt32(produto["estoque"]);
            TextBox txtQuantidade = e.Item.FindControl("txtQuantidade") as TextBox;
            Button btnAdicionar = e.Item.FindControl("btnAdicionarCarrinho") as Button;
            if (txtQuantidade != null)
            {
                txtQuantidade.Attributes["min"] = "1";
                txtQuantidade.Attributes["max"] = Math.Max(1, estoque).ToString();
                txtQuantidade.Enabled = estoque > 0;
            }
            if (btnAdicionar != null)
            {
                btnAdicionar.Enabled = estoque > 0;
                if (estoque <= 0) btnAdicionar.Text = "Indisponível";
            }
        }

        protected void rptProdutos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "AdicionarCarrinho") return;

            int idProduto;
            TextBox txtQuantidade = e.Item.FindControl("txtQuantidade") as TextBox;
            int quantidade;
            if (!Int32.TryParse(Convert.ToString(e.CommandArgument), out idProduto) ||
                txtQuantidade == null || !Int32.TryParse(txtQuantidade.Text, out quantidade) || quantidade < 1)
            {
                MostrarErro("Informe uma quantidade válida para adicionar ao carrinho.");
                return;
            }

            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarErro("A conexão MercadoOnline não está configurada.");
                return;
            }

            string descricao;
            int estoque;
            try
            {
                using (SqlConnection conexao = conexaoBanco)
                using (SqlCommand comando = new SqlCommand("SELECT descricao, estoque FROM Produto WHERE idProduto = @idProduto", conexao))
                {
                    comando.Parameters.Add("@idProduto", SqlDbType.Int).Value = idProduto;
                    conexao.Open();
                    using (SqlDataReader leitor = comando.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!leitor.Read())
                        {
                            MostrarErro("Este produto não está mais disponível.");
                            return;
                        }
                        descricao = leitor["descricao"].ToString();
                        estoque = Convert.ToInt32(leitor["estoque"]);
                    }
                }
            }
            catch (SqlException)
            {
                MostrarErro("Não foi possível validar o produto no banco MercadoOnline.");
                return;
            }

            if (quantidade > estoque)
            {
                MostrarErro("A quantidade escolhida excede o estoque disponível (" + estoque + ").");
                return;
            }

            Dictionary<int, int> carrinho = Session["CarrinhoProdutos"] as Dictionary<int, int>;
            if (carrinho == null)
                carrinho = new Dictionary<int, int>();

            int quantidadeNoCarrinho;
            carrinho.TryGetValue(idProduto, out quantidadeNoCarrinho);
            if (quantidadeNoCarrinho + quantidade > estoque)
            {
                MostrarErro("A quantidade total no carrinho excederia o estoque disponível (" + estoque + ").");
                return;
            }

            carrinho[idProduto] = quantidadeNoCarrinho + quantidade;
            Session["CarrinhoProdutos"] = carrinho;
            lblPopupMensagem.Text = HttpUtility.HtmlEncode(quantidade + " × " + descricao + " adicionado(s) ao carrinho.");
            pnlPopupAdicionado.Visible = true;
        }

        protected void btnFecharPopup_Click(object sender, EventArgs e)
        {
            pnlPopupAdicionado.Visible = false;
        }

        private void CarregarCategorias()
        {
            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarErro("A conexão MercadoOnline não está configurada.");
                return;
            }

            const string sql = "SELECT idCategoria, descricao FROM Categoria ORDER BY descricao";
            try
            {
                using (SqlConnection conexao = conexaoBanco)
                using (SqlDataAdapter adaptador = new SqlDataAdapter(sql, conexao))
                {
                    DataTable categorias = new DataTable();
                    adaptador.Fill(categorias);
                    cblCategorias.DataSource = categorias;
                    cblCategorias.DataTextField = "descricao";
                    cblCategorias.DataValueField = "idCategoria";
                    cblCategorias.DataBind();
                }
            }
            catch (SqlException)
            {
                MostrarErro("Não foi possível carregar as categorias. Confira se o banco MercadoOnline está disponível.");
            }
        }

        private void CarregarProdutos()
        {
            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarErro("A conexão MercadoOnline não está configurada.");
                return;
            }

            List<int> categoriasSelecionadas = new List<int>();
            foreach (ListItem item in cblCategorias.Items)
            {
                int idCategoria;
                if (item.Selected && Int32.TryParse(item.Value, out idCategoria))
                    categoriasSelecionadas.Add(idCategoria);
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT P.idProduto, P.descricao, P.preco, P.estoque, P.imagem, P.tipoImagem, C.descricao AS categoria ");
            sql.Append("FROM Produto P INNER JOIN Categoria C ON P.idCategoria = C.idCategoria ");
            sql.Append("WHERE (@busca = '' OR P.descricao LIKE @termo OR C.descricao LIKE @termo) ");

            if (categoriasSelecionadas.Count > 0)
            {
                sql.Append("AND P.idCategoria IN (");
                for (int i = 0; i < categoriasSelecionadas.Count; i++)
                {
                    if (i > 0) sql.Append(", ");
                    sql.Append("@categoria").Append(i);
                }
                sql.Append(") ");
            }
            sql.Append("ORDER BY C.descricao, P.descricao");

            string busca = (txtPesquisa.Text ?? String.Empty).Trim();
            try
            {
                using (SqlConnection conexao = conexaoBanco)
                using (SqlCommand comando = new SqlCommand(sql.ToString(), conexao))
                using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                {
                    comando.Parameters.Add("@busca", SqlDbType.VarChar, 100).Value = busca;
                    comando.Parameters.Add("@termo", SqlDbType.VarChar, 202).Value = "%" + busca + "%";
                    for (int i = 0; i < categoriasSelecionadas.Count; i++)
                        comando.Parameters.Add("@categoria" + i, SqlDbType.Int).Value = categoriasSelecionadas[i];

                    DataTable produtos = new DataTable();
                    adaptador.Fill(produtos);
                    rptProdutos.DataSource = produtos;
                    rptProdutos.DataBind();
                    lblContagem.Text = String.Format("{0} produto(s) encontrado(s)", produtos.Rows.Count);
                    pnlSemResultados.Visible = produtos.Rows.Count == 0;
                }
            }
            catch (SqlException)
            {
                rptProdutos.DataSource = null;
                rptProdutos.DataBind();
                lblContagem.Text = String.Empty;
                pnlSemResultados.Visible = false;
                MostrarErro("Não foi possível carregar os produtos. Confira se o banco MercadoOnline está disponível.");
            }
        }

        private void MostrarErro(string mensagem)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.Visible = true;
        }
    }
}
