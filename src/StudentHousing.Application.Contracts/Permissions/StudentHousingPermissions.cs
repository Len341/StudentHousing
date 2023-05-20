namespace StudentHousing.Permissions
{
    public static class StudentHousingPermissions
    {
        public const string GroupName = "StudentHousing";

        //Add your own permission names. Example:
        //public const string MyPermission1 = GroupName + ".MyPermission1";
        public class Housing
        {
            public const string Default = GroupName + ".Housing";
            public const string View = Default + ".View";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }
    }
}