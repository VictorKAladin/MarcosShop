<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="meusPedidos.aspx.cs" Inherits="PrjMercado.meusPedidos" Culture="pt-BR" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Meus pedidos | MarcosShop</title>
    <link rel="stylesheet" type="text/css" href="css/meusPedidos.css" />
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
            <a class="botaoMenu" href="produtos.aspx">Produtos</a>
            <a class="botaoMenu" href="sobreNos.aspx">Sobre Nós</a>
            <a class="botaoMenu" href="faleConosco.aspx">Fale Conosco</a>
        </nav>

        <main class="conteudo">
            <section class="titulo-pagina">
                <p class="sobretitulo">Acompanhe suas compras</p>
                <h1>Meus pedidos</h1>
                <p>Confira os produtos, valores e o status de cada pedido feito na MarcosShop.</p>
            </section>

            <asp:Label ID="lblMensagem" runat="server" CssClass="mensagem" Visible="false" />

            <asp:Repeater ID="rptPedidos" runat="server">
                <ItemTemplate>
                    <article class="pedido-card">
                        <div class="cabecalho-pedido">
                            <div>
                                <p class="rotulo-pedido">Pedido</p>
                                <h2>#<%#: Eval("idPedido") %></h2>
                            </div>
                            <span class='status-badge <%# ClasseStatus(Eval("status")) %>'><%#: Eval("status") %></span>
                        </div>
                        <div class="detalhes-pedido">
                            <div class="detalhe"><span>Data</span><strong><%# Eval("dataPedido", "{0:dd/MM/yyyy 'às' HH:mm}") %></strong></div>
                            <div class="detalhe"><span>Pagamento</span><strong><%#: Eval("formaPagamento") %></strong></div>
                            <div class="detalhe total"><span>Total</span><strong><%# Eval("valorTotal", "{0:C}") %></strong></div>
                        </div>
                        <div class="itens-pedido">
                            <span>Produtos</span>
                            <p><%#: Eval("itens") %></p>
                        </div>
                    </article>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlSemPedidos" runat="server" CssClass="estado-vazio" Visible="false">
                <div aria-hidden="true">🛍️</div>
                <h2>Você ainda não tem pedidos</h2>
                <p>Quando fizer uma compra, ela aparecerá aqui com seu status atualizado.</p>
                <a class="botao-link" href="produtos.aspx">Ver produtos</a>
            </asp:Panel>

            <a class="link-voltar" href="perfil.aspx">Voltar ao meu perfil</a>
        </main>

        <footer class="rodape">
            <p>© 2026 MarcosShop - Todos os direitos reservados.</p>
        </footer>
    </form>
</body>
</html>
