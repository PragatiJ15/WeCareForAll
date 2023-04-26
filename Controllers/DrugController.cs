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
   

    public class DrugController : ControllerBase
    {
        private readonly DatabaseContext context;
        public DrugController(DatabaseContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetALlDrugs()
        {
            return Ok(await context.drugs.ToListAsync());
        }
        [HttpGet]
        [Route("{id}")]
        public IActionResult searchDrug([FromRoute] string id )
        {
            var obj = context.drugs.Find(id);
            if(obj == null)
            {
                return NotFound("required medicine cannot be found");
            }
            else 
                return Ok(obj); 
        }

        [HttpPost]
        //[Authorize(Roles ="admin")]

        public async Task<IActionResult> AddNewDrug(Drugs drug)
        {
            if (UserRoles.Admin == "Admin")
            {


                var d = new Drugs()
                {
                    Id = drug.Id,
                    DrugName = drug.DrugName,
                    Quantity = drug.Quantity,
                    Price = drug.Price,

                };
                await context.drugs.AddAsync(d);
                await context.SaveChangesAsync();
                return Ok(d);
            }
            else
                return BadRequest("Not Authorized");
        }
        [HttpPut]

        [Route("{id}")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateDrug([FromRoute] string id, Drugs updatedrug)
        {

            if (UserRoles.Admin == "Admin")
            {
                var obj = await context.drugs.FindAsync(id);
                if (obj != null)
                {


                    obj.DrugName = updatedrug.DrugName;
                    obj.Quantity = updatedrug.Quantity;
                    obj.Price = updatedrug.Price;


                    await context.SaveChangesAsync();
                    return Ok(obj);
                }
                return NotFound();
            }
            else
                return BadRequest("Not Authorized");

        }

        [HttpDelete]
        //[Authorize(Roles = "admin")]
        [Route("{id}")]
        public async Task<IActionResult> DeleteContact([FromRoute] string id)
        {

            if (UserRoles.Admin == "Admin")
            {
                var obj = context.drugs.Find(id);
                if (obj != null)
                {
                    context.drugs.Remove(obj);
                    await context.SaveChangesAsync();
                    return Ok(obj);
                }
                return NotFound();
            }
            else
                return BadRequest("Not Authorized");
        }
    }
}
