using System.Threading.Tasks;

namespace StudentHousing.Data
{
    public interface IStudentHousingDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
