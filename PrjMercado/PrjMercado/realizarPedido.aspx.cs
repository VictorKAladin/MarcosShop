using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;

namespace PrjMercado
{
    public partial class realizarPedido : Page
    {
        private sealed class ItemPedido
        {
            public int IdProduto { get; set; }
            public int Quantidade { get; set; }
            public int Estoque { get; set; }
            public string Descricao { get; set; }
            public decimal Preco { get; set; }
        }

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
            int idCliente;
            if (!TentarObterCliente(out idCliente))
            {
                Response.Redirect("login.aspx?returnUrl=realizarPedido.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
                CarregarCheckout(idCliente);
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


        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            int idCliente;
            if (!TentarObterCliente(out idCliente))
            {
                Response.Redirect("login.aspx?returnUrl=realizarPedido.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            string endereco = (txtEndereco.Text ?? String.Empty).Trim();
            string pagamento = ddlPagamento.SelectedValue;
            if (endereco.Length == 0 || endereco.Length > 200)
            {
                MostrarMensagem("Informe um endereço de entrega com até 200 caracteres.");
                return;
            }
            if (pagamento != "Pix" && pagamento != "Dinheiro" && pagamento != "Cartao")
            {
                MostrarMensagem("Selecione uma forma de pagamento válida.");
                return;
            }

            Dictionary<int, int> carrinho = Session["CarrinhoProdutos"] as Dictionary<int, int>;
            if (carrinho == null || carrinho.Count == 0)
            {
                ExibirCarrinhoVazio();
                return;
            }

            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarMensagem("A conexão MercadoOnline não está configurada.");
                return;
            }

            try
            {
                int idPedido;
                decimal total;
                using (SqlConnection conexao = conexaoBanco)
                {
                    conexao.Open();
                    using (SqlTransaction transacao = conexao.BeginTransaction(IsolationLevel.Serializable))
                    {
                        List<ItemPedido> itens = CarregarItensBloqueandoEstoque(conexao, transacao, carrinho);
                        total = 0m;
                        foreach (ItemPedido item in itens)
                            total += item.Preco * item.Quantidade;

                        using (SqlCommand atualizarEndereco = new SqlCommand("UPDATE dbo.Cliente SET endereco = @endereco WHERE idCliente = @idCliente", conexao, transacao))
                        {
                            atualizarEndereco.Parameters.Add("@endereco", SqlDbType.VarChar, 200).Value = endereco;
                            atualizarEndereco.Parameters.Add("@idCliente", SqlDbType.Int).Value = idCliente;
                            if (atualizarEndereco.ExecuteNonQuery() == 0)
                                throw new InvalidOperationException("Não foi possível localizar a conta do cliente.");
                        }

                        using (SqlCommand criarPedido = new SqlCommand("INSERT INTO dbo.Pedido (idCliente, valorTotal, status, formaPagamento) VALUES (@idCliente, @valorTotal, 'Pendente', @pagamento); SELECT CAST(SCOPE_IDENTITY() AS INT);", conexao, transacao))
                        {
                            criarPedido.Parameters.Add("@idCliente", SqlDbType.Int).Value = idCliente;
                            SqlParameter valor = criarPedido.Parameters.Add("@valorTotal", SqlDbType.Decimal);
                            valor.Precision = 10;
                            valor.Scale = 2;
                            valor.Value = total;
                            criarPedido.Parameters.Add("@pagamento", SqlDbType.VarChar, 30).Value = pagamento;
                            idPedido = Convert.ToInt32(criarPedido.ExecuteScalar());
                        }

                        foreach (ItemPedido item in itens)
                        {
                            using (SqlCommand adicionarItem = new SqlCommand("INSERT INTO dbo.Produto_Pedido (idPedido, idProduto, quantidade, precoUnitario) VALUES (@idPedido, @idProduto, @quantidade, @preco)", conexao, transacao))
                            {
                                adicionarItem.Parameters.Add("@idPedido", SqlDbType.Int).Value = idPedido;
                                adicionarItem.Parameters.Add("@idProduto", SqlDbType.Int).Value = item.IdProduto;
                                adicionarItem.Parameters.Add("@quantidade", SqlDbType.Int).Value = item.Quantidade;
                                SqlParameter preco = adicionarItem.Parameters.Add("@preco", SqlDbType.Decimal);
                                preco.Precision = 10;
                                preco.Scale = 2;
                                preco.Value = item.Preco;
                                adicionarItem.ExecuteNonQuery();
                            }

                            using (SqlCommand atualizarEstoque = new SqlCommand("UPDATE dbo.Produto SET estoque = estoque - @quantidade WHERE idProduto = @idProduto AND estoque >= @quantidade", conexao, transacao))
                            {
                                atualizarEstoque.Parameters.Add("@quantidade", SqlDbType.Int).Value = item.Quantidade;
                                atualizarEstoque.Parameters.Add("@idProduto", SqlDbType.Int).Value = item.IdProduto;
                                if (atualizarEstoque.ExecuteNonQuery() != 1)
                                    throw new InvalidOperationException("O estoque de um produto mudou durante a confirmação. Revise o carrinho.");
                            }
                        }

                        transacao.Commit();
                    }
                }

                Session.Remove("CarrinhoProdutos");
                pnlCheckout.Visible = false;
                pnlCarrinhoVazio.Visible = false;
                lblNumeroPedido.Text = idPedido.ToString(CultureInfo.GetCultureInfo("pt-BR"));
                lblTotalConfirmado.Text = total.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));
                pnlConcluido.Visible = true;
            }
            catch (InvalidOperationException erro)
            {
                MostrarMensagem(erro.Message);
            }
            catch (SqlException)
            {
                MostrarMensagem("Não foi possível registrar o pedido no banco MercadoOnline. Nenhuma alteração foi confirmada.");
            }
        }

        private void CarregarCheckout(int idCliente)
        {
            Dictionary<int, int> carrinho = Session["CarrinhoProdutos"] as Dictionary<int, int>;
            if (carrinho == null || carrinho.Count == 0)
            {
                ExibirCarrinhoVazio();
                return;
            }

            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarMensagem("A conexão MercadoOnline não está configurada.");
                pnlCheckout.Visible = false;
                return;
            }

            try
            {
                using (SqlConnection conexao = conexaoBanco)
                {
                    conexao.Open();
                    using (SqlCommand buscarEndereco = new SqlCommand("SELECT endereco FROM dbo.Cliente WHERE idCliente = @idCliente", conexao))
                    {
                        buscarEndereco.Parameters.Add("@idCliente", SqlDbType.Int).Value = idCliente;
                        object endereco = buscarEndereco.ExecuteScalar();
                        if (endereco == null)
                        {
                            Session.Clear();
                            Response.Redirect("login.aspx", false);
                            Context.ApplicationInstance.CompleteRequest();
                            return;
                        }
                        txtEndereco.Text = endereco == DBNull.Value ? String.Empty : endereco.ToString();
                    }

                    DataTable itens = new DataTable();
                    itens.Columns.Add("descricao", typeof(string));
                    itens.Columns.Add("quantidade", typeof(int));
                    itens.Columns.Add("preco", typeof(decimal));
                    itens.Columns.Add("subtotal", typeof(decimal));
                    decimal total = 0m;
                    bool estoqueValido = true;

                    foreach (KeyValuePair<int, int> entrada in carrinho)
                    {
                        using (SqlCommand buscarProduto = new SqlCommand("SELECT descricao, preco, estoque FROM dbo.Produto WHERE idProduto = @idProduto", conexao))
                        {
                            buscarProduto.Parameters.Add("@idProduto", SqlDbType.Int).Value = entrada.Key;
                            using (SqlDataReader leitor = buscarProduto.ExecuteReader(CommandBehavior.SingleRow))
                            {
                                if (!leitor.Read())
                                {
                                    estoqueValido = false;
                                    continue;
                                }

                                decimal preco = Convert.ToDecimal(leitor["preco"]);
                                int estoque = Convert.ToInt32(leitor["estoque"]);
                                int quantidade = entrada.Value;
                                if (quantidade < 1 || quantidade > estoque)
                                    estoqueValido = false;

                                decimal subtotal = preco * quantidade;
                                DataRow linha = itens.NewRow();
                                linha["descricao"] = leitor["descricao"].ToString();
                                linha["quantidade"] = quantidade;
                                linha["preco"] = preco;
                                linha["subtotal"] = subtotal;
                                itens.Rows.Add(linha);
                                total += subtotal;
                            }
                        }
                    }

                    rptItens.DataSource = itens;
                    rptItens.DataBind();
                    lblTotal.Text = total.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));
                    pnlTotal.Visible = itens.Rows.Count > 0;
                    pnlCheckout.Visible = itens.Rows.Count > 0;
                    pnlCarrinhoVazio.Visible = itens.Rows.Count == 0;
                    btnConfirmar.Enabled = estoqueValido && itens.Rows.Count > 0;
                    if (!estoqueValido)
                        MostrarMensagem("Um ou mais produtos não têm estoque suficiente. Volte ao carrinho para ajustar as quantidades.");
                }
            }
            catch (SqlException)
            {
                pnlCheckout.Visible = false;
                MostrarMensagem("Não foi possível carregar o pedido do banco MercadoOnline.");
            }
        }

        private static List<ItemPedido> CarregarItensBloqueandoEstoque(SqlConnection conexao, SqlTransaction transacao, Dictionary<int, int> carrinho)
        {
            List<ItemPedido> itens = new List<ItemPedido>();
            foreach (KeyValuePair<int, int> entrada in carrinho)
            {
                if (entrada.Value < 1)
                    throw new InvalidOperationException("Há uma quantidade inválida no carrinho. Revise seus produtos.");

                using (SqlCommand comando = new SqlCommand("SELECT descricao, preco, estoque FROM dbo.Produto WITH (UPDLOCK, ROWLOCK) WHERE idProduto = @idProduto", conexao, transacao))
                {
                    comando.Parameters.Add("@idProduto", SqlDbType.Int).Value = entrada.Key;
                    using (SqlDataReader leitor = comando.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!leitor.Read())
                            throw new InvalidOperationException("Um produto do carrinho não está mais disponível. Revise o carrinho.");

                        int estoque = Convert.ToInt32(leitor["estoque"]);
                        if (entrada.Value > estoque)
                            throw new InvalidOperationException("O estoque de " + leitor["descricao"] + " não é suficiente. Revise a quantidade no carrinho.");

                        itens.Add(new ItemPedido
                        {
                            IdProduto = entrada.Key,
                            Quantidade = entrada.Value,
                            Estoque = estoque,
                            Descricao = leitor["descricao"].ToString(),
                            Preco = Convert.ToDecimal(leitor["preco"])
                        });
                    }
                }
            }

            if (itens.Count == 0)
                throw new InvalidOperationException("O carrinho está vazio.");
            return itens;
        }

        private bool TentarObterCliente(out int idCliente)
        {
            idCliente = 0;
            return String.Equals(Convert.ToString(Session["UsuarioTipo"]), "Cliente", StringComparison.OrdinalIgnoreCase) &&
                   Int32.TryParse(Convert.ToString(Session["UsuarioId"]), out idCliente);
        }

        private void ExibirCarrinhoVazio()
        {
            pnlCheckout.Visible = false;
            pnlCarrinhoVazio.Visible = true;
            pnlConcluido.Visible = false;
        }

        private void MostrarMensagem(string mensagem)
        {
            lblMensagem.Text = Server.HtmlEncode(mensagem);
            lblMensagem.Visible = true;
        }
    }
}
