using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TodoApi.Models.Mapping
{
    public class tblUserMap : EntityTypeConfiguration<tblUser>
    {
        public tblUserMap()
        {
            // Primary Key
            this.HasKey(t => t.RecId);

            // Properties
            this.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.LastName)
                .HasMaxLength(50);

            this.Property(t => t.UserName)
                .IsFixedLength()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tblUsers");
            this.Property(t => t.RecId).HasColumnName("RecId");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.UserName).HasColumnName("UserName");
        }
    }
}
