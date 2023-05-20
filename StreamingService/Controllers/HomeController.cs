using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreamingService.Data;
using StreamingService.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Dynamic;
using System;
using System.IO;
using System.Linq;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Text.RegularExpressions;

namespace StreamingService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(IServiceProvider serviceProvider)
        {
            _db = serviceProvider.GetRequiredService<ApplicationDbContext>();
        }

        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> SearchPage(string query)
        {
            // Use LINQ to search the database for relevant data
            var users = await _db.Users.Where(u => u.Nickname.Contains(query)).ToListAsync();
            var videos = await _db.Videos.Where(u => u.Title.Contains(query)).ToListAsync();
            dynamic mymodel = new ExpandoObject();
            mymodel.FoundVideos = videos;
            mymodel.FoundUsers = users;
            mymodel.Query = query;
            mymodel.AllUsers = await _db.Users.ToListAsync();

            return View(mymodel);
        }

        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Index()
        {
            try
            {
                Random rng = new Random();
                List<Video> videos = await _db.Videos.ToListAsync();
                List<Video> shuffledVideos = videos.OrderBy(a => rng.Next()).ToList();
                dynamic mymodel = new ExpandoObject();
                mymodel.Videos = shuffledVideos;
                mymodel.Users = await _db.Users.ToListAsync();
                return View(mymodel);
            }
            catch (Exception)
            {
                return RedirectToAction("SomethingWentWrong");
            }
        }

        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> WatchPage(int VideoId, int UserId)
        {
            try
            {
                dynamic mymodel = new ExpandoObject();
                mymodel.Video = await _db.Videos.Where(x => x.Id == VideoId).SingleOrDefaultAsync();
                mymodel.Creator = await _db.Users.Where(u => u.Id == UserId).SingleOrDefaultAsync();
                mymodel.RelatedVideos = await _db.Videos.Where(v => v.Id != VideoId).ToListAsync();
                List<Comment> comments = await _db.Comments.Where(c => c.VideoId == VideoId).ToListAsync();
                comments.Reverse();
                mymodel.Comments = comments;
                mymodel.RelatedVideoCreators = await _db.Users.ToListAsync();
                mymodel.SubscribersCount = await _db.Subscriptions.Where(s => s.CreatorId == UserId).CountAsync();
                mymodel.VideoLikes = await _db.LikedDislikedVideoVotes.Where(s => s.Vote == true && s.VideoId == VideoId).CountAsync();
                mymodel.VideoDislikes = await _db.LikedDislikedVideoVotes.Where(s => s.Vote == false && s.VideoId == VideoId).CountAsync();
                mymodel.IsLikePressed = await _db.LikedDislikedVideoVotes.Where(s => s.UserId == HttpContext.Session.GetInt32("UserId") && s.VideoId == VideoId && s.Vote == true).AnyAsync();
                mymodel.IsDislikePressed = await _db.LikedDislikedVideoVotes.Where(s => s.UserId == HttpContext.Session.GetInt32("UserId") && s.VideoId == VideoId && s.Vote == false).AnyAsync();
                mymodel.IsUserSubscribed = await _db.Subscriptions.Where(s => s.CreatorId == UserId && s.FollowerId == HttpContext.Session.GetInt32("UserId")).AnyAsync();
                mymodel.CurrentUserId = 0;
                mymodel.CurrentUserImagePath = 0;
                mymodel.CurrentUserNickname = 0;
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    mymodel.CurrentUserId = HttpContext.Session.GetInt32("UserId");
                    mymodel.CurrentUserImagePath = (await _db.Users.Where(u => u.Id == HttpContext.Session.GetInt32("UserId")).SingleOrDefaultAsync()).ImagePath;
                    mymodel.CurrentUserNickname = (await _db.Users.Where(u => u.Id == HttpContext.Session.GetInt32("UserId")).SingleOrDefaultAsync()).Nickname;
                }
                if (mymodel.Creator.Id != mymodel.Video.CreatorId)
                {
                    return RedirectToAction("SomethingWentWrong");
                }
                var video = await _db.Videos.FindAsync(VideoId);
                if (video != null) { video.VideoViews++; }
                await _db.SaveChangesAsync();
                return View(mymodel);
            }
            catch (Exception)
            {
                return RedirectToAction("SomethingWentWrong");
            }
        }
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> ChannelPage(int UserId)
        {
            try
            {
                dynamic mymodel = new ExpandoObject();
                mymodel.Creator = await _db.Users.Where(u => u.Id == UserId).SingleOrDefaultAsync();
                mymodel.SubscribersCount = await _db.Subscriptions.Where(s => s.CreatorId == UserId).CountAsync();
                mymodel.IsUserSubscribed = await _db.Subscriptions.Where(s => s.CreatorId == UserId && s.FollowerId == HttpContext.Session.GetInt32("UserId")).AnyAsync();

                mymodel.CurrentUser = 0;
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    mymodel.CurrentUser = HttpContext.Session.GetInt32("UserId");
                }
                if (mymodel.Creator == null)
                {
                    return RedirectToAction("SomethingWentWrong");
                }
                mymodel.CreatorVideos = await _db.Videos.Where(v => v.CreatorId == UserId).ToListAsync();
                return View(mymodel);
            }
            catch (Exception)
            {
                return RedirectToAction("SomethingWentWrong");
            }
        }

        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> MyPage()
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") == null)
                {


                    return RedirectToAction("SomethingWentWrong");
                }

                {

                }
                dynamic mymodel = new ExpandoObject();
                mymodel.Creator = await _db.Users.Where(u => u.Id == HttpContext.Session.GetInt32("UserId")).SingleOrDefaultAsync();
                mymodel.MyVideos = await _db.Videos.Where(v => v.CreatorId == HttpContext.Session.GetInt32("UserId")).ToListAsync();
                mymodel.NewName = "";
                string? message = TempData["Message"]?.ToString();
                ViewData["Message"] = message;
                return View(mymodel);
            }
            catch (Exception)
            {
                return RedirectToAction("SomethingWentWrong");
            }
        }


        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> SubscriptionsPage()
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    return RedirectToAction("SomethingWentWrong");
                }

                dynamic mymodel = new ExpandoObject();
                List<User> creators = new List<User>();
                Dictionary<int, int> map = new Dictionary<int, int>();
                List<Subscription> subscriptions = await _db.Subscriptions.Where(s => s.FollowerId == HttpContext.Session.GetInt32("UserId")).ToListAsync();
                foreach (var item in subscriptions)
                {
                    creators.Add(await _db.Users.Where(u => u.Id == item.CreatorId).FirstOrDefaultAsync());
                    map.Add(item.CreatorId, await _db.Subscriptions.Where(s => s.CreatorId == item.CreatorId).CountAsync());
                }
                mymodel.Creators = creators;
                mymodel.SubscribersCount = subscriptions;
                mymodel.Map = map;
                mymodel.CurrentUser = 0;
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    mymodel.CurrentUser = HttpContext.Session.GetInt32("UserId");
                }
                return View(mymodel);
            }
            catch (Exception)
            {
                return RedirectToAction("SomethingWentWrong");
            }
        }

        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> LikedPage()
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    return RedirectToAction("SomethingWentWrong");
                }

                dynamic mymodel = new ExpandoObject();

                List<LikedDislikedVideoVote> votes = await _db.LikedDislikedVideoVotes.Where(v => v.UserId == HttpContext.Session.GetInt32("UserId")).ToListAsync();
                List<Video> likedVideos = new List<Video>();
                Video temp = new Video();
                foreach (var vote in votes)
                {
                    if (vote.Vote == true)
                    {
                        temp = await _db.Videos.Where(v => v.Id == vote.VideoId).FirstOrDefaultAsync();
                        if (temp != null)
                        {
                            likedVideos.Add(temp);
                        }
                    }
                }

                mymodel.LikedVideos = likedVideos;
                mymodel.VideoCreators = await _db.Users.ToListAsync();
                return View(mymodel);
            }
            catch (Exception)
            {
                return RedirectToAction("SomethingWentWrong");
            }
        }



        [HttpPost]
        public async Task Subscribe(int CreatorId, int FollowerId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null && HttpContext.Session.GetInt32("UserId") != FollowerId)
            {
                RedirectToAction("SomethingWentWrong");
            }
            else
            {
                Subscription sub = new Subscription() { CreatorId = CreatorId, FollowerId = FollowerId };

                if (await _db.Subscriptions.Where(s => s.CreatorId == sub.CreatorId && s.FollowerId == FollowerId).AnyAsync())
                {
                    _db.Subscriptions.Remove(sub);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    await _db.AddAsync(sub);
                    await _db.SaveChangesAsync();
                }
            }
        }


        [HttpPost]
        public async Task AddComment(int UserId, int VideoId, string CommentText)
        {
            if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserId") != UserId || CommentText == null)
            {
                RedirectToAction("SomethingWentWrong");
            }
            else
            {
                try
                {

                    Comment comment = new Comment() { UserId = UserId, VideoId = VideoId, CommentText = CommentText };
                    await _db.AddAsync(comment);
                    await _db.SaveChangesAsync();

                }
                catch (Exception)
                {

                }
            }

        }


        [HttpPost]
        public async Task AddLike(int VideoId, int UserId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null && HttpContext.Session.GetInt32("UserId") != UserId)
            {
                RedirectToAction("SomethingWentWrong");
            }
            else
            {
                LikedDislikedVideoVote vote = new LikedDislikedVideoVote() { UserId = UserId, VideoId = VideoId, Vote = true };

                if (await _db.LikedDislikedVideoVotes.Where(s => s.UserId == UserId && s.VideoId == VideoId && s.Vote == false).AnyAsync())
                {
                    var currentVote = await _db.LikedDislikedVideoVotes.FindAsync(UserId, VideoId);
                    if (currentVote != null) { currentVote.Vote = true; }
                    await _db.SaveChangesAsync();
                }
                else if (await _db.LikedDislikedVideoVotes.Where(s => s.UserId == UserId && s.VideoId == VideoId && s.Vote == true).AnyAsync())
                {
                    _db.LikedDislikedVideoVotes.Remove(vote);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    await _db.AddAsync(vote);
                    await _db.SaveChangesAsync();
                }
            }

        }


        [HttpPost]
        public async Task AddDislike(int VideoId, int UserId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null && HttpContext.Session.GetInt32("UserId") != UserId)
            {
                RedirectToAction("SomethingWentWrong");
            }
            else
            {
                LikedDislikedVideoVote vote = new LikedDislikedVideoVote() { UserId = UserId, VideoId = VideoId, Vote = false };
                if (await _db.LikedDislikedVideoVotes.Where(s => s.UserId == UserId && s.VideoId == VideoId && s.Vote == true).AnyAsync())
                {
                    var currentVote = await _db.LikedDislikedVideoVotes.FindAsync(UserId, VideoId);
                    if (currentVote != null) { currentVote.Vote = false; }
                    await _db.SaveChangesAsync();
                }
                else if (_db.LikedDislikedVideoVotes.Where(s => s.UserId == UserId && s.VideoId == VideoId && s.Vote == false).Any())
                {
                    _db.LikedDislikedVideoVotes.Remove(vote);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    await _db.AddAsync(vote);
                    await _db.SaveChangesAsync();
                }
            }

        }

        public async Task<IActionResult> ChangeUserName(int UserId, string NewName)
        {
            if (HttpContext.Session.GetInt32("UserId") == null && HttpContext.Session.GetInt32("UserId") != UserId)
            {
                return RedirectToAction("SomethingWentWrong");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(NewName) || string.IsNullOrEmpty(NewName))
                {
                    return RedirectToAction("MyPage");

                }
                var currentUser = await _db.Users.FindAsync(UserId);
                currentUser.Nickname = NewName;
                await _db.SaveChangesAsync();
                return RedirectToAction("MyPage");
            }

        }

        public async Task<IActionResult> ChangeVideoTitle(int VideoId, string NewName)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("SomethingWentWrong");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(NewName) || string.IsNullOrEmpty(NewName))
                {
                    return RedirectToAction("MyPage");
                }

                var currentVideo = await _db.Videos.FindAsync(VideoId);
                currentVideo.Title = NewName;
                await _db.SaveChangesAsync();

                return RedirectToAction("MyPage");
            }
        }

        public async Task<IActionResult> DeleteVideo(int VideoId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("SomethingWentWrong");
            }
            else
            {
                var currentVideo = await _db.Videos.FindAsync(VideoId);
                if (currentVideo == null)
                {
                    return RedirectToAction("SomethingWentWrong");
                }
                var filePathCurrentImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "VideoThumbnails", currentVideo.VideoThumbnailPath.Substring(17));
                var filePathCurrentVideo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Videos", currentVideo.VideoPath.Substring(8));

                using (var stream = new FileStream(filePathCurrentImage, FileMode.Open, FileAccess.ReadWrite, FileShare.Delete))
                {
                    if (System.IO.File.Exists(filePathCurrentImage))
                    {
                        System.IO.File.Delete(filePathCurrentImage);
                    }
                }

                using (var stream = new FileStream(filePathCurrentVideo, FileMode.Open, FileAccess.ReadWrite, FileShare.Delete))
                {
                    if (System.IO.File.Exists(filePathCurrentVideo))
                    {
                        System.IO.File.Delete(filePathCurrentVideo);
                    }
                }

                _db.Videos.Remove(currentVideo);
                await _db.SaveChangesAsync();

                return RedirectToAction("MyPage");
            }
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = (209715200))] //200 mb maximum size
        public async Task<IActionResult> UploadVideo([FromForm] IFormFile imageFile, [FromForm] IFormFile videoFile, Video video)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("SomethingWentWrong");
            }
            else if(videoFile == null)
            {
                TempData["Message"] = "Maximum video size is 200 mb!";
                return RedirectToAction("MyPage");
            }
            else
            {
                if (imageFile != null && videoFile != null)
                {
                    string imageFileName = GetUniqueName(imageFile.FileName);


                    string videoFileName = GetUniqueName(videoFile.FileName);


                    var filePathNewImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "VideoThumbnails", imageFileName);
                    var filePathNewVideo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Videos", videoFileName);

                    using (var stream = new FileStream(filePathNewImage, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    using (var stream = new FileStream(filePathNewVideo, FileMode.Create))
                    {
                        await videoFile.CopyToAsync(stream);
                    }

                    video.VideoPath = "/Videos/" + videoFileName;
                    video.VideoThumbnailPath = "/VideoThumbnails/" + imageFileName;
                    video.VideoViews = 0;
                    video.CreatorId = (int)HttpContext.Session.GetInt32("UserId");

                    await _db.AddAsync(video);
                    await _db.SaveChangesAsync();
                }
                return RedirectToAction("MyPage");
            }

        }

        public async Task<IActionResult> ChangeVideoImage(IFormFile imageFile, int VideoId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("SomethingWentWrong");
            }
            else
            {
                var currentVideo = await _db.Videos.FindAsync(VideoId);
                var filePathCurrentImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "VideoThumbnails", currentVideo.VideoThumbnailPath.Substring(17));
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = GetUniqueName(imageFile.FileName);
                    var filePathNewImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "VideoThumbnails", fileName);
                    if (filePathNewImage == filePathCurrentImage)
                    {
                        return RedirectToAction("MyPage");
                    }
                    using (var stream = new FileStream(filePathNewImage, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    if (currentVideo != null) { currentVideo.VideoThumbnailPath = "/VideoThumbnails/" + fileName.ToString(); }
                    await _db.SaveChangesAsync();
                }
                using (var stream = new FileStream(filePathCurrentImage, FileMode.Open, FileAccess.ReadWrite, FileShare.Delete))
                {
                    if (System.IO.File.Exists(filePathCurrentImage))
                    {
                        System.IO.File.Delete(filePathCurrentImage);
                    }
                }
                return RedirectToAction("MyPage");
            }

        }

        public async Task<IActionResult> UploadImage(IFormFile imageFile, int UserId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("SomethingWentWrong");
            }
            else
            {
                var currentUser = await _db.Users.FindAsync(UserId);
                var filePathCurrentImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UserLogos", currentUser.ImagePath.Substring(11));
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = GetUniqueName(imageFile.FileName);
                    var filePathNewImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UserLogos", fileName);
                    if (filePathNewImage == filePathCurrentImage)
                    {
                        return RedirectToAction("MyPage");
                    }
                    using (var stream = new FileStream(filePathNewImage, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    if (currentUser != null) { currentUser.ImagePath = "/UserLogos/" + fileName.ToString(); }
                    await _db.SaveChangesAsync();
                }
                using (var stream = new FileStream(filePathCurrentImage, FileMode.Open, FileAccess.ReadWrite, FileShare.Delete))
                {
                    if (System.IO.File.Exists(filePathCurrentImage))
                    {
                        System.IO.File.Delete(filePathCurrentImage);
                    }
                }
                return RedirectToAction("MyPage");
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult SomethingWentWrong()
        {
            return View();
        }

        string GetUniqueName(string fileName)
        {
            return  DateTime.UtcNow.Year.ToString() + "-" +
                                        DateTime.UtcNow.Month.ToString() + "-" +
                                        DateTime.UtcNow.Day.ToString() + "-" +
                                        DateTime.UtcNow.Hour.ToString() + "-" +
                                        DateTime.UtcNow.Minute.ToString() + "-" +
                                        DateTime.UtcNow.Second.ToString() + "-" +
                                        DateTime.UtcNow.Millisecond.ToString() + "-" + Regex.Replace(fileName, "[^a-zA-Z0-9.]", "-").Replace(" ", "-");
        }
    }
}

