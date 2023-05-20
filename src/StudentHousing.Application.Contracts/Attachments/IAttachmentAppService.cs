using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace StudentHousing.Attachments
{
    public interface IAttachmentAppService : IApplicationService
    {
        Task<List<AttachmentDto>> GetListAsync();
        Task DeleteAsync(int id);
        Task<AttachmentDto> AddAsync(AttachmentDto attachment);
    }
}
