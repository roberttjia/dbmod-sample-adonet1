USE [adonet1]
GO
/****** Object:  Table [dbo].[tbl_customers]    Script Date: 8/19/2025 5:43:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_customers](
	[cust_id] [int] IDENTITY(1,1) NOT NULL,
	[first_nm] [nvarchar](50) NOT NULL,
	[last_nm] [nvarchar](50) NOT NULL,
	[phone] [nvarchar](25) NOT NULL,
 CONSTRAINT [PK_tbl_customers] PRIMARY KEY CLUSTERED 
(
	[cust_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_products]    Script Date: 8/19/2025 5:43:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_products](
	[product_id] [int] IDENTITY(1,1) NOT NULL,
	[code] [nvarchar](20) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](200) NOT NULL,
	[price] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_tbl_products] PRIMARY KEY CLUSTERED 
(
	[product_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[proc_DeleteCustomerById]    Script Date: 8/19/2025 5:43:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[proc_DeleteCustomerById] 
	@Cust_Id INT,
	@Deleted_Count INT OUT
AS
BEGIN
	SET NOCOUNT ON;
    DELETE FROM tbl_customers WHERE cust_id = @Cust_Id
    SET @Deleted_Count = @@ROWCOUNT
END
GO
/****** Object:  StoredProcedure [dbo].[proc_SearchCustomersByLastName]    Script Date: 8/19/2025 5:43:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[proc_SearchCustomersByLastName]
    @LastNameSearch nvarchar(50),
    @MaxRows int
AS
BEGIN
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT TOP(@MaxRows) * FROM tbl_customers
    WHERE last_nm LIKE @LastNameSearch
    ORDER BY last_nm DESC
END
GO
