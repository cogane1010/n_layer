using App.BookingOnline.Data.Models;
using App.BookingOnline.Data.Paging;
using App.Core.Domain;
using App.Core.Repositories;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using App.Core.Helper;
using System;
using App.BookingOnline.Data.Identity;
using System.Linq.Expressions;

namespace App.BookingOnline.Data.Repositories
{
    public class RoleRepository : GenericGridRepository<AspRole, RolePagingModel, string>, IRoleRepository
    {
        public RoleRepository(BookingOnlineDbContext context)
            : base(context)
        { }

        public override PagingResponseGenericEntity<AspRole, string> GetPaging(RolePagingModel pagingModel)
        {
            var query = this.dbSet.AsQueryable().Where(x => pagingModel.Name.IsNullOrEmpty() || x.DisplayName.Contains(pagingModel.Name));
            var result = new PagingResponseGenericEntity<AspRole, string>
            {
                Data = query.OrderBy(x => x.Id)
                           .Skip(pagingModel.PageIndex * pagingModel.PageSize)
                           .Take(pagingModel.PageSize).ToList(),
                Count = query.Count()
            };
            return result;

        }

        public override void Update(AspRole entity)
        {
            var dbEntity = dbSet.Find(entity.Id);
            dbEntity.Name = entity.Name;
            dbEntity.NormalizedName = entity.NormalizedName;
            dbEntity.IsActive = entity.IsActive;
            dbEntity.Description = entity.Description;
            dbEntity.DisplayName = entity.DisplayName;
            base.UpdateByProperties(dbEntity, new List<Expression<Func<AspRole, object>>>() {
                { x => x.Name },
                { x => x.NormalizedName },
                { x => x.IsActive },
                { x => x.Description },
                { x => x.DisplayName },
            });
            Context.SaveChanges();
        }
        public List<Menu> GetMenuPermisstion(Guid Id)
        {
            string conString = this.Context.Database.GetDbConnection().ConnectionString;
            using (var conn = new SqlConnection(conString))
            {
                var p = new DynamicParameters();
                p.Add("Id", Id);
                var res = conn.ExecuteProcedure<Menu>("App_Role_getMenuPermisstion", p);
                return res.Item1.ToList();
            }
        }





    }
}