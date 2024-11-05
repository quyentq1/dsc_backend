USE [master]
GO
/****** Object:  Database [DSC]    Script Date: 6/11/2024 2:00:54 AM ******/
CREATE DATABASE [DSC]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DSC', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.QUYENTQ\MSSQL\DATA\DSC.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DSC_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.QUYENTQ\MSSQL\DATA\DSC_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [DSC] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DSC].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DSC] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DSC] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DSC] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DSC] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DSC] SET ARITHABORT OFF 
GO
ALTER DATABASE [DSC] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DSC] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DSC] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DSC] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DSC] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DSC] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DSC] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DSC] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DSC] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DSC] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DSC] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DSC] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DSC] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DSC] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DSC] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DSC] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DSC] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DSC] SET RECOVERY FULL 
GO
ALTER DATABASE [DSC] SET  MULTI_USER 
GO
ALTER DATABASE [DSC] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DSC] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DSC] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DSC] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [DSC] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [DSC] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'DSC', N'ON'
GO
ALTER DATABASE [DSC] SET QUERY_STORE = OFF
GO
USE [DSC]
GO
/****** Object:  Table [dbo].[Activities]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Activities](
	[ActivityID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[LevelID] [int] NULL,
	[ActivityName] [nvarchar](255) NULL,
	[StartDate] [datetime] NULL,
	[Location] [nvarchar](255) NULL,
	[NumberOfTeams] [int] NULL,
	[Expense] [decimal](10, 2) NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ActivityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Admin]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admin](
	[AdminID] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [int] NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[Fund] [decimal](18, 0) NULL,
PRIMARY KEY CLUSTERED 
(
	[AdminID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Club]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Club](
	[ClubID] [int] IDENTITY(1,1) NOT NULL,
	[SportID] [int] NULL,
	[LevelID] [int] NULL,
	[ClubName] [nvarchar](255) NULL,
	[Status] [nvarchar](50) NULL,
	[Rules] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[Fund] [decimal](18, 0) NULL,
PRIMARY KEY CLUSTERED 
(
	[ClubID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Comment]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Comment](
	[CommentID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[ActivityID] [int] NULL,
	[TournamentID] [int] NULL,
	[Comment] [nvarchar](max) NULL,
	[Star] [int] NULL,
	[Level] [int] NULL,
	[Image] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[CommentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Fee]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Fee](
	[FeeID] [int] IDENTITY(1,1) NOT NULL,
	[TournamentID] [int] NULL,
	[RequestFee] [decimal](18, 0) NULL,
PRIMARY KEY CLUSTERED 
(
	[FeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Level]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Level](
	[LevelID] [int] IDENTITY(1,1) NOT NULL,
	[LevelName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[LevelID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Match]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Match](
	[MatchID] [int] IDENTITY(1,1) NOT NULL,
	[RoundID] [int] NULL,
	[Team1ID] [int] NULL,
	[Team2ID] [int] NULL,
	[MatchNumber] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[MatchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notification]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notification](
	[NotificationID] [int] IDENTITY(1,1) NOT NULL,
	[ActivityID] [int] NULL,
	[TournamentID] [int] NULL,
	[Content] [nvarchar](max) NULL,
	[UserID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[NotificationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[PaymentID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[ClubID] [int] NULL,
	[TournamentID] [int] NULL,
	[Price] [decimal](18, 0) NULL,
	[PaymentDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[PaymentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[requestJoinActivity]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[requestJoinActivity](
	[requestJoinActivityID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[ActivitiesID] [int] NOT NULL,
	[Status] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[requestJoinActivityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RequestJoinClub]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RequestJoinClub](
	[RequestClubId] [int] IDENTITY(1,1) NOT NULL,
	[User_id] [int] NOT NULL,
	[Club_id] [int] NOT NULL,
	[Status] [varchar](50) NULL,
	[Createdate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[RequestClubId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Result]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Result](
	[ResultID] [int] IDENTITY(1,1) NOT NULL,
	[MatchID] [int] NULL,
	[ScoreTeam1] [int] NULL,
	[ScoreTeam2] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ResultID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResultOfActivities]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResultOfActivities](
	[ResultID] [int] IDENTITY(1,1) NOT NULL,
	[ActivityID] [int] NULL,
	[Team1Score] [int] NULL,
	[Team2Score] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ResultID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Round]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Round](
	[RoundID] [int] IDENTITY(1,1) NOT NULL,
	[TournamentID] [int] NULL,
	[RoundNumber] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[RoundID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sport]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sport](
	[SportID] [int] IDENTITY(1,1) NOT NULL,
	[SportName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[SportID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Team]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Team](
	[TeamID] [int] IDENTITY(1,1) NOT NULL,
	[TournamentID] [int] NULL,
	[UserTeamID] [int] NULL,
	[TeamName] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[TeamID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeamTournament]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeamTournament](
	[TeamTournamentID] [int] IDENTITY(1,1) NOT NULL,
	[TournamentID] [int] NULL,
	[TeamID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[TeamTournamentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tournaments]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tournaments](
	[TournamentID] [int] IDENTITY(1,1) NOT NULL,
	[LevelID] [int] NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Location] [nvarchar](255) NULL,
	[NumberOfTeams] [int] NULL,
	[Status] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[UserID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[TournamentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransferHistory]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransferHistory](
	[TransferID] [int] IDENTITY(1,1) NOT NULL,
	[SenderClubID] [int] NULL,
	[ReceiverClubID] [int] NULL,
	[SenderAdminID] [int] NULL,
	[ReceiverAdminID] [int] NULL,
	[TransferAmount] [decimal](18, 0) NULL,
	[TransferDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[TransferID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [int] NULL,
	[UserTeamID] [int] NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[FullName] [nvarchar](255) NULL,
	[Phone] [nvarchar](20) NULL,
	[Address] [nvarchar](255) NULL,
	[Gender] [nvarchar](10) NULL,
	[Age] [int] NULL,
	[Height] [decimal](5, 2) NULL,
	[Weight] [decimal](5, 2) NULL,
	[Avatar] [nvarchar](255) NULL,
	[Status] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserActivities]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserActivities](
	[UserActivityID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[ActivityID] [int] NULL,
	[JoinDate] [datetime] NULL,
	[RoleInActivity] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserActivityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserClub]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserClub](
	[UserClubID] [int] IDENTITY(1,1) NOT NULL,
	[ClubID] [int] NULL,
	[UserID] [int] NULL,
	[Role] [nvarchar](50) NULL,
	[JoinDate] [datetime] NULL,
	[Status] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserClubID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserSport]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSport](
	[UserSportID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[SportID] [int] NULL,
	[LevelID] [int] NULL,
	[Achievement] [nvarchar](max) NULL,
	[Position] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserSportID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserTeam]    Script Date: 6/11/2024 2:00:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTeam](
	[UserTeamID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[TeamID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserTeamID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Activities] ON 

INSERT [dbo].[Activities] ([ActivityID], [UserID], [LevelID], [ActivityName], [StartDate], [Location], [NumberOfTeams], [Expense], [Description]) VALUES (1, 1, 2, N'Kèo đá bóng', CAST(N'2024-10-12T18:00:00.000' AS DateTime), N'Stadium A', 2, CAST(50000.00 AS Decimal(10, 2)), N'Friendly match')
INSERT [dbo].[Activities] ([ActivityID], [UserID], [LevelID], [ActivityName], [StartDate], [Location], [NumberOfTeams], [Expense], [Description]) VALUES (2, 2, 3, N'Kèo cầu lông', CAST(N'2024-11-11T17:00:00.000' AS DateTime), N'Gym B', 2, CAST(40000.00 AS Decimal(10, 2)), N'Local tournament')
INSERT [dbo].[Activities] ([ActivityID], [UserID], [LevelID], [ActivityName], [StartDate], [Location], [NumberOfTeams], [Expense], [Description]) VALUES (3, 2, 2, N'Kèo bóng bàn', CAST(N'2024-11-11T16:00:00.000' AS DateTime), N'Stadium C', 4, CAST(4000000.00 AS Decimal(10, 2)), N'Local tournament')
INSERT [dbo].[Activities] ([ActivityID], [UserID], [LevelID], [ActivityName], [StartDate], [Location], [NumberOfTeams], [Expense], [Description]) VALUES (4, 2, 3, N'Kèo Bóng Rổ', CAST(N'2024-11-02T18:00:00.000' AS DateTime), N'Khu C', 5, CAST(400000.00 AS Decimal(10, 2)), N'hoge')
SET IDENTITY_INSERT [dbo].[Activities] OFF
GO
SET IDENTITY_INSERT [dbo].[Admin] ON 

INSERT [dbo].[Admin] ([AdminID], [RoleID], [Email], [Password], [Fund]) VALUES (1, 1, N'admin@gmail.com', N'admin123', CAST(100000 AS Decimal(18, 0)))
SET IDENTITY_INSERT [dbo].[Admin] OFF
GO
SET IDENTITY_INSERT [dbo].[Club] ON 

INSERT [dbo].[Club] ([ClubID], [SportID], [LevelID], [ClubName], [Status], [Rules], [CreateDate], [Fund]) VALUES (1, 1, 3, N'CLB Bóng đá Ngũ Hành Sơn', N'Active', NULL, CAST(N'2024-01-15T00:00:00.000' AS DateTime), CAST(50000 AS Decimal(18, 0)))
INSERT [dbo].[Club] ([ClubID], [SportID], [LevelID], [ClubName], [Status], [Rules], [CreateDate], [Fund]) VALUES (2, 2, 2, N'CLB Cầu lông ĐN', N'Active', NULL, CAST(N'2024-04-03T00:00:00.000' AS DateTime), CAST(50000 AS Decimal(18, 0)))
INSERT [dbo].[Club] ([ClubID], [SportID], [LevelID], [ClubName], [Status], [Rules], [CreateDate], [Fund]) VALUES (3, 3, 1, N'CLB Bóng rổ', N'Active', NULL, CAST(N'2024-08-15T00:00:00.000' AS DateTime), CAST(50000 AS Decimal(18, 0)))
SET IDENTITY_INSERT [dbo].[Club] OFF
GO
SET IDENTITY_INSERT [dbo].[Comment] ON 

INSERT [dbo].[Comment] ([CommentID], [UserID], [ActivityID], [TournamentID], [Comment], [Star], [Level], [Image]) VALUES (1, 1, NULL, 2, N'hay!!!!!', 5, NULL, NULL)
INSERT [dbo].[Comment] ([CommentID], [UserID], [ActivityID], [TournamentID], [Comment], [Star], [Level], [Image]) VALUES (2, 2, NULL, 2, N'Giải đấu rất vui', 4, NULL, NULL)
INSERT [dbo].[Comment] ([CommentID], [UserID], [ActivityID], [TournamentID], [Comment], [Star], [Level], [Image]) VALUES (3, 3, 1, NULL, N'Exciting', 5, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Comment] OFF
GO
SET IDENTITY_INSERT [dbo].[Fee] ON 

INSERT [dbo].[Fee] ([FeeID], [TournamentID], [RequestFee]) VALUES (1, 2, CAST(150000 AS Decimal(18, 0)))
INSERT [dbo].[Fee] ([FeeID], [TournamentID], [RequestFee]) VALUES (2, 1, CAST(100000 AS Decimal(18, 0)))
SET IDENTITY_INSERT [dbo].[Fee] OFF
GO
SET IDENTITY_INSERT [dbo].[Level] ON 

INSERT [dbo].[Level] ([LevelID], [LevelName]) VALUES (1, N'Mới biết chơi')
INSERT [dbo].[Level] ([LevelID], [LevelName]) VALUES (2, N'Trung bình - Khá')
INSERT [dbo].[Level] ([LevelID], [LevelName]) VALUES (3, N'Chuyên nghiệp')
SET IDENTITY_INSERT [dbo].[Level] OFF
GO
SET IDENTITY_INSERT [dbo].[Match] ON 

INSERT [dbo].[Match] ([MatchID], [RoundID], [Team1ID], [Team2ID], [MatchNumber]) VALUES (1, 1, 1, 2, 1)
INSERT [dbo].[Match] ([MatchID], [RoundID], [Team1ID], [Team2ID], [MatchNumber]) VALUES (2, 2, 3, 4, 2)
INSERT [dbo].[Match] ([MatchID], [RoundID], [Team1ID], [Team2ID], [MatchNumber]) VALUES (3, 3, 1, 3, 3)
SET IDENTITY_INSERT [dbo].[Match] OFF
GO
SET IDENTITY_INSERT [dbo].[requestJoinActivity] ON 

INSERT [dbo].[requestJoinActivity] ([requestJoinActivityID], [UserID], [ActivitiesID], [Status], [CreateDate]) VALUES (6, 7, 4, N'2', CAST(N'2024-10-25T01:01:47.487' AS DateTime))
INSERT [dbo].[requestJoinActivity] ([requestJoinActivityID], [UserID], [ActivitiesID], [Status], [CreateDate]) VALUES (7, 8, 4, N'2', CAST(N'2024-10-25T01:01:47.487' AS DateTime))
INSERT [dbo].[requestJoinActivity] ([requestJoinActivityID], [UserID], [ActivitiesID], [Status], [CreateDate]) VALUES (10, 9, 4, N'3', CAST(N'2024-11-06T01:36:19.173' AS DateTime))
INSERT [dbo].[requestJoinActivity] ([requestJoinActivityID], [UserID], [ActivitiesID], [Status], [CreateDate]) VALUES (11, 10, 4, N'3', CAST(N'2024-10-25T01:01:47.487' AS DateTime))
SET IDENTITY_INSERT [dbo].[requestJoinActivity] OFF
GO
SET IDENTITY_INSERT [dbo].[RequestJoinClub] ON 

INSERT [dbo].[RequestJoinClub] ([RequestClubId], [User_id], [Club_id], [Status], [Createdate]) VALUES (1, 5, 1, N'3', CAST(N'2024-10-25T01:01:47.487' AS DateTime))
INSERT [dbo].[RequestJoinClub] ([RequestClubId], [User_id], [Club_id], [Status], [Createdate]) VALUES (2, 5, 1, N'1', CAST(N'2024-10-25T01:37:16.947' AS DateTime))
SET IDENTITY_INSERT [dbo].[RequestJoinClub] OFF
GO
SET IDENTITY_INSERT [dbo].[Result] ON 

INSERT [dbo].[Result] ([ResultID], [MatchID], [ScoreTeam1], [ScoreTeam2]) VALUES (1, 1, 3, 2)
INSERT [dbo].[Result] ([ResultID], [MatchID], [ScoreTeam1], [ScoreTeam2]) VALUES (2, 2, 5, 4)
INSERT [dbo].[Result] ([ResultID], [MatchID], [ScoreTeam1], [ScoreTeam2]) VALUES (3, 3, 2, 2)
SET IDENTITY_INSERT [dbo].[Result] OFF
GO
SET IDENTITY_INSERT [dbo].[ResultOfActivities] ON 

INSERT [dbo].[ResultOfActivities] ([ResultID], [ActivityID], [Team1Score], [Team2Score]) VALUES (1, 1, 5, 4)
SET IDENTITY_INSERT [dbo].[ResultOfActivities] OFF
GO
SET IDENTITY_INSERT [dbo].[Role] ON 

INSERT [dbo].[Role] ([RoleID], [RoleName]) VALUES (1, N'Admin')
INSERT [dbo].[Role] ([RoleID], [RoleName]) VALUES (2, N'User')
INSERT [dbo].[Role] ([RoleID], [RoleName]) VALUES (3, N'Organizer')
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
SET IDENTITY_INSERT [dbo].[Round] ON 

INSERT [dbo].[Round] ([RoundID], [TournamentID], [RoundNumber]) VALUES (1, 1, 1)
INSERT [dbo].[Round] ([RoundID], [TournamentID], [RoundNumber]) VALUES (2, 1, 2)
INSERT [dbo].[Round] ([RoundID], [TournamentID], [RoundNumber]) VALUES (3, 1, 3)
INSERT [dbo].[Round] ([RoundID], [TournamentID], [RoundNumber]) VALUES (4, 2, 1)
INSERT [dbo].[Round] ([RoundID], [TournamentID], [RoundNumber]) VALUES (5, 2, 2)
INSERT [dbo].[Round] ([RoundID], [TournamentID], [RoundNumber]) VALUES (6, 2, 3)
SET IDENTITY_INSERT [dbo].[Round] OFF
GO
SET IDENTITY_INSERT [dbo].[Sport] ON 

INSERT [dbo].[Sport] ([SportID], [SportName]) VALUES (1, N'Bóng dá')
INSERT [dbo].[Sport] ([SportID], [SportName]) VALUES (2, N'Bóng chuyền')
INSERT [dbo].[Sport] ([SportID], [SportName]) VALUES (3, N'Bóng rổ')
INSERT [dbo].[Sport] ([SportID], [SportName]) VALUES (4, N'Cầu lông ')
INSERT [dbo].[Sport] ([SportID], [SportName]) VALUES (5, N'Pickleball')
INSERT [dbo].[Sport] ([SportID], [SportName]) VALUES (6, N'Bida')
SET IDENTITY_INSERT [dbo].[Sport] OFF
GO
SET IDENTITY_INSERT [dbo].[Team] ON 

INSERT [dbo].[Team] ([TeamID], [TournamentID], [UserTeamID], [TeamName]) VALUES (1, 2, NULL, N'Team A')
INSERT [dbo].[Team] ([TeamID], [TournamentID], [UserTeamID], [TeamName]) VALUES (2, 2, NULL, N'Team B')
INSERT [dbo].[Team] ([TeamID], [TournamentID], [UserTeamID], [TeamName]) VALUES (3, 2, NULL, N'Team C')
INSERT [dbo].[Team] ([TeamID], [TournamentID], [UserTeamID], [TeamName]) VALUES (4, 2, NULL, N'Team D')
SET IDENTITY_INSERT [dbo].[Team] OFF
GO
SET IDENTITY_INSERT [dbo].[TeamTournament] ON 

INSERT [dbo].[TeamTournament] ([TeamTournamentID], [TournamentID], [TeamID]) VALUES (1, 2, 1)
INSERT [dbo].[TeamTournament] ([TeamTournamentID], [TournamentID], [TeamID]) VALUES (2, 2, 2)
INSERT [dbo].[TeamTournament] ([TeamTournamentID], [TournamentID], [TeamID]) VALUES (3, 2, 3)
INSERT [dbo].[TeamTournament] ([TeamTournamentID], [TournamentID], [TeamID]) VALUES (4, 2, 4)
SET IDENTITY_INSERT [dbo].[TeamTournament] OFF
GO
SET IDENTITY_INSERT [dbo].[Tournaments] ON 

INSERT [dbo].[Tournaments] ([TournamentID], [LevelID], [Name], [Description], [StartDate], [EndDate], [Location], [NumberOfTeams], [Status], [CreatedDate], [UserID]) VALUES (1, 2, N'Giải bóng đá', N'Chỉ dành cho trung bình khá', CAST(N'2024-12-12T00:00:00.000' AS DateTime), CAST(N'2024-12-17T00:00:00.000' AS DateTime), N'Sân A', 8, N'Sắp diễn ra', CAST(N'2024-10-11T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[Tournaments] ([TournamentID], [LevelID], [Name], [Description], [StartDate], [EndDate], [Location], [NumberOfTeams], [Status], [CreatedDate], [UserID]) VALUES (2, 3, N'Giải cầu lông', N'Giải chuyên nghiệp', CAST(N'2024-09-15T00:00:00.000' AS DateTime), CAST(N'2024-09-22T00:00:00.000' AS DateTime), N'Nhà thi đấu B', 4, N'Kết thúc', CAST(N'2024-08-15T00:00:00.000' AS DateTime), 2)
SET IDENTITY_INSERT [dbo].[Tournaments] OFF
GO
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (1, 2, NULL, N'khanhhung@gmail.com', N'25d55ad283aa400af464c76d713c07ad', N'Khánh Hưng', N'0912345678', N'Đà Nẵng', N'Nam', 25, CAST(180.00 AS Decimal(5, 2)), CAST(75.00 AS Decimal(5, 2)), NULL, N'Active')
INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (2, 2, NULL, N'tiendat@gmail.com', N'tiendat123', N'Tiến Đạt', N'0987654321', N'Đà Nẵng', N'Nam', 23, CAST(170.00 AS Decimal(5, 2)), CAST(60.00 AS Decimal(5, 2)), NULL, N'Active')
INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (3, 2, NULL, N'hoangvu@gmail.com', N'hoangvu123', N'Hoàng Vũ', N'0912345678', N'Đà Nẵng', N'Nam', 23, CAST(170.00 AS Decimal(5, 2)), CAST(65.00 AS Decimal(5, 2)), NULL, N'Active')
INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (4, 2, NULL, N'thanhtung@gmail.com', N'thanhtung123', N'Thanh Tùng', N'0987654321', N'Đà Nẵng', N'Nam', 23, CAST(173.00 AS Decimal(5, 2)), CAST(70.00 AS Decimal(5, 2)), NULL, N'Active')
INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (5, 2, NULL, N'dat1@gmail.com', N'e10adc3949ba59abbe56e057f20f883e', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (6, 2, NULL, N'datnguyen1@gmail.com', N'25d55ad283aa400af464c76d713c07ad', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (7, 2, NULL, N'datnguyen2@gmail.com', N'25d55ad283aa400af464c76d713c07ad', N'Nguyen Van A', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (8, 2, NULL, N'datnguyen3@gmail.com', N'25d55ad283aa400af464c76d713c07ad', N'Nguyen Van B', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (9, 2, NULL, N'datnguyen4@gmail.com', N'25d55ad283aa400af464c76d713c07ad', N'Nguyen Van C', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (10, 2, NULL, N'datnguyen5@gmail.com', N'25d55ad283aa400af464c76d713c07ad', N'Tran Van D', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (11, 2, NULL, N'datnguyen6@gmail.com', N'25d55ad283aa400af464c76d713c07ad', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (12, 2, NULL, N'datnguyen7@gmail.com', N'25d55ad283aa400af464c76d713c07ad', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (13, 2, NULL, N'datnguyen9@gmail.com', N'25d55ad283aa400af464c76d713c07ad', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (14, 2, NULL, N'datnguyen10@gmail.com', N'25d55ad283aa400af464c76d713c07ad', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (1006, 2, NULL, N'datnguye1@gmail.com', N'25d55ad283aa400af464c76d713c07ad', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (1007, 2, NULL, N'datnguyen123@gmail.com', N'25d55ad283aa400af464c76d713c07ad', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[User] ([UserID], [RoleID], [UserTeamID], [Email], [Password], [FullName], [Phone], [Address], [Gender], [Age], [Height], [Weight], [Avatar], [Status]) VALUES (1008, 2, NULL, N'test1@gmail.com', N'25d55ad283aa400af464c76d713c07ad', N'Đạt Nguyễn', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[User] OFF
GO
SET IDENTITY_INSERT [dbo].[UserActivities] ON 

INSERT [dbo].[UserActivities] ([UserActivityID], [UserID], [ActivityID], [JoinDate], [RoleInActivity]) VALUES (1, 1, 4, CAST(N'2024-10-12T00:00:00.000' AS DateTime), N'Admin')
INSERT [dbo].[UserActivities] ([UserActivityID], [UserID], [ActivityID], [JoinDate], [RoleInActivity]) VALUES (2, 3, 4, CAST(N'2024-10-12T00:00:00.000' AS DateTime), N'Player')
INSERT [dbo].[UserActivities] ([UserActivityID], [UserID], [ActivityID], [JoinDate], [RoleInActivity]) VALUES (15, 7, 4, CAST(N'2024-11-05T18:50:02.407' AS DateTime), N'Player')
INSERT [dbo].[UserActivities] ([UserActivityID], [UserID], [ActivityID], [JoinDate], [RoleInActivity]) VALUES (16, 8, 4, CAST(N'2024-11-05T18:50:09.167' AS DateTime), N'Player')
SET IDENTITY_INSERT [dbo].[UserActivities] OFF
GO
SET IDENTITY_INSERT [dbo].[UserClub] ON 

INSERT [dbo].[UserClub] ([UserClubID], [ClubID], [UserID], [Role], [JoinDate], [Status]) VALUES (1, 1, 1, N'Leader', CAST(N'2024-01-15T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[UserClub] ([UserClubID], [ClubID], [UserID], [Role], [JoinDate], [Status]) VALUES (2, 2, 3, N'Leader', CAST(N'2024-04-03T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[UserClub] ([UserClubID], [ClubID], [UserID], [Role], [JoinDate], [Status]) VALUES (3, 3, 4, N'Leader', CAST(N'2024-08-15T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[UserClub] ([UserClubID], [ClubID], [UserID], [Role], [JoinDate], [Status]) VALUES (4, 1, 2, N'Member', CAST(N'2024-02-10T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[UserClub] ([UserClubID], [ClubID], [UserID], [Role], [JoinDate], [Status]) VALUES (5, 1, 3, N'Member', CAST(N'2024-03-09T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[UserClub] ([UserClubID], [ClubID], [UserID], [Role], [JoinDate], [Status]) VALUES (6, 1, 4, N'Member', CAST(N'2024-01-20T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[UserClub] ([UserClubID], [ClubID], [UserID], [Role], [JoinDate], [Status]) VALUES (7, 2, 1, N'Member', CAST(N'2024-04-10T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[UserClub] ([UserClubID], [ClubID], [UserID], [Role], [JoinDate], [Status]) VALUES (8, 2, 2, N'Member', CAST(N'2024-05-01T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[UserClub] ([UserClubID], [ClubID], [UserID], [Role], [JoinDate], [Status]) VALUES (9, 2, 4, N'Member', CAST(N'2024-05-11T00:00:00.000' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[UserClub] OFF
GO
SET IDENTITY_INSERT [dbo].[UserSport] ON 

INSERT [dbo].[UserSport] ([UserSportID], [UserID], [SportID], [LevelID], [Achievement], [Position]) VALUES (1, 1, 1, 2, NULL, NULL)
INSERT [dbo].[UserSport] ([UserSportID], [UserID], [SportID], [LevelID], [Achievement], [Position]) VALUES (2, 2, 1, 2, NULL, NULL)
INSERT [dbo].[UserSport] ([UserSportID], [UserID], [SportID], [LevelID], [Achievement], [Position]) VALUES (3, 3, 1, 2, NULL, NULL)
INSERT [dbo].[UserSport] ([UserSportID], [UserID], [SportID], [LevelID], [Achievement], [Position]) VALUES (4, 4, 1, 2, NULL, NULL)
INSERT [dbo].[UserSport] ([UserSportID], [UserID], [SportID], [LevelID], [Achievement], [Position]) VALUES (6, 7, 1, 2, NULL, NULL)
INSERT [dbo].[UserSport] ([UserSportID], [UserID], [SportID], [LevelID], [Achievement], [Position]) VALUES (7, 8, 1, 3, NULL, NULL)
SET IDENTITY_INSERT [dbo].[UserSport] OFF
GO
SET IDENTITY_INSERT [dbo].[UserTeam] ON 

INSERT [dbo].[UserTeam] ([UserTeamID], [UserID], [TeamID]) VALUES (1, 1, 1)
INSERT [dbo].[UserTeam] ([UserTeamID], [UserID], [TeamID]) VALUES (2, 2, 2)
INSERT [dbo].[UserTeam] ([UserTeamID], [UserID], [TeamID]) VALUES (3, 3, 3)
INSERT [dbo].[UserTeam] ([UserTeamID], [UserID], [TeamID]) VALUES (4, 4, 4)
SET IDENTITY_INSERT [dbo].[UserTeam] OFF
GO
ALTER TABLE [dbo].[requestJoinActivity] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[RequestJoinClub] ADD  DEFAULT (getdate()) FOR [Createdate]
GO
ALTER TABLE [dbo].[UserClub] ADD  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Activities]  WITH CHECK ADD  CONSTRAINT [FK_Activities_Level] FOREIGN KEY([LevelID])
REFERENCES [dbo].[Level] ([LevelID])
GO
ALTER TABLE [dbo].[Activities] CHECK CONSTRAINT [FK_Activities_Level]
GO
ALTER TABLE [dbo].[Activities]  WITH CHECK ADD  CONSTRAINT [FK_Activities_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Activities] CHECK CONSTRAINT [FK_Activities_User]
GO
ALTER TABLE [dbo].[Admin]  WITH CHECK ADD  CONSTRAINT [FK_Admin_Role] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Role] ([RoleID])
GO
ALTER TABLE [dbo].[Admin] CHECK CONSTRAINT [FK_Admin_Role]
GO
ALTER TABLE [dbo].[Club]  WITH CHECK ADD  CONSTRAINT [FK_Club_Level] FOREIGN KEY([LevelID])
REFERENCES [dbo].[Level] ([LevelID])
GO
ALTER TABLE [dbo].[Club] CHECK CONSTRAINT [FK_Club_Level]
GO
ALTER TABLE [dbo].[Club]  WITH CHECK ADD  CONSTRAINT [FK_Club_Sport] FOREIGN KEY([SportID])
REFERENCES [dbo].[Sport] ([SportID])
GO
ALTER TABLE [dbo].[Club] CHECK CONSTRAINT [FK_Club_Sport]
GO
ALTER TABLE [dbo].[Comment]  WITH CHECK ADD  CONSTRAINT [FK_Comment_Activity] FOREIGN KEY([ActivityID])
REFERENCES [dbo].[Activities] ([ActivityID])
GO
ALTER TABLE [dbo].[Comment] CHECK CONSTRAINT [FK_Comment_Activity]
GO
ALTER TABLE [dbo].[Comment]  WITH CHECK ADD  CONSTRAINT [FK_Comment_Tournament] FOREIGN KEY([TournamentID])
REFERENCES [dbo].[Tournaments] ([TournamentID])
GO
ALTER TABLE [dbo].[Comment] CHECK CONSTRAINT [FK_Comment_Tournament]
GO
ALTER TABLE [dbo].[Comment]  WITH CHECK ADD  CONSTRAINT [FK_Comment_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Comment] CHECK CONSTRAINT [FK_Comment_User]
GO
ALTER TABLE [dbo].[Fee]  WITH CHECK ADD  CONSTRAINT [FK_Fee_Tournament] FOREIGN KEY([TournamentID])
REFERENCES [dbo].[Tournaments] ([TournamentID])
GO
ALTER TABLE [dbo].[Fee] CHECK CONSTRAINT [FK_Fee_Tournament]
GO
ALTER TABLE [dbo].[Match]  WITH CHECK ADD  CONSTRAINT [FK_Match_Round] FOREIGN KEY([RoundID])
REFERENCES [dbo].[Round] ([RoundID])
GO
ALTER TABLE [dbo].[Match] CHECK CONSTRAINT [FK_Match_Round]
GO
ALTER TABLE [dbo].[Match]  WITH CHECK ADD  CONSTRAINT [FK_Match_Team1] FOREIGN KEY([Team1ID])
REFERENCES [dbo].[Team] ([TeamID])
GO
ALTER TABLE [dbo].[Match] CHECK CONSTRAINT [FK_Match_Team1]
GO
ALTER TABLE [dbo].[Match]  WITH CHECK ADD  CONSTRAINT [FK_Match_Team2] FOREIGN KEY([Team2ID])
REFERENCES [dbo].[Team] ([TeamID])
GO
ALTER TABLE [dbo].[Match] CHECK CONSTRAINT [FK_Match_Team2]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_Activity] FOREIGN KEY([ActivityID])
REFERENCES [dbo].[Activities] ([ActivityID])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_Activity]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_Tournament] FOREIGN KEY([TournamentID])
REFERENCES [dbo].[Tournaments] ([TournamentID])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_Tournament]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_User]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_Club] FOREIGN KEY([ClubID])
REFERENCES [dbo].[Club] ([ClubID])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_Payment_Club]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_Tournament] FOREIGN KEY([TournamentID])
REFERENCES [dbo].[Tournaments] ([TournamentID])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_Payment_Tournament]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_Payment_User]
GO
ALTER TABLE [dbo].[requestJoinActivity]  WITH CHECK ADD  CONSTRAINT [FK_requestJoinActivity_Activity] FOREIGN KEY([ActivitiesID])
REFERENCES [dbo].[Activities] ([ActivityID])
GO
ALTER TABLE [dbo].[requestJoinActivity] CHECK CONSTRAINT [FK_requestJoinActivity_Activity]
GO
ALTER TABLE [dbo].[requestJoinActivity]  WITH CHECK ADD  CONSTRAINT [FK_requestJoinActivity_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[requestJoinActivity] CHECK CONSTRAINT [FK_requestJoinActivity_User]
GO
ALTER TABLE [dbo].[RequestJoinClub]  WITH CHECK ADD  CONSTRAINT [FK_RequestJoinClub_Club] FOREIGN KEY([Club_id])
REFERENCES [dbo].[Club] ([ClubID])
GO
ALTER TABLE [dbo].[RequestJoinClub] CHECK CONSTRAINT [FK_RequestJoinClub_Club]
GO
ALTER TABLE [dbo].[RequestJoinClub]  WITH CHECK ADD  CONSTRAINT [FK_RequestJoinClub_User] FOREIGN KEY([User_id])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[RequestJoinClub] CHECK CONSTRAINT [FK_RequestJoinClub_User]
GO
ALTER TABLE [dbo].[Result]  WITH CHECK ADD  CONSTRAINT [FK_Result_Match] FOREIGN KEY([MatchID])
REFERENCES [dbo].[Match] ([MatchID])
GO
ALTER TABLE [dbo].[Result] CHECK CONSTRAINT [FK_Result_Match]
GO
ALTER TABLE [dbo].[ResultOfActivities]  WITH CHECK ADD  CONSTRAINT [FK_ResultOfActivities_Activity] FOREIGN KEY([ActivityID])
REFERENCES [dbo].[Activities] ([ActivityID])
GO
ALTER TABLE [dbo].[ResultOfActivities] CHECK CONSTRAINT [FK_ResultOfActivities_Activity]
GO
ALTER TABLE [dbo].[Round]  WITH CHECK ADD  CONSTRAINT [FK_Round_Tournament] FOREIGN KEY([TournamentID])
REFERENCES [dbo].[Tournaments] ([TournamentID])
GO
ALTER TABLE [dbo].[Round] CHECK CONSTRAINT [FK_Round_Tournament]
GO
ALTER TABLE [dbo].[Team]  WITH CHECK ADD  CONSTRAINT [FK_Team_Tournament] FOREIGN KEY([TournamentID])
REFERENCES [dbo].[Tournaments] ([TournamentID])
GO
ALTER TABLE [dbo].[Team] CHECK CONSTRAINT [FK_Team_Tournament]
GO
ALTER TABLE [dbo].[TeamTournament]  WITH CHECK ADD  CONSTRAINT [FK_TeamTournament_Team] FOREIGN KEY([TeamID])
REFERENCES [dbo].[Team] ([TeamID])
GO
ALTER TABLE [dbo].[TeamTournament] CHECK CONSTRAINT [FK_TeamTournament_Team]
GO
ALTER TABLE [dbo].[TeamTournament]  WITH CHECK ADD  CONSTRAINT [FK_TeamTournament_Tournament] FOREIGN KEY([TournamentID])
REFERENCES [dbo].[Tournaments] ([TournamentID])
GO
ALTER TABLE [dbo].[TeamTournament] CHECK CONSTRAINT [FK_TeamTournament_Tournament]
GO
ALTER TABLE [dbo].[Tournaments]  WITH CHECK ADD  CONSTRAINT [FK_Tournaments_Level] FOREIGN KEY([LevelID])
REFERENCES [dbo].[Level] ([LevelID])
GO
ALTER TABLE [dbo].[Tournaments] CHECK CONSTRAINT [FK_Tournaments_Level]
GO
ALTER TABLE [dbo].[Tournaments]  WITH CHECK ADD  CONSTRAINT [FK_Tournaments_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Tournaments] CHECK CONSTRAINT [FK_Tournaments_User]
GO
ALTER TABLE [dbo].[TransferHistory]  WITH CHECK ADD  CONSTRAINT [FK_ReceiverAdmin] FOREIGN KEY([ReceiverAdminID])
REFERENCES [dbo].[Admin] ([AdminID])
GO
ALTER TABLE [dbo].[TransferHistory] CHECK CONSTRAINT [FK_ReceiverAdmin]
GO
ALTER TABLE [dbo].[TransferHistory]  WITH CHECK ADD  CONSTRAINT [FK_ReceiverClub] FOREIGN KEY([ReceiverClubID])
REFERENCES [dbo].[Club] ([ClubID])
GO
ALTER TABLE [dbo].[TransferHistory] CHECK CONSTRAINT [FK_ReceiverClub]
GO
ALTER TABLE [dbo].[TransferHistory]  WITH CHECK ADD  CONSTRAINT [FK_SenderAdmin] FOREIGN KEY([SenderAdminID])
REFERENCES [dbo].[Admin] ([AdminID])
GO
ALTER TABLE [dbo].[TransferHistory] CHECK CONSTRAINT [FK_SenderAdmin]
GO
ALTER TABLE [dbo].[TransferHistory]  WITH CHECK ADD  CONSTRAINT [FK_SenderClub] FOREIGN KEY([SenderClubID])
REFERENCES [dbo].[Club] ([ClubID])
GO
ALTER TABLE [dbo].[TransferHistory] CHECK CONSTRAINT [FK_SenderClub]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Role] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Role] ([RoleID])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Role]
GO
ALTER TABLE [dbo].[UserActivities]  WITH CHECK ADD  CONSTRAINT [FK_UserActivities_Activity] FOREIGN KEY([ActivityID])
REFERENCES [dbo].[Activities] ([ActivityID])
GO
ALTER TABLE [dbo].[UserActivities] CHECK CONSTRAINT [FK_UserActivities_Activity]
GO
ALTER TABLE [dbo].[UserActivities]  WITH CHECK ADD  CONSTRAINT [FK_UserActivities_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[UserActivities] CHECK CONSTRAINT [FK_UserActivities_User]
GO
ALTER TABLE [dbo].[UserClub]  WITH CHECK ADD  CONSTRAINT [FK_UserClub_Club] FOREIGN KEY([ClubID])
REFERENCES [dbo].[Club] ([ClubID])
GO
ALTER TABLE [dbo].[UserClub] CHECK CONSTRAINT [FK_UserClub_Club]
GO
ALTER TABLE [dbo].[UserClub]  WITH CHECK ADD  CONSTRAINT [FK_UserClub_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[UserClub] CHECK CONSTRAINT [FK_UserClub_User]
GO
ALTER TABLE [dbo].[UserSport]  WITH CHECK ADD  CONSTRAINT [FK_UserSport_Level] FOREIGN KEY([LevelID])
REFERENCES [dbo].[Level] ([LevelID])
GO
ALTER TABLE [dbo].[UserSport] CHECK CONSTRAINT [FK_UserSport_Level]
GO
ALTER TABLE [dbo].[UserSport]  WITH CHECK ADD  CONSTRAINT [FK_UserSport_Sport] FOREIGN KEY([SportID])
REFERENCES [dbo].[Sport] ([SportID])
GO
ALTER TABLE [dbo].[UserSport] CHECK CONSTRAINT [FK_UserSport_Sport]
GO
ALTER TABLE [dbo].[UserSport]  WITH CHECK ADD  CONSTRAINT [FK_UserSport_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[UserSport] CHECK CONSTRAINT [FK_UserSport_User]
GO
ALTER TABLE [dbo].[UserTeam]  WITH CHECK ADD  CONSTRAINT [FK_UserTeam_Team] FOREIGN KEY([TeamID])
REFERENCES [dbo].[Team] ([TeamID])
GO
ALTER TABLE [dbo].[UserTeam] CHECK CONSTRAINT [FK_UserTeam_Team]
GO
ALTER TABLE [dbo].[UserTeam]  WITH CHECK ADD  CONSTRAINT [FK_UserTeam_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[UserTeam] CHECK CONSTRAINT [FK_UserTeam_User]
GO
USE [master]
GO
ALTER DATABASE [DSC] SET  READ_WRITE 
GO
