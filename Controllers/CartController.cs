using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeCareForAll.Models.Domain;
using WeCareForAll.Models.DTO;

namespace WeCareForAll.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles="User")]
    public class CartController : ControllerBase
    {
        private readonly DatabaseContext context;
        public CartController(DatabaseContext context)
        {
            this.context = context;
        }
        [HttpPost]
        public async Task<IActionResult> AddtoCart(string id, int quantity)
        {
                var med = await context.drugs.FindAsync(id);
                if (med == null)
                {
                    return NotFound();
                }
                if (med.Quantity < quantity)

                {
                    return BadRequest($"In Sufficient quantity of {med.DrugName} in stock ");
                }

                if (context.carts.Any(c => c.Id == id))
                {
                    var existingitem = context.carts.FirstOrDefault(c => c.Id == id);
                    existingitem.Quantity += quantity;
                    existingitem.TotalPrice += quantity * med.Price;

                }
                else
                    context.carts.Add(new Cart
                    {
                        Id = id,

                        Quantity = quantity,
                        TotalPrice = quantity * med.Price
                    });
                med.Quantity -= quantity;

                await context.SaveChangesAsync();
                return Ok();
            }
      
        

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
                var item = await context.carts.Include(x => x.drugs).ToListAsync();
                var cartitem = item.Select(x => new Cart
                {
                    itemid = x.itemid,
                    drugs = x.drugs,
                    Quantity = x.Quantity,
                    TotalPrice = x.TotalPrice,
                }).ToList();

                return Ok(cartitem);
            }
         
        
    }
}
