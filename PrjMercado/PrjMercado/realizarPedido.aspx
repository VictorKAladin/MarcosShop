<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="realizarPedido.aspx.cs" Inherits="PrjMercado.realizarPedido" Culture="pt-BR" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Finalizar pedido | MarcosShop</title>
    <link rel="stylesheet" type="text/css" href="css/realizarPedido.css" />
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
            <a class="botaoMenu" href="produtos.aspx">Produtos</a>
            <a class="botaoMenu" href="sobreNos.aspx">Sobre Nós</a>
            <a class="botaoMenu" href="faleConosco.aspx">Fale Conosco</a>
        </nav>

        <main class="conteudo">
            <section class="titulo-pagina">
                <p class="sobretitulo">Só falta confirmar</p>
                <h1>Finalizar pedido</h1>
                <p>Revise seus produtos, informe o endereço e escolha como deseja pagar.</p>
            </section>

            <asp:Label ID="lblMensagem" runat="server" CssClass="mensagem erro" Visible="false" />

            <asp:Panel ID="pnlCheckout" runat="server" CssClass="checkout">
                <section class="resumo-pedido" aria-labelledby="itens-titulo">
                    <div class="titulo-secao">
                        <h2 id="itens-titulo">Seus produtos</h2>
                        <a href="carrinho.aspx">Voltar ao carrinho</a>
                    </div>
                    <asp:Repeater ID="rptItens" runat="server">
                        <HeaderTemplate><div class="lista-itens"></HeaderTemplate>
                        <ItemTemplate>
                            <article class="item-pedido">
                                <div class="icone-produto" aria-hidden="true">🛒</div>
                                <div class="dados-item">
                                    <h3><%#: Eval("descricao") %></h3>
                                    <span><%# Eval("quantidade") %> × <%# Eval("preco", "{0:C}") %></span>
                                </div>
                                <strong class="subtotal"><%# Eval("subtotal", "{0:C}") %></strong>
                            </article>
                        </ItemTemplate>
                        <FooterTemplate></div></FooterTemplate>
                    </asp:Repeater>
                    <asp:Panel ID="pnlTotal" runat="server" CssClass="linha-total">
                        <span>Total do pedido</span>
                        <asp:Label ID="lblTotal" runat="server" Text="R$ 0,00" />
                    </asp:Panel>
                </section>

                <section class="formulario-checkout" aria-labelledby="entrega-titulo">
                    <h2 id="entrega-titulo">Entrega e pagamento</h2>
                    <div class="campo-formulario">
                        <asp:Label ID="lblEndereco" runat="server" AssociatedControlID="txtEndereco" Text="Endereço de entrega" />
                        <asp:TextBox ID="txtEndereco" runat="server" CssClass="entrada" MaxLength="200" placeholder="Rua, número, bairro, cidade e CEP" />
                        <small>O endereço será salvo no cadastro da sua conta.</small>
                    </div>
                    <div class="campo-formulario">
                        <asp:Label ID="lblPagamento" runat="server" AssociatedControlID="ddlPagamento" Text="Forma de pagamento" />
                        <asp:DropDownList ID="ddlPagamento" runat="server" CssClass="entrada">
                            <asp:ListItem Text="Pix" Value="Pix" />
                            <asp:ListItem Text="Dinheiro" Value="Dinheiro" />
                            <asp:ListItem Text="Cartão" Value="Cartao" />
                        </asp:DropDownList>
                    </div>
                    <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar pedido" CssClass="botao-confirmar" CausesValidation="false" OnClick="btnConfirmar_Click" />
                </section>
            </asp:Panel>

            <asp:Panel ID="pnlCarrinhoVazio" runat="server" CssClass="estado-vazio" Visible="false">
                <div aria-hidden="true">🛒</div>
                <h2>Seu carrinho está vazio</h2>
                <p>Adicione produtos antes de finalizar um pedido.</p>
                <a class="botao-link" href="produtos.aspx">Ver produtos</a>
            </asp:Panel>

            <asp:Panel ID="pnlConcluido" runat="server" CssClass="pedido-concluido" Visible="false">
                <span class="icone-sucesso" aria-hidden="true">✓</span>
                <p class="sobretitulo">Pedido registrado</p>
                <h2>Obrigado pela sua compra!</h2>
                <p>Pedido <strong>#<asp:Label ID="lblNumeroPedido" runat="server" /></strong> confirmado para processamento.</p>
                <p class="total-confirmado">Total: <asp:Label ID="lblTotalConfirmado" runat="server" /></p>
                <a class="botao-link" href="produtos.aspx">Continuar comprando</a>
            </asp:Panel>
        </main>

        <footer class="rodape">
            <p>© 2026 MarcosShop - Todos os direitos reservados.</p>
        </footer>
    </form>
</body>
</html>
