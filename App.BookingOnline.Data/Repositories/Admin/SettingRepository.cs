using App.BookingOnline.Data.Models;
using App.BookingOnline.Data.Paging;
using App.Core.Domain;
using App.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Core.Helper;

namespace App.BookingOnline.Data.Repositories
{
    public class SettingRepository : GridRepository<Setting, SettingPagingModel>, ISettingRepository
    {
        public SettingRepository(BookingOnlineDbContext context)
            : base(context)
        { }

        public override PagingResponseEntity<Setting> GetPaging(SettingPagingModel pagingModel)
        {
            var query = this.dbSet.AsQueryable().Where(x => pagingModel.Code.IsNullOrEmpty() || x.Code.Contains(pagingModel.Code)).OrderByDescending(d => d.CreatedDate);
            var result = new PagingResponseEntity<Setting>
            {
                Data = query.OrderBy(x => x.Id)
                           .Skip(pagingModel.PageIndex * pagingModel.PageSize)
                           .Take(pagingModel.PageSize).ToList(),
                Count = query.Count()
            };
            return result;
            
        }

        public string GetSetting(string code)
        {
            return this.dbSet.Where(x => x.Code == code).FirstOrDefault()?.Value;
        }
    }
}