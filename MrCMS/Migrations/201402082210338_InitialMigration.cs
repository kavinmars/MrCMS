namespace MrCMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ACLRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Site_Id = c.Int(),
                        UserRole_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .ForeignKey("dbo.UserRoles", t => t.UserRole_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.UserRole_Id);
            
            CreateTable(
                "dbo.Sites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        BaseUrl = c.String(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RedirectedDomains",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.Site_Id);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.Site_Id);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        DisplayOrder = c.Int(nullable: false),
                        UrlSegment = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Hidden = c.Boolean(),
                        MetaTitle = c.String(),
                        MetaDescription = c.String(),
                        HideInAdminNav = c.Boolean(),
                        MetaTitle1 = c.String(maxLength: 250),
                        MetaDescription1 = c.String(maxLength: 250),
                        MetaKeywords = c.String(maxLength: 250),
                        RevealInNavigation = c.Boolean(),
                        CustomHeaderScripts = c.String(),
                        CustomFooterScripts = c.String(),
                        RequiresSSL = c.Boolean(),
                        PublishOn = c.DateTime(),
                        BodyContent = c.String(),
                        BlockAnonymousAccess = c.Boolean(),
                        FormSubmittedMessage = c.String(maxLength: 500),
                        FormEmailTitle = c.String(maxLength: 250),
                        SendFormTo = c.String(maxLength: 500),
                        FormMessage = c.String(),
                        FormDesign = c.String(),
                        SubmitButtonCssClass = c.String(maxLength: 100),
                        SubmitButtonText = c.String(maxLength: 100),
                        InheritFrontEndRolesFromParent = c.Boolean(),
                        RedirectUrl = c.String(),
                        Permanent = c.Boolean(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Parent_Id = c.Int(),
                        Site_Id = c.Int(),
                        Widget_Id = c.Int(),
                        Widget_Id1 = c.Int(),
                        Layout_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.Parent_Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .ForeignKey("dbo.Widgets", t => t.Widget_Id)
                .ForeignKey("dbo.Widgets", t => t.Widget_Id1)
                .ForeignKey("dbo.Documents", t => t.Layout_Id)
                .Index(t => t.Parent_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.Widget_Id)
                .Index(t => t.Widget_Id1)
                .Index(t => t.Layout_Id);
            
            CreateTable(
                "dbo.LayoutAreas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AreaName = c.String(nullable: false, maxLength: 250),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Layout_Id = c.Int(),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.Layout_Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.Layout_Id)
                .Index(t => t.Site_Id);
            
            CreateTable(
                "dbo.PageWidgetSorts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Order = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        LayoutArea_Id = c.Int(),
                        Site_Id = c.Int(),
                        Webpage_Id = c.Int(),
                        Widget_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LayoutAreas", t => t.LayoutArea_Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .ForeignKey("dbo.Documents", t => t.Webpage_Id)
                .ForeignKey("dbo.Widgets", t => t.Widget_Id)
                .Index(t => t.LayoutArea_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.Webpage_Id)
                .Index(t => t.Widget_Id);
            
            CreateTable(
                "dbo.Widgets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CustomLayout = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        Name2 = c.String(),
                        IsRecursive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        LayoutArea_Id = c.Int(),
                        Site_Id = c.Int(),
                        Webpage_Id = c.Int(),
                        Webpage_Id1 = c.Int(),
                        Webpage_Id2 = c.Int(),
                        Webpage_Id3 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LayoutAreas", t => t.LayoutArea_Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .ForeignKey("dbo.Documents", t => t.Webpage_Id)
                .ForeignKey("dbo.Documents", t => t.Webpage_Id1)
                .ForeignKey("dbo.Documents", t => t.Webpage_Id2)
                .ForeignKey("dbo.Documents", t => t.Webpage_Id3)
                .Index(t => t.LayoutArea_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.Webpage_Id)
                .Index(t => t.Webpage_Id1)
                .Index(t => t.Webpage_Id2)
                .Index(t => t.Webpage_Id3);
            
            CreateTable(
                "dbo.MediaFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileExtension = c.String(),
                        ContentType = c.String(),
                        FileUrl = c.String(),
                        ContentLength = c.Long(nullable: false),
                        FileName = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        MediaCategory_Id = c.Int(),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.MediaCategory_Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.MediaCategory_Id)
                .Index(t => t.Site_Id);
            
            CreateTable(
                "dbo.ResizedImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        MediaFile_Id = c.Int(),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MediaFiles", t => t.MediaFile_Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.MediaFile_Id)
                .Index(t => t.Site_Id);
            
            CreateTable(
                "dbo.FormPostings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Site_Id = c.Int(),
                        Webpage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .ForeignKey("dbo.Documents", t => t.Webpage_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.Webpage_Id);
            
            CreateTable(
                "dbo.FormValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Value = c.String(),
                        IsFile = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        FormPosting_Id = c.Int(),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FormPostings", t => t.FormPosting_Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.FormPosting_Id)
                .Index(t => t.Site_Id);
            
            CreateTable(
                "dbo.FormListOptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        Selected = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        FormProperty_Id = c.Int(),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FormProperties", t => t.FormProperty_Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.FormProperty_Id)
                .Index(t => t.Site_Id);
            
            CreateTable(
                "dbo.FormProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LabelText = c.String(),
                        Required = c.Boolean(nullable: false),
                        CssClass = c.String(),
                        HtmlId = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Site_Id = c.Int(),
                        Webpage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .ForeignKey("dbo.Documents", t => t.Webpage_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.Webpage_Id);
            
            CreateTable(
                "dbo.UrlHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UrlSegment = c.String(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Site_Id = c.Int(),
                        Webpage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .ForeignKey("dbo.Documents", t => t.Webpage_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.Webpage_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        PasswordHash = c.Binary(),
                        PasswordSalt = c.Binary(),
                        Guid = c.Guid(nullable: false),
                        CurrentEncryption = c.String(),
                        Email = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        LastLoginDate = c.DateTime(),
                        LoginAttempts = c.Int(nullable: false),
                        ResetPasswordGuid = c.Guid(),
                        ResetPasswordExpiry = c.DateTime(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Issuer = c.String(),
                        Claim = c.String(),
                        Value = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.UserLogins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoginProvider = c.String(),
                        ProviderKey = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.DocumentVersions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Data = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Document_Id = c.Int(),
                        Site_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.Document_Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.Document_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AdminAllowedRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsRecursive = c.Boolean(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Site_Id = c.Int(),
                        UserRole_Id = c.Int(),
                        Webpage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .ForeignKey("dbo.UserRoles", t => t.UserRole_Id)
                .ForeignKey("dbo.Documents", t => t.Webpage_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.UserRole_Id)
                .Index(t => t.Webpage_Id);
            
            CreateTable(
                "dbo.AdminDisallowedRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsRecursive = c.Boolean(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Site_Id = c.Int(),
                        UserRole_Id = c.Int(),
                        Webpage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .ForeignKey("dbo.UserRoles", t => t.UserRole_Id)
                .ForeignKey("dbo.Documents", t => t.Webpage_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.UserRole_Id)
                .Index(t => t.Webpage_Id);
            
            CreateTable(
                "dbo.FrontEndAllowedRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsRecursive = c.Boolean(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Site_Id = c.Int(),
                        UserRole_Id = c.Int(),
                        Webpage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .ForeignKey("dbo.UserRoles", t => t.UserRole_Id)
                .ForeignKey("dbo.Documents", t => t.Webpage_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.UserRole_Id)
                .Index(t => t.Webpage_Id);
            
            CreateTable(
                "dbo.FrontEndDisallowedRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsRecursive = c.Boolean(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Site_Id = c.Int(),
                        UserRole_Id = c.Int(),
                        Webpage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .ForeignKey("dbo.UserRoles", t => t.UserRole_Id)
                .ForeignKey("dbo.Documents", t => t.Webpage_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.UserRole_Id)
                .Index(t => t.Webpage_Id);
            
            CreateTable(
                "dbo.QueuedMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FromAddress = c.String(),
                        FromName = c.String(),
                        ToAddress = c.String(),
                        ToName = c.String(),
                        Cc = c.String(),
                        Bcc = c.String(),
                        Subject = c.String(),
                        Body = c.String(),
                        SentOn = c.DateTime(),
                        Tries = c.Int(nullable: false),
                        IsHtml = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.Site_Id);
            
            CreateTable(
                "dbo.QueuedMessageAttachments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        QueuedMessage_Id = c.Int(),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QueuedMessages", t => t.QueuedMessage_Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.QueuedMessage_Id)
                .Index(t => t.Site_Id);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Value = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.Site_Id);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Guid = c.Guid(nullable: false),
                        Error_ApplicationName = c.String(),
                        Error_HostName = c.String(),
                        Error_Type = c.String(),
                        Error_Source = c.String(),
                        Error_Message = c.String(),
                        Error_Detail = c.String(),
                        Error_User = c.String(),
                        Error_Time = c.DateTime(nullable: false),
                        Error_StatusCode = c.Int(nullable: false),
                        Error_WebHostHtmlMessage = c.String(),
                        Message = c.String(),
                        Detail = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.Site_Id);
            
            CreateTable(
                "dbo.ScheduledTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        EveryXSeconds = c.Int(nullable: false),
                        LastComplete = c.DateTime(),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.Site_Id);
            
            CreateTable(
                "dbo.QueuedTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        Data = c.String(),
                        Status = c.Int(nullable: false),
                        Tries = c.Int(nullable: false),
                        Priority = c.Int(nullable: false),
                        QueuedAt = c.DateTime(),
                        StartedAt = c.DateTime(),
                        CompletedAt = c.DateTime(),
                        FailedAt = c.DateTime(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.Site_Id);
            
            CreateTable(
                "dbo.TagDocuments",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        Document_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Document_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .ForeignKey("dbo.Documents", t => t.Document_Id, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.Document_Id);
            
            CreateTable(
                "dbo.WebpageUserRoles",
                c => new
                    {
                        Webpage_Id = c.Int(nullable: false),
                        UserRole_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Webpage_Id, t.UserRole_Id })
                .ForeignKey("dbo.Documents", t => t.Webpage_Id, cascadeDelete: true)
                .ForeignKey("dbo.UserRoles", t => t.UserRole_Id, cascadeDelete: true)
                .Index(t => t.Webpage_Id)
                .Index(t => t.UserRole_Id);
            
            CreateTable(
                "dbo.UserUserRoles",
                c => new
                    {
                        User_Id = c.Int(nullable: false),
                        UserRole_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.UserRole_Id })
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.UserRoles", t => t.UserRole_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.UserRole_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QueuedTasks", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.ScheduledTasks", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.Logs", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.Settings", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.QueuedMessages", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.QueuedMessageAttachments", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.QueuedMessageAttachments", "QueuedMessage_Id", "dbo.QueuedMessages");
            DropForeignKey("dbo.FrontEndDisallowedRoles", "Webpage_Id", "dbo.Documents");
            DropForeignKey("dbo.FrontEndDisallowedRoles", "UserRole_Id", "dbo.UserRoles");
            DropForeignKey("dbo.FrontEndDisallowedRoles", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.FrontEndAllowedRoles", "Webpage_Id", "dbo.Documents");
            DropForeignKey("dbo.FrontEndAllowedRoles", "UserRole_Id", "dbo.UserRoles");
            DropForeignKey("dbo.FrontEndAllowedRoles", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.AdminDisallowedRoles", "Webpage_Id", "dbo.Documents");
            DropForeignKey("dbo.AdminDisallowedRoles", "UserRole_Id", "dbo.UserRoles");
            DropForeignKey("dbo.AdminDisallowedRoles", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.AdminAllowedRoles", "Webpage_Id", "dbo.Documents");
            DropForeignKey("dbo.AdminAllowedRoles", "UserRole_Id", "dbo.UserRoles");
            DropForeignKey("dbo.AdminAllowedRoles", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.DocumentVersions", "User_Id", "dbo.Users");
            DropForeignKey("dbo.DocumentVersions", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.DocumentVersions", "Document_Id", "dbo.Documents");
            DropForeignKey("dbo.UserLogins", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserClaims", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserUserRoles", "UserRole_Id", "dbo.UserRoles");
            DropForeignKey("dbo.UserUserRoles", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Widgets", "Webpage_Id3", "dbo.Documents");
            DropForeignKey("dbo.UrlHistories", "Webpage_Id", "dbo.Documents");
            DropForeignKey("dbo.UrlHistories", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.Widgets", "Webpage_Id2", "dbo.Documents");
            DropForeignKey("dbo.Widgets", "Webpage_Id1", "dbo.Documents");
            DropForeignKey("dbo.WebpageUserRoles", "UserRole_Id", "dbo.UserRoles");
            DropForeignKey("dbo.WebpageUserRoles", "Webpage_Id", "dbo.Documents");
            DropForeignKey("dbo.FormProperties", "Webpage_Id", "dbo.Documents");
            DropForeignKey("dbo.FormProperties", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.FormListOptions", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.FormListOptions", "FormProperty_Id", "dbo.FormProperties");
            DropForeignKey("dbo.FormPostings", "Webpage_Id", "dbo.Documents");
            DropForeignKey("dbo.FormPostings", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.FormValues", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.FormValues", "FormPosting_Id", "dbo.FormPostings");
            DropForeignKey("dbo.MediaFiles", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.ResizedImages", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.ResizedImages", "MediaFile_Id", "dbo.MediaFiles");
            DropForeignKey("dbo.MediaFiles", "MediaCategory_Id", "dbo.Documents");
            DropForeignKey("dbo.Documents", "Layout_Id", "dbo.Documents");
            DropForeignKey("dbo.LayoutAreas", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.Widgets", "Webpage_Id", "dbo.Documents");
            DropForeignKey("dbo.Widgets", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.Documents", "Widget_Id1", "dbo.Widgets");
            DropForeignKey("dbo.PageWidgetSorts", "Widget_Id", "dbo.Widgets");
            DropForeignKey("dbo.Widgets", "LayoutArea_Id", "dbo.LayoutAreas");
            DropForeignKey("dbo.Documents", "Widget_Id", "dbo.Widgets");
            DropForeignKey("dbo.PageWidgetSorts", "Webpage_Id", "dbo.Documents");
            DropForeignKey("dbo.PageWidgetSorts", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.PageWidgetSorts", "LayoutArea_Id", "dbo.LayoutAreas");
            DropForeignKey("dbo.LayoutAreas", "Layout_Id", "dbo.Documents");
            DropForeignKey("dbo.Tags", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.TagDocuments", "Document_Id", "dbo.Documents");
            DropForeignKey("dbo.TagDocuments", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.Documents", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.Documents", "Parent_Id", "dbo.Documents");
            DropForeignKey("dbo.ACLRoles", "UserRole_Id", "dbo.UserRoles");
            DropForeignKey("dbo.ACLRoles", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.RedirectedDomains", "Site_Id", "dbo.Sites");
            DropIndex("dbo.QueuedTasks", new[] { "Site_Id" });
            DropIndex("dbo.ScheduledTasks", new[] { "Site_Id" });
            DropIndex("dbo.Logs", new[] { "Site_Id" });
            DropIndex("dbo.Settings", new[] { "Site_Id" });
            DropIndex("dbo.QueuedMessages", new[] { "Site_Id" });
            DropIndex("dbo.QueuedMessageAttachments", new[] { "Site_Id" });
            DropIndex("dbo.QueuedMessageAttachments", new[] { "QueuedMessage_Id" });
            DropIndex("dbo.FrontEndDisallowedRoles", new[] { "Webpage_Id" });
            DropIndex("dbo.FrontEndDisallowedRoles", new[] { "UserRole_Id" });
            DropIndex("dbo.FrontEndDisallowedRoles", new[] { "Site_Id" });
            DropIndex("dbo.FrontEndAllowedRoles", new[] { "Webpage_Id" });
            DropIndex("dbo.FrontEndAllowedRoles", new[] { "UserRole_Id" });
            DropIndex("dbo.FrontEndAllowedRoles", new[] { "Site_Id" });
            DropIndex("dbo.AdminDisallowedRoles", new[] { "Webpage_Id" });
            DropIndex("dbo.AdminDisallowedRoles", new[] { "UserRole_Id" });
            DropIndex("dbo.AdminDisallowedRoles", new[] { "Site_Id" });
            DropIndex("dbo.AdminAllowedRoles", new[] { "Webpage_Id" });
            DropIndex("dbo.AdminAllowedRoles", new[] { "UserRole_Id" });
            DropIndex("dbo.AdminAllowedRoles", new[] { "Site_Id" });
            DropIndex("dbo.DocumentVersions", new[] { "User_Id" });
            DropIndex("dbo.DocumentVersions", new[] { "Site_Id" });
            DropIndex("dbo.DocumentVersions", new[] { "Document_Id" });
            DropIndex("dbo.UserLogins", new[] { "User_Id" });
            DropIndex("dbo.UserClaims", new[] { "User_Id" });
            DropIndex("dbo.UserUserRoles", new[] { "UserRole_Id" });
            DropIndex("dbo.UserUserRoles", new[] { "User_Id" });
            DropIndex("dbo.Widgets", new[] { "Webpage_Id3" });
            DropIndex("dbo.UrlHistories", new[] { "Webpage_Id" });
            DropIndex("dbo.UrlHistories", new[] { "Site_Id" });
            DropIndex("dbo.Widgets", new[] { "Webpage_Id2" });
            DropIndex("dbo.Widgets", new[] { "Webpage_Id1" });
            DropIndex("dbo.WebpageUserRoles", new[] { "UserRole_Id" });
            DropIndex("dbo.WebpageUserRoles", new[] { "Webpage_Id" });
            DropIndex("dbo.FormProperties", new[] { "Webpage_Id" });
            DropIndex("dbo.FormProperties", new[] { "Site_Id" });
            DropIndex("dbo.FormListOptions", new[] { "Site_Id" });
            DropIndex("dbo.FormListOptions", new[] { "FormProperty_Id" });
            DropIndex("dbo.FormPostings", new[] { "Webpage_Id" });
            DropIndex("dbo.FormPostings", new[] { "Site_Id" });
            DropIndex("dbo.FormValues", new[] { "Site_Id" });
            DropIndex("dbo.FormValues", new[] { "FormPosting_Id" });
            DropIndex("dbo.MediaFiles", new[] { "Site_Id" });
            DropIndex("dbo.ResizedImages", new[] { "Site_Id" });
            DropIndex("dbo.ResizedImages", new[] { "MediaFile_Id" });
            DropIndex("dbo.MediaFiles", new[] { "MediaCategory_Id" });
            DropIndex("dbo.Documents", new[] { "Layout_Id" });
            DropIndex("dbo.LayoutAreas", new[] { "Site_Id" });
            DropIndex("dbo.Widgets", new[] { "Webpage_Id" });
            DropIndex("dbo.Widgets", new[] { "Site_Id" });
            DropIndex("dbo.Documents", new[] { "Widget_Id1" });
            DropIndex("dbo.PageWidgetSorts", new[] { "Widget_Id" });
            DropIndex("dbo.Widgets", new[] { "LayoutArea_Id" });
            DropIndex("dbo.Documents", new[] { "Widget_Id" });
            DropIndex("dbo.PageWidgetSorts", new[] { "Webpage_Id" });
            DropIndex("dbo.PageWidgetSorts", new[] { "Site_Id" });
            DropIndex("dbo.PageWidgetSorts", new[] { "LayoutArea_Id" });
            DropIndex("dbo.LayoutAreas", new[] { "Layout_Id" });
            DropIndex("dbo.Tags", new[] { "Site_Id" });
            DropIndex("dbo.TagDocuments", new[] { "Document_Id" });
            DropIndex("dbo.TagDocuments", new[] { "Tag_Id" });
            DropIndex("dbo.Documents", new[] { "Site_Id" });
            DropIndex("dbo.Documents", new[] { "Parent_Id" });
            DropIndex("dbo.ACLRoles", new[] { "UserRole_Id" });
            DropIndex("dbo.ACLRoles", new[] { "Site_Id" });
            DropIndex("dbo.RedirectedDomains", new[] { "Site_Id" });
            DropTable("dbo.UserUserRoles");
            DropTable("dbo.WebpageUserRoles");
            DropTable("dbo.TagDocuments");
            DropTable("dbo.QueuedTasks");
            DropTable("dbo.ScheduledTasks");
            DropTable("dbo.Logs");
            DropTable("dbo.Settings");
            DropTable("dbo.QueuedMessageAttachments");
            DropTable("dbo.QueuedMessages");
            DropTable("dbo.FrontEndDisallowedRoles");
            DropTable("dbo.FrontEndAllowedRoles");
            DropTable("dbo.AdminDisallowedRoles");
            DropTable("dbo.AdminAllowedRoles");
            DropTable("dbo.DocumentVersions");
            DropTable("dbo.UserLogins");
            DropTable("dbo.UserClaims");
            DropTable("dbo.Users");
            DropTable("dbo.UrlHistories");
            DropTable("dbo.FormProperties");
            DropTable("dbo.FormListOptions");
            DropTable("dbo.FormValues");
            DropTable("dbo.FormPostings");
            DropTable("dbo.ResizedImages");
            DropTable("dbo.MediaFiles");
            DropTable("dbo.Widgets");
            DropTable("dbo.PageWidgetSorts");
            DropTable("dbo.LayoutAreas");
            DropTable("dbo.Documents");
            DropTable("dbo.Tags");
            DropTable("dbo.UserRoles");
            DropTable("dbo.RedirectedDomains");
            DropTable("dbo.Sites");
            DropTable("dbo.ACLRoles");
        }
    }
}
