using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McbaSystem.Models;

public class Customer
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int CustomerID { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; }
    [StringLength(11, MinimumLength = 11, ErrorMessage = "Incorrect length - please follow XXX XXX XXX")]
    public string TFN { get; set; }

    [StringLength(50)]
    public string Address { get; set; }

    [StringLength(40)]
    public string City { get; set; }

    [StringLength(3, MinimumLength = 2)]
    public string State { get; set; }

    [StringLength(4, MinimumLength = 4, ErrorMessage = "Invalid postcode")]
    public string PostCode { get; set; }
    
    [StringLength(12, MinimumLength = 12, ErrorMessage = "Incorrect length - please follow 04XX XXX XXX")]
    public string Mobile { get; set; }

    public virtual List<Account> Accounts { get; set; }
    
    public virtual Login Login { get; set; }
    
    public virtual ProfilePicture ProfilePicture { get; set; }
}
