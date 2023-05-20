using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace StudentHousing.Attachments
{
    public interface IAttachmentRepository : IRepository
    {
        Task<List<Attachment>> GetListAsync();
        Task DeleteAsync(int id);
        Task<Attachment> AddAsync(Attachment attachment);
    }
}
