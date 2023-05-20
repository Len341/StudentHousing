using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using StudentHousing.Localization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace StudentHousing.Attachments
{
    public class AttachmentAppService : ApplicationService, IAttachmentAppService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<StudentHousingResource> _l;

        public AttachmentAppService(
            IAttachmentRepository attachmentRepository,
            IConfiguration configuration,
            IStringLocalizer<StudentHousingResource> localizer)
        {
            _attachmentRepository = attachmentRepository;
            _configuration = configuration;
            _l = localizer;
        }

        public async Task<AttachmentDto> AddAsync(AttachmentDto attachment)
        {
            //add to server
            Directory.CreateDirectory(Path.GetDirectoryName(attachment.Url));
            File.Create(attachment.Url);
            //add to db
            var newAttachment = await _attachmentRepository.AddAsync(ObjectMapper.Map<AttachmentDto, Attachment>(attachment));
            return ObjectMapper.Map<Attachment, AttachmentDto>(newAttachment);
        }

        public async Task DeleteAsync(int id)
        {
            //delete from db
            var file = (await _attachmentRepository.GetListAsync()).FirstOrDefault(z => z.Id == id);
            await _attachmentRepository.DeleteAsync(id);
            //delete from server
            if(file != null)
            {
                File.Delete(file.Url);
            }
        }

        public async Task<List<AttachmentDto>> GetListAsync()
        {
            return ObjectMapper.Map<List<Attachment>, List<AttachmentDto>>(await _attachmentRepository.GetListAsync());
        }
    }
}
