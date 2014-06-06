namespace PhotoGallery2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Userremovedfromcommentmodel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Comment", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.Comment", new[] { "UserID" });
            AddColumn("dbo.Comment", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Comment", "User_Id");
            AddForeignKey("dbo.Comment", "User_Id", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.Comment", "UserID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comment", "UserID", c => c.String(maxLength: 128));
            DropForeignKey("dbo.Comment", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Comment", new[] { "User_Id" });
            DropColumn("dbo.Comment", "User_Id");
            CreateIndex("dbo.Comment", "UserID");
            AddForeignKey("dbo.Comment", "UserID", "dbo.AspNetUsers", "Id");
        }
    }
}
