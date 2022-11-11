using Microsoft.AspNetCore.Mvc.ModelBinding;
using NuGet.Packaging.Core;
using System.Xml.Linq;

namespace P3_Project.Models
{
    public class PackModel
    {

        public int PackID { get; set; }
        public string Name  { get; set; }
        //public List<ItemModel> ItemModels { get; set; }

        public PackModel(int PackID, string Name)
        {
            this.PackID = PackID;
            this.Name = Name;

        }

    }
}
