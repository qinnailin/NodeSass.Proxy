/************************************************************************
* Copyright (c) 2018 All Rights Reserved.
*命名空间：NodeSass.Proxy.Web.Controllers
*文件名： MirrorsController
*创建人： 覃乃林
*创建时间：2018/12/25 16:08:17
*描述
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NodeSass.Proxy.Web.Controllers
{
    //[Route("mirrors")]
    public class MirrorsController : Controller
    {
        // GET: /<controller>/
        [Route("mirrors/node-sass/{version}")]
        [HttpGet]
        public async Task<IActionResult> NodeSass(string version)
        {
            using (WebClient web = new WebClient())
            {
                var url = $"https://npm.taobao.org/mirrors/node-sass/{version}/";
                var res = await web.DownloadStringTaskAsync(new Uri(url));
                return Content(res, "text/html");
            }
        }

        [Route("mirrors/node-sass")]
        [HttpGet]
        public async Task<IActionResult> NodeSass()
        {
            using (WebClient web=new WebClient())
            {
                var res=await web.DownloadStringTaskAsync(new Uri("https://npm.taobao.org/mirrors/node-sass/"));
                return Content(res, "text/html");
            }
            
        }

        [Route("mirrors/node-sass/{version}/{filename}")]
        [HttpGet]
        public async Task<IActionResult> Download(string filename,string version)
        {
            using (WebClient web = new WebClient())
            {
                string path = System.IO.Directory.GetCurrentDirectory() + $"/files/{version}";
#if DEBUG
                path = $@"F:\works\NodeSass.Proxy\NodeSass.Proxy.Web\files\{version}";
#endif
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                var url = $"http://cdn.npm.taobao.org/dist/node-sass/{version}/{filename}";
                var filePath = $"{path}/{filename}";
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
                    if (fileInfo.Length > 0)
                    {
                        return Downloads(path, filename);
                    }
                }
                await web.DownloadFileTaskAsync(url, filePath);
                return Downloads(path, filename);
            }

        }

        private IActionResult Downloads(string path, string name)
        {
            IFileProvider provider = new PhysicalFileProvider(path);
            IFileInfo fileInfo = provider.GetFileInfo(name);
            var readStream = fileInfo.CreateReadStream();
            return File(readStream, "application/x-zip-compressed", name);
        }
    }
}
