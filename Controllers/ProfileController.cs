using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proiect_IR.Interfaces;
using Proiect_IR.Models;
using Proiect_IR.Repository;
using Proiect_IR.ViewModels;

namespace Proiect_IR.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ProfileController(IProfileRepository profileRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _profileRepository = profileRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }
        private void MapUserEdit(AppUser user,EditProfileViewModel editProfileVM,ImageUploadResult photoResult)
        {
            user.Id = editProfileVM.Id;
            user.ProfileImageUrl = photoResult.Url.ToString();
            user.UserName = editProfileVM.UserName;
            user.Address = editProfileVM.Address;
        }
        public async Task<IActionResult> Index()
        {
            var userProducts = await _profileRepository.GetAllUserProducts();
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _profileRepository.GetUserById(currentUserId);
            if (user == null) return View("Error");
            var profileViewModel = new ProfileViewModel()
            {
                Id = currentUserId,
                ProfileImageUrl = user.ProfileImageUrl,
                UserName = user.UserName,
                Address = user.Address,
                Products = userProducts,
                
            };
            return View(profileViewModel);
        }

        public async Task<IActionResult> EditUserProfile()
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _profileRepository.GetUserById(currentUserId);
            if (user == null) return View("Error");
            var editProfileViewModel = new EditProfileViewModel()
            {
                Id = currentUserId,
                ProfileImageUrl = user.ProfileImageUrl,
                UserName = user.UserName,
                Address = user.Address,

            };
            return View(editProfileViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditProfileViewModel editProfileVM)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View("EditUserProfile", editProfileVM);
            }
            //maybe switch appuser to var
            AppUser user = await _profileRepository.GetByIdNoTracking(editProfileVM.Id);

            if(user.ProfileImageUrl == "" || user.ProfileImageUrl == null)
            {
                var photoResult = await _photoService.AddPhotoAsync(editProfileVM.Image);
                //Optimistic Concurrency - for "Trackign error" (Use no tracking)
                //use no Tracking
                MapUserEdit(user,editProfileVM,photoResult);
                _profileRepository.Update(user);
                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    await _photoService.DeletePhotoAsync(user.ProfileImageUrl);
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                    return View(editProfileVM);
                }
                var photoResult = await _photoService.AddPhotoAsync(editProfileVM.Image);
                MapUserEdit(user, editProfileVM, photoResult);
                _profileRepository.Update(user);
                return RedirectToAction("Index");
            }

        }



        public IActionResult AddToCart(int id)
        {
            
            var product = _profileRepository.GetById(id);

            
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("cart") ?? new List<CartItemViewModel>();

           
            var cartItem = cart.FirstOrDefault(item => item.Product.Id == id);

            if (cartItem == null)
            {
               
                cart.Add(new CartItemViewModel { Product = product, Quantity = 1 });
            }
            else
            {
               
                cartItem.Quantity++;
            }

            
            HttpContext.Session.SetObjectAsJson("cart", cart);

           
            Response.Cookies.Append("lastAddedProductId", id.ToString(), new CookieOptions { Expires = DateTime.Now.AddDays(1) });

            
            return RedirectToAction("Cart");

        }
        public IActionResult Cart()
            {
                
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("cart") ?? new List<CartItemViewModel>();

                
                return View(cart);
            }
        public IActionResult Checkout()
        {
            
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemViewModel>>("cart");

            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "Product");
            }
  
            HttpContext.Session.Remove("cart");
          
            TempData["Message"] = "Thank you for your purchase!";
            return RedirectToAction("Index", "Product");
        }

    }
}