using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentHousing.Attachments;
using StudentHousing.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace StudentHousing.Housing
{
    public class EfCoreHousingRepository : EfCoreRepository<StudentHousingDbContext, Property, Guid>, IHousingRepository
    {
        private readonly IConfiguration _configuration;
        public EfCoreHousingRepository(
            IDbContextProvider<StudentHousingDbContext> dbContextProvider,
            IConfiguration iConfig) : base(dbContextProvider)
        {
            _configuration = iConfig;
        }

        public async Task<Property> CreateAsync(Property property)
        {
            var ctx = await GetDbContextAsync();
            if (property.Attachments != null)
            {
                property.Attachments.ForEach(attachment =>
                {
                    if (attachment.Id == 0)
                    {
                        //add
                        ctx.Entry(attachment).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    }
                    else
                    {
                        //update
                        ctx.Entry(attachment).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                });
            }
            if (property.RegistrationNumbers != null)
            {
                property.RegistrationNumbers.ForEach(reg =>
                {
                    if (reg.Id == Guid.Empty)
                    {
                        //add
                        ctx.Entry(reg).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    }
                    else
                    {
                        //update
                        ctx.Entry(reg).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                });
            }

            await ctx.AddAsync(property);
            await ctx.SaveChangesAsync();
            return property;
        }

        public async Task<PropertyEnquiry> CreatePropertyEnquiryAsync(PropertyEnquiry enquiry)
        {
            var ctx = await GetDbContextAsync();
            await ctx.PropertyEnquiry.AddAsync(enquiry);
            await ctx.SaveChangesAsync();
            return enquiry;
        }

        public async Task DeleteAsync(Guid id)
        {
            var ctx = await GetDbContextAsync();
            var property = await ctx.Property
                .Include(z => z.RegistrationNumbers)
                .Include(z => z.Attachments)
                .FirstOrDefaultAsync(z => z.Id == id);
            ctx.Property.Remove(property);
            await ctx.SaveChangesAsync();
        }

        public async Task<Property> GetAsync(Guid id)
        {
            return await (await GetDbContextAsync()).Property
                .Include(z => z.RegistrationNumbers)
                .Include(z => z.Attachments)
                .FirstOrDefaultAsync(z => z.Id == id);
        }

        public async Task<List<Property>> GetListAsync()
        {
            return await (await GetDbContextAsync()).Property
                .Include(z => z.RegistrationNumbers)
                .Include(z => z.Attachments)
                .ToListAsync();
        }

        public async Task<List<PropertyEnquiry>> GetPropertyEnquiryListAsync()
        {
            var ctx = await GetDbContextAsync();
            return await ctx.PropertyEnquiry.ToListAsync();
        }

        public async Task<Property> UpdateAsync(Property property)
        {
            try
            {
                var ctx = await GetDbContextAsync();
                ctx.DetachAllEntities();
                if (property.Attachments != null)
                {
                    property.Attachments.ForEach(attachment =>
                    {
                        if (attachment.Id == 0)
                        {
                            //add
                            ctx.Entry(attachment).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                        }
                        else
                        {
                            //update
                            ctx.Entry(attachment).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                        }
                    });
                }
                if (property.RegistrationNumbers != null)
                {
                    property.RegistrationNumbers.ForEach(reg =>
                    {
                        if (reg.Id == Guid.Empty)
                        {
                            //add
                            ctx.Entry(reg).State = EntityState.Added;
                        }
                        else
                        {
                            //update
                            ctx.Entry(reg).State = EntityState.Modified;
                        }
                    });
                }

                ctx.Update(property);
                await ctx.SaveChangesAsync();
                return property;
            }catch(Exception ex) {
                throw ex;
            }
        }
    }
}
