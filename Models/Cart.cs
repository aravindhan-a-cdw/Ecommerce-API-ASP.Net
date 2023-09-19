using System.Diagnostics.CodeAnalysis;

namespace EcommerceAPI.Models
{
    /*
     * @author Aravindhan A
     * @description This is the Cart Model which will be used in DbContext to create Table in Db
     */
    [ExcludeFromCodeCoverage]
    public class Cart
	{
        public int ProductId { get; set; }
        public virtual required Product Product { get; set; }

        public required string UserId { get; set; }
        public virtual required User User { get; set; }

        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

