<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="sobreNos.aspx.cs" Inherits="PrjMercado.sobreNos" Culture="pt-BR" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Sobre Nós | MarcosShop</title>
    <link rel="stylesheet" type="text/css" href="css/sobreNos.css" />
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
            <a class="botaoSelecionado" href="sobreNos.aspx">Sobre Nós</a>
            <a class="botaoMenu" href="faleConosco.aspx">Fale Conosco</a>
        </nav>

        <main class="conteudo">
            <section class="introducao">
                <p class="sobretitulo">Conheça a MarcosShop</p>
                <h1>Sobre Nós</h1>
                <p>A MarcosShop é um projeto de mercado online criado para facilitar a busca por produtos, a montagem do carrinho e o acompanhamento dos pedidos em um só lugar.</p>
            </section>

            <section class="historia" aria-labelledby="historia-titulo">
                <div>
                    <h2 id="historia-titulo">Sobre o projeto</h2>
                    <p>A loja reúne produtos em categorias para que cada pessoa encontre com facilidade o que procura. É possível conferir preços e disponibilidade antes de adicionar os itens ao carrinho.</p>
                    <p>Na finalização, o cliente revisa os produtos, informa o endereço de entrega e escolhe a forma de pagamento. Depois, pode acompanhar o status do pedido na área da conta.</p>
                </div>
                <div>
                    <h2>Como funciona</h2>
                    <p>A navegação por categorias ajuda a filtrar o catálogo. Na página de produtos, cada item mostra informações como preço e estoque, e pode ser adicionado ao carrinho na quantidade desejada.</p>
                    <p>A MarcosShop também oferece uma área de contato e páginas de perfil para que clientes consultem seus pedidos e administradores cuidem do catálogo.</p>
                </div>
            </section>

            <section class="galeria" aria-labelledby="galeria-titulo">
                <div class="titulo-galeria">
                    <p class="sobretitulo">Um pouco mais de perto</p>
                    <h2 id="galeria-titulo">Pessoas por trás do projeto</h2>
                </div>
                <div class="cartoes-imagem">
                    <figure class="cartao-imagem">
                        <img class="imagem-sobre" src="imagens/nos.png" alt="Retrato de Felipe Bruno e Victor Aladin, integrantes do projeto MarcosShop" />
                        <figcaption><strong>Felipe Bruno, Victor Aladin e <a class="link-easter-egg" href="capri.aspx" title="Conheça Capri">Capri</a></strong><span>Participação no desenvolvimento da loja virtual MarcosShop.</span></figcaption>
                    </figure>
                    
                </div>
            </section>
        </main>

        <footer class="rodape">
            <p>© 2026 MarcosShop - Todos os direitos reservados.</p>
            <p>Uma experiência simples para encontrar produtos, organizar compras e acompanhar pedidos.</p>
        </footer>
    </form>
</body>
</html>
