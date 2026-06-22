using System.Data.Entity.Migrations;

namespace Laboratorio_del_Tema_5_2.Data.EntityModel.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ModeloDualContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Laboratorio_del_Tema_5_2.Data.EntityModel.ModeloDualContext";
        }

        protected override void Seed(ModeloDualContext context)
        {
            // Datos iniciales (seed) se agregan aquí cuando sea necesario
        }
    }
}
