<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cadastro.aspx.cs" Inherits="PrjMercado.Cadastro" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Criar conta | MarcosShop</title>
    <link rel="stylesheet" type="text/css" href="css/cadastro.css" />
</head>
<body>
    <form id="form1" runat="server">
        <main class="cadastro-page">
            <section class="cadastro-card" aria-labelledby="tituloCadastro">
                <a class="voltar" href="login.aspx">← Já tenho conta</a>
                <div class="marca">Marcos<span>Shop</span></div>
                <h1 id="tituloCadastro">Criar sua conta</h1>
                <p class="subtitulo">Preencha seus dados para começar a comprar.</p>

                <asp:Label ID="lblMensagem" runat="server" CssClass="mensagem" EnableViewState="false" />

                <div class="campo">
                    <asp:Label ID="lblNome" runat="server" AssociatedControlID="txtNome" Text="Nome completo" />
                    <asp:TextBox ID="txtNome" runat="server" CssClass="entrada" MaxLength="100" autocomplete="name" />
                    <asp:RequiredFieldValidator ID="valNome" runat="server" ControlToValidate="txtNome" ErrorMessage="Informe seu nome." CssClass="erro-validacao" Display="Dynamic" />
                </div>
                <div class="campo">
                    <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" Text="E-mail" />
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="entrada" TextMode="Email" MaxLength="100" autocomplete="email" />
                    <asp:RequiredFieldValidator ID="valEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Informe seu e-mail." CssClass="erro-validacao" Display="Dynamic" />
                </div>
                <div class="campo">
                    <asp:Label ID="lblTelefone" runat="server" AssociatedControlID="txtTelefone" Text="Telefone (opcional)" />
                    <asp:TextBox ID="txtTelefone" runat="server" CssClass="entrada" MaxLength="20" autocomplete="tel" />
                </div>
                <div class="campo">
                    <asp:Label ID="lblEndereco" runat="server" AssociatedControlID="txtEndereco" Text="Endereço (opcional)" />
                    <asp:TextBox ID="txtEndereco" runat="server" CssClass="entrada" MaxLength="200" autocomplete="street-address" />
                </div>
                <div class="campo">
                    <asp:Label ID="lblSenha" runat="server" AssociatedControlID="txtSenha" Text="Senha" />
                    <asp:TextBox ID="txtSenha" runat="server" CssClass="entrada" TextMode="Password" MaxLength="128" autocomplete="new-password" />
                    <asp:RequiredFieldValidator ID="valSenha" runat="server" ControlToValidate="txtSenha" ErrorMessage="Informe uma senha." CssClass="erro-validacao" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="valTamanhoSenha" runat="server" ControlToValidate="txtSenha" ValidationExpression="^.{8,128}$" ErrorMessage="A senha deve ter entre 8 e 128 caracteres." CssClass="erro-validacao" Display="Dynamic" />
                </div>
                <div class="campo">
                    <asp:Label ID="lblConfirmarSenha" runat="server" AssociatedControlID="txtConfirmarSenha" Text="Confirme a senha" />
                    <asp:TextBox ID="txtConfirmarSenha" runat="server" CssClass="entrada" TextMode="Password" MaxLength="128" autocomplete="new-password" />
                    <asp:RequiredFieldValidator ID="valConfirmarSenha" runat="server" ControlToValidate="txtConfirmarSenha" ErrorMessage="Confirme sua senha." CssClass="erro-validacao" Display="Dynamic" />
                    <asp:CompareValidator ID="valCompararSenha" runat="server" ControlToValidate="txtConfirmarSenha" ControlToCompare="txtSenha" ErrorMessage="As senhas não coincidem." CssClass="erro-validacao" Display="Dynamic" />
                </div>
                <asp:Button ID="btnCadastrar" runat="server" Text="Criar conta" CssClass="botao-cadastrar" OnClick="btnCadastrar_Click" />
                <p class="nota">Seus dados serão usados para identificar seus pedidos.</p>
            </section>
        </main>
    </form>
</body>
</html>
