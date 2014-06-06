namespace PhotoGallery2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Commenttablechanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comment", "Description", c => c.String());
            DropColumn("dbo.Comment", "Comments");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comment", "Comments", c => c.String());
            DropColumn("dbo.Comment", "Description");
        }
    }
}
