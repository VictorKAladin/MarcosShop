<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="faleConosco.aspx.cs" Inherits="PrjMercado.faleConosco" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Fale Conosco | MarcosShop</title>
    <link rel="stylesheet" type="text/css" href="css/faleConosco.css" />
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
            <a class="botaoSelecionado" href="faleConosco.aspx">Fale Conosco</a>
        </nav>

        <main class="conteudo">
            <section class="introducao">
                <p class="sobretitulo">Estamos aqui para ouvir você</p>
                <h1>Fale Conosco</h1>
                <p>Envie sua mensagem para a equipe MarcosShop. Preencha os campos abaixo para deixar seu comentário ou dúvida.</p>
            </section>

            <section class="cartao-contato" aria-labelledby="formulario-titulo">
                <h2 id="formulario-titulo">Envie sua mensagem</h2>
                <div class="campo-formulario">
                    <asp:Label ID="lblNome" runat="server" AssociatedControlID="txtNome" Text="Seu nome" />
                    <asp:TextBox ID="txtNome" runat="server" CssClass="entrada" MaxLength="100" placeholder="Digite seu nome" />
                </div>
                <div class="campo-formulario">
                    <asp:Label ID="lblMotivo" runat="server" AssociatedControlID="ddlMotivo" Text="Motivo do contato" />
                    <asp:DropDownList ID="ddlMotivo" runat="server" CssClass="entrada">
                        <asp:ListItem Text="Selecione um motivo" Value="" />
                        <asp:ListItem Text="Dúvida" Value="duvida" />
                        <asp:ListItem Text="Sugestão" Value="sugestao" />
                        <asp:ListItem Text="Elogio" Value="elogio" />
                        <asp:ListItem Text="Reclamação" Value="reclamacao" />
                        <asp:ListItem Text="Outro" Value="outro" />
                    </asp:DropDownList>
                </div>
                <div class="campo-formulario">
                    <asp:Label ID="lblDetalhes" runat="server" AssociatedControlID="txtDetalhes" Text="Detalhes da mensagem" />
                    <asp:TextBox ID="txtDetalhes" runat="server" CssClass="entrada detalhes" TextMode="MultiLine" Rows="6" MaxLength="2000" placeholder="Escreva sua mensagem aqui..." />
                </div>
                <asp:Button ID="btnEnviar" runat="server" Text="Enviar mensagem" CssClass="botao-enviar" CausesValidation="false" OnClick="btnEnviar_Click" />
            </section>

            <section class="redes-sociais" aria-labelledby="redes-titulo">
                <h2 id="redes-titulo">Acompanhe a MarcosShop</h2>
                <p>Encontre nossos canais nas redes sociais.</p>
                <div class="botoes-redes">
                    <asp:ImageButton ID="btnInstagram" runat="server" ImageUrl="imagens/instagram.svg" AlternateText="Instagram" ToolTip="Abrir Instagram" CssClass="botao-rede instagram" CausesValidation="false" OnClientClick="window.location.href='https://www.instagram.com/'; return false;" />
                    <asp:ImageButton ID="btnYoutube" runat="server" ImageUrl="imagens/youtube.svg" AlternateText="YouTube" ToolTip="Abrir YouTube" CssClass="botao-rede youtube" CausesValidation="false" OnClientClick="window.location.href='https://www.youtube.com/'; return false;" />
                    <asp:ImageButton ID="btnDiscord" runat="server" ImageUrl="imagens/discord.svg" AlternateText="Discord" ToolTip="Abrir Discord" CssClass="botao-rede discord" CausesValidation="false" OnClientClick="window.location.href='https://discord.com/'; return false;" />
                </div>
            </section>
        </main>

        <footer class="rodape">
            <p>© 2026 MarcosShop - Todos os direitos reservados.</p>
            <p>Estamos prontos para ouvir você.</p>
        </footer>
    </form>
</body>
</html>
