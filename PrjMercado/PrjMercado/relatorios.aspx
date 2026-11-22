<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="relatorios.aspx.cs" Inherits="PrjMercado.relatorios" Culture="pt-BR" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Relatórios | MarcosShop</title>
    <link rel="stylesheet" type="text/css" href="css/relatorios.css" />
</head>
<body>
    <form id="form1" runat="server">
        <header class="cabecalho">
            <a class="marca" href="index.aspx">MarcosShop <span>Administração</span></a>
            <nav aria-label="Navegação administrativa">
                <a href="perfilAdmin.aspx">Painel</a>
                <a href="gerenciarProdutos.aspx">Produtos</a>
                <a href="gerenciarUsuarios.aspx">Usuários</a>
                <a class="selecionado" href="relatorios.aspx" aria-current="page">Relatórios</a>
            </nav>
        </header>

        <main class="conteudo">
            <section class="titulo-pagina">
                <p class="sobretitulo">Painel administrativo</p>
                <h1>Relatórios de vendas</h1>
                <p>Consulte os pedidos realizados e os valores arrecadados.</p>
            </section>

            <section class="filtro-periodo" aria-label="Filtrar período do relatório">
                <div class="campo-periodo">
                    <asp:Label ID="lblDataInicio" runat="server" AssociatedControlID="txtDataInicio" Text="De" />
                    <asp:TextBox ID="txtDataInicio" runat="server" CssClass="entrada-data" TextMode="Date" />
                </div>
                <div class="campo-periodo">
                    <asp:Label ID="lblDataFim" runat="server" AssociatedControlID="txtDataFim" Text="Até" />
                    <asp:TextBox ID="txtDataFim" runat="server" CssClass="entrada-data" TextMode="Date" />
                </div>
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="botao-filtrar" CausesValidation="false" OnClick="btnFiltrar_Click" />
                <asp:Button ID="btnLimpar" runat="server" Text="Limpar período" CssClass="botao-limpar" CausesValidation="false" OnClick="btnLimpar_Click" />
            </section>

            <asp:Label ID="lblMensagem" runat="server" CssClass="mensagem" Visible="false" />

            <section class="indicadores" aria-label="Resumo de vendas">
                <article class="indicador">
                    <span>Pedidos no período</span>
                    <strong><asp:Label ID="lblTotalPedidos" runat="server" Text="0" /></strong>
                </article>
                <article class="indicador destaque">
                    <span>Valor arrecadado</span>
                    <strong><asp:Label ID="lblArrecadado" runat="server" Text="R$ 0,00" /></strong>
                    <small>Considera pedidos concluídos</small>
                </article>
                <article class="indicador">
                    <span>Pedidos em andamento</span>
                    <strong><asp:Label ID="lblEmAndamento" runat="server" Text="0" /></strong>
                </article>
                <article class="indicador">
                    <span>Ticket médio concluído</span>
                    <strong><asp:Label ID="lblTicketMedio" runat="server" Text="R$ 0,00" /></strong>
                </article>
            </section>

            <section class="secao-relatorio" aria-labelledby="status-titulo">
                <div class="titulo-secao">
                    <div>
                        <p class="sobretitulo">Acompanhamento</p>
                        <h2 id="status-titulo">Pedidos por status</h2>
                        <p class="legenda-secao">A soma ao lado representa os valores dos pedidos em cada status; o total arrecadado considera somente pedidos concluídos.</p>
                    </div>
                </div>
                <asp:Repeater ID="rptStatus" runat="server">
                    <HeaderTemplate><div class="lista-status"></HeaderTemplate>
                    <ItemTemplate>
                        <article class="linha-status">
                            <span class="nome-status"><%#: Eval("status") %></span>
                            <span><strong><%# Eval("quantidade") %></strong> pedido(s)</span>
                            <strong class="valor-status"><%# Eval("valor", "{0:C}") %></strong>
                        </article>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlSemStatus" runat="server" CssClass="estado-vazio" Visible="false">
                    Nenhum pedido foi encontrado neste período.
                </asp:Panel>
            </section>

            <section class="secao-relatorio" aria-labelledby="pedidos-titulo">
                <div class="titulo-secao">
                    <div>
                        <p class="sobretitulo">Detalhamento</p>
                        <h2 id="pedidos-titulo">Pedidos realizados</h2>
                    </div>
                    <asp:Label ID="lblContagemPedidos" runat="server" CssClass="contagem" />
                </div>
                <asp:Repeater ID="rptPedidos" runat="server" OnItemCommand="rptPedidos_ItemCommand">
                    <HeaderTemplate>
                        <div class="tabela-scroll"><table class="tabela-pedidos">
                            <thead><tr><th>Pedido</th><th>Data</th><th>Cliente</th><th>Itens</th><th>Pagamento</th><th>Status</th><th>Total</th><th>Ações</th></tr></thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>#<%#: Eval("idPedido") %></td>
                            <td><%# Eval("dataPedido", "{0:dd/MM/yyyy HH:mm}") %></td>
                            <td><%#: Eval("cliente") %></td>
                            <td><%# Eval("quantidadeItens") %></td>
                            <td><%#: Eval("formaPagamento") %></td>
                            <td><span class="status-badge"><%#: Eval("status") %></span></td>
                            <td class="valor-pedido"><%# Eval("valorTotal", "{0:C}") %></td>
                            <td class="acoes-pedido">
                                <asp:LinkButton ID="btnAprovar" runat="server" Text="Aprovar" CssClass="botao-acao aprovar"
                                    CommandName="Aprovar" CommandArgument='<%# Eval("idPedido") %>' CausesValidation="false"
                                    Visible='<%# Convert.ToString(Eval("status")) == "Pendente" %>' />
                                <asp:LinkButton ID="btnCancelar" runat="server" Text="Cancelar" CssClass="botao-acao cancelar"
                                    CommandName="Cancelar" CommandArgument='<%# Eval("idPedido") %>' CausesValidation="false"
                                    Visible='<%# Convert.ToString(Eval("status")) == "Pendente" %>' />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate></tbody></table></div></FooterTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlSemPedidos" runat="server" CssClass="estado-vazio" Visible="false">
                    Nenhum pedido foi encontrado neste período.
                </asp:Panel>
            </section>
        </main>

        <footer class="rodape">
            <p>© 2026 MarcosShop - Área de administração.</p>
        </footer>
    </form>
</body>
</html>
