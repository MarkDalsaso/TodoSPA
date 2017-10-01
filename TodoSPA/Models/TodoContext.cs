using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using TodoApi.Models.Mapping;
using System.Configuration;

namespace TodoApi.Models
{
    public partial class TodoContext : DbContext
    {
        private const bool default_EnableDemoMode = false;   // false, not a demo

        static TodoContext()
        {
            Database.SetInitializer<TodoContext>(null);
        }

        public TodoContext()
            : base("Name=TodoContext")
        {
            
        }

        private static bool AppInDemoMode()
        {
            bool boolReturn = default_EnableDemoMode;
            bool boolTemp;

            // Convert from string to check for null (missing AppSetting), and set default based on 'dbpacranerigging:EnableDemoMode'
            string strSettingValue = ConfigurationManager.AppSettings["EnableDemoMode"];

            // try String to Boolean conversion
            if (strSettingValue != null && System.Boolean.TryParse(strSettingValue, out boolTemp))
                boolReturn = boolTemp;



            return boolReturn;
        }

        public DbSet<tblTodo> tblTodos { get; set; }
        public DbSet<tblUser> tblUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new tblTodoMap());
            modelBuilder.Configurations.Add(new tblUserMap());
        }

        public override int SaveChanges()
        {
            // Done by DB triggers, see method below
            if (!AppInDemoMode())     // Only allow changes for demo SuperUser 
            {
                return base.SaveChanges();
            }
            else
            {
                return 1;
            }

        }

    }
}
