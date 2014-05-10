namespace PhotoGallery2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PhotoContentLenghtAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Photo", "ContentLength", c => c.Int(nullable: false));
            DropColumn("dbo.Photo", "DateTaken");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Photo", "DateTaken", c => c.DateTime(nullable: false));
            DropColumn("dbo.Photo", "ContentLength");
        }
    }
}
