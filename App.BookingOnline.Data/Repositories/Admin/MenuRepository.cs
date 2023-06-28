using App.BookingOnline.Data.Models;
using App.BookingOnline.Data.Paging;
using App.Core.Domain;
using App.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using App.Core.Helper;
using Dapper;
using System.Data;

namespace App.BookingOnline.Data.Repositories
{
    public class MenuRepository : GridRepository<Menu, MenuPagingModel>, IMenuRepository
    {
        public MenuRepository(BookingOnlineDbContext context)
            : base(context)
        { }

        public override PagingResponseEntity<Menu> GetPaging(MenuPagingModel pagingModel)
        {
            string conString = this.Context.Database.GetDbConnection().ConnectionString;
            using (var conn = new SqlConnection(conString))
            {
                var p = new DynamicParameters();
                p.Add("name", pagingModel.Name);
                p.Add("parentName", pagingModel.ParentName);
                p.Add("pageIndex", pagingModel.PageIndex);
                p.Add("pageSize", pagingModel.PageSize);
                p.Add("totalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var res = conn.ExecuteProcedure<Menu>("App_Menu_getPaging", p);
                return new PagingResponseEntity<Menu>
                {
                    Data = res.Item1,
                    Count = res.Item2.Get<int>("totalCount")
                };
            }     
        }


    }
}