<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="produtos.aspx.cs" Inherits="PrjMercado.produtos" Culture="pt-BR" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Produtos | MarcosShop</title>
    <link rel="stylesheet" type="text/css" href="css/produtos.css" />
    <link rel="stylesheet" type="text/css" href="css/cabecalho.css" />
    <link rel="stylesheet" type="text/css" href="css/menu.css" />
</head>
<body>
    <form id="form1" runat="server">
        <header class="cabecalho">

            <!-- LOGO -->
            <div class="logo">
                <asp:ImageButton
                    ID="imgLogo"
                    runat="server"
                    ImageUrl="imagens/logo.png"
                    AlternateText="MarcosShop"
                    CausesValidation="false"
                    OnClick="btnHomePage_Click" />

                <asp:Label
                    ID="Label1"
                    runat="server"
                    Text="MarcosShop">
                </asp:Label>
            </div>

            <!-- BARRA DE PESQUISA -->
            <div class="busca">

                <asp:TextBox
                    ID="txtPesquisa"
                    runat="server"
                    CssClass="campoPesquisa"
                    placeholder="Busque por produtos ou categorias...">
                </asp:TextBox>

                <asp:Button
                    ID="btnPesquisar"
                    runat="server"
                    Text="⌕"
                    CssClass="botaoPesquisa"
                    CausesValidation="false"
                    OnClick="btnPesquisar_Click"/>

            </div>

            <!-- USUÁRIO E CARRINHO -->
            <div class="usuario">

                <div class="login">

                    <asp:ImageButton
                        ID="imgPerfil"
                        runat="server"
                        ImageUrl="imagens/perfil.png"
                        AlternateText="Perfil"
                        CausesValidation="false"
                        OnClick="btnLogin_Click"/>

                    <asp:Button
                        ID="btnLogin"
                        runat="server"
                        Text="Acesse sua conta"
                        CssClass="botaoLogin"
                        CausesValidation="false"
                        OnClick="btnLogin_Click"/>

                </div>

                <div class="carrinho">

                    <asp:ImageButton
                        ID="imgCarrinho"
                        runat="server"
                        ImageUrl="imagens/carrinho.png"
                        AlternateText="Carrinho"
                    CausesValidation="false"
                    OnClick="btnCarrinho_Click" />

                </div>

            </div>

        </header>
        <nav class="menu" aria-label="Navegação principal">
            <a class="botaoMenu" href="index.aspx">Início</a>
            <a class="botaoSelecionado" href="produtos.aspx">Produtos</a>
            <a class="botaoMenu" href="sobreNos.aspx">Sobre Nós</a>
            <a class="botaoMenu" href="faleConosco.aspx">Fale Conosco</a>
        </nav>

        <main class="conteudo">
            <section class="titulo-pagina">
                <p class="sobretitulo">Fresquinho para você</p>
                <h1>Nossos produtos</h1>
                <p>Escolha uma ou mais categorias para filtrar os produtos.</p>
            </section>

            <asp:Label ID="lblMensagem" runat="server" CssClass="mensagem" Visible="false" />

            <div class="layout-produtos">
                <aside class="painel-filtros" aria-labelledby="filtros-titulo">
                    <h2 id="filtros-titulo">Categorias</h2>
                    <p>Marque para ativar o filtro:</p>
                    <asp:CheckBoxList ID="cblCategorias" runat="server" CssClass="lista-categorias" RepeatLayout="Flow" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="cblCategorias_SelectedIndexChanged" />
                    <a class="limpar-filtros" href="produtos.aspx">Limpar filtros</a>
                </aside>

                <section class="resultados" aria-label="Lista de produtos">
                    <asp:Label ID="lblContagem" runat="server" CssClass="contagem-produtos" />
                    <asp:Repeater ID="rptProdutos" runat="server" OnItemDataBound="rptProdutos_ItemDataBound" OnItemCommand="rptProdutos_ItemCommand">
                        <HeaderTemplate><div class="grade-produtos"></HeaderTemplate>
                        <ItemTemplate>
                            <article class="cartao-produto">
                                <div id="pnlIconeProduto" runat="server" class="icone-produto" aria-hidden="true">🛒</div>
                                <asp:Image ID="imgImagemProduto" runat="server" CssClass="imagem-produto" Visible="false" />
                                <span class="categoria-produto"><%#: Eval("categoria") %></span>
                                <h2><%#: Eval("descricao") %></h2>
                                <p class="preco-produto">R$ <%# Eval("preco", "{0:N2}") %></p>
                                <p class="estoque-produto">Estoque: <%# Eval("estoque") %></p>
                                <div class="adicionar-carrinho">
                                    <span class="rotulo-quantidade">Quantidade</span>
                                    <asp:TextBox ID="txtQuantidade" runat="server" TextMode="Number" Text="1" CssClass="quantidade-produto" aria-label="Quantidade" />
                                    <asp:Button ID="btnAdicionarCarrinho" runat="server" Text="Adicionar ao carrinho" CssClass="botao-adicionar" CommandName="AdicionarCarrinho" CommandArgument='<%# Eval("idProduto") %>' CausesValidation="false" />
                                </div>
                            </article>
                        </ItemTemplate>
                        <FooterTemplate></div></FooterTemplate>
                    </asp:Repeater>
                    <asp:Panel ID="pnlSemResultados" runat="server" CssClass="sem-resultados" Visible="false">
                        <h2>Nenhum produto encontrado</h2>
                        <p>Tente mudar a busca ou desmarcar algumas categorias.</p>
                    </asp:Panel>
                </section>
            </div>
        </main>

        <asp:Panel ID="pnlPopupAdicionado" runat="server" CssClass="popup-fundo" Visible="false" role="dialog" aria-modal="true" aria-labelledby="popup-titulo">
            <div class="popup-cart">
                <span class="popup-confirmacao" aria-hidden="true">✓</span>
                <h2 id="popup-titulo">Produto adicionado!</h2>
                <asp:Label ID="lblPopupMensagem" runat="server" CssClass="popup-mensagem" />
                <asp:LinkButton ID="btnFecharPopup" runat="server" CssClass="botao-popup" Text="Continuar comprando" CausesValidation="false" OnClick="btnFecharPopup_Click" />
            </div>
        </asp:Panel>

        <footer class="rodape">
            <p>© 2026 MarcosShop - Todos os direitos reservados.</p>
            <p>Seu mercado completo, da nossa fazenda para você!</p>
        </footer>
    </form>
</body>
</html>
