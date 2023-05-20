using Volo.Abp.Application.Dtos;

namespace StudentHousing.Attachments
{
    public class AttachmentDto : EntityDto<int>
    {
        public string FileName { get; set; }
        public string AttachmentName { get; set; }
        public string Url { get; set; }
    }
}
