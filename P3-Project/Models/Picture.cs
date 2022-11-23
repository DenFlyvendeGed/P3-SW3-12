using System.Web.Http;
using Microsoft.AspNetCore.Mvc;

namespace P3_Project.Models
{
    public class Picture
    {
        public string? Name { get; set; }
        public int? Size { get; set; }
        public string? Type { get; set; }

        Picture()
        {
            Name = "";
            Size = 0;
            Type = "Image/File";

        }

        //lastModified: 1659267354291
        //lastModifiedDate:Sun Jul 31 2022 13:35:54 GMT+0200 (Centraleuropæisk sommertid) {}
        //name: "os.jpg"
        //size: 158659
        //type: "image/jpeg"
        //webkitRelativePath: ""
    }
}
