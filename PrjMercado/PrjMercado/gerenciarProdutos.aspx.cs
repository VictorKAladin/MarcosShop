using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PrjMercado
{
    public partial class gerenciarProdutos : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!GarantirAdministrador())
            {
                return;
            }

            lblMensagem.Visible = false;
            if (!IsPostBack)
            {
                CarregarCategorias();
                CarregarProdutos();
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!GarantirAdministrador()) return;

            string descricao = (txtDescricao.Text ?? String.Empty).Trim();
            decimal preco;
            int estoque;
            int idCategoria;
            byte[] imagem = null;
            string tipoImagem = null;
            if (fuImagem.HasFile)
            {
                tipoImagem = ObterTipoImagem(fuImagem.FileName);
                if (tipoImagem == null || fuImagem.PostedFile.ContentLength > 3 * 1024 * 1024)
                {
                    MostrarMensagem("Selecione uma imagem PNG, JPG, WEBP, GIF ou SVG com até 3 MB.", false);
                    return;
                }
                imagem = fuImagem.FileBytes;
            }

            if (descricao.Length == 0 || descricao.Length > 100)
            {
                MostrarMensagem("Informe uma descrição com até 100 caracteres.", false);
                return;
            }
            if (!TentarLerPreco(txtPreco.Text, out preco) || preco < 0m || preco > 99999999.99m)
            {
                MostrarMensagem("Informe um preço válido entre R$ 0,00 e R$ 99.999.999,99.", false);
                return;
            }
            if (!Int32.TryParse(txtEstoque.Text, out estoque) || estoque < 0)
            {
                MostrarMensagem("Informe um estoque inteiro igual ou maior que zero.", false);
                return;
            }
            if (!Int32.TryParse(ddlCategoria.SelectedValue, out idCategoria))
            {
                MostrarMensagem("Selecione uma categoria válida.", false);
                return;
            }

            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarMensagem("A conexão MercadoOnline não está configurada.", false);
                return;
            }

            bool editando = !String.IsNullOrWhiteSpace(hfIdProduto.Value);
            int idProdutoEditado = 0;
            if (editando && !Int32.TryParse(hfIdProduto.Value, out idProdutoEditado))
            {
                MostrarMensagem("O produto selecionado para edição é inválido.", false);
                ResetarFormulario();
                return;
            }
            string sql = editando
                ? "UPDATE Produto SET descricao = @descricao, preco = @preco, estoque = @estoque, idCategoria = @idCategoria, imagem = CASE WHEN @temImagem = 1 THEN @imagem ELSE imagem END, tipoImagem = CASE WHEN @temImagem = 1 THEN @tipoImagem ELSE tipoImagem END WHERE idProduto = @idProduto"
                : "INSERT INTO Produto (descricao, preco, estoque, idCategoria, imagem, tipoImagem) VALUES (@descricao, @preco, @estoque, @idCategoria, @imagem, @tipoImagem)";

            try
            {
                using (SqlConnection conexao = conexaoBanco)
                using (SqlCommand comando = new SqlCommand(sql, conexao))
                {
                    comando.Parameters.Add("@descricao", SqlDbType.VarChar, 100).Value = descricao;
                    SqlParameter parametroPreco = comando.Parameters.Add("@preco", SqlDbType.Decimal);
                    parametroPreco.Precision = 10;
                    parametroPreco.Scale = 2;
                    parametroPreco.Value = preco;
                    comando.Parameters.Add("@estoque", SqlDbType.Int).Value = estoque;
                    comando.Parameters.Add("@idCategoria", SqlDbType.Int).Value = idCategoria;
                    comando.Parameters.Add("@temImagem", SqlDbType.Bit).Value = imagem != null;
                    comando.Parameters.Add("@imagem", SqlDbType.VarBinary, -1).Value = (object)imagem ?? DBNull.Value;
                    comando.Parameters.Add("@tipoImagem", SqlDbType.VarChar, 100).Value = (object)tipoImagem ?? DBNull.Value;
                    if (editando)
                        comando.Parameters.Add("@idProduto", SqlDbType.Int).Value = idProdutoEditado;

                    conexao.Open();
                    int linhasAfetadas = comando.ExecuteNonQuery();
                    if (linhasAfetadas == 0)
                    {
                        MostrarMensagem("O produto não foi encontrado para atualização.", false);
                        ResetarFormulario();
                        CarregarProdutos();
                        return;
                    }
                }

                ResetarFormulario();
                MostrarMensagem(editando ? "Produto atualizado com sucesso." : "Produto adicionado com sucesso.", true);
                CarregarProdutos();
            }
            catch (SqlException)
            {
                MostrarMensagem("Não foi possível salvar o produto. Confira a conexão e os dados do banco MercadoOnline.", false);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            if (!GarantirAdministrador()) return;
            ResetarFormulario();
        }

        protected void rptProdutos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!GarantirAdministrador()) return;

            int idProduto;
            if (!Int32.TryParse(Convert.ToString(e.CommandArgument), out idProduto)) return;

            if (e.CommandName == "EditarProduto")
                PrepararEdicao(idProduto);
            else if (e.CommandName == "ExcluirProduto")
                ExcluirProduto(idProduto);
        }

        private bool EhAdministrador()
        {
            return String.Equals(Convert.ToString(Session["UsuarioTipo"]), "Admin", StringComparison.OrdinalIgnoreCase);
        }

        private bool GarantirAdministrador()
        {
            if (EhAdministrador()) return true;
            Response.Redirect("login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return false;
        }

        private static bool TentarLerPreco(string texto, out decimal preco)
        {
            texto = (texto ?? String.Empty).Trim();
            if (texto.IndexOf(',') >= 0)
                return Decimal.TryParse(texto, NumberStyles.Number, CultureInfo.GetCultureInfo("pt-BR"), out preco);
            return Decimal.TryParse(texto, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out preco);
        }

        private static string ObterTipoImagem(string nomeArquivo)
        {
            switch (Path.GetExtension(nomeArquivo).ToLowerInvariant())
            {
                case ".png": return "image/png";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".webp": return "image/webp";
                case ".gif": return "image/gif";
                case ".svg": return "image/svg+xml";
                default: return null;
            }
        }

        private void CarregarCategorias()
        {
            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarMensagem("A conexão MercadoOnline não está configurada.", false);
                return;
            }

            try
            {
                using (SqlConnection conexao = conexaoBanco)
                using (SqlDataAdapter adaptador = new SqlDataAdapter("SELECT idCategoria, descricao FROM Categoria ORDER BY descricao", conexao))
                {
                    DataTable categorias = new DataTable();
                    adaptador.Fill(categorias);
                    ddlCategoria.DataSource = categorias;
                    ddlCategoria.DataTextField = "descricao";
                    ddlCategoria.DataValueField = "idCategoria";
                    ddlCategoria.DataBind();
                }
            }
            catch (SqlException)
            {
                MostrarMensagem("Não foi possível carregar as categorias do banco MercadoOnline.", false);
            }
        }

        private void CarregarProdutos()
        {
            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarMensagem("A conexão MercadoOnline não está configurada.", false);
                return;
            }

            const string sql = "SELECT P.idProduto, P.descricao, P.preco, P.estoque, C.descricao AS categoria, P.imagem, P.tipoImagem FROM Produto P INNER JOIN Categoria C ON P.idCategoria = C.idCategoria ORDER BY P.descricao";
            try
            {
                using (SqlConnection conexao = conexaoBanco)
                using (SqlDataAdapter adaptador = new SqlDataAdapter(sql, conexao))
                {
                    DataTable produtos = new DataTable();
                    adaptador.Fill(produtos);
                    produtos.Columns.Add("imagemUrl", typeof(string));
                    foreach (DataRow produto in produtos.Rows)
                        if (produto["imagem"] != DBNull.Value && produto["tipoImagem"] != DBNull.Value)
                            produto["imagemUrl"] = "data:" + produto["tipoImagem"] + ";base64," + Convert.ToBase64String((byte[])produto["imagem"]);
                    rptProdutos.DataSource = produtos;
                    rptProdutos.DataBind();
                    lblContagem.Text = String.Format("{0} produto(s)", produtos.Rows.Count);
                    pnlSemProdutos.Visible = produtos.Rows.Count == 0;
                }
            }
            catch (SqlException)
            {
                rptProdutos.DataSource = null;
                rptProdutos.DataBind();
                pnlSemProdutos.Visible = false;
                MostrarMensagem("Não foi possível carregar os produtos do banco MercadoOnline.", false);
            }
        }

        private void PrepararEdicao(int idProduto)
        {
            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco)) return;

            const string sql = "SELECT idProduto, descricao, preco, estoque, idCategoria, imagem FROM Produto WHERE idProduto = @idProduto";
            try
            {
                using (SqlConnection conexao = conexaoBanco)
                using (SqlCommand comando = new SqlCommand(sql, conexao))
                {
                    comando.Parameters.Add("@idProduto", SqlDbType.Int).Value = idProduto;
                    conexao.Open();
                    using (SqlDataReader leitor = comando.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!leitor.Read())
                        {
                            MostrarMensagem("O produto selecionado não foi encontrado.", false);
                            CarregarProdutos();
                            return;
                        }

                        hfIdProduto.Value = leitor["idProduto"].ToString();
                        txtDescricao.Text = leitor["descricao"].ToString();
                        txtPreco.Text = Convert.ToDecimal(leitor["preco"]).ToString("0.00", CultureInfo.InvariantCulture);
                        txtEstoque.Text = leitor["estoque"].ToString();
                        lblImagemAtual.Text = leitor["imagem"] == DBNull.Value ? "Nenhuma imagem cadastrada." : "Imagem cadastrada. Escolha outro arquivo para substituir.";
                        string idCategoria = leitor["idCategoria"].ToString();
                        if (ddlCategoria.Items.FindByValue(idCategoria) != null)
                            ddlCategoria.SelectedValue = idCategoria;
                    }
                }

                lblModo.Text = "Editar produto";
                btnSalvar.Text = "Salvar alterações";
                btnCancelar.Visible = true;
                MostrarMensagem("Edite os campos e salve as alterações.", true);
            }
            catch (SqlException)
            {
                MostrarMensagem("Não foi possível abrir o produto para edição.", false);
            }
        }

        private void ExcluirProduto(int idProduto)
        {
            SqlConnection conexaoBanco;
            if (!Conexao.TentarCriarConexao(out conexaoBanco))
            {
                MostrarMensagem("A conexão MercadoOnline não está configurada.", false);
                return;
            }

            try
            {
                using (SqlConnection conexao = conexaoBanco)
                using (SqlCommand comando = new SqlCommand("DELETE FROM Produto WHERE idProduto = @idProduto", conexao))
                {
                    comando.Parameters.Add("@idProduto", SqlDbType.Int).Value = idProduto;
                    conexao.Open();
                    int linhasAfetadas = comando.ExecuteNonQuery();
                    MostrarMensagem(linhasAfetadas > 0 ? "Produto removido com sucesso." : "O produto já não existe.", linhasAfetadas > 0);
                }
            }
            catch (SqlException erro)
            {
                if (erro.Number == 547)
                    MostrarMensagem("Este produto está vinculado a pedidos e não pode ser excluído.", false);
                else
                    MostrarMensagem("Não foi possível excluir o produto no banco MercadoOnline.", false);
            }

            CarregarProdutos();
        }

        private void ResetarFormulario()
        {
            hfIdProduto.Value = String.Empty;
            txtDescricao.Text = String.Empty;
            txtPreco.Text = String.Empty;
            txtEstoque.Text = String.Empty;
            lblImagemAtual.Text = String.Empty;
            if (ddlCategoria.Items.Count > 0) ddlCategoria.SelectedIndex = 0;
            lblModo.Text = "Adicionar produto";
            btnSalvar.Text = "Adicionar produto";
            btnCancelar.Visible = false;
        }

        private void MostrarMensagem(string mensagem, bool sucesso)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.CssClass = sucesso ? "mensagem sucesso" : "mensagem erro";
            lblMensagem.Visible = true;
        }
    }
}
