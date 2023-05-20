using StudentHousing.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace StudentHousing.Permissions
{
    public class StudentHousingPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var StudentHousingGroup = context.AddGroup(StudentHousingPermissions.GroupName);

            var grpHousing = StudentHousingGroup.AddPermission(StudentHousingPermissions.Housing.Default, L("Permission:StudentHousing.Housing.Default"));
            grpHousing.AddChild(StudentHousingPermissions.Housing.Create, L("Permission:StudentHousing.Housing.Create"));
            grpHousing.AddChild(StudentHousingPermissions.Housing.View, L("Permission:StudentHousing.Housing.View"));
            grpHousing.AddChild(StudentHousingPermissions.Housing.Update, L("Permission:StudentHousing.Housing.Update"));
            grpHousing.AddChild(StudentHousingPermissions.Housing.Delete, L("Permission:StudentHousing.Housing.Delete"));

        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<StudentHousingResource>(name);
        }
    }
}
