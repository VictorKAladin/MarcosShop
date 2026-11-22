<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="gerenciarUsuarios.aspx.cs" Inherits="PrjMercado.gerenciarUsuarios" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Gerenciar usuários | MarcosShop</title>
    <link rel="stylesheet" type="text/css" href="css/gerenciarUsuarios.css" />
</head>
<body>
    <form id="form1" runat="server">
        <header class="cabecalho">
            <a class="marca" href="index.aspx">MarcosShop <span>Administração</span></a>
            <nav aria-label="Navegação administrativa">
                <a href="perfilAdmin.aspx">Painel</a>
                <a href="gerenciarProdutos.aspx">Produtos</a>
                <a class="selecionado" href="gerenciarUsuarios.aspx" aria-current="page">Usuários</a>
                <a href="relatorios.aspx">Relatórios</a>
            </nav>
        </header>

        <main class="conteudo">
            <section class="titulo-pagina">
                <p class="sobretitulo">Painel administrativo</p>
                <h1>Gerenciar usuários</h1>
                <p>Consulte, cadastre, edite ou remova contas de clientes e administradores.</p>
            </section>

            <asp:Label ID="lblMensagem" runat="server" CssClass="mensagem erro" Visible="false" />

            <section class="formulario-usuario" aria-labelledby="formulario-titulo">
                <div class="titulo-formulario">
                    <div>
                        <p class="sobretitulo">Contas do sistema</p>
                        <h2 id="formulario-titulo"><asp:Label ID="lblModo" runat="server" Text="Adicionar usuário" /></h2>
                    </div>
                    <asp:HiddenField ID="hfIdUsuario" runat="server" />
                    <asp:HiddenField ID="hfTipoOriginal" runat="server" />
                </div>

                <div class="campos-formulario">
                    <div class="campo">
                        <asp:Label ID="lblTipo" runat="server" AssociatedControlID="ddlTipo" Text="Tipo de conta" />
                        <asp:DropDownList ID="ddlTipo" runat="server" CssClass="entrada" AutoPostBack="true" OnSelectedIndexChanged="ddlTipo_SelectedIndexChanged">
                            <asp:ListItem Text="Cliente" Value="Cliente" />
                            <asp:ListItem Text="Administrador" Value="Admin" />
                        </asp:DropDownList>
                    </div>
                    <div class="campo">
                        <asp:Label ID="lblNome" runat="server" AssociatedControlID="txtNome" Text="Nome" />
                        <asp:TextBox ID="txtNome" runat="server" CssClass="entrada" MaxLength="100" placeholder="Nome completo" />
                    </div>
                    <div class="campo">
                        <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" Text="E-mail" />
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="entrada" MaxLength="100" TextMode="Email" placeholder="nome@exemplo.com" />
                    </div>
                    <div class="campo">
                        <asp:Label ID="lblTelefone" runat="server" AssociatedControlID="txtTelefone" Text="Telefone" />
                        <asp:TextBox ID="txtTelefone" runat="server" CssClass="entrada" MaxLength="20" placeholder="Opcional" />
                    </div>
                    <asp:Panel ID="pnlEndereco" runat="server" CssClass="campo campo-endereco">
                        <asp:Label ID="lblEndereco" runat="server" AssociatedControlID="txtEndereco" Text="Endereço" />
                        <asp:TextBox ID="txtEndereco" runat="server" CssClass="entrada" MaxLength="200" placeholder="Opcional" />
                    </asp:Panel>
                    <div class="campo">
                        <asp:Label ID="lblSenha" runat="server" AssociatedControlID="txtSenha" Text="Senha" />
                        <asp:TextBox ID="txtSenha" runat="server" CssClass="entrada" TextMode="Password" MaxLength="128" autocomplete="new-password" placeholder="Mínimo de 8 caracteres" />
                    </div>
                </div>
                <asp:Label ID="lblAjudaSenha" runat="server" CssClass="ajuda-senha" Text="Ao editar, deixe a senha vazia para manter a senha atual." Visible="false" />
                <div class="acoes-formulario">
                    <asp:Button ID="btnSalvar" runat="server" Text="Adicionar usuário" CssClass="botao-primario" CausesValidation="false" OnClick="btnSalvar_Click" />
                    <asp:Button ID="btnCancelar" runat="server" Text="Cancelar edição" CssClass="botao-secundario" CausesValidation="false" Visible="false" OnClick="btnCancelar_Click" />
                </div>
            </section>

            <section class="secao-lista" aria-labelledby="lista-titulo">
                <div class="titulo-lista">
                    <div>
                        <p class="sobretitulo">Contas cadastradas</p>
                        <h2 id="lista-titulo">Usuários do sistema</h2>
                    </div>
                    <asp:Label ID="lblContagem" runat="server" CssClass="contagem" />
                </div>
                <asp:Repeater ID="rptUsuarios" runat="server" OnItemCommand="rptUsuarios_ItemCommand">
                    <HeaderTemplate><div class="lista-usuarios"></HeaderTemplate>
                    <ItemTemplate>
                        <article class="usuario-admin">
                            <div class="dados-usuario">
                                <span class='tipo-conta <%# Eval("tipo") %>'><%#: Eval("tipoExibicao") %></span>
                                <h3><%#: Eval("nome") %></h3>
                                <span class="detalhe-usuario"><%#: Eval("email") %> · <%#: Eval("telefoneExibicao") %></span>
                            </div>
                            <div class="acoes-usuario">
                                <asp:Button ID="btnEditar" runat="server" Text="Editar" CssClass="botao-editar" CommandName="EditarUsuario" CommandArgument='<%# Eval("tipo") + "|" + Eval("idUsuario") %>' CausesValidation="false" />
                                <asp:Button ID="btnExcluir" runat="server" Text="Excluir" CssClass="botao-excluir" CommandName="ExcluirUsuario" CommandArgument='<%# Eval("tipo") + "|" + Eval("idUsuario") %>' CausesValidation="false" OnClientClick="return confirm('Tem certeza de que deseja excluir esta conta? Se for um cliente, os pedidos associados também serão removidos.');" />
                            </div>
                        </article>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlSemUsuarios" runat="server" CssClass="sem-usuarios" Visible="false">
                    <h3>Nenhuma conta encontrada</h3>
                    <p>Use o formulário acima para cadastrar um usuário.</p>
                </asp:Panel>
            </section>
        </main>

        <footer class="rodape">
            <p>© 2026 MarcosShop - Área de administração.</p>
        </footer>
    </form>
</body>
</html>
