using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace PrjMercado
{
    public partial class meusPedidos : Page
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

            if (!String.Equals(Convert.ToString(Session["UsuarioTipo"]), "Cliente", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("login.aspx?returnUrl=meusPedidos.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
                CarregarPedidos();
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

        protected string ClasseStatus(object valor)
        {
            string status = Convert.ToString(valor);
            if (status == "Pendente") return "pendente";
            if (status == "Concluido") return "concluido";
            if (status == "Cancelado") return "cancelado";
            return "andamento";
        }

        private void CarregarPedidos()
        {
            int idCliente;
            if (!Int32.TryParse(Convert.ToString(Session["UsuarioId"]), out idCliente) || idCliente < 1)
            {
                Session.Clear();
                Response.Redirect("login.aspx?returnUrl=meusPedidos.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarMensagem("A conexão MercadoOnline não está configurada.");
                return;
            }

            const string sql = @"
                SELECT P.idPedido, P.dataPedido, P.valorTotal, P.status, P.formaPagamento,
                       COALESCE(STRING_AGG(
                           CAST(CONCAT(PR.descricao, N' - ', PP.quantidade, N' un.') AS NVARCHAR(MAX)), N', '), N'') AS itens
                FROM dbo.Pedido P
                LEFT JOIN dbo.Produto_Pedido PP ON PP.idPedido = P.idPedido
                LEFT JOIN dbo.Produto PR ON PR.idProduto = PP.idProduto
                WHERE P.idCliente = @idCliente
                GROUP BY P.idPedido, P.dataPedido, P.valorTotal, P.status, P.formaPagamento
                ORDER BY P.dataPedido DESC, P.idPedido DESC";

            try
            {
                using (SqlConnection conexao = conexaoBanco)
                using (SqlCommand comando = new SqlCommand(sql, conexao))
                using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                {
                    comando.Parameters.Add("@idCliente", SqlDbType.Int).Value = idCliente;
                    DataTable pedidos = new DataTable();
                    adaptador.Fill(pedidos);
                    rptPedidos.DataSource = pedidos;
                    rptPedidos.DataBind();
                    rptPedidos.Visible = pedidos.Rows.Count > 0;
                    pnlSemPedidos.Visible = pedidos.Rows.Count == 0;
                }
            }
            catch (SqlException)
            {
                rptPedidos.Visible = false;
                pnlSemPedidos.Visible = false;
                MostrarMensagem("Não foi possível carregar seus pedidos no banco MercadoOnline.");
            }
        }

        private void MostrarMensagem(string mensagem)
        {
            lblMensagem.Text = HttpUtility.HtmlEncode(mensagem);
            lblMensagem.Visible = true;
        }
    }
}
