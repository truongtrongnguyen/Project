
using elFinder.NetCore;
using elFinder.NetCore.Drivers.FileSystem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace BanHang.Areas.FileManager.Controllers
{
    [Area("Files")]
    public class FileManagerController : Controller
    {
        [Route("/file-manager")]
        public IActionResult Index()
        {
            return View();
        }


        IWebHostEnvironment _env;
        public FileManagerController(IWebHostEnvironment env) => _env = env;

        // Url để client-side kết nối đến backend
        // /el-finder-file-system/connector
        [Route("/file-manager-connector")]
        public async Task<IActionResult> Connector()
        {
            var connector = GetConnector();
            return await connector.ProcessAsync(Request);
        }

        // Địa chỉ để truy vấn thumbnail
        // /el-finder-file-system/thumb
        [Route("/file-manager-thumb/{hash}")]
        public async Task<IActionResult> Thumbs(string hash)
        {
            var connector = GetConnector();
            return await connector.GetThumbnailAsync(HttpContext.Request, HttpContext.Response, hash);
        }

        private Connector GetConnector()
        {
            // Thư mục gốc lưu trữ là wwwwroot/files (đảm bảo có tạo thư mục này)
            string pathroot = "Uploads";     // Thiết lập địa chỉ lưu file
            string requestUrl = "Contents"; // Thiết lập địa chỉ truy cập 

            var driver = new FileSystemDriver();

            string absoluteUrl = UriHelper.BuildAbsolute(Request.Scheme, Request.Host);
            var uri = new Uri(absoluteUrl);

            // Làm việc với File Uploads
            // env.ContentRootPath trả về đường dẫn vật lí thật lưu trên dự án
            string rootDirectory = Path.Combine(_env.ContentRootPath, pathroot);

            // https://localhost:5001/files/    // Đường dẫn truy cập file
            string url = $"{uri.Scheme}://{uri.Authority}/{requestUrl}/";
            string urlthumb = $"{uri.Scheme}://{uri.Authority}/file-manager-thumb/";


            var root = new RootVolume(rootDirectory, url, urlthumb)
            {
                //IsReadOnly = !User.IsInRole("Administrators")
                IsReadOnly = false, // Can be readonly according to user's membership permission
                IsLocked = false, // If locked, files and directories cannot be deleted, renamed or moved
                Alias = "Thư mục ứng dụng", // Beautiful name given to the root/home folder
                //MaxUploadSizeInKb = 2048, // Limit imposed to user uploaded file <= 2048 KB
                //LockedFolders = new List<string>(new string[] { "Folder1" }
                ThumbnailSize = 100,
            };


            driver.AddRoot(root);

            return new Connector(driver)
            {
                // This allows support for the "onlyMimes" option on the client.
                MimeDetect = MimeDetectOption.Internal
            };
        }
    }
}
