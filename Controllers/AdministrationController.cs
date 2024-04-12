using IntexLegoSecure.Data;
using IntexLegoSecure.Models;
using IntexLegoSecure.Models.ViewModels;
using IntexLegoSecure.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace IntexLegoSecure.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {



        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }



    // User Management -------------------------------------------------------------------------------------------
        
        // ListUsers
        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users;
            return View(users);
        }

        // EditUser Get
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            // GetRolesAsync returns the list of user Roles
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Country = user.Country,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);
        }
        // EditUser Post
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Country = model.Country;
                user.DateOfBirth = model.DateOfBirth;
                user.Gender = model.Gender;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        //DeleteUser Post
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListUsers");
            }
        }

    // Role Management -------------------------------------------------------------------------------------------
        
        public IActionResult AdminTools()
        {
            return View();
        }
        
        
        
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);

                //userRoleViewModel.IsSelected = await _userManager.IsInRoleAsync(user, role.Name);
                //model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);
                
                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }

        // Product Management -------------------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult AdminProducts()
        {
            var products = _context.Products;
            return View(products);
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                Product product = new Product
                {
                    Name = model.Name,
                    Year = model.Year,
                    NumParts = model.NumParts,
                    Price = model.Price,
                    ImgLink = model.ImgLink,
                    PrimaryColor = model.PrimaryColor,
                    SecondaryColor = model.SecondaryColor,
                    Description = model.Description,
                    Category = model.Category
                };

                _context.Add(product);

                await _context.SaveChangesAsync();

                return RedirectToAction("AdminProducts", "Administration");
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Query the database for the product with the specific ID
            var product = await _context.Products
                                        .FirstOrDefaultAsync(p => p.ProductId == id);

            // If no product is found, return a NotFound result
            if (product == null)
            {
                ViewBag.ErrorMessage = $"Product with Id = {id} cannot be found";
                return View("NotFound");
            }

            // Remove the product from the database context
            _context.Products.Remove(product);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Redirect to a view showing all products after the deletion is completed
            return RedirectToAction("AdminProducts", "Administration");  // Adjust "Index" to your actual list view if necessary
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products
                                        .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                ViewBag.ErrorMessage = $"Product with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Year = product.Year,
                NumParts = product.NumParts,
                Price = product.Price,
                ImgLink = product.ImgLink,
                PrimaryColor = product.PrimaryColor,
                SecondaryColor = product.SecondaryColor,
                Description = product.Description,
                Category = product.Category
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(int id, EditProductViewModel model)
        {
            var product = await _context.Products
                                        .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.ProductId} cannot be found";
                return View("NotFound");
            }
            else
            {
                product.Name = model.Name;
                product.Year = model.Year;
                product.NumParts = model.NumParts;
                product.Price = model.Price;
                product.ImgLink = model.ImgLink;
                product.PrimaryColor = model.PrimaryColor;
                product.SecondaryColor = model.SecondaryColor;
                product.Description = model.Description;
                product.Category = model.Category;


                _context.Update(product);

                await _context.SaveChangesAsync();

                return RedirectToAction("AdminProducts", "Administration");
            }
        }
    }
}
