using System.ComponentModel.DataAnnotations;

namespace McbaSystem.Models;

public class Payee
{
    public int PayeeID { get; set; }

    [Required, StringLength(50)] 
    public string Name { get; set; }

    [Required, StringLength(50)]
    public string Address { get; set; }

    [Required, StringLength(40)]
    public string City { get; set; }

    [Required, StringLength(3, MinimumLength = 2)]
    public string State { get; set; }

    [Required, StringLength(4, MinimumLength = 4, ErrorMessage = "Invalid postcode")]
    public string PostCode { get; set; }

    [Required, StringLength(14, MinimumLength = 14, ErrorMessage = "Incorrect length - please follow (0X) XXXX XXXX")]
    public string Phone { get; set; }
}