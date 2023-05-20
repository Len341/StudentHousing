using Volo.Abp.Domain.Entities;

namespace StudentHousing.Attachments
{
    public class Attachment : Entity<int>
    {
        public string FileName { get; set; }
        public string AttachmentName { get; set; }
        public string Url { get; set; }
    }
}
