SET IDENTITY_INSERT [Accounts] ON 

INSERT [Accounts] ([ID], [Email], [Password], [RegDate]) VALUES (1012, N'sarpay@gmail.com', N'$2a$10$RyP1GcBcZZuMcpK0kj7z.uoddtz88KMRzEVgLpKmm8R/We63nk2ui', CAST(0xA4D400C0 AS SmallDateTime))
INSERT [Accounts] ([ID], [Email], [Password], [RegDate]) VALUES (1014, N'meltem@gmail.com', N'$2a$10$TdprV09cnzeaC4FYiOrlfOc/bXLLY46LYJHdDLLD6JJdlCcpbCd0S', CAST(0xA4D5037C AS SmallDateTime))
INSERT [Accounts] ([ID], [Email], [Password], [RegDate]) VALUES (1015, N'scott@gmail.com', N'$2a$10$jB3ePuCN.ZV.JJgVoL0bvOfeqtC3yxDEYEuSdO9q9nsyCsD.WKOfa', CAST(0xA4D5037E AS SmallDateTime))
INSERT [Accounts] ([ID], [Email], [Password], [RegDate]) VALUES (1016, N'ozlem@gmail.com', N'$2a$10$Bn2Jy.xfztfRJ/KP6bAC1u5rz2fyu9ZA8pj4FuS61GIEtQyMzxGSC', CAST(0xA4D60537 AS SmallDateTime))
INSERT [Accounts] ([ID], [Email], [Password], [RegDate]) VALUES (1017, N'ioner@refleksyangin.com.tr', N'$2a$10$utpAoHWvI8bhOKJVI27mk.izlJfwTavL2vhTqngSN242duX7zOglq', CAST(0xA4D60538 AS SmallDateTime))
INSERT [Accounts] ([ID], [Email], [Password], [RegDate]) VALUES (1018, N'mertay@hotmail.com', N'$2a$10$n/D74cjKiBit9Y2UJ5LxfeFGYGX58P.aCbXN4cBVBrscxRy.4oGHm', CAST(0xA4DC0442 AS SmallDateTime))
INSERT [Accounts] ([ID], [Email], [Password], [RegDate]) VALUES (1019, N'baharo@refleksyangin.com.tr', N'$2a$10$6CUsNAlKJJF1mztfJfzDteSSCqOOo8JjW2AMdEGDXYMOMr7.kw/wC', CAST(0xA4DC0444 AS SmallDateTime))
SET IDENTITY_INSERT [Accounts] OFF
INSERT [Genders] ([IX], [Text]) VALUES (1, N'Male')
INSERT [Genders] ([IX], [Text]) VALUES (2, N'Female')
SET IDENTITY_INSERT [Goods] ON 

INSERT [Goods] ([ID], [Name], [Description], [Price]) VALUES (1, N'Hover Copter', N'4 propeler system and advanced GPS controlling.', 449.9900)
INSERT [Goods] ([ID], [Name], [Description], [Price]) VALUES (2, N'iPhone 6 Case', N'Silicon case for iPhone 6', 29.9800)
INSERT [Goods] ([ID], [Name], [Description], [Price]) VALUES (3, N'Apple Keyboard', N'Bluetooh connected mini keyboard. Compatible with all OS and Apple notebooks and desktops', 139.0000)
SET IDENTITY_INSERT [Goods] OFF
SET IDENTITY_INSERT [Purchases] ON 

INSERT [Purchases] ([ID], [AccountID], [GoodID]) VALUES (1, 1012, 1)
INSERT [Purchases] ([ID], [AccountID], [GoodID]) VALUES (2, 1012, 3)
INSERT [Purchases] ([ID], [AccountID], [GoodID]) VALUES (3, 1014, 2)
INSERT [Purchases] ([ID], [AccountID], [GoodID]) VALUES (4, 1015, 1)
INSERT [Purchases] ([ID], [AccountID], [GoodID]) VALUES (1004, 1016, 3)
INSERT [Purchases] ([ID], [AccountID], [GoodID]) VALUES (1005, 1017, 3)
INSERT [Purchases] ([ID], [AccountID], [GoodID]) VALUES (2004, 1016, 1)
INSERT [Purchases] ([ID], [AccountID], [GoodID]) VALUES (2005, 1012, 1)
INSERT [Purchases] ([ID], [AccountID], [GoodID]) VALUES (2006, 1016, 2)
INSERT [Purchases] ([ID], [AccountID], [GoodID]) VALUES (2008, 1017, 3)
SET IDENTITY_INSERT [Purchases] OFF
INSERT [Shoppers] ([AccountID], [Name], [GenderIX], [OptIn]) VALUES (1012, N'SARPAY ÖNER', 1, 0)
INSERT [Shoppers] ([AccountID], [Name], [GenderIX], [OptIn]) VALUES (1014, N'MELTEM TARIKAHYA', 2, 0)
INSERT [Shoppers] ([AccountID], [Name], [GenderIX], [OptIn]) VALUES (1015, N'SCOTT GRAVES', 1, 0)
INSERT [Shoppers] ([AccountID], [Name], [GenderIX], [OptIn]) VALUES (1016, N'ÖZLEM ÖNER', 2, 0)
INSERT [Shoppers] ([AccountID], [Name], [GenderIX], [OptIn]) VALUES (1017, N'İRFAN ÖNER', NULL, 1)
INSERT [Shoppers] ([AccountID], [Name], [GenderIX], [OptIn]) VALUES (1018, N'MERTAY ÖNER', 1, 0)
INSERT [Shoppers] ([AccountID], [Name], [GenderIX], [OptIn]) VALUES (1019, N'BAHAR ÖNER', 2, 1)
INSERT [UserTickets] ([UserID], [Ticket], [CreatedOn], [ExpiresOn], [SignInCount]) VALUES (1012, N'AB45268C-E4F8-427D-AE0C-624BC6DA70BB', CAST(0x0000A4E50045F216 AS DateTime), CAST(0x0000A4E5005A8CF5 AS DateTime), 45)
