﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Laptop_store_e_comerce.Models;

namespace Laptop_store_e_comerce.Controllers
{
    [Route("data/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly StoreContext _context;

        public BillController(StoreContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bill>>> GetDonHangs()
        {
            return await _context.Bills.Include(bill => bill.BillDetails).ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<List<Bill>>> GetBillsByID(string id)
        {
            List<Bill> bills = await _context.Bills.Include(bill => bill.IduserNavigation)
                                                    .Include(bill => bill.BillDetails)
                                                    .Where(bill => bill.Id == id).ToListAsync();
            if (bills.Count == 0)
            {
                return NotFound();
            }
            return bills;
        }
        [HttpGet("idCustomer={idCustomer}")]
        public async Task<ActionResult<List<Bill>>> GetBillsByIDCustomer(int idCustomer)
        {
            List<Bill> bills = await _context.Bills.Include(bill => bill.IduserNavigation)
                                                   .Include(bill => bill.BillDetails)
                                                   .Where(bill => bill.Iduser == idCustomer).ToListAsync();
            if (bills.Count == 0)
            {
                return NotFound();
            }
            return bills;
        }
        [HttpGet("getbill/update/{id}")]
        public async Task<ActionResult<Bill>> GetBillToUpdate(string id)
        {
            var bill = await _context.Bills.Include(bill => bill.IduserNavigation)
                                                    .Include(bill => bill.BillDetails).ThenInclude(bill => bill.IdProductNavigation)
                                                   .FirstOrDefaultAsync(bill => bill.Id == id);
            if (bill == null)
            {
                return NotFound();
            }
            return bill;
        }
        [HttpGet("getbill/{id}")]
        public async Task<ActionResult<Bill>> GetBill(string id)
        {
            var bill = await _context.Bills.Include(bill => bill.IduserNavigation)
                                                    .Include(bill => bill.BillDetails).ThenInclude(bill => bill.IdProductNavigation)
                                                   .FirstOrDefaultAsync(bill => bill.Id == id && bill.Tinhtrang == "Đã xác nhận");
            if (bill == null)
            {
                return NotFound();
            }
            return bill;
        }
        [HttpGet("status={status}")]
        public async Task<ActionResult<List<Bill>>> getBillsWithStatus(string status)
        {
            List<Bill> bills;
            if(status != "all")  bills = await _context.Bills.Include(bill => bill.IduserNavigation)
                                                             .Include(bill => bill.BillDetails)
                                                             .Where(bill => bill.Tinhtrang == status).ToListAsync();

            else  bills = await _context.Bills.Include(bill => bill.IduserNavigation).ToListAsync();
            if (bills.Count == 0) return NotFound();
            else return bills;
        }
        [HttpGet("address={value}/{id}")]
        public async Task<ActionResult> updateAddressBill(string value,string id)
        {
            try
            {
                var bill = await _context.Bills.FirstOrDefaultAsync(bill => bill.Id == id);
                bill.Diachinhan = value;
                _context.Entry(bill).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }catch (Exception) { return BadRequest(); }

        }
        [HttpGet("action={action}/{id}")]
        public async Task<ActionResult<Bill>> ActionBill(string action,string id)
        {
            var bill = await _context.Bills.Include(bill => bill.BillDetails).FirstOrDefaultAsync(bill => bill.Id == id);
            if (bill == null) return NotFound();
            if(action == "accept")
            {
                  bill.Tinhtrang = "Đã xác nhận";
                  try
                  {
                        _context.Entry(bill).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        return NoContent();
                  }catch (Exception) { return BadRequest(); }
            }
            if (action == "done")
            {
                if (bill.Tinhtrang != "Đã xác nhận") return BadRequest();
                bill.Tinhtrang = "Đã hoàn thành";
                try
                {
                        _context.Entry(bill).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        return NoContent();
                }
                catch (Exception) { return BadRequest(); }
            }
            if (action == "cancel")
            {
                try
                {
                    _context.BillDetails.RemoveRange(bill.BillDetails);
                    _context.Bills.Remove(bill);
                    await _context.SaveChangesAsync();
                    return NoContent();
                } catch (Exception) { return BadRequest(); }
            }
            return bill;
        }
        [HttpGet("action={action}/billdetail/idbill={value1}/idproduct={value2}")]
        public async Task<ActionResult<Object>> addABillDetail (String action,String value1,String value2)
        {
            var billDetail = await _context.BillDetails.FirstOrDefaultAsync(detail => detail.IdBill == value1 && detail.IdProduct == value2);
            if (billDetail == null) return NotFound();

            var product = await _context.Products.FindAsync(value2);
            if (product == null) return BadRequest();

            var bill = await _context.Bills.FindAsync(value1);
            if(action == "increase")
            {
                billDetail.Soluong += 1;
                billDetail.Tongtien += product.Gia;
                bill.Tongtien += product.Gia;
            }
            if(action == "decrease")
            {
                billDetail.Soluong -= 1;
                billDetail.Tongtien -= product.Gia;
                bill.Tongtien -= product.Gia;
            }
            if(action == "delete")
            {
                bill.Tongtien -= billDetail.Tongtien;
                _context.BillDetails.Remove(billDetail);
            }
            try
            {
                await _context.SaveChangesAsync();

            }catch(Exception) {return BadRequest();
            }
            return NoContent();
        }
        [HttpGet("iduser={id}")]
        public async Task<ActionResult<List<Bill>>> getBillsByUserID(int id)
        {
            if (!_context.Users.Any(user => user.Id == id)) return BadRequest();
            List<Bill> bills = await _context.Bills.Include(bill => bill.BillDetails).Include(bill => bill.IduserNavigation).Where(bill => bill.Iduser == id).ToListAsync();
            if (bills.Count == 0) return NotFound();
            else return bills;
        }
        [HttpPut]
        public async Task<ActionResult<Bill>> PutDonHang(Bill donHang)
        {
            if (!DonHangExists(donHang.Id)) return NotFound();
            var oldBill = await _context.Bills.Include(bill => bill.BillDetails)
                .FirstOrDefaultAsync(bill => bill.Id == donHang.Id);
            _context.BillDetails.RemoveRange(oldBill.BillDetails);
            _context.Bills.Remove(oldBill);
            try
            {
                _context.Bills.Add(donHang);
                await _context.SaveChangesAsync();
            }
            catch (Exception) {
                return BadRequest();
            }
            return CreatedAtAction("GetBill", new { id = donHang.Id }, donHang);
        }
        [HttpPost]
        public async Task<ActionResult<Bill>> PostBill(Bill donHang)
        {
            if (DonHangExists(donHang.Id)) return Conflict();
            try{
                _context.Bills.Add(donHang);
                var cartOrders = await _context.CartDetails.Where(detail => detail.IdUser == donHang.Iduser && detail.Selected == 1).ToListAsync();
                _context.CartDetails.RemoveRange(cartOrders);
                await _context.SaveChangesAsync();}
            catch (Exception)
            {throw;}
            return CreatedAtAction("GetBill", new { id = donHang.Id }, donHang);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<Bill>> DeleteDonHang(string id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null) return NotFound();
            else
            {
                var listBillDetail = await _context.BillDetails.Where(billDetail => billDetail.IdBill == id).ToListAsync();
                try
                {
                    _context.BillDetails.RemoveRange(listBillDetail);
                    _context.Bills.Remove(bill);
                    await _context.SaveChangesAsync();
                }
                catch(Exception) { return BadRequest(); }
            }
            return bill;
        }
        private bool DonHangExists(string id)
        {
            return _context.Bills.Any(e => e.Id == id);
        }

        private bool checkBillDetailExist(string id,string idProduct)
        {
            return _context.BillDetails.Any(detail => detail.IdBill == id && detail.IdProduct == idProduct);
        }
    }
}
