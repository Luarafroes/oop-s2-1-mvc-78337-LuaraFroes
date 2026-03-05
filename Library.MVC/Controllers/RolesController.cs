using Library.MVC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = AppRoles.Admin)]
public class RolesController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    // GET: Roles
    public IActionResult Index()
    {
        return View(_roleManager.Roles);
    }

    // GET: Roles/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Roles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string roleName)
    {
        if (!string.IsNullOrWhiteSpace(roleName))
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (result.Succeeded)
                return RedirectToAction(nameof(Index));

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        return View();
    }

    // POST: Roles/Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role != null)
            await _roleManager.DeleteAsync(role);

        return RedirectToAction(nameof(Index));
    }
}