using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace PrjMercado
{
    public partial class perfilAdmin : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int idAdmin;
            if (!String.Equals(Convert.ToString(Session["UsuarioTipo"]), "Admin", StringComparison.OrdinalIgnoreCase) ||
                !Int32.TryParse(Convert.ToString(Session["UsuarioId"]), out idAdmin))
            {
                RedirecionarParaLogin();
                return;
            }

            if (!IsPostBack)
                CarregarPerfil(idAdmin);
        }

        protected void btnSair_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("index.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void CarregarPerfil(int idAdmin)
        {
            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarMensagem("A conexão MercadoOnline não está configurada.");
                return;
            }

            try
            {
                using (SqlConnection conexao = conexaoBanco)
                using (SqlCommand comando = new SqlCommand("SELECT nome, email, telefone FROM Admin WHERE idAdmin = @idAdmin", conexao))
                {
                    comando.Parameters.Add("@idAdmin", SqlDbType.Int).Value = idAdmin;
                    conexao.Open();
                    using (SqlDataReader leitor = comando.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!leitor.Read())
                        {
                            Session.Clear();
                            RedirecionarParaLogin();
                            return;
                        }

                        lblNome.Text = HttpUtility.HtmlEncode(leitor["nome"].ToString());
                        lblEmail.Text = HttpUtility.HtmlEncode(leitor["email"].ToString());
                        string telefone = leitor["telefone"] == DBNull.Value ? String.Empty : leitor["telefone"].ToString();
                        lblTelefone.Text = HttpUtility.HtmlEncode(String.IsNullOrWhiteSpace(telefone) ? "Não informado" : telefone);
                        pnlDadosAdmin.Visible = true;
                    }
                }
            }
            catch (SqlException)
            {
                MostrarMensagem("Não foi possível carregar o perfil no banco MercadoOnline.");
            }
        }

        private void MostrarMensagem(string mensagem)
        {
            lblMensagem.Text = HttpUtility.HtmlEncode(mensagem);
            lblMensagem.Visible = true;
        }

        private void RedirecionarParaLogin()
        {
            Response.Redirect("login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
