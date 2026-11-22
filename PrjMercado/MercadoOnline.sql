-- =====================================================
-- PROJETO: E-COMMERCE DE MERCADO
-- BANCO DE DADOS: MercadoOnline
-- SQL Server
--
-- Script seguro para execucao repetida: nao apaga o banco,
-- evita recriar tabelas e nao duplica categorias/produtos/admin.
-- =====================================================

IF DB_ID(N'MercadoOnline') IS NULL
BEGIN
    CREATE DATABASE MercadoOnline;
END;
GO

USE MercadoOnline;
GO

-- CLIENTES
IF OBJECT_ID(N'dbo.Cliente', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cliente (
        idCliente INT IDENTITY(1,1) PRIMARY KEY,
        nome VARCHAR(100) NOT NULL,
        email VARCHAR(100) NOT NULL UNIQUE,
        senhaHash VARCHAR(255) NOT NULL,
        telefone VARCHAR(20),
        endereco VARCHAR(200)
    );
END;
GO

-- ADMINISTRADORES
IF OBJECT_ID(N'dbo.Admin', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Admin (
        idAdmin INT IDENTITY(1,1) PRIMARY KEY,
        nome VARCHAR(100) NOT NULL,
        email VARCHAR(100) NOT NULL UNIQUE,
        senhaHash VARCHAR(255) NOT NULL,
        telefone VARCHAR(20)
    );
END;
GO

-- CATEGORIAS
IF OBJECT_ID(N'dbo.Categoria', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categoria (
        idCategoria INT IDENTITY(1,1) PRIMARY KEY,
        descricao VARCHAR(100) NOT NULL UNIQUE
    );
END;
GO

-- PRODUTOS
IF OBJECT_ID(N'dbo.Produto', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Produto (
        idProduto INT IDENTITY(1,1) PRIMARY KEY,
        descricao VARCHAR(100) NOT NULL,
        preco DECIMAL(10,2) NOT NULL,
        estoque INT NOT NULL,
        idCategoria INT NOT NULL,
        imagem VARBINARY(MAX) NULL,
        tipoImagem VARCHAR(100) NULL,
        CONSTRAINT FK_Produto_Categoria FOREIGN KEY (idCategoria)
            REFERENCES dbo.Categoria(idCategoria),
        CONSTRAINT CK_Produto_Preco CHECK (preco >= 0),
        CONSTRAINT CK_Produto_Estoque CHECK (estoque >= 0)
    );
END;
GO

-- SUPORTE A IMAGENS DOS PRODUTOS
-- Adiciona as colunas tambem em bancos MercadoOnline ja existentes.
IF COL_LENGTH(N'dbo.Produto', N'imagem') IS NULL
    ALTER TABLE dbo.Produto ADD imagem VARBINARY(MAX) NULL;
IF COL_LENGTH(N'dbo.Produto', N'tipoImagem') IS NULL
    ALTER TABLE dbo.Produto ADD tipoImagem VARCHAR(100) NULL;
GO

-- PEDIDOS
IF OBJECT_ID(N'dbo.Pedido', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Pedido (
        idPedido INT IDENTITY(1,1) PRIMARY KEY,
        idCliente INT NOT NULL,
        dataPedido DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        valorTotal DECIMAL(10,2) NOT NULL,
        status VARCHAR(30) NOT NULL DEFAULT 'Pendente',
        formaPagamento VARCHAR(30) NOT NULL,
        CONSTRAINT FK_Pedido_Cliente FOREIGN KEY (idCliente)
            REFERENCES dbo.Cliente(idCliente),
        CONSTRAINT CK_Pedido_Valor CHECK (valorTotal >= 0),
        CONSTRAINT CK_Pedido_Status CHECK (status IN (
            'Pendente', 'Confirmado', 'Em preparo', 'Enviado', 'Concluido', 'Cancelado'
        )),
        CONSTRAINT CK_Pedido_Pagamento CHECK (formaPagamento IN (
            'Pix', 'Dinheiro', 'Cartao'
        ))
    );
END;
GO

-- ITENS DOS PEDIDOS
IF OBJECT_ID(N'dbo.Produto_Pedido', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Produto_Pedido (
        idProdutoPedido INT IDENTITY(1,1) PRIMARY KEY,
        idPedido INT NOT NULL,
        idProduto INT NOT NULL,
        quantidade INT NOT NULL,
        precoUnitario DECIMAL(10,2) NOT NULL,
        CONSTRAINT FK_ProdutoPedido_Pedido FOREIGN KEY (idPedido)
            REFERENCES dbo.Pedido(idPedido),
        CONSTRAINT FK_ProdutoPedido_Produto FOREIGN KEY (idProduto)
            REFERENCES dbo.Produto(idProduto),
        CONSTRAINT CK_ProdutoPedido_Quantidade CHECK (quantidade > 0),
        CONSTRAINT CK_ProdutoPedido_Preco CHECK (precoUnitario >= 0)
    );
END;
GO

-- INDICES
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Produto_Categoria' AND object_id = OBJECT_ID(N'dbo.Produto'))
    CREATE INDEX IX_Produto_Categoria ON dbo.Produto(idCategoria);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Pedido_Cliente' AND object_id = OBJECT_ID(N'dbo.Pedido'))
    CREATE INDEX IX_Pedido_Cliente ON dbo.Pedido(idCliente);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ProdutoPedido_Pedido' AND object_id = OBJECT_ID(N'dbo.Produto_Pedido'))
    CREATE INDEX IX_ProdutoPedido_Pedido ON dbo.Produto_Pedido(idPedido);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ProdutoPedido_Produto' AND object_id = OBJECT_ID(N'dbo.Produto_Pedido'))
    CREATE INDEX IX_ProdutoPedido_Produto ON dbo.Produto_Pedido(idProduto);
GO

-- ADMINISTRADOR INICIAL
-- Login: admin@gmail.com | Senha inicial: adminsenha
-- Hash no mesmo formato PBKDF2/120000 usado pelo login ASP.NET.
IF NOT EXISTS (SELECT 1 FROM dbo.Admin WHERE email = 'admin@gmail.com')
BEGIN
    INSERT INTO dbo.Admin (nome, email, senhaHash, telefone)
    VALUES (
        'admin_user',
        'admin@gmail.com',
        'PBKDF2$120000$96280c3fa701b4a8789179c5b56a0123$a820640610877dbb390da95aaa2c3635ff87d9686159529461bc4584e56cf6b6',
        '11912349123'
    );
END;
GO

-- CATEGORIAS INICIAIS
INSERT INTO dbo.Categoria (descricao)
SELECT novas.descricao
FROM (VALUES
    ('Frutas'), ('Verduras'), ('Bebidas'), ('Laticinios'),
    ('Mercearia'), ('Limpeza'), ('Padaria')
) AS novas(descricao)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.Categoria existente WHERE existente.descricao = novas.descricao
);
GO

-- PRODUTOS INICIAIS
INSERT INTO dbo.Produto (descricao, preco, estoque, idCategoria)
SELECT novos.descricao, novos.preco, novos.estoque, categoria.idCategoria
FROM (VALUES
    ('Banana', CAST(5.99 AS DECIMAL(10,2)), 50, 'Frutas'),
    ('Maca', 8.50, 40, 'Frutas'),
    ('Laranja', 4.99, 60, 'Frutas'),
    ('Uva', 9.99, 30, 'Frutas'),
    ('Alface', 3.99, 30, 'Verduras'),
    ('Tomate', 6.99, 45, 'Verduras'),
    ('Cenoura', 4.50, 35, 'Verduras'),
    ('Refrigerante 2L', 9.99, 60, 'Bebidas'),
    ('Suco de Laranja 1L', 7.50, 40, 'Bebidas'),
    ('Agua Mineral 1,5L', 3.50, 80, 'Bebidas'),
    ('Leite Integral 1L', 5.49, 80, 'Laticinios'),
    ('Queijo Mussarela 500g', 22.90, 20, 'Laticinios'),
    ('Iogurte 170g', 3.99, 50, 'Laticinios'),
    ('Arroz 5kg', 28.99, 35, 'Mercearia'),
    ('Feijao 1kg', 8.99, 50, 'Mercearia'),
    ('Macarrao 500g', 5.49, 45, 'Mercearia'),
    ('Acucar 1kg', 4.99, 40, 'Mercearia'),
    ('Detergente', 2.99, 70, 'Limpeza'),
    ('Sabao em Po 1kg', 12.99, 30, 'Limpeza'),
    ('Desinfetante 1L', 6.50, 40, 'Limpeza'),
    ('Pao de Forma', 7.50, 25, 'Padaria'),
    ('Pao Frances (kg)', 14.99, 20, 'Padaria')
) AS novos(descricao, preco, estoque, categoria)
INNER JOIN dbo.Categoria categoria ON categoria.descricao = novos.categoria
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.Produto existente
    WHERE existente.descricao = novos.descricao
      AND existente.idCategoria = categoria.idCategoria
);
GO

-- CONSULTAS DE CONFERENCIA
SELECT idAdmin, nome, email, telefone FROM dbo.Admin;
SELECT * FROM dbo.Categoria ORDER BY descricao;
SELECT P.idProduto, P.descricao AS Produto, P.preco, P.estoque, C.descricao AS Categoria
FROM dbo.Produto P
INNER JOIN dbo.Categoria C ON C.idCategoria = P.idCategoria
ORDER BY C.descricao, P.descricao;
