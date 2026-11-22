<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="perfil.aspx.cs" Inherits="PrjMercado.perfil" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Meu perfil | MarcosShop</title>
    <link rel="stylesheet" type="text/css" href="css/perfil.css" />
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
            <section class="perfil-card" aria-labelledby="titulo-perfil">
                <div class="avatar-perfil" aria-hidden="true">👤</div>
                <p class="sobretitulo"><asp:Label ID="lblTipoConta" runat="server" /></p>
                <h1 id="titulo-perfil"><asp:Label ID="lblNome" runat="server" /></h1>
                <asp:Label ID="lblErro" runat="server" CssClass="status" Visible="false" />

                <asp:Panel ID="pnlVisualizacao" runat="server">
                    <div class="dados-perfil">
                        <div class="dado"><span>E-mail</span><strong><asp:Label ID="lblEmail" runat="server" /></strong></div>
                        <div class="dado"><span>Telefone</span><strong><asp:Label ID="lblTelefone" runat="server" /></strong></div>
                        <asp:Panel ID="pnlEndereco" runat="server" CssClass="dado" Visible="false">
                            <span>Endereço</span><strong><asp:Label ID="lblEndereco" runat="server" /></strong>
                        </asp:Panel>
                    </div>
                    <div class="acoes-perfil">
                        <asp:Panel ID="pnlPedidosCliente" runat="server" Visible="false">
                            <a class="botao-secundario" href="meusPedidos.aspx">Meus pedidos</a>
                        </asp:Panel>
                        <asp:Button ID="btnEditar" runat="server" Text="Editar dados" CssClass="botao-primario" CausesValidation="false" OnClick="btnEditar_Click" />
                        <asp:Panel ID="pnlAdmin" runat="server" CssClass="acao-admin" Visible="false">
                            <a class="botao-secundario" href="gerenciarProdutos.aspx">Gerenciar produtos</a>
                        </asp:Panel>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlEdicao" runat="server" CssClass="form-edicao" Visible="false">
                    <div class="campo-edicao">
                        <asp:Label ID="lblEditNome" runat="server" AssociatedControlID="txtEditNome" Text="Nome" />
                        <asp:TextBox ID="txtEditNome" runat="server" CssClass="entrada" MaxLength="100" />
                    </div>
                    <div class="campo-edicao">
                        <asp:Label ID="lblEditEmail" runat="server" AssociatedControlID="txtEditEmail" Text="E-mail" />
                        <asp:TextBox ID="txtEditEmail" runat="server" CssClass="entrada" MaxLength="100" TextMode="Email" />
                    </div>
                    <div class="campo-edicao">
                        <asp:Label ID="lblEditTelefone" runat="server" AssociatedControlID="txtEditTelefone" Text="Telefone" />
                        <asp:TextBox ID="txtEditTelefone" runat="server" CssClass="entrada" MaxLength="20" />
                    </div>
                    <asp:Panel ID="pnlEditEndereco" runat="server" CssClass="campo-edicao" Visible="false">
                        <asp:Label ID="lblEditEndereco" runat="server" AssociatedControlID="txtEditEndereco" Text="Endereço" />
                        <asp:TextBox ID="txtEditEndereco" runat="server" CssClass="entrada" MaxLength="200" />
                    </asp:Panel>
                    <div class="acoes-edicao">
                        <asp:Button ID="btnSalvar" runat="server" Text="Salvar alterações" CssClass="botao-primario" CausesValidation="false" OnClick="btnSalvar_Click" />
                        <asp:Button ID="btnCancelarEdicao" runat="server" Text="Cancelar" CssClass="botao-secundario" CausesValidation="false" OnClick="btnCancelarEdicao_Click" />
                    </div>
                </asp:Panel>

                <div class="acoes-conta">
                    <asp:Button ID="btnSair" runat="server" Text="Sair da conta" CssClass="botao-secundario" CausesValidation="false" OnClick="btnSair_Click" />
                    <asp:Button ID="btnExcluir" runat="server" Text="Excluir conta" CssClass="botao-perigo-link" CausesValidation="false" OnClick="btnExcluir_Click" />
                </div>
                <a class="link-voltar" href="index.aspx">Voltar para a loja</a>
            </section>
        </main>

        <asp:Panel ID="pnlConfirmacao" runat="server" CssClass="popup-fundo" Visible="false" role="dialog" aria-modal="true" aria-labelledby="titulo-confirmacao">
            <div class="popup-confirmacao">
                <span class="icone-alerta" aria-hidden="true">!</span>
                <h2 id="titulo-confirmacao">Excluir conta?</h2>
                <asp:Label ID="lblConfirmacao" runat="server" CssClass="texto-confirmacao" />
                <div class="acoes-confirmacao">
                    <asp:Button ID="btnCancelarExclusao" runat="server" Text="Manter minha conta" CssClass="botao-secundario" CausesValidation="false" OnClick="btnCancelarExclusao_Click" />
                    <asp:Button ID="btnConfirmarExclusao" runat="server" Text="Sim, excluir conta" CssClass="botao-perigo" CausesValidation="false" OnClick="btnConfirmarExclusao_Click" />
                </div>
            </div>
        </asp:Panel>

        <footer class="rodape">
            <p>© 2026 MarcosShop - Todos os direitos reservados.</p>
        </footer>
    </form>
</body>
</html>
