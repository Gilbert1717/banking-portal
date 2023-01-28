using System.Text.RegularExpressions;
using ImageMagick;
using McbaSystem.Data;
using McbaSystem.Filters;
using McbaSystem.Models;
using McbaSystem.ViewModels.Profile;
using Microsoft.AspNetCore.Mvc;
using SimpleHashing.Net;

namespace McbaSystem.Controllers;

[AuthorizeCustomer]
public class ProfileController : Controller
{
    private readonly McbaContext _context;

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public ProfileController(McbaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Customers.FindAsync(CustomerID));
    }

    public async Task<IActionResult> EditProfile()
    {
        return View(await _context.Customers.FindAsync(CustomerID));
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(Customer customer)
    {
        EditProfileValidation(customer);
        if (!ModelState.IsValid)
        {
            return View(customer);
        }

        _context.Update(customer);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> UpdatePassword()
    {
        Customer customer = await _context.Customers.FindAsync(CustomerID);
        return View(
            new UpdatePasswordViewModel
            {
                Login = customer.Login
            }
        );
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel model)
    {
        UpdatePasswordValidation(model);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        Login login = model.Login;
        login.PasswordHash = new SimpleHash().Compute(model.NewPassword);
        _context.Update(login);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> UpdateProfilePicture()
    {
        return View(await _context.Customers.FindAsync(CustomerID));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfilePicture(IFormFile file)
    {
        Customer customer = await _context.Customers.FindAsync(CustomerID);

        FileValidation(file);
        if (!ModelState.IsValid)
        {
            return View(customer);
        }

        byte[] imageInBytes = await ConvertImageToBytes(file);
        if (customer.ProfilePicture != null)
        {
            customer.ProfilePicture.Image = imageInBytes;
            _context.Update(customer.ProfilePicture);
        }
        else
        {
            _context.Add(new ProfilePicture
            {
                Image = imageInBytes,
                CustomerID = CustomerID
            });
        }

        HttpContext.Session.SetString(nameof(Customer.ProfilePicture), Convert.ToBase64String(imageInBytes));

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DeleteProfilePicture()
    {
        var customer = await _context.Customers.FindAsync(CustomerID);
        _context.Remove(customer.ProfilePicture);
        await _context.SaveChangesAsync();

        HttpContext.Session.Remove(nameof(Customer.ProfilePicture));

        return RedirectToAction(nameof(Index));
    }

    private void EditProfileValidation(Customer customer)
    {
        if (customer.PostCode != null && !Regex.IsMatch(customer.PostCode, @"[0-9]{4}"))
        {
            ModelState.AddModelError(nameof(customer.PostCode), "Invalid postcode");
        }

        if (customer.TFN != null && !Regex.IsMatch(customer.TFN, @"[0-9]{3} [0-9]{3} [0-9]{3}"))
        {
            ModelState.AddModelError(nameof(customer.TFN), "Incorrect format - please follow XXX XXX XXX");
        }

        if (customer.Mobile != null && !Regex.IsMatch(customer.Mobile, @"04[0-9]{2} [0-9]{3} [0-9]{3}"))
        {
            ModelState.AddModelError(nameof(customer.Mobile), "Incorrect format - please follow 04XX XXX XXX");
        }
    }

    private void UpdatePasswordValidation(UpdatePasswordViewModel model)
    {
        if (!model.NewPassword.Equals(model.PasswordConfirm))
        {
            ModelState.AddModelError(nameof(model.PasswordConfirm), "New passwords do not match");
        }

        if (!new SimpleHash().Verify(model.OldPassword, model.Login.PasswordHash))
        {
            ModelState.AddModelError(nameof(model.OldPassword), "Incorrect old password");
        }
    }

    private void FileValidation(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError(nameof(Customer.ProfilePicture), "Please choose a file before upload");
        }
    }

    private async Task<byte[]> ConvertImageToBytes(IFormFile file)
    {
        // Set the image location under WWWRoot folder. 
        var path = Path.Combine("wwwroot", "temp", file.FileName);
        // Saving the image in that folder 
        await using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
            stream.Close();
        }

        // Read image from file 
        byte[] data;
        using (var image = new MagickImage(path))
        {
            // Sets the output format to jpeg
            image.Format = MagickFormat.Jpeg;

            var size = new MagickGeometry(400, 400);
            image.Resize(size);

            data = image.ToByteArray();
        }

        // Clean up file in WWWRoot
        System.IO.File.Delete(path);
        return data;
    }
}