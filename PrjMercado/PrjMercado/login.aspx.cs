using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web.UI;

namespace PrjMercado
{
    public partial class Login : Page
    {
        protected void btnEntrar_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string senha = txtSenha.Text;
            string tabela = ddlTipoConta.SelectedValue == "Admin" ? "Admin" : "Cliente";
            string idColuna = tabela == "Admin" ? "idAdmin" : "idCliente";

            if (email.Length == 0 || senha.Length == 0)
            {
                MostrarMensagem("Informe seu e-mail e sua senha.");
                return;
            }

            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarMensagem("A conexão com o banco não está configurada. Confira a connection string MercadoOnline no Web.config.");
                return;
            }

            try
            {
                string sql = "SELECT " + idColuna + ", nome, senhaHash FROM " + tabela + " WHERE email = @email";
                object id = null;
                string nome = null;
                string senhaHash = null;

                using (SqlConnection conexao = conexaoBanco)
                using (SqlCommand comando = new SqlCommand(sql, conexao))
                {
                    comando.Parameters.Add("@email", SqlDbType.VarChar, 100).Value = email;
                    conexao.Open();
                    using (SqlDataReader leitor = comando.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (leitor.Read())
                        {
                            id = leitor[idColuna];
                            nome = leitor["nome"].ToString();
                            senhaHash = leitor["senhaHash"].ToString();
                        }
                    }
                }

                if (id == null || !VerificarSenha(senha, senhaHash))
                {
                    MostrarMensagem("E-mail, senha ou tipo de conta inválido.");
                    return;
                }

                Session["UsuarioId"] = id;
                Session["UsuarioNome"] = nome;
                Session["UsuarioTipo"] = tabela;
                Response.Redirect(ObterDestinoAposLogin(tabela), false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (SqlException)
            {
                MostrarMensagem("Não foi possível acessar o banco de dados. Confira a conexão e tente novamente.");
            }
        }

        private void MostrarMensagem(string mensagem)
        {
            lblMensagem.Text = Server.HtmlEncode(mensagem);
            lblMensagem.Visible = true;
        }

        private string ObterDestinoAposLogin(string tipoConta)
        {
            string destino = Request.QueryString["returnUrl"];
            if (String.Equals(tipoConta, "Cliente", StringComparison.OrdinalIgnoreCase))
            {
                if (String.Equals(destino, "realizarPedido.aspx", StringComparison.OrdinalIgnoreCase))
                    return "realizarPedido.aspx";
                if (String.Equals(destino, "meusPedidos.aspx", StringComparison.OrdinalIgnoreCase))
                    return "meusPedidos.aspx";
            }
            return "index.aspx";
        }

        // Formato armazenado em senhaHash: PBKDF2$iterações$salHex$hashHex.
        private static bool VerificarSenha(string senha, string valorArmazenado)
        {
            if (String.IsNullOrWhiteSpace(valorArmazenado)) return false;
            string[] partes = valorArmazenado.Split('$');
            int iteracoes;
            if (partes.Length != 4 || partes[0] != "PBKDF2" ||
                !Int32.TryParse(partes[1], out iteracoes) || iteracoes < 10000 || iteracoes > 2000000)
                return false;

            byte[] sal;
            byte[] hashEsperado;
            if (!TentarDecodificarHex(partes[2], out sal) || !TentarDecodificarHex(partes[3], out hashEsperado) ||
                sal.Length < 16 || hashEsperado.Length < 32)
                return false;

            using (Rfc2898DeriveBytes derivador = new Rfc2898DeriveBytes(senha, sal, iteracoes))
            {
                byte[] hashCalculado = derivador.GetBytes(hashEsperado.Length);
                return ComparacaoConstante(hashCalculado, hashEsperado);
            }
        }

        private static bool TentarDecodificarHex(string texto, out byte[] bytes)
        {
            bytes = new byte[0];
            if (String.IsNullOrEmpty(texto) || texto.Length % 2 != 0) return false;
            try
            {
                bytes = new byte[texto.Length / 2];
                for (int i = 0; i < bytes.Length; i++)
                    bytes[i] = Convert.ToByte(texto.Substring(i * 2, 2), 16);
                return true;
            }
            catch (FormatException) { return false; }
        }

        private static bool ComparacaoConstante(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            int diferenca = 0;
            for (int i = 0; i < a.Length; i++) diferenca |= a[i] ^ b[i];
            return diferenca == 0;
        }
    }
}
