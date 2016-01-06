
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/05/2015 14:10:55
-- Generated from EDMX file: C:\Users\shaneb\Dropbox\GitHub\Label Printing with Bartender\Label Printing with Bartender\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [TestModel];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Entity1'
CREATE TABLE [dbo].[Entity1] (
    [E1ID] int IDENTITY(1,1) NOT NULL,
    [Property1] nvarchar(max)  NOT NULL,
    [Property2] nvarchar(max)  NOT NULL,
    [Property3] nvarchar(max)  NOT NULL,
    [Entity2_E2ID] int  NOT NULL
);
GO

-- Creating table 'Entity2'
CREATE TABLE [dbo].[Entity2] (
    [E2ID] int IDENTITY(1,1) NOT NULL,
    [Property1] nvarchar(max)  NOT NULL,
    [Property2] nvarchar(max)  NOT NULL,
    [Property3] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Entity3'
CREATE TABLE [dbo].[Entity3] (
    [E3ID] int IDENTITY(1,1) NOT NULL,
    [Property1] nvarchar(max)  NOT NULL,
    [Property2] nvarchar(max)  NOT NULL,
    [Property3] nvarchar(max)  NOT NULL,
    [Entity2_E2ID] int  NOT NULL
);
GO

-- Creating table 'Entity1Entity3'
CREATE TABLE [dbo].[Entity1Entity3] (
    [Entity1_E1ID] int  NOT NULL,
    [Entity3_E3ID] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [E1ID] in table 'Entity1'
ALTER TABLE [dbo].[Entity1]
ADD CONSTRAINT [PK_Entity1]
    PRIMARY KEY CLUSTERED ([E1ID] ASC);
GO

-- Creating primary key on [E2ID] in table 'Entity2'
ALTER TABLE [dbo].[Entity2]
ADD CONSTRAINT [PK_Entity2]
    PRIMARY KEY CLUSTERED ([E2ID] ASC);
GO

-- Creating primary key on [E3ID] in table 'Entity3'
ALTER TABLE [dbo].[Entity3]
ADD CONSTRAINT [PK_Entity3]
    PRIMARY KEY CLUSTERED ([E3ID] ASC);
GO

-- Creating primary key on [Entity1_E1ID], [Entity3_E3ID] in table 'Entity1Entity3'
ALTER TABLE [dbo].[Entity1Entity3]
ADD CONSTRAINT [PK_Entity1Entity3]
    PRIMARY KEY CLUSTERED ([Entity1_E1ID], [Entity3_E3ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Entity2_E2ID] in table 'Entity1'
ALTER TABLE [dbo].[Entity1]
ADD CONSTRAINT [FK_Entity1Entity2]
    FOREIGN KEY ([Entity2_E2ID])
    REFERENCES [dbo].[Entity2]
        ([E2ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Entity1Entity2'
CREATE INDEX [IX_FK_Entity1Entity2]
ON [dbo].[Entity1]
    ([Entity2_E2ID]);
GO

-- Creating foreign key on [Entity2_E2ID] in table 'Entity3'
ALTER TABLE [dbo].[Entity3]
ADD CONSTRAINT [FK_Entity3Entity2]
    FOREIGN KEY ([Entity2_E2ID])
    REFERENCES [dbo].[Entity2]
        ([E2ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Entity3Entity2'
CREATE INDEX [IX_FK_Entity3Entity2]
ON [dbo].[Entity3]
    ([Entity2_E2ID]);
GO

-- Creating foreign key on [Entity1_E1ID] in table 'Entity1Entity3'
ALTER TABLE [dbo].[Entity1Entity3]
ADD CONSTRAINT [FK_Entity1Entity3_Entity1]
    FOREIGN KEY ([Entity1_E1ID])
    REFERENCES [dbo].[Entity1]
        ([E1ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Entity3_E3ID] in table 'Entity1Entity3'
ALTER TABLE [dbo].[Entity1Entity3]
ADD CONSTRAINT [FK_Entity1Entity3_Entity3]
    FOREIGN KEY ([Entity3_E3ID])
    REFERENCES [dbo].[Entity3]
        ([E3ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Entity1Entity3_Entity3'
CREATE INDEX [IX_FK_Entity1Entity3_Entity3]
ON [dbo].[Entity1Entity3]
    ([Entity3_E3ID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------