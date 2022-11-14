using Microsoft.AspNetCore.Mvc;
using P3_Project.Models.DB;
using P3_Project.Models;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace P3_Project.Controllers
{
    [Route("api")]
    [ApiController]
    public class CutomerApi : ControllerBase
    {
        // GET: api/<Api>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<Api>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<Api>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Api>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Api>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

       

    }

    [Route("api/Admin")]
    [ApiController]
    public class AdminApi : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> GetAdmin()
        {
            return new string[] { "test" };
        }

        #region ItemModel

        //Create item model
        [HttpPut("ItemModelTable")]
        public  ActionResult ItemModelTable(ItemModel test)
        {
            ItemModel itemModel = test;
            if (itemModel.Id == 0)
            {
                itemModel.Create();
            }
            else
            {
                itemModel.Update();
            }
            return RedirectToActionPermanent("Stock", "Admin");
        }

        //Delete item model
        [HttpGet("deleteModel")]
        public ActionResult deleteModel(int Id)
        {
            ItemModel.Delete(Id);
            return RedirectToAction("Stock","Admin");
        }



        #endregion
    
    
    
    }
}
