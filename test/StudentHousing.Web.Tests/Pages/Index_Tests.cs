using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace StudentHousing.Pages
{
    public class Index_Tests : StudentHousingWebTestBase
    {
        [Fact]
        public async Task Welcome_Page()
        {
            var response = await GetResponseAsStringAsync("/");
            response.ShouldNotBeNull();
        }
    }
}
