using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    [Table("Order15")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        [DataType(DataType.Currency)]
        public decimal OrderAmount { get; set; }
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }
        public string UserID { get; set; }
    }
}
