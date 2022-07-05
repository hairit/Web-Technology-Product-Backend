using Laptop_store_e_comerce.Models;
using Laptop_store_e_comerce.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laptop_store_e_comerce.Services
{
    public class BillService
    {
        private IBillRepository billRepository;
        public BillService(IBillRepository BillRepository)
        {
            this.billRepository = BillRepository;
        }
        public Task<ActionResult<List<Bill>>> getBillsByIduser(string id)
        {
            return billRepository.getBillsByIdUser(id);
        }
    }
}
