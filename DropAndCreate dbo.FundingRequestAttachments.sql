USE [Finance]
GO

/****** Object: Table [dbo].[FundingRequestAttachments] Script Date: 2/12/2019 2:09:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[FundingRequestAttachments];


GO
CREATE TABLE [dbo].[FundingRequestAttachments] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [CapFundingRequestId] INT            NULL,
    [QuotesId] INT            NULL,
    [FileName]            NVARCHAR (100) NULL,
    [FileLocation]        NVARCHAR (250) NULL
);


