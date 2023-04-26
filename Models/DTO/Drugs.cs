using System.ComponentModel.DataAnnotations;

namespace WeCareForAll.Models.DTO
{
    public class Drugs
    {

       
        public string Id { get; set; }

       [Required(ErrorMessage = "Name is required")]   //not null 
        [MaxLength(30)]
        public string? DrugName { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }



    }
}
