namespace PhotoGallery2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlbumPlaceTakenAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Album", "PlaceTaken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Album", "PlaceTaken");
        }
    }
}
