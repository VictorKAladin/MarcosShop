<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="index.aspx.cs"
    Inherits="MarcosShop.Index" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>

    <title>MarcosShop - Seu mercado completo!</title>

    <link rel="stylesheet" type="text/css" href="css/index.css" />
    <link rel="stylesheet" type="text/css" href="css/cabecalho.css" />
    <link rel="stylesheet" type="text/css" href="css/menu.css" />
    <script src="js/carrosselIndex.js" type="text/javascript" defer="defer"></script>
</head>

<body>
    <form id="form1" runat="server">

        <!-- CABEÇALHO PRINCIPAL -->
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


        <!-- MENU PRINCIPAL -->
        <nav class="menu" aria-label="Navegação principal">
            <a class="botaoSelecionado" href="index.aspx" aria-current="page">Início</a>
            <a class="botaoMenu" href="produtos.aspx">Produtos</a>
            <a class="botaoMenu" href="sobreNos.aspx">Sobre Nós</a>
            <a class="botaoMenu" href="faleConosco.aspx">Fale Conosco</a>
        </nav>


        <!-- CONTEÚDO PRINCIPAL -->
        <main class="conteudo">

            <!-- BANNER PRINCIPAL -->
            <section id="carrosselDestaques" class="carrossel" role="region" aria-roledescription="carrossel" aria-label="Destaques da MarcosShop">
                <div id="slidesDestaques" class="slides-carrossel">
                    <article class="carrossel-slide ativo" role="group" aria-roledescription="slide" aria-label="1 de 5">
                        <img class="imagem-slide" src="imagens/carrosel1.png"/>
                        <div class="legenda-slide"><h2>Destaque 1</h2><p>Seu lugar de compras é aqui</p></div>
                    </article>
                    <article class="carrossel-slide" role="group" aria-roledescription="slide" aria-label="2 de 5">
                        <img class="imagem-slide" src="imagens/carrosel2.png"/>
                        <div class="legenda-slide"><h2>Destaque 2</h2><p>Nunca precisará desconfiar</p></div>
                    </article>
                    <article class="carrossel-slide" role="group" aria-roledescription="slide" aria-label="3 de 5">
                        <img class="imagem-slide" src="imagens/carrosel3.png"/>
                        <div class="legenda-slide"><h2>Destaque 3</h2><p>Fresco, bom e barato</p></div>
                    </article>
                    <article class="carrossel-slide" role="group" aria-roledescription="slide" aria-label="4 de 5">
                        <img class="imagem-slide" src="imagens/carrosel4.png"/>
                        <div class="legenda-slide"><h2>Destaque 4</h2><p>Carrinho cheio, bolso cheio</p></div>
                    </article>
                    <article class="carrossel-slide" role="group" aria-roledescription="slide" aria-label="5 de 5">
                        <img class="imagem-slide" src="imagens/carrosel5.png"/>
                        <div class="legenda-slide"><h2>Destaque 5</h2><p>Use proteção</p></div>
                    </article>
                </div>
                <button type="button" class="controle-carrossel anterior" data-direcao="anterior" aria-label="Slide anterior" aria-controls="slidesDestaques">&#10094;</button>
                <button type="button" class="controle-carrossel proximo" data-direcao="proximo" aria-label="Próximo slide" aria-controls="slidesDestaques">&#10095;</button>
                <div class="indicadores-carrossel" role="group" aria-label="Selecionar slide">
                    <button type="button" class="indicador ativo" data-slide="0" aria-label="Slide 1" aria-controls="slidesDestaques" aria-pressed="true"></button>
                    <button type="button" class="indicador" data-slide="1" aria-label="Slide 2" aria-controls="slidesDestaques" aria-pressed="false"></button>
                    <button type="button" class="indicador" data-slide="2" aria-label="Slide 3" aria-controls="slidesDestaques" aria-pressed="false"></button>
                    <button type="button" class="indicador" data-slide="3" aria-label="Slide 4" aria-controls="slidesDestaques" aria-pressed="false"></button>
                    <button type="button" class="indicador" data-slide="4" aria-label="Slide 5" aria-controls="slidesDestaques" aria-pressed="false"></button>
                </div>
            </section>

            <!-- ÁREA DE DESTAQUES -->
            <section class="destaques">

                <h2>Explore nossas categorias</h2>

                <div class="categorias">

                    <a class="categoria" href="produtos.aspx?categorias=Frutas%2CVerduras" aria-label="Ver frutas e verduras">
                        <span>🍎</span>
                        <p>Hortifruti</p>
                    </a>

                    <a class="categoria" href="produtos.aspx?categorias=Bebidas" aria-label="Ver bebidas">
                        <span>🥤</span>
                        <p>Bebidas</p>
                    </a>

                    <a class="categoria" href="produtos.aspx?categorias=Padaria" aria-label="Ver produtos de padaria">
                        <span>🥖</span>
                        <p>Padaria</p>
                    </a>

                    <a class="categoria" href="produtos.aspx?categorias=Laticinios" aria-label="Ver laticínios">
                        <span>🥛</span>
                        <p>Laticínios</p>
                    </a>

                    <a class="categoria" href="produtos.aspx?categorias=Mercearia" aria-label="Ver produtos de mercearia">
                        <span>🥫</span>
                        <p>Mercearia</p>
                    </a>

                    <a class="categoria" href="produtos.aspx?categorias=Limpeza" aria-label="Ver produtos de limpeza">
                        <span>🧽</span>
                        <p>Limpeza</p>
                    </a>

                </div>

            </section>

        </main>


        <!-- RODAPÉ -->
        <footer class="rodape">

            <p>© 2026 MarcosShop - Todos os direitos reservados.</p>
            <p>Seu mercado completo, da nossa fazenda para você!</p>

        </footer>

    </form>
</body>
</html>
