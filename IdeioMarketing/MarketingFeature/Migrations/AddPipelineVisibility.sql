IF COL_LENGTH(N'dbo.MarketingLeads', N'IsInPipeline') IS NULL
BEGIN
    ALTER TABLE [dbo].[MarketingLeads]
        ADD [IsInPipeline] bit NOT NULL
            CONSTRAINT [DF_MarketingLeads_IsInPipeline] DEFAULT (1);
END;
