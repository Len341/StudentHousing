using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentHousing.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace StudentHousing.Attachments
{
    public class EfCoreAttachmentRepository : EfCoreRepository<StudentHousingDbContext, Attachment, int>, IAttachmentRepository
    {
        private readonly IConfiguration _configuration;
        public EfCoreAttachmentRepository(
            IDbContextProvider<StudentHousingDbContext> dbContextProvider,
            IConfiguration iConfig) : base(dbContextProvider)
        {
            _configuration = iConfig;
        }

        public async Task<Attachment> AddAsync(Attachment attachment)
        {
            var ctx = await GetDbContextAsync();
            var newAttachment = (await ctx.AddAsync(attachment)).Entity;
            await ctx.SaveChangesAsync();
            return newAttachment;
        }

        public async Task DeleteAsync(int id)
        {
            var ctx = await GetDbContextAsync();
            var toDelete = await ctx.Attachment.FirstOrDefaultAsync(z => z.Id == id);
            if (toDelete != null)
            {
                ctx.Remove(toDelete);
                await ctx.SaveChangesAsync();
            }
        }

        public async Task<List<Attachment>> GetListAsync()
        {
            var ctx = await GetDbContextAsync();
            return await ctx.Attachment.ToListAsync();
        }
    }
}
