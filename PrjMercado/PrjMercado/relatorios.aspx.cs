using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;

namespace PrjMercado
{
    public partial class relatorios : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!GarantirAdministrador()) return;

            lblMensagem.Visible = false;
            if (!IsPostBack)
                CarregarRelatorios(null, null);
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (!GarantirAdministrador()) return;

            DateTime? inicio;
            DateTime? fim;
            if (!LerPeriodo(out inicio, out fim)) return;
            CarregarRelatorios(inicio, fim);
        }

        protected void btnLimpar_Click(object sender, EventArgs e)
        {
            if (!GarantirAdministrador()) return;
            txtDataInicio.Text = String.Empty;
            txtDataFim.Text = String.Empty;
            CarregarRelatorios(null, null);
        }

        protected void rptPedidos_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (!GarantirAdministrador()) return;

            string novoStatus;
            if (e.CommandName == "Aprovar")
                novoStatus = "Concluido";
            else if (e.CommandName == "Cancelar")
                novoStatus = "Cancelado";
            else
                return;

            int idPedido;
            if (!Int32.TryParse(Convert.ToString(e.CommandArgument), out idPedido) || idPedido < 1)
            {
                MostrarMensagem("Não foi possível identificar o pedido selecionado.");
                return;
            }

            DateTime? inicio;
            DateTime? fim;
            if (!LerPeriodo(out inicio, out fim)) return;

            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarMensagem("A conexão MercadoOnline não está configurada.");
                return;
            }

            try
            {
                int atualizados;
                using (SqlConnection conexao = conexaoBanco)
                {
                    conexao.Open();
                    using (SqlTransaction transacao = conexao.BeginTransaction(IsolationLevel.Serializable))
                    {
                        using (SqlCommand comando = new SqlCommand(
                            "UPDATE dbo.Pedido SET status = @status WHERE idPedido = @idPedido AND status = 'Pendente'", conexao, transacao))
                        {
                            comando.Parameters.Add("@status", SqlDbType.VarChar, 30).Value = novoStatus;
                            comando.Parameters.Add("@idPedido", SqlDbType.Int).Value = idPedido;
                            atualizados = comando.ExecuteNonQuery();
                        }

                        if (atualizados == 1 && novoStatus == "Cancelado")
                        {
                            using (SqlCommand reporEstoque = new SqlCommand(@"
                                UPDATE P
                                SET P.estoque = P.estoque + PP.quantidade
                                FROM dbo.Produto P
                                INNER JOIN dbo.Produto_Pedido PP ON PP.idProduto = P.idProduto
                                WHERE PP.idPedido = @idPedido", conexao, transacao))
                            {
                                reporEstoque.Parameters.Add("@idPedido", SqlDbType.Int).Value = idPedido;
                                reporEstoque.ExecuteNonQuery();
                            }
                        }

                        transacao.Commit();
                    }
                }

                if (atualizados == 1)
                    MostrarMensagem(novoStatus == "Concluido" ? "Pedido aprovado e marcado como concluído." : "Pedido cancelado.");
                else
                    MostrarMensagem("Este pedido não está mais pendente. Atualizei a lista com o status atual.");

                CarregarRelatorios(inicio, fim);
            }
            catch (SqlException)
            {
                MostrarMensagem("Não foi possível atualizar o pedido no banco MercadoOnline.");
            }
        }

        private bool LerPeriodo(out DateTime? inicio, out DateTime? fim)
        {
            inicio = null;
            fim = null;
            DateTime data;

            if (!String.IsNullOrWhiteSpace(txtDataInicio.Text))
            {
                if (!DateTime.TryParseExact(txtDataInicio.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out data))
                {
                    MostrarMensagem("Informe uma data inicial válida.");
                    return false;
                }
                inicio = data.Date;
            }

            if (!String.IsNullOrWhiteSpace(txtDataFim.Text))
            {
                if (!DateTime.TryParseExact(txtDataFim.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out data))
                {
                    MostrarMensagem("Informe uma data final válida.");
                    return false;
                }
                fim = data.Date;
            }

            if (inicio.HasValue && fim.HasValue && fim.Value < inicio.Value)
            {
                MostrarMensagem("A data final deve ser igual ou posterior à data inicial.");
                return false;
            }
            return true;
        }

        private void CarregarRelatorios(DateTime? inicio, DateTime? fim)
        {
            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarMensagem("A conexão MercadoOnline não está configurada.");
                return;
            }

            const string sqlResumo = @"
                SELECT COUNT(*) AS totalPedidos,
                       COALESCE(SUM(CASE WHEN status = 'Concluido' THEN valorTotal ELSE 0 END), 0) AS arrecadado,
                       COALESCE(SUM(CASE WHEN status NOT IN ('Concluido', 'Cancelado') THEN 1 ELSE 0 END), 0) AS emAndamento,
                       COALESCE(AVG(CASE WHEN status = 'Concluido' THEN CAST(valorTotal AS DECIMAL(10,2)) END), 0) AS ticketMedio
                FROM dbo.Pedido
                WHERE (@inicio IS NULL OR dataPedido >= @inicio)
                  AND (@fimExclusivo IS NULL OR dataPedido < @fimExclusivo)";

            const string sqlStatus = @"
                SELECT status, COUNT(*) AS quantidade, COALESCE(SUM(valorTotal), 0) AS valor
                FROM dbo.Pedido
                WHERE (@inicio IS NULL OR dataPedido >= @inicio)
                  AND (@fimExclusivo IS NULL OR dataPedido < @fimExclusivo)
                GROUP BY status
                ORDER BY CASE status
                    WHEN 'Pendente' THEN 1 WHEN 'Confirmado' THEN 2 WHEN 'Em preparo' THEN 3
                    WHEN 'Enviado' THEN 4 WHEN 'Concluido' THEN 5 WHEN 'Cancelado' THEN 6 ELSE 7 END";

            const string sqlPedidos = @"
                SELECT P.idPedido, P.dataPedido, C.nome AS cliente, P.formaPagamento,
                       P.status, P.valorTotal, COALESCE(Itens.quantidadeItens, 0) AS quantidadeItens
                FROM dbo.Pedido P
                INNER JOIN dbo.Cliente C ON C.idCliente = P.idCliente
                LEFT JOIN (
                    SELECT idPedido, SUM(quantidade) AS quantidadeItens
                    FROM dbo.Produto_Pedido
                    GROUP BY idPedido
                ) Itens ON Itens.idPedido = P.idPedido
                WHERE (@inicio IS NULL OR P.dataPedido >= @inicio)
                  AND (@fimExclusivo IS NULL OR P.dataPedido < @fimExclusivo)
                ORDER BY P.dataPedido DESC, P.idPedido DESC";

            DateTime? fimExclusivo = fim.HasValue ? fim.Value.Date.AddDays(1) : (DateTime?)null;
            try
            {
                using (SqlConnection conexao = conexaoBanco)
                {
                    conexao.Open();
                    using (SqlCommand comandoResumo = new SqlCommand(sqlResumo, conexao))
                    {
                        AdicionarParametrosPeriodo(comandoResumo, inicio, fimExclusivo);
                        using (SqlDataReader leitor = comandoResumo.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (leitor.Read())
                            {
                                lblTotalPedidos.Text = Convert.ToInt32(leitor["totalPedidos"]).ToString("N0", CultureInfo.GetCultureInfo("pt-BR"));
                                lblArrecadado.Text = Convert.ToDecimal(leitor["arrecadado"]).ToString("C", CultureInfo.GetCultureInfo("pt-BR"));
                                lblEmAndamento.Text = Convert.ToInt32(leitor["emAndamento"]).ToString("N0", CultureInfo.GetCultureInfo("pt-BR"));
                                lblTicketMedio.Text = Convert.ToDecimal(leitor["ticketMedio"]).ToString("C", CultureInfo.GetCultureInfo("pt-BR"));
                            }
                        }
                    }

                    using (SqlCommand comandoStatus = new SqlCommand(sqlStatus, conexao))
                    using (SqlDataAdapter adaptadorStatus = new SqlDataAdapter(comandoStatus))
                    {
                        AdicionarParametrosPeriodo(comandoStatus, inicio, fimExclusivo);
                        DataTable status = new DataTable();
                        adaptadorStatus.Fill(status);
                        rptStatus.DataSource = status;
                        rptStatus.DataBind();
                        rptStatus.Visible = status.Rows.Count > 0;
                        pnlSemStatus.Visible = status.Rows.Count == 0;
                    }

                    using (SqlCommand comandoPedidos = new SqlCommand(sqlPedidos, conexao))
                    using (SqlDataAdapter adaptadorPedidos = new SqlDataAdapter(comandoPedidos))
                    {
                        AdicionarParametrosPeriodo(comandoPedidos, inicio, fimExclusivo);
                        DataTable pedidos = new DataTable();
                        adaptadorPedidos.Fill(pedidos);
                        rptPedidos.DataSource = pedidos;
                        rptPedidos.DataBind();
                        rptPedidos.Visible = pedidos.Rows.Count > 0;
                        lblContagemPedidos.Text = String.Format("{0} pedido(s)", pedidos.Rows.Count);
                        pnlSemPedidos.Visible = pedidos.Rows.Count == 0;
                    }
                }
            }
            catch (SqlException)
            {
                lblTotalPedidos.Text = "0";
                lblArrecadado.Text = "R$ 0,00";
                lblEmAndamento.Text = "0";
                lblTicketMedio.Text = "R$ 0,00";
                rptStatus.DataSource = null;
                rptStatus.DataBind();
                rptStatus.Visible = false;
                rptPedidos.DataSource = null;
                rptPedidos.DataBind();
                rptPedidos.Visible = false;
                pnlSemStatus.Visible = false;
                pnlSemPedidos.Visible = false;
                MostrarMensagem("Não foi possível carregar os relatórios do banco MercadoOnline.");
            }
        }

        private static void AdicionarParametrosPeriodo(SqlCommand comando, DateTime? inicio, DateTime? fimExclusivo)
        {
            comando.Parameters.Add("@inicio", SqlDbType.DateTime2).Value = inicio.HasValue ? (object)inicio.Value : DBNull.Value;
            comando.Parameters.Add("@fimExclusivo", SqlDbType.DateTime2).Value = fimExclusivo.HasValue ? (object)fimExclusivo.Value : DBNull.Value;
        }

        private bool GarantirAdministrador()
        {
            if (String.Equals(Convert.ToString(Session["UsuarioTipo"]), "Admin", StringComparison.OrdinalIgnoreCase))
                return true;
            Response.Redirect("login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return false;
        }

        private void MostrarMensagem(string mensagem)
        {
            lblMensagem.Text = Server.HtmlEncode(mensagem);
            lblMensagem.Visible = true;
        }
    }
}
