<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="gerenciarProdutos.aspx.cs" Inherits="PrjMercado.gerenciarProdutos" Culture="pt-BR" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Gerenciar produtos | MarcosShop</title>
    <link rel="stylesheet" type="text/css" href="css/gerenciarProdutos.css" />
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
        <header class="cabecalho">
            <a class="marca" href="index.aspx">MarcosShop <span>Administração</span></a>
            <nav aria-label="Navegação administrativa">
                <a href="perfilAdmin.aspx">Painel</a>
                <a class="selecionado" href="gerenciarProdutos.aspx">Produtos</a>
                <a href="gerenciarUsuarios.aspx" aria-current="page">Usuários</a>
                <a href="relatorios.aspx">Relatórios</a>
            </nav>
        </header>

        <main class="conteudo">
            <section class="titulo-pagina">
                <p class="sobretitulo">Painel administrativo</p>
                <h1>Gerenciar produtos</h1>
                <p>Cadastre produtos novos ou atualize as informações do catálogo.</p>
            </section>

            <asp:Label ID="lblMensagem" runat="server" CssClass="mensagem" Visible="false" />

            <section class="formulario-produto" aria-labelledby="formulario-titulo">
                <div class="titulo-formulario">
                    <div>
                        <p class="sobretitulo">Cadastro do catálogo</p>
                        <h2 id="formulario-titulo"><asp:Label ID="lblModo" runat="server" Text="Adicionar produto" /></h2>
                    </div>
                    <asp:HiddenField ID="hfIdProduto" runat="server" />
                </div>
                <div class="campos-formulario">
                    <div class="campo">
                        <asp:Label ID="lblImagem" runat="server" AssociatedControlID="fuImagem" Text="Imagem do produto (PNG, JPG, WEBP, GIF ou SVG; até 3 MB)" />
                        <asp:FileUpload ID="fuImagem" runat="server" CssClass="entrada" accept="image/png,image/jpeg,image/webp,image/gif,image/svg+xml" />
                        <asp:Label ID="lblImagemAtual" runat="server" CssClass="detalhe-produto" />
                    </div>
                    <div class="campo campo-descricao">
                        <asp:Label ID="lblDescricao" runat="server" AssociatedControlID="txtDescricao" Text="Nome do produto" />
                        <asp:TextBox ID="txtDescricao" runat="server" CssClass="entrada" MaxLength="100" placeholder="Ex.: Maçã" />
                    </div>
                    <div class="campo">
                        <asp:Label ID="lblPreco" runat="server" AssociatedControlID="txtPreco" Text="Preço (R$)" />
                        <asp:TextBox ID="txtPreco" runat="server" CssClass="entrada" TextMode="Number" min="0" step="0.01" placeholder="0,00" />
                    </div>
                    <div class="campo">
                        <asp:Label ID="lblEstoque" runat="server" AssociatedControlID="txtEstoque" Text="Estoque" />
                        <asp:TextBox ID="txtEstoque" runat="server" CssClass="entrada" TextMode="Number" min="0" step="1" placeholder="0" />
                    </div>
                    <div class="campo">
                        <asp:Label ID="lblCategoria" runat="server" AssociatedControlID="ddlCategoria" Text="Categoria" />
                        <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="entrada" />
                    </div>
                </div>
                <div class="acoes-formulario">
                    <asp:Button ID="btnSalvar" runat="server" Text="Adicionar produto" CssClass="botao-primario" CausesValidation="false" OnClick="btnSalvar_Click" />
                    <asp:Button ID="btnCancelar" runat="server" Text="Cancelar edição" CssClass="botao-secundario" CausesValidation="false" Visible="false" OnClick="btnCancelar_Click" />
                </div>
            </section>

            <section class="secao-lista" aria-labelledby="lista-titulo">
                <div class="titulo-lista">
                    <div>
                        <p class="sobretitulo">Catálogo atual</p>
                        <h2 id="lista-titulo">Produtos cadastrados</h2>
                    </div>
                    <asp:Label ID="lblContagem" runat="server" CssClass="contagem" />
                </div>
                <asp:Repeater ID="rptProdutos" runat="server" OnItemCommand="rptProdutos_ItemCommand">
                    <HeaderTemplate><div class="lista-produtos"></HeaderTemplate>
                    <ItemTemplate>
                        <article class="produto-admin">
                            <asp:Image ID="imgProdutoAdmin" runat="server" CssClass="imagem-admin" Visible="false" ImageUrl='<%# Eval("imagemUrl") %>' AlternateText='<%# "Imagem de " + Eval("descricao") %>' />
                            <div class="dados-produto">
                                <span class="categoria"><%#: Eval("categoria") %></span>
                                <h3><%#: Eval("descricao") %></h3>
                                <span class="detalhe-produto"><%# Eval("preco", "{0:C}") %> · Estoque: <%# Eval("estoque") %></span>
                            </div>
                            <div class="acoes-produto">
                                <asp:Button ID="btnEditar" runat="server" Text="Editar" CssClass="botao-editar" CommandName="EditarProduto" CommandArgument='<%# Eval("idProduto") %>' CausesValidation="false" />
                                <asp:Button ID="btnExcluir" runat="server" Text="Excluir" CssClass="botao-excluir" CommandName="ExcluirProduto" CommandArgument='<%# Eval("idProduto") %>' CausesValidation="false" OnClientClick="return confirm('Deseja excluir este produto?');" />
                            </div>
                        </article>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlSemProdutos" runat="server" CssClass="sem-produtos" Visible="false">
                    <h3>Nenhum produto cadastrado</h3>
                    <p>Use o formulário acima para adicionar o primeiro produto.</p>
                </asp:Panel>
            </section>
        </main>

        <footer class="rodape">
            <p>© 2026 MarcosShop - Área de administração.</p>
        </footer>
    </form>
</body>
</html>
