USE [adonet1]
GO
/****** Object:  Schema [reporting]    Script Date: 8/20/2025 4:38:00 PM ******/
CREATE SCHEMA [reporting]
GO
/****** Object:  Table [dbo].[tbl_customers]    Script Date: 8/20/2025 4:38:00 PM ******/
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
/****** Object:  Table [dbo].[tbl_orders]    Script Date: 8/20/2025 4:38:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_orders](
	[order_id] [int] IDENTITY(1,1) NOT NULL,
	[cust_id] [int] NOT NULL,
	[product_id] [int] NOT NULL,
	[quantity] [int] NOT NULL,
	[total_price] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_tbl_orders] PRIMARY KEY CLUSTERED 
(
	[order_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_products]    Script Date: 8/20/2025 4:38:00 PM ******/
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
ALTER TABLE [dbo].[tbl_orders]  WITH CHECK ADD  CONSTRAINT [FK_tbl_orders_tbl_customers] FOREIGN KEY([cust_id])
REFERENCES [dbo].[tbl_customers] ([cust_id])
GO
ALTER TABLE [dbo].[tbl_orders] CHECK CONSTRAINT [FK_tbl_orders_tbl_customers]
GO
ALTER TABLE [dbo].[tbl_orders]  WITH CHECK ADD  CONSTRAINT [FK_tbl_orders_tbl_products] FOREIGN KEY([product_id])
REFERENCES [dbo].[tbl_products] ([product_id])
GO
ALTER TABLE [dbo].[tbl_orders] CHECK CONSTRAINT [FK_tbl_orders_tbl_products]
GO
/****** Object:  StoredProcedure [dbo].[proc_DeleteCustomerById]    Script Date: 8/20/2025 4:38:00 PM ******/
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
/****** Object:  StoredProcedure [dbo].[proc_SearchCustomersByLastName]    Script Date: 8/20/2025 4:38:00 PM ******/
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
/****** Object:  StoredProcedure [reporting].[proc_ArchiveOrders]    Script Date: 8/20/2025 4:38:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [reporting].[proc_ArchiveOrders] 
@CutoffDate as datetime,
@ArchivedCount as int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Do nothing
	SET @ArchivedCount = 0
END
GO
