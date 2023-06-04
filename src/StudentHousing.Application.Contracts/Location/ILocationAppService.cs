using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace StudentHousing.Location
{
    public interface ILocationAppService : IApplicationService
    {
        public Task<int?> GetDistanceInMetersAsync(string fromCoords, string toCoords);
        public Task<string> GetCoordsFromAddress(string Address);
    }
}
