using Lab3_DuongLeMinhQuang.Models;

namespace Lab3_DuongLeMinhQuang.Areas.Admin.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
