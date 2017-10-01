using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace TodoApi.Models.Mapping
{
    public class tblTodoMap : EntityTypeConfiguration<tblTodo>
    {
        public tblTodoMap()
        {
            // Primary Key
            this.HasKey(t => t.RecId);

            // Properties
            this.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(40);

            // Table & Column Mappings
            this.ToTable("tblTodo");
            this.Property(t => t.RecId).HasColumnName("RecId");
            this.Property(t => t.fkUserId).HasColumnName("fkUserId");
            this.Property(t => t.DateCreated).HasColumnName("DateCreated");
            this.Property(t => t.Complete).HasColumnName("Complete");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Details).HasColumnName("Details");
        }
    }
}
