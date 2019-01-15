﻿CREATE TABLE [fresh].[chat] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Message]     NVARCHAR (MAX) NOT NULL,
    [GiverId]     NVARCHAR (450) NOT NULL,
    [ReceiverId]  NVARCHAR (450) NOT NULL,
    [ProductId]   INT            NOT NULL,
    [PublishDate] DATETIME2 (7)  NOT NULL,
    [IsServer]    BIT            NOT NULL,
	IsDeleted bit NOT NULL,
	SentById nvarchar(450) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([GiverId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    FOREIGN KEY ([ReceiverId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
	FOREIGN KEY ([SentById]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    FOREIGN KEY ([ProductId]) REFERENCES [fresh].[product] ([Id]),
);