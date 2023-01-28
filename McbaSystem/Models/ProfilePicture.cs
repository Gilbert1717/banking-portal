using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McbaSystem.Models;

public class ProfilePicture
{
    [Key]
    [ForeignKey(nameof(Customer))]
    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }

    [Required,Column(TypeName = "varbinary(max)")]
    public byte[] Image { get; set; }
}
