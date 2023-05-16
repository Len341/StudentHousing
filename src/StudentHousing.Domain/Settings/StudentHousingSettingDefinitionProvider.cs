using Volo.Abp.Settings;

namespace StudentHousing.Settings
{
    public class StudentHousingSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(StudentHousingSettings.MySetting1));
        }
    }
}
