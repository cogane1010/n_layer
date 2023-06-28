using App.BookingOnline.Data.Models;
using App.BookingOnline.Data.Paging;
using App.Core.Domain;
using App.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using App.Core.Helper;
using Dapper;
using System.Data;
using System.Linq;
using System;
using App.Core;

namespace App.BookingOnline.Data.Repositories
{
    public class UploadFileRepository : Repository<UploadFile>, IUploadFileRepository
    {
        public UploadFileRepository(BookingOnlineDbContext context)
            : base(context)
        {
        
        }
    }

   
}