<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="carrinho.aspx.cs" Inherits="PrjMercado.carrinho" Culture="pt-BR" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Meu Carrinho | MarcosShop</title>
    <link rel="stylesheet" type="text/css" href="css/carrinho.css" />
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
                <p class="sobretitulo">Sua seleção</p>
                <h1>Meu carrinho</h1>
                <p>Confira os produtos que você adicionou.</p>
            </section>

            <asp:Label ID="lblMensagem" runat="server" CssClass="mensagem" Visible="false" />

            <asp:Panel ID="pnlCarrinhoVazio" runat="server" CssClass="carrinho-vazio" Visible="false">
                <div class="icone-vazio" aria-hidden="true">🛒</div>
                <h2>Seu carrinho está vazio</h2>
                <p>Explore os produtos e adicione seus favoritos por aqui.</p>
                <a class="botao-continuar" href="produtos.aspx">Ver produtos</a>
            </asp:Panel>

            <asp:Panel ID="pnlItensCarrinho" runat="server" CssClass="painel-carrinho" Visible="false">
                <div class="lista-itens">
                    <asp:Repeater ID="rptCarrinho" runat="server" OnItemCommand="rptCarrinho_ItemCommand">
                        <ItemTemplate>
                            <article class="item-carrinho">
                                <div class="icone-produto" aria-hidden="true">🛍️</div>
                                <div class="dados-produto">
                                    <span class="categoria"><%#: Eval("categoria") %></span>
                                    <h2><%#: Eval("descricao") %></h2>
                                    <p class="preco-unitario">Unidade: <%# Eval("preco", "{0:C}") %></p>
                                    <p class="estoque">Estoque disponível: <%# Eval("estoque") %></p>
                                </div>
                                <div class="quantidade-item">
                                    <span>Quantidade</span>
                                    <strong><%# Eval("quantidade") %></strong>
                                </div>
                                <div class="subtotal-item">
                                    <span>Subtotal</span>
                                    <strong><%# Eval("subtotal", "{0:C}") %></strong>
                                </div>
                                <asp:Button ID="btnRemover" runat="server" Text="Remover" CssClass="botao-remover" CommandName="Remover" CommandArgument='<%# Eval("idProduto") %>' CausesValidation="false" />
                            </article>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div class="resumo-carrinho">
                    <asp:LinkButton ID="btnLimparCarrinho" runat="server" CssClass="link-limpar" Text="Esvaziar carrinho" CausesValidation="false" OnClick="btnLimparCarrinho_Click" />
                    <div class="total-carrinho">
                        <span>Total</span>
                        <asp:Label ID="lblTotal" runat="server" CssClass="valor-total" />
                    </div>
                    <a class="botao-continuar" href="produtos.aspx">Continuar comprando</a>
                    <a class="botao-finalizar" href="realizarPedido.aspx">Ir para pagamento</a>
                </div>
            </asp:Panel>
        </main>

        <footer class="rodape">
            <p>© 2026 MarcosShop - Todos os direitos reservados.</p>
            <p>Seu mercado completo, da nossa fazenda para você!</p>
        </footer>
    </form>
</body>
</html>
