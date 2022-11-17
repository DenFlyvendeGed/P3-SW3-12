using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

using P3_Project.Models.Mail;
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

        [HttpGet("ValidatePromoCode/{code}")]
		[Produces("application/json")]
        public IActionResult ValidatePromoCode(string code) {
			var (validate, promoCode) = PromoCode.Validate(code, new StorageDB());
			var promoCodeJson = JsonSerializer.Serialize(promoCode);
			return Ok(JsonDocument.Parse($"{{\"result\" : {validate.ToString().ToLower()}, \"promoCode\" : {promoCodeJson}}}"));
        } 
    }

    [Route("api/Admin")]
    [ApiController]
    public class AdminApi : ControllerBase
    {
        [HttpPost("CreatePromoCode")]
        public async void CreatePromoCode() {
			string json;
			using (var reader = new StreamReader(Request.Body)) json = await reader.ReadToEndAsync();
			var code = JsonSerializer.Deserialize<PromoCode>(json);
			if(code == null) return;
			code.PushToDB(new StorageDB());
		}

		[HttpPut("EditPromoCode/{id}")]
		public async void EditPromoCode(int id) {
			var db = new StorageDB();
			var code = new PromoCode(id, db); 
			string json;
			using (var reader = new StreamReader(Request.Body)) json = await reader.ReadToEndAsync();
			var data = JsonSerializer.Deserialize<PromoCode>(json);
			if( data == null ) {Response.StatusCode = 418; return;}

			code.Code = data.Code;
			code.DiscountType = data.DiscountType;
			code.ItemType = data.ItemType;
			code.ExpirationDate = data.ExpirationDate;
			code.Items = data.Items;

			code.PushToDB(db);
		}

		[HttpDelete("DeletePromoCode")]
		public async void DeletePromoCode() {
			string json;
			using (var reader = new StreamReader(Request.Body)) json = await reader.ReadToEndAsync();
			var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
			if(dict == null) { Response.StatusCode = 418; return; }
			var id = dict["Id"];
			var db = new StorageDB();
			new PromoCode(id, db).DeleteFromDB(db);	
		}


		[HttpGet]
		public ActionResult NotificationEmails() {
			return Ok(JsonSerializer.Serialize(new MailList(new StorageDB()).List));
		}
		[HttpPut("NotificationEmails")]
		public async void PutNotificationEmails() {
			string json;
			using (var reader = new StreamReader(Request.Body)) json = await reader.ReadToEndAsync();
			var list = JsonSerializer.Deserialize<List<string>>(json);
			if(list == null) return;
			new MailList(){ List = list }.PushToDB(new StorageDB());
		}
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
