<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="PrjMercado.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Entrar | MarcosShop</title>
    <link rel="stylesheet" type="text/css" href="css/login.css" />
</head>
<body>
    <form id="form1" runat="server">
        <main class="login-page">
            <section class="login-card" aria-labelledby="tituloLogin">
                <a class="voltar" href="index.aspx">← Voltar à loja</a>
                <div class="marca">Marcos<span>Shop</span></div>
                <h1 id="tituloLogin">Acesse sua conta</h1>
                <p class="subtitulo">Entre para continuar suas compras.</p>

                <asp:Label ID="lblMensagem" runat="server" CssClass="mensagem" EnableViewState="false" />

                <div class="campo">
                    <asp:Label ID="lblTipoConta" runat="server" AssociatedControlID="ddlTipoConta" Text="Tipo de conta" />
                    <asp:DropDownList ID="ddlTipoConta" runat="server" CssClass="entrada">
                        <asp:ListItem Text="Cliente" Value="Cliente" />
                        <asp:ListItem Text="Administrador" Value="Admin" />
                    </asp:DropDownList>
                </div>
                <div class="campo">
                    <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" Text="E-mail" />
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="entrada" TextMode="Email" MaxLength="100" autocomplete="username" />
                </div>
                <div class="campo">
                    <asp:Label ID="lblSenha" runat="server" AssociatedControlID="txtSenha" Text="Senha" />
                    <asp:TextBox ID="txtSenha" runat="server" CssClass="entrada" TextMode="Password" MaxLength="255" autocomplete="current-password" />
                </div>
                <asp:Button ID="btnEntrar" runat="server" Text="Entrar" CssClass="botao-entrar" OnClick="btnEntrar_Click" />
                <p class="nota">Use o e-mail e a senha cadastrados na sua conta.</p>
                <p class="criar-conta">Ainda não tem conta? <a href="cadastro.aspx">Cadastre-se</a></p>
            </section>
        </main>
    </form>
</body>
</html>
