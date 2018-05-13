INSERT INTO companies ([Id]      ,[CVR]     ,[Name]      ,[Address]      ,PostCode
      ,[City]      ,[Phone]      ,[Web]      ,[Email]      ,[PersonName]      ,[Chairman]
      ,[IndustryCode]      ,[IndustryText]      ,[AdvertisingProtection]      ,[Owner], connections)  
SELECT [Id]      ,[CVR]      ,[CompanyName] as [Name]      ,[Address]      ,[ZipCode] as PostCode
      ,[City]      ,[Phone]      ,[Web]      ,[Email]      ,[Name] as [PersonName]      ,[Chairman]
      ,[IndustryCode]      ,[IndustryText]      ,
	  
	  case when [AdvertisingProtection]='true' then 1 else 0 end      ,
	  
	  [Owner], 0 as Connections
  FROM [dbo].companies2

  delete from companies

  select count(*) from companies2