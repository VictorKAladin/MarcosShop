using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web;
using System.Web.UI;

namespace PrjMercado
{
    public partial class perfil : Page
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

            bool administrador;
            int idUsuario;
            if (!TentarObterConta(out administrador, out idUsuario))
            {
                RedirecionarParaLogin();
                return;
            }

            pnlAdmin.Visible = administrador;
            pnlPedidosCliente.Visible = !administrador;
            pnlEditEndereco.Visible = !administrador;
            lblTipoConta.Text = administrador ? "Perfil do administrador" : "Perfil do cliente";
            lblConfirmacao.Text = HttpUtility.HtmlEncode(administrador
                ? "Tem certeza de que deseja excluir sua conta de administrador?"
                : "Tem certeza de que deseja excluir sua conta? Os pedidos associados a ela também serão excluídos.");

            if (!IsPostBack)
                CarregarPerfil(idUsuario, administrador);
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

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            if (!GarantirConta()) return;
            lblErro.Visible = false;
            pnlVisualizacao.Visible = false;
            pnlEdicao.Visible = true;
        }

        protected void btnCancelarEdicao_Click(object sender, EventArgs e)
        {
            bool administrador;
            int idUsuario;
            if (!TentarObterConta(out administrador, out idUsuario)) { RedirecionarParaLogin(); return; }
            pnlEdicao.Visible = false;
            pnlVisualizacao.Visible = true;
            CarregarPerfil(idUsuario, administrador);
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            bool administrador;
            int idUsuario;
            if (!TentarObterConta(out administrador, out idUsuario)) { RedirecionarParaLogin(); return; }

            string nome = (txtEditNome.Text ?? String.Empty).Trim();
            string email = (txtEditEmail.Text ?? String.Empty).Trim();
            string telefone = (txtEditTelefone.Text ?? String.Empty).Trim();
            string endereco = (txtEditEndereco.Text ?? String.Empty).Trim();

            if (nome.Length == 0 || nome.Length > 100)
            {
                MostrarMensagem("Informe um nome com até 100 caracteres.", false);
                return;
            }
            if (email.Length == 0 || email.Length > 100 || !EmailValido(email))
            {
                MostrarMensagem("Informe um endereço de e-mail válido com até 100 caracteres.", false);
                return;
            }
            if (telefone.Length > 20)
            {
                MostrarMensagem("O telefone pode ter no máximo 20 caracteres.", false);
                return;
            }
            if (!administrador && endereco.Length > 200)
            {
                MostrarMensagem("O endereço pode ter no máximo 200 caracteres.", false);
                return;
            }

            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarMensagem("A conexão MercadoOnline não está configurada.", false);
                return;
            }

            string sql = administrador
                ? "UPDATE Admin SET nome = @nome, email = @email, telefone = @telefone WHERE idAdmin = @id"
                : "UPDATE Cliente SET nome = @nome, email = @email, telefone = @telefone, endereco = @endereco WHERE idCliente = @id";

            try
            {
                using (SqlConnection conexao = conexaoBanco)
                using (SqlCommand comando = new SqlCommand(sql, conexao))
                {
                    comando.Parameters.Add("@nome", SqlDbType.VarChar, 100).Value = nome;
                    comando.Parameters.Add("@email", SqlDbType.VarChar, 100).Value = email;
                    comando.Parameters.Add("@telefone", SqlDbType.VarChar, 20).Value = String.IsNullOrEmpty(telefone) ? (object)DBNull.Value : telefone;
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario;
                    if (!administrador)
                        comando.Parameters.Add("@endereco", SqlDbType.VarChar, 200).Value = String.IsNullOrEmpty(endereco) ? (object)DBNull.Value : endereco;

                    conexao.Open();
                    if (comando.ExecuteNonQuery() == 0)
                    {
                        MostrarMensagem("A conta não foi encontrada para atualização.", false);
                        return;
                    }
                }

                Session["UsuarioNome"] = nome;
                pnlEdicao.Visible = false;
                pnlVisualizacao.Visible = true;
                CarregarPerfil(idUsuario, administrador);
                MostrarMensagem("Dados atualizados com sucesso.", true);
            }
            catch (SqlException erro)
            {
                if (erro.Number == 2601 || erro.Number == 2627)
                    MostrarMensagem("Este e-mail já está cadastrado em outra conta.", false);
                else
                    MostrarMensagem("Não foi possível atualizar os dados no banco MercadoOnline.", false);
            }
        }

        protected void btnSair_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("index.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            if (!GarantirConta()) return;
            lblErro.Visible = false;
            pnlConfirmacao.Visible = true;
        }

        protected void btnCancelarExclusao_Click(object sender, EventArgs e)
        {
            pnlConfirmacao.Visible = false;
        }

        protected void btnConfirmarExclusao_Click(object sender, EventArgs e)
        {
            bool administrador;
            int idUsuario;
            if (!TentarObterConta(out administrador, out idUsuario)) { RedirecionarParaLogin(); return; }

            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                pnlConfirmacao.Visible = false;
                MostrarMensagem("A conexão MercadoOnline não está configurada.", false);
                return;
            }

            try
            {
                using (SqlConnection conexao = conexaoBanco)
                {
                    conexao.Open();
                    using (SqlTransaction transacao = conexao.BeginTransaction())
                    {
                        if (administrador)
                        {
                            using (SqlCommand contar = new SqlCommand("SELECT COUNT(*) FROM Admin", conexao, transacao))
                            {
                                int quantidadeAdmins = Convert.ToInt32(contar.ExecuteScalar());
                                if (quantidadeAdmins <= 1)
                                {
                                    transacao.Rollback();
                                    pnlConfirmacao.Visible = false;
                                    MostrarMensagem("Não é possível excluir o último administrador do sistema.", false);
                                    return;
                                }
                            }

                            using (SqlCommand excluir = new SqlCommand("DELETE FROM Admin WHERE idAdmin = @id", conexao, transacao))
                            {
                                excluir.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario;
                                if (excluir.ExecuteNonQuery() == 0)
                                {
                                    transacao.Rollback();
                                    pnlConfirmacao.Visible = false;
                                    MostrarMensagem("A conta não foi encontrada.", false);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            using (SqlCommand excluirItens = new SqlCommand("DELETE FROM Produto_Pedido WHERE idPedido IN (SELECT idPedido FROM Pedido WHERE idCliente = @id)", conexao, transacao))
                            {
                                excluirItens.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario;
                                excluirItens.ExecuteNonQuery();
                            }
                            using (SqlCommand excluirPedidos = new SqlCommand("DELETE FROM Pedido WHERE idCliente = @id", conexao, transacao))
                            {
                                excluirPedidos.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario;
                                excluirPedidos.ExecuteNonQuery();
                            }
                            using (SqlCommand excluirCliente = new SqlCommand("DELETE FROM Cliente WHERE idCliente = @id", conexao, transacao))
                            {
                                excluirCliente.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario;
                                if (excluirCliente.ExecuteNonQuery() == 0)
                                {
                                    transacao.Rollback();
                                    pnlConfirmacao.Visible = false;
                                    MostrarMensagem("A conta não foi encontrada.", false);
                                    return;
                                }
                            }
                        }

                        transacao.Commit();
                    }
                }

                Session.Clear();
                Session.Abandon();
                Response.Redirect("index.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (SqlException)
            {
                pnlConfirmacao.Visible = false;
                MostrarMensagem("Não foi possível excluir a conta no banco MercadoOnline.", false);
            }
        }

        private void CarregarPerfil(int idUsuario, bool administrador)
        {
            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarMensagem("A conexão com o banco MercadoOnline não está configurada.", false);
                return;
            }

            string sql = administrador
                ? "SELECT nome, email, telefone, CAST(NULL AS VARCHAR(200)) AS endereco FROM Admin WHERE idAdmin = @id"
                : "SELECT nome, email, telefone, endereco FROM Cliente WHERE idCliente = @id";

            try
            {
                using (SqlConnection conexao = conexaoBanco)
                using (SqlCommand comando = new SqlCommand(sql, conexao))
                {
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario;
                    conexao.Open();
                    using (SqlDataReader leitor = comando.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!leitor.Read())
                        {
                            Session.Clear();
                            RedirecionarParaLogin();
                            return;
                        }

                        string nome = leitor["nome"].ToString();
                        string email = leitor["email"].ToString();
                        string telefone = leitor["telefone"] == DBNull.Value ? String.Empty : leitor["telefone"].ToString();
                        string endereco = leitor["endereco"] == DBNull.Value ? String.Empty : leitor["endereco"].ToString();

                        lblNome.Text = HttpUtility.HtmlEncode(nome);
                        lblEmail.Text = HttpUtility.HtmlEncode(email);
                        lblTelefone.Text = HttpUtility.HtmlEncode(String.IsNullOrWhiteSpace(telefone) ? "Não informado" : telefone);
                        lblEndereco.Text = HttpUtility.HtmlEncode(endereco);
                        pnlEndereco.Visible = !administrador && !String.IsNullOrWhiteSpace(endereco);

                        txtEditNome.Text = nome;
                        txtEditEmail.Text = email;
                        txtEditTelefone.Text = telefone;
                        txtEditEndereco.Text = endereco;
                    }
                }
            }
            catch (SqlException)
            {
                MostrarMensagem("Não foi possível carregar os dados do perfil no banco MercadoOnline.", false);
            }
        }

        private bool GarantirConta()
        {
            bool administrador;
            int idUsuario;
            if (TentarObterConta(out administrador, out idUsuario)) return true;
            RedirecionarParaLogin();
            return false;
        }

        private bool TentarObterConta(out bool administrador, out int idUsuario)
        {
            idUsuario = 0;
            string tipo = Convert.ToString(Session["UsuarioTipo"]);
            administrador = String.Equals(tipo, "Admin", StringComparison.OrdinalIgnoreCase);
            bool cliente = String.Equals(tipo, "Cliente", StringComparison.OrdinalIgnoreCase);
            return (administrador || cliente) && Int32.TryParse(Convert.ToString(Session["UsuarioId"]), out idUsuario);
        }

        private static bool EmailValido(string email)
        {
            try { return new MailAddress(email).Address == email; }
            catch (FormatException) { return false; }
        }

        private void MostrarMensagem(string mensagem, bool sucesso)
        {
            lblErro.Text = HttpUtility.HtmlEncode(mensagem);
            lblErro.CssClass = sucesso ? "status sucesso" : "status erro";
            lblErro.Visible = true;
        }

        private void RedirecionarParaLogin()
        {
            Response.Redirect("login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
