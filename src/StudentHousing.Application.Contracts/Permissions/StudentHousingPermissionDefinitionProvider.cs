using StudentHousing.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace StudentHousing.Permissions
{
    public class StudentHousingPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(StudentHousingPermissions.GroupName);
            //Define your own permissions here. Example:
            //myGroup.AddPermission(StudentHousingPermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<StudentHousingResource>(name);
        }
    }
}
