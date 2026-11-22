using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web.UI;

namespace PrjMercado
{
    public partial class Cadastro : Page
    {
        private const int IteracoesPbkdf2 = 120000;

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid) return;

            string nome = txtNome.Text.Trim();
            string email = txtEmail.Text.Trim();
            string telefone = txtTelefone.Text.Trim();
            string endereco = txtEndereco.Text.Trim();
            string senha = txtSenha.Text;

            if (nome.Length == 0 || nome.Length > 100 || email.Length > 100 ||
                telefone.Length > 20 || endereco.Length > 200 || senha.Length > 128)
            {
                MostrarMensagem("Confira os dados informados e tente novamente.");
                return;
            }

            if (!EmailValido(email))
            {
                MostrarMensagem("Informe um endereço de e-mail válido.");
                return;
            }

            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarMensagem("A conexão com o banco não está configurada. Confira a connection string MercadoOnline no Web.config.");
                return;
            }

            string senhaHash = CriarHashSenha(senha);
            try
            {
                const string sql = @"INSERT INTO Cliente (nome, email, senhaHash, telefone, endereco)
                                     VALUES (@nome, @email, @senhaHash, @telefone, @endereco);
                                     SELECT CAST(SCOPE_IDENTITY() AS INT);";
                int idCliente;
                using (SqlConnection conexao = conexaoBanco)
                using (SqlCommand comando = new SqlCommand(sql, conexao))
                {
                    comando.Parameters.Add("@nome", SqlDbType.VarChar, 100).Value = nome;
                    comando.Parameters.Add("@email", SqlDbType.VarChar, 100).Value = email;
                    comando.Parameters.Add("@senhaHash", SqlDbType.VarChar, 255).Value = senhaHash;
                    comando.Parameters.Add("@telefone", SqlDbType.VarChar, 20).Value =
                        String.IsNullOrWhiteSpace(telefone) ? (object)DBNull.Value : telefone;
                    comando.Parameters.Add("@endereco", SqlDbType.VarChar, 200).Value =
                        String.IsNullOrWhiteSpace(endereco) ? (object)DBNull.Value : endereco;

                    conexao.Open();
                    idCliente = Convert.ToInt32(comando.ExecuteScalar());
                }

                Session["UsuarioId"] = idCliente;
                Session["UsuarioNome"] = nome;
                Session["UsuarioTipo"] = "Cliente";
                Response.Redirect("index.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                    MostrarMensagem("Já existe uma conta cadastrada com esse e-mail.");
                else
                    MostrarMensagem("Não foi possível salvar o cadastro. Confira a conexão com o banco e tente novamente.");
            }
        }

        private static bool EmailValido(string email)
        {
            try
            {
                MailAddress endereco = new MailAddress(email);
                return String.Equals(endereco.Address, email, StringComparison.OrdinalIgnoreCase);
            }
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

        private void MostrarMensagem(string mensagem)
        {
            lblMensagem.Text = Server.HtmlEncode(mensagem);
            lblMensagem.Visible = true;
        }
    }
}
