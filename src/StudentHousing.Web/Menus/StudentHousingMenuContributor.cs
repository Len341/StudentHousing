using System.Threading.Tasks;
using StudentHousing.Localization;
using StudentHousing.MultiTenancy;
using StudentHousing.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;

namespace StudentHousing.Web.Menus
{
    public class StudentHousingMenuContributor : IMenuContributor
    {
        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenuAsync(context);
            }
        }

        private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            var administration = context.Menu.GetAdministration();
            var l = context.GetLocalizer<StudentHousingResource>();

            context.Menu.Items.Insert(
                0,
                new ApplicationMenuItem(
                    StudentHousingMenus.Home,
                    l["Menu:Home"],
                    "~/",
                    icon: "fas fa-home",
                    order: 0
                )
            );

            if (await context.IsGrantedAsync(StudentHousingPermissions.Housing.View))
            {
                context.Menu.AddItem(
                new ApplicationMenuItem(
                    StudentHousingMenus.Housing,
                    "Housing",
                    "/Housing",
                    "fa fa-house"
                ));
            }


            if (MultiTenancyConsts.IsEnabled)
            {
                administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
            }
            else
            {
                administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
            }

            administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
            administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);
        }
    }
}
