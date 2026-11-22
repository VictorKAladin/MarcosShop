<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="perfilAdmin.aspx.cs" Inherits="PrjMercado.perfilAdmin" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Perfil do administrador | MarcosShop</title>
    <link rel="stylesheet" type="text/css" href="css/perfilAdmin.css" />
</head>
<body>
    <form id="form1" runat="server">
        <header class="cabecalho">
            <a class="marca" href="index.aspx">MarcosShop <span>Administração</span></a>
            <nav aria-label="Navegação administrativa">
                <a href="index.aspx">Ver loja</a>
                <a href="gerenciarProdutos.aspx">Produtos</a>
                <a href="gerenciarUsuarios.aspx">Usuários</a>
                <a href="relatorios.aspx">Relatórios</a>
            </nav>
        </header>

        <main class="conteudo">
            <section class="perfil-admin" aria-labelledby="titulo-perfil">
                <div class="icone-admin" aria-hidden="true">⚙</div>
                <p class="sobretitulo">Área administrativa</p>
                <h1 id="titulo-perfil">Perfil do administrador</h1>
                <asp:Label ID="lblMensagem" runat="server" CssClass="mensagem" Visible="false" />

                <asp:Panel ID="pnlDadosAdmin" runat="server" CssClass="dados-admin" Visible="false">
                    <div class="dado"><span>Nome</span><strong><asp:Label ID="lblNome" runat="server" /></strong></div>
                    <div class="dado"><span>E-mail</span><strong><asp:Label ID="lblEmail" runat="server" /></strong></div>
                    <div class="dado"><span>Telefone</span><strong><asp:Label ID="lblTelefone" runat="server" /></strong></div>
                </asp:Panel>

                <div class="atalhos-admin" aria-label="Ações administrativas">
                    <a class="atalho" href="gerenciarProdutos.aspx"><span aria-hidden="true">🛒</span><strong>Gerenciar produtos</strong><small>Adicionar, editar e remover produtos</small></a>
                    <a class="atalho" href="gerenciarUsuarios.aspx"><span aria-hidden="true">👥</span><strong>Gerenciar usuários</strong><small>Consultar e administrar contas</small></a>
                    <a class="atalho" href="relatorios.aspx"><span aria-hidden="true">📊</span><strong>Relatórios</strong><small>Acessar os relatórios da loja</small></a>
                </div>

                <asp:Button ID="btnSair" runat="server" Text="Sair da conta" CssClass="botao-sair" CausesValidation="false" OnClick="btnSair_Click" />
            </section>
        </main>

        <footer class="rodape">
            <p>© 2026 MarcosShop - Administração.</p>
        </footer>
    </form>
</body>
</html>
