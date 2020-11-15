using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using TransportCompany.Models;

[assembly: OwinStartupAttribute(typeof(TransportCompany.Startup))]
namespace TransportCompany
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateAdminAndUserRoles();
        }

        private void CreateAdminAndUserRoles()
        {
            //Luam contextul bazei de date
            var ctx = new ApplicationDbContext();
            //role manager
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ctx));
            //user manager
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ctx));

            // adaugam rolurile din cadrul aplicatiei

            //Rol administrator + cont default
            if (!roleManager.RoleExists("Admin"))
            {
                // adaugam rolul de administrator
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                // se adauga utilizatorul administrator
                var user = new ApplicationUser();
                user.UserName = "admin@admin.com";
                user.Email = "admin@admin.com";
                var adminCreated = userManager.Create(user, "Admin2020!");
                if (adminCreated.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");
                }
            }

            //Rol de sofer
            if (!roleManager.RoleExists("Driver"))
            {
                // adaugati rolul specific aplicatiei voastre
                var role = new IdentityRole();
                role.Name = "Driver";
                roleManager.Create(role);
            }

            //Rol de client
            if (!roleManager.RoleExists("Client"))
            {
                // adaugati rolul specific aplicatiei voastre
                var role = new IdentityRole();
                role.Name = "Client";
                roleManager.Create(role);
            }
        }
    }
}
