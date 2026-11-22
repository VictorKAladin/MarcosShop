<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="capri.aspx.cs" Inherits="PrjMercado.capri" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Capri | MarcosShop</title>
    <link rel="stylesheet" type="text/css" href="css/capri.css" />
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
            <section class="apresentacao">
                <p class="sobretitulo">Uma presença nos bastidores</p>
                <h1>Capri</h1>
                <p class="resumo">O terceiro integrante que ajudou a equipe a enxergar possibilidades, organizar ideias e seguir com mais confiança.</p>
            </section>
            <section class="cartoes" aria-label="A contribuição de Capri">
                <article class="cartao">
                    <span class="icone" aria-hidden="true">✦</span>
                    <h2>Motivações</h2>
                    <p>Capri se juntou ao grupo movido pela curiosidade de ver uma ideia ganhar forma e pela vontade de contribuir com um projeto feito em equipe. Acredita que boas soluções surgem quando todos podem compartilhar dúvidas, testar caminhos e aprender durante o processo.</p>
                </article>
                <article class="cartao">
                    <span class="icone" aria-hidden="true">↗</span>
                    <h2>Ambições</h2>
                    <p>Seu objetivo é continuar aprendendo e participar de projetos cada vez mais completos. Para Capri, cada desafio é uma oportunidade de desenvolver novas habilidades, colaborar melhor e ajudar a transformar uma proposta em algo útil para quem vai usar.</p>
                </article>
                <article class="cartao destaque">
                    <span class="icone" aria-hidden="true">◎</span>
                    <h2>Um bom caminho</h2>
                    <p>Ao longo da MarcosShop, Capri ajudou o grupo a refletir sobre as escolhas do projeto e a encontrar um rumo claro para as próximas etapas. Sua contribuição foi estar por perto, fazer perguntas e apoiar decisões que mantivessem a equipe avançando.</p>
                </article>
            </section>
            <section class="galeria" aria-labelledby="galeria-titulo">
                <div class="titulo-galeria">
                    <p class="sobretitulo">Um pouco mais de perto</p>
                    <h2 id="galeria-titulo">Pessoas por trás do projeto</h2>
                </div>
                <div class="cartoes-imagem">
                    <figure class="cartao-imagem">
                        <img class="imagem-sobre" src="imagens/all.png" alt="Retrato de Felipe Bruno e Victor Aladin, integrantes do projeto MarcosShop" />
                        <figcaption><strong>Felipe Bruno, Victor Aladin e <a class="link-easter-egg" href="capri.aspx" title="Conheça Capri">Capri</a></strong><span>Participação no desenvolvimento da loja virtual MarcosShop.</span></figcaption>
                    </figure>
                    
                </div>
            </section>
            <p class="retorno"><a href="sobreNos.aspx">← Voltar para Sobre Nós</a></p>
        </main>
        <footer class="rodape">
            <p>© 2026 MarcosShop - Todos os direitos reservados.</p>
            <p>Ideias melhores crescem quando são construídas em conjunto.</p>
        </footer>
    </form>
</body>
</html>
