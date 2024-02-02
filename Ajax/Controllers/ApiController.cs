using Ajax.Models;
using Ajax.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Ajax.Controllers
{
    public class ApiController : Controller
    {
        private readonly MyDBContext _context;
        private readonly IWebHostEnvironment _environment;
        public ApiController (MyDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        //IActionResult可以回傳 View(回傳網頁頁面)、Content(回傳文字資料)、Json(回傳Json格式資料)等
        public IActionResult Index()
        {
            //int x = 10;
            //int y = 0;
            //int z = x/y;
            Thread.Sleep(15000);
            return Content("Hi Content! 今天是2024年1月30日","text/plain",Encoding.UTF8);
            
        }

        //尚未完成：新增至資料庫

        //public IActionResult Register(string name, int age = 20,string email)
        public IActionResult Register(UserDTO _user)
        {
            if (string.IsNullOrEmpty(_user.Name))
            {
                _user.Name = "guest";
            }

            //儲存上傳的檔案(路徑及檔案名稱寫死)
            //此時檔案名稱並非使用網頁上面選擇檔案中的檔名 Cinnamoroll
            //而是寫上codes上的 noimage.jpg
            //string uploadPath = @"C:\QNote\Codes\Ajax\Ajax\Ajax\Ajax\wwwroot\uploads\noimage.jpg";
            //using(var fileStream=new FileStream(uploadPath,FileMode.Create))
            //{
            //    _user.Avatar?.CopyTo(fileStream);
            //}

            //儲存上傳的檔案：動態生成檔案路徑(wwwroot)及檔案名稱
            //使用 IWebHostEnvironment介面 已存在於DI容器中
            string fileName = "empty.jpg";
            if(_user.Avatar!=null)
            {
                fileName=_user.Avatar.FileName;
            }
            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads",fileName);
            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                _user.Avatar?.CopyTo(fileStream);
            }

            //return Content($"Hello {_user.Name},{_user.Age}歲了，電子郵件是{_user.Email}","text/plain",Encoding.UTF8);

            //測試回傳IFormFile的內容至view上DivInfo
            return Content($"{_user.Avatar?.FileName} - {_user.Avatar?.ContentType} - {_user.Avatar?.Length}");
        }
        //練習：回傳圖片
        public IActionResult Avatar(int id=1) //  localhost:7182/api/avatar 預設
        {
            Member? member = _context.Members.Find(id);
            if (member != null) {
                byte[] img = member.FileData;
                if(img != null)
                {
                    return File(img, "image/jpeg");
                }
            }
            return NotFound();
        }
        
        //測試建置環境，使用 postman
        //[HttpPost]

        //------綜合練習：下拉是選單：地址--------

        //1.讀取城市
        public IActionResult Cities()
        {
            var cities=_context.Addresses.Select(c=>c.City).Distinct();
            return Json(cities);
        }
        //2.根據城市名稱讀取鄉鎮區
        public IActionResult Districts(string city)
        {
            var districts=_context.Addresses.Where(a=>a.City == city).Select(b=>b.SiteId).Distinct();
            return Json(districts);
        }
        //3.根據鄉鎮區名稱讀取路名
        public IActionResult Roads(string city,string district)
        {
            var roads = _context.Addresses.Where(a=>a.City==city && a.SiteId==district).Select(b=>b.Road).Distinct();
            return Json(roads);                
        }

        //景點搜尋練習
        [HttpPost]
        public IActionResult Spots([FromBody]SearchDTO _searchDTO)
        {
            //API資料搜尋

            //根據景點分類讀取景點資料
            var spots=_searchDTO.CategoryId==0?_context.SpotImagesSpots:_context.SpotImagesSpots.Where(s=>s.CategoryId==_searchDTO.CategoryId);

            //根據景點分類讀取景點資料，再使用關鍵字搜尋
            if (!string.IsNullOrEmpty(_searchDTO.Keyword))
            {
                spots=spots.Where(s=>s.SpotTitle.Contains(_searchDTO.Keyword) || s.SpotDescription.Contains(_searchDTO.Keyword));
            }

            //搜尋後的資料進行排序
            switch (_searchDTO.SortBy)
            {
                case "spotTitle":
                    spots=_searchDTO.SortType=="asc"?spots.OrderBy(s=>s.SpotTitle):spots.OrderByDescending(s=>s.SpotTitle);
                    break;

                case "categoryId":
                    spots=_searchDTO.SortType=="asc"?spots.OrderBy(s=>s.CategoryId):spots.OrderByDescending(s=>s.CategoryId);
                    break;
                default:   //預設使用spotId進行排序
                    spots=_searchDTO.SortType=="asc"?spots.OrderBy(s=>s.SpotId):spots.OrderByDescending(s=>s.SpotId); 
                    break;
            }

            //計算分頁相關數量

            //總共有多少筆資料
            int totalCount=spots.Count();
            //設定每頁顯示多少筆資料
            int pageSize=_searchDTO.PageSize??10; 
            //計算總共有幾頁
            int totalPages=(int)Math.Ceiling((decimal)totalCount/pageSize);
            //目前要顯示第幾頁
            int page = _searchDTO.Page ?? 1;

            //分頁
            spots=spots.Skip((page-1)*pageSize).Take(pageSize);

            SpotsPagingDTO spotsPaging = new SpotsPagingDTO();
            spotsPaging.TotalPages = totalPages;
            spotsPaging.SpotsResult = spots.ToList();

            return Json(spotsPaging);
        }

        //
        public IActionResult spotTitle(string title)
        {
            var titles=_context.Spots.Where(s=>s.SpotTitle.Contains(title)).Select(s=>s.SpotTitle).Take(8);
            return Json(titles);
        }
    }
}
