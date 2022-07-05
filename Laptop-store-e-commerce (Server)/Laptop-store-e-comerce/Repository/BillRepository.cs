using Laptop_store_e_comerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laptop_store_e_comerce.Repository
{
    public interface IBillRepository
    {
        Task<ActionResult<List<Bill>>> getBillsByIdUser(string id);
    }
    public class BillRepository : IBillRepository
    {
        StoreContext _context;
        public BillRepository(StoreContext context)
        {
            this._context = context;
        }
        public virtual async Task<ActionResult<List<Bill>>> getBillsByIdUser(string id)
        {
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@idCustomer",id)
            };
            List<Bill> bills = await _context.Bills.FromSqlRaw($"getBillsByCustomer {id}").ToListAsync();

            return bills;
        }
    }
}
