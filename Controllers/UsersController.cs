using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Library.Models; 
using Library.Data;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
[Authorize]

public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public UsersController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var userdata = await _userManager.FindByNameAsync(model.Email);
                userdata.EmailConfirmed = true;
                await _userManager.UpdateAsync(userdata);

                return RedirectToAction(nameof(Index));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var model = new EditUserViewModel
        {
            Id = user.Id,
            Email = user.Email
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = model.Email;
            user.UserName = model.Email;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

   public IActionResult Index()
{
    // Get the list of users
    var users = _userManager.Users.ToList();

    // Get the borrowing records once (no ToList() on _context.BorrowingSystems)
    var borrowingSystems = _context.BorrowingSystems
                                   .Include(b => b.BookFk) // Include related BookFk for the title
                                   .ToList();

    // Create the view model
    var model = from x in users
                join p in borrowingSystems on x.Id equals p.UserId into joined
                from subP in joined.DefaultIfEmpty()
                select new USerViewModel
                {
                    Id = x.Id,
                    Email = x.Email,
                    Penality = subP?.Penalty ?? 0,
                    BorrowedDate = subP?.BorrowingDate ?? default(DateTime),
                    ReturnDate = subP?.ReturnDate ?? default(DateTime),
                    ActualReturnDate = subP?.ActualReturnDate ?? default(DateTime),
                    BookTitle = subP?.BookFk?.Title ?? "No Book"
                };

    return View(model.ToList());
}


    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var model = new DeleteUserViewModel
        {
            Id = user.Id,
            Email = user.Email
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var borrowings = await _context.BorrowingSystems.Where(b => b.UserId == id).ToListAsync();

        if (borrowings.Any())
        {
            _context.BorrowingSystems.RemoveRange(borrowings);
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            await _context.SaveChangesAsync(); 
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(new DeleteUserViewModel { Id = user.Id, Email = user.Email }); 
    }
}
