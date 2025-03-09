using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork1.Data;
using SocialNetwork1.Entities;
using SocialNetwork1.Models;
using System.Diagnostics;

namespace SocialNetwork1.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<CustomIdentityUser> _userManager;
        private readonly SocialNetworkDbContext _context;

        public HomeController(ILogger<HomeController> logger,
            UserManager<CustomIdentityUser> userManager,
            SocialNetworkDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = user;
            return View();
        }

        public async Task<ActionResult> GetAllUsers()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var myRequests = _context.FriendRequests.Where(r => r.SenderId == user.Id);


            var users = await _context.Users
                .Where(u => u.Id != user.Id)
                .Select(u=>new CustomIdentityUser
                {
                    Id=u.Id,
                    UserName=u.UserName,
                    IsOnline=u.IsOnline,
                    Image=u.Image,
                    Email=u.Email,
                    HasRequestPending=(myRequests.FirstOrDefault(r=>r.ReceiverId==u.Id && r.Status=="Request")!=null)
                })
                .ToListAsync();

            return Ok(users);
        }

        public async Task<ActionResult> SendFollow(string id)
        {
            var sender = await _userManager.GetUserAsync(HttpContext.User);
            var receiverUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (receiverUser != null)
            {
                await _context.FriendRequests.AddAsync(new FriendRequest
                {
                    Content = $"{sender.UserName} sent friend request at {DateTime.Now.ToLongDateString()}",
                    SenderId = sender.Id,
                    Sender = sender,
                    ReceiverId = id,
                    Status = "Request"
                });

                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }


        [HttpDelete]
        public async Task<ActionResult> TakeRequest(string id)
        {
            var current = await _userManager.GetUserAsync(HttpContext.User);
            var request=await _context.FriendRequests.FirstOrDefaultAsync(r=>r.SenderId==current.Id&& r.ReceiverId==id);
            if (request == null) return NotFound();
            _context.FriendRequests.Remove(request);
            await _context.SaveChangesAsync();
            return Ok();
        }

        public async Task<ActionResult> GetAllRequests()
        {
            var current = await _userManager.GetUserAsync(HttpContext.User);
            var requests = _context.FriendRequests.Where(r => r.ReceiverId == current.Id);
            return Ok(requests);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
