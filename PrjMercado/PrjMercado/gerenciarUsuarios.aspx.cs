using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PrjMercado
{
    public partial class gerenciarUsuarios : Page
    {
        private const int IteracoesPbkdf2 = 120000;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!GarantirAdministrador()) return;

            lblMensagem.Visible = false;
            pnlEndereco.Visible = ddlTipo.SelectedValue == "Cliente";
            if (!IsPostBack)
                CarregarUsuarios();
        }

        protected void ddlTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!GarantirAdministrador()) return;
            pnlEndereco.Visible = ddlTipo.SelectedValue == "Cliente";
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!GarantirAdministrador()) return;

            bool editando = !String.IsNullOrWhiteSpace(hfIdUsuario.Value);
            string tipo = editando ? hfTipoOriginal.Value : ddlTipo.SelectedValue;
            string nome = (txtNome.Text ?? String.Empty).Trim();
            string email = (txtEmail.Text ?? String.Empty).Trim();
            string telefone = (txtTelefone.Text ?? String.Empty).Trim();
            string endereco = (txtEndereco.Text ?? String.Empty).Trim();
            string senha = txtSenha.Text;
            int idUsuario = 0;

            if (tipo != "Cliente" && tipo != "Admin")
            {
                MostrarMensagem("Selecione um tipo de conta válido.", false);
                return;
            }
            if (editando && !Int32.TryParse(hfIdUsuario.Value, out idUsuario))
            {
                MostrarMensagem("A conta selecionada para edição é inválida.", false);
                ResetarFormulario();
                return;
            }
            if (nome.Length == 0 || nome.Length > 100)
            {
                MostrarMensagem("Informe um nome com até 100 caracteres.", false);
                return;
            }
            if (email.Length == 0 || email.Length > 100 || !EmailValido(email))
            {
                MostrarMensagem("Informe um e-mail válido com até 100 caracteres.", false);
                return;
            }
            if (telefone.Length > 20 || (tipo == "Cliente" && endereco.Length > 200))
            {
                MostrarMensagem("Confira o telefone (máximo 20 caracteres) e o endereço (máximo 200 caracteres).", false);
                return;
            }
            if ((!editando || senha.Length > 0) && (senha.Length < 8 || senha.Length > 128))
            {
                MostrarMensagem("A senha deve ter entre 8 e 128 caracteres.", false);
                return;
            }

            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarMensagem("A conexão MercadoOnline não está configurada.", false);
                return;
            }

            string senhaHash = senha.Length == 0 ? null : CriarHashSenha(senha);
            try
            {
                using (SqlConnection conexao = conexaoBanco)
                using (SqlCommand comando = new SqlCommand(CriarSqlSalvar(tipo, editando, senhaHash != null), conexao))
                {
                    comando.Parameters.Add("@nome", SqlDbType.VarChar, 100).Value = nome;
                    comando.Parameters.Add("@email", SqlDbType.VarChar, 100).Value = email;
                    comando.Parameters.Add("@telefone", SqlDbType.VarChar, 20).Value = String.IsNullOrEmpty(telefone) ? (object)DBNull.Value : telefone;
                    if (tipo == "Cliente")
                        comando.Parameters.Add("@endereco", SqlDbType.VarChar, 200).Value = String.IsNullOrEmpty(endereco) ? (object)DBNull.Value : endereco;
                    if (senhaHash != null)
                        comando.Parameters.Add("@senhaHash", SqlDbType.VarChar, 255).Value = senhaHash;
                    if (editando)
                        comando.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario;

                    conexao.Open();
                    int linhasAfetadas = comando.ExecuteNonQuery();
                    if (linhasAfetadas == 0)
                    {
                        MostrarMensagem("A conta não foi encontrada para atualização.", false);
                        return;
                    }
                }

                ResetarFormulario();
                int adminAtual;
                if (tipo == "Admin" && Int32.TryParse(Convert.ToString(Session["UsuarioId"]), out adminAtual) && adminAtual == idUsuario)
                    Session["UsuarioNome"] = nome;
                MostrarMensagem(editando ? "Usuário atualizado com sucesso." : "Usuário adicionado com sucesso.", true);
                CarregarUsuarios();
            }
            catch (SqlException erro)
            {
                if (erro.Number == 2601 || erro.Number == 2627)
                    MostrarMensagem("Já existe uma conta deste tipo com esse e-mail.", false);
                else
                    MostrarMensagem("Não foi possível salvar o usuário no banco MercadoOnline.", false);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            if (!GarantirAdministrador()) return;
            ResetarFormulario();
        }

        protected void rptUsuarios_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!GarantirAdministrador()) return;

            string[] argumento = Convert.ToString(e.CommandArgument).Split('|');
            int idUsuario;
            if (argumento.Length != 2 || (argumento[0] != "Cliente" && argumento[0] != "Admin") || !Int32.TryParse(argumento[1], out idUsuario))
                return;

            if (e.CommandName == "EditarUsuario")
                PrepararEdicao(argumento[0], idUsuario);
            else if (e.CommandName == "ExcluirUsuario")
                ExcluirUsuario(argumento[0], idUsuario);
        }

        private string CriarSqlSalvar(string tipo, bool editando, bool alterarSenha)
        {
            string tabela = tipo == "Admin" ? "Admin" : "Cliente";
            string colunaId = tipo == "Admin" ? "idAdmin" : "idCliente";
            string enderecoInsert = tipo == "Cliente" ? ", endereco" : String.Empty;
            string enderecoValue = tipo == "Cliente" ? ", @endereco" : String.Empty;

            if (!editando)
            {
                string colunas = "nome, email, senhaHash, telefone" + enderecoInsert;
                string valores = "@nome, @email, @senhaHash, @telefone" + enderecoValue;
                return "INSERT INTO dbo." + tabela + " (" + colunas + ") VALUES (" + valores + ")";
            }

            string atualizarSenha = alterarSenha ? ", senhaHash = @senhaHash" : String.Empty;
            string atualizarEndereco = tipo == "Cliente" ? ", endereco = @endereco" : String.Empty;
            return "UPDATE dbo." + tabela + " SET nome = @nome, email = @email, telefone = @telefone" + atualizarEndereco + atualizarSenha + " WHERE " + colunaId + " = @idUsuario";
        }

        private void CarregarUsuarios()
        {
            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarMensagem("A conexão MercadoOnline não está configurada.", false);
                return;
            }

            const string sql = @"
                SELECT idCliente AS idUsuario, nome, email, telefone, 'Cliente' AS tipo,
                       'Cliente' AS tipoExibicao,
                       COALESCE(NULLIF(telefone, ''), 'Sem telefone') AS telefoneExibicao
                FROM dbo.Cliente
                UNION ALL
                SELECT idAdmin AS idUsuario, nome, email, telefone, 'Admin' AS tipo,
                       'Administrador' AS tipoExibicao,
                       COALESCE(NULLIF(telefone, ''), 'Sem telefone') AS telefoneExibicao
                FROM dbo.Admin
                ORDER BY tipoExibicao, nome";

            try
            {
                using (SqlConnection conexao = conexaoBanco)
                using (SqlDataAdapter adaptador = new SqlDataAdapter(sql, conexao))
                {
                    DataTable usuarios = new DataTable();
                    adaptador.Fill(usuarios);
                    rptUsuarios.DataSource = usuarios;
                    rptUsuarios.DataBind();
                    lblContagem.Text = String.Format("{0} conta(s)", usuarios.Rows.Count);
                    pnlSemUsuarios.Visible = usuarios.Rows.Count == 0;
                }
            }
            catch (SqlException)
            {
                rptUsuarios.DataSource = null;
                rptUsuarios.DataBind();
                pnlSemUsuarios.Visible = false;
                MostrarMensagem("Não foi possível carregar os usuários do banco MercadoOnline.", false);
            }
        }

        private void PrepararEdicao(string tipo, int idUsuario)
        {
            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco)) return;

            string tabela = tipo == "Admin" ? "Admin" : "Cliente";
            string colunaId = tipo == "Admin" ? "idAdmin" : "idCliente";
            string colunaEndereco = tipo == "Cliente" ? ", endereco" : String.Empty;
            string sql = "SELECT nome, email, telefone" + colunaEndereco + " FROM dbo." + tabela + " WHERE " + colunaId + " = @idUsuario";

            try
            {
                using (SqlConnection conexao = conexaoBanco)
                using (SqlCommand comando = new SqlCommand(sql, conexao))
                {
                    comando.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario;
                    conexao.Open();
                    using (SqlDataReader leitor = comando.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!leitor.Read())
                        {
                            MostrarMensagem("A conta selecionada não foi encontrada.", false);
                            CarregarUsuarios();
                            return;
                        }

                        hfIdUsuario.Value = idUsuario.ToString();
                        hfTipoOriginal.Value = tipo;
                        ddlTipo.SelectedValue = tipo;
                        ddlTipo.Enabled = false;
                        pnlEndereco.Visible = tipo == "Cliente";
                        txtNome.Text = leitor["nome"].ToString();
                        txtEmail.Text = leitor["email"].ToString();
                        txtTelefone.Text = leitor["telefone"] == DBNull.Value ? String.Empty : leitor["telefone"].ToString();
                        txtEndereco.Text = tipo == "Cliente" && leitor["endereco"] != DBNull.Value ? leitor["endereco"].ToString() : String.Empty;
                    }
                }

                txtSenha.Text = String.Empty;
                lblModo.Text = "Editar usuário";
                lblSenha.Text = "Nova senha (opcional)";
                lblAjudaSenha.Visible = true;
                btnSalvar.Text = "Salvar alterações";
                btnCancelar.Visible = true;
                MostrarMensagem("Altere os campos desejados. Deixe a senha vazia para mantê-la.", true);
            }
            catch (SqlException)
            {
                MostrarMensagem("Não foi possível abrir a conta para edição.", false);
            }
        }

        private void ExcluirUsuario(string tipo, int idUsuario)
        {
            int adminAtual;
            if (tipo == "Admin" && Int32.TryParse(Convert.ToString(Session["UsuarioId"]), out adminAtual) && idUsuario == adminAtual)
            {
                MostrarMensagem("Use o perfil administrativo para encerrar ou excluir a própria conta.", false);
                return;
            }

            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
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
                        int linhasAfetadas;
                        if (tipo == "Admin")
                        {
                            using (SqlCommand contar = new SqlCommand("SELECT COUNT(*) FROM dbo.Admin WITH (UPDLOCK, HOLDLOCK)", conexao, transacao))
                            {
                                if (Convert.ToInt32(contar.ExecuteScalar()) <= 1)
                                {
                                    transacao.Rollback();
                                    MostrarMensagem("Não é possível excluir o último administrador.", false);
                                    return;
                                }
                            }

                            using (SqlCommand comando = new SqlCommand("DELETE FROM dbo.Admin WHERE idAdmin = @id", conexao, transacao))
                            {
                                comando.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario;
                                linhasAfetadas = comando.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            using (SqlCommand comando = new SqlCommand("DELETE FROM dbo.Produto_Pedido WHERE idPedido IN (SELECT idPedido FROM dbo.Pedido WHERE idCliente = @id); DELETE FROM dbo.Pedido WHERE idCliente = @id; DELETE FROM dbo.Cliente WHERE idCliente = @id;", conexao, transacao))
                            {
                                comando.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario;
                                linhasAfetadas = comando.ExecuteNonQuery();
                            }
                        }

                        transacao.Commit();
                        MostrarMensagem(linhasAfetadas > 0 ? "Conta removida com sucesso." : "A conta já não existe.", linhasAfetadas > 0);
                    }
                }

                if (hfIdUsuario.Value == idUsuario.ToString() && hfTipoOriginal.Value == tipo)
                    ResetarFormulario();
                CarregarUsuarios();
            }
            catch (SqlException)
            {
                MostrarMensagem("Não foi possível remover a conta no banco MercadoOnline.", false);
            }
        }

        private void ResetarFormulario()
        {
            hfIdUsuario.Value = String.Empty;
            hfTipoOriginal.Value = String.Empty;
            ddlTipo.Enabled = true;
            ddlTipo.SelectedValue = "Cliente";
            pnlEndereco.Visible = true;
            txtNome.Text = String.Empty;
            txtEmail.Text = String.Empty;
            txtTelefone.Text = String.Empty;
            txtEndereco.Text = String.Empty;
            txtSenha.Text = String.Empty;
            lblModo.Text = "Adicionar usuário";
            lblSenha.Text = "Senha";
            lblAjudaSenha.Visible = false;
            btnSalvar.Text = "Adicionar usuário";
            btnCancelar.Visible = false;
        }

        private bool GarantirAdministrador()
        {
            if (String.Equals(Convert.ToString(Session["UsuarioTipo"]), "Admin", StringComparison.OrdinalIgnoreCase))
                return true;
            Response.Redirect("login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return false;
        }

        private static bool EmailValido(string email)
        {
            try { return String.Equals(new MailAddress(email).Address, email, StringComparison.OrdinalIgnoreCase); }
            catch (FormatException) { return false; }
        }

        private static string CriarHashSenha(string senha)
        {
            byte[] sal = new byte[16];
            using (RandomNumberGenerator gerador = RandomNumberGenerator.Create())
                gerador.GetBytes(sal);

            byte[] hash;
            using (Rfc2898DeriveBytes derivador = new Rfc2898DeriveBytes(senha, sal, IteracoesPbkdf2))
                hash = derivador.GetBytes(32);

            return "PBKDF2$" + IteracoesPbkdf2 + "$" + ConverterHex(sal) + "$" + ConverterHex(hash);
        }

        private static string ConverterHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", String.Empty).ToLowerInvariant();
        }

        private void MostrarMensagem(string mensagem, bool sucesso)
        {
            lblMensagem.Text = Server.HtmlEncode(mensagem);
            lblMensagem.CssClass = sucesso ? "mensagem sucesso" : "mensagem erro";
            lblMensagem.Visible = true;
        }
    }
}
