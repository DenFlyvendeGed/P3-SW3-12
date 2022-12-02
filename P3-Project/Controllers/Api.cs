using P3_Project.Models.Mail;
using P3_Project.Models.DB;
using P3_Project.Models.Orders;
using P3_Project.Models;

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net;
using System.Web.Http;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using HttpDeleteAttribute = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;

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
        public IActionResult ValidatePromoCode(string code)
        {
            var (validate, promoCode) = PromoCode.Validate(code, new StorageDB());
            var promoCodeJson = JsonSerializer.Serialize(promoCode);
            return Ok(JsonDocument.Parse($"{{\"result\" : {validate.ToString().ToLower()}, \"promoCode\" : {promoCodeJson}}}"));
        }

		[HttpPost("CreateOrder")]
		public async Task<IActionResult> CreateOrder(InputOrder input_order){
			var order = input_order.ToOrder();
			P3_Project.Models.Orders.Globals.OrderDB.Push(order);
			await P3_Project.Models.ReservationPdf.ReservationPdf.FromOrder(order);

			var compile_folder = P3_Project.Models.ReservationPdf.ReservationPdf.COMPILE_FOLDER;

			new MailClient()
				.To(order.Email)
				.Attachment(compile_folder + "/order.pdf")
				.Subject($"Reservation Ved Aalborg Sportshøjskole {order.Id}")
				.Body("Du har nu lavet en reservation ved Aalborg Sportshøjskole")
				.SendMail();

			return Ok();
		}
    }

    [Route("api/Admin")]
    [ApiController]
    public class AdminController :  ControllerBase
    {
        static StorageDB db = new StorageDB();

        #region Email
        [HttpGet("NotificationEmails")]
		public ActionResult NotificationEmails() {
			try {
				return Ok(JsonSerializer.Serialize(new MailList(db)));
			} catch {
				return Ok("[]");
			}
		}
		[HttpPut("NotificationEmails")]
		public async void PutNotificationEmails() {
			string json;
			using (var reader = new StreamReader(Request.Body)) json = await reader.ReadToEndAsync();
			var list = JsonSerializer.Deserialize<List<string>>(json);
			if(list == null) return;
			new MailList(){ List = list }.PushToDB(db);
		}

        #endregion

        #region PromoCode
        [HttpPost("CreatePromoCode")]
        public async void CreatePromoCode() {
			string json;


            using (var reader = new StreamReader(Request.Body)) json = await reader.ReadToEndAsync();
			Console.WriteLine(json);
			var code = JsonSerializer.Deserialize<PromoCode>(json);
			if(code == null) return;
			code.PushToDB(db);
		}

		[HttpPut("EditPromoCode/{id}")]
		public async Task<StatusCodeResult> EditPromoCode(int id) {
			var code = new PromoCode(id, db); 
			string json;
            using (var reader = new StreamReader(Request.Body)) json = await reader.ReadToEndAsync();
			var data = JsonSerializer.Deserialize<PromoCode>(json);
			if( data == null ) { return new StatusCodeResult(418); }
            
			code.Code = data.Code;
            code.Value = data.Value;
			code.DiscountType = data.DiscountType;
			code.ItemType = data.ItemType;
			code.ExpirationDate = data.ExpirationDate;
			code.Items = data.Items;

			code.PushToDB(db);
            return new StatusCodeResult((int)HttpStatusCode.OK); ;
		}
		
		[HttpDelete("DeletePromoCode")]
		public async Task<IActionResult> DeletePromoCode() {
			string json;

            using (var reader = new StreamReader(Request.Body)) json = await reader.ReadToEndAsync();
			var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
			if(dict == null) {  return new StatusCodeResult(418); }
			var id = dict["Id"];
			new PromoCode(id, db).DeleteFromDB(db);	
          
            return new StatusCodeResult((int)HttpStatusCode.OK);
		}

        
		[HttpPost("CreatePackModel")]
        public IActionResult CreatePackModel(PackModel data) {
            data.PushToDB(db);
            return RedirectToActionPermanent("PackViewModel", "Admin");
        }

		[HttpPut("EditPackModel/{id}")]
		public IActionResult EditPackModel(int id, PackModel data) {
			var db = new StorageDB();
			var code = new PackModel(id, db); 

            code.Description = data.Description;
            code.Name = data.Name;
            code.Price = data.Price;
            code.Options = data.Options;
            code.Tags = data.Tags;
            code.Pictures = data.Pictures;

            code.PushToDB(db);
            return RedirectToActionPermanent("PackViewModel", "Admin");
        }

		[HttpDelete("DeletePackModel")]
		public async Task<IActionResult> DeletePackModel() {
			string json;
			using (var reader = new StreamReader(Request.Body)) json = await reader.ReadToEndAsync();
			var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
			if(dict == null) { Response.StatusCode = 418; return BadRequest(); }
			var id = dict["Id"];
			var db = new StorageDB();
			new PackModel(id, db).DeleteFromDB(db);

            return RedirectToActionPermanent("PackViewModel", "Admin");
        }

        [HttpGet]
        public IEnumerable<string> GetAdmin()
        {
            return new string[] { "test" };
        }
        #endregion

        #region ItemModel

        //Create item model
        [HttpPut("ItemModelTable")]
        public IActionResult ItemModelTable(ItemModel test)
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
           
            //return Redirect(new Uri(newUrl.ToString(), UriKind.RelativeOrAbsolute));
            //RedirectToAction("Index", "Clients");
            return RedirectToActionPermanent("Stock", "Admin");
        }

        //Delete item model
        [HttpGet("deleteModel")]
        public IActionResult deleteModel(int Id)
        {
            ItemModel.Delete(Id);
            return RedirectToAction("Stock","Admin");
        }


        //api/Admin/DeleteImage? Id = 000 & Name = a1.PNG
        [HttpDelete("DeleteImage")]
        //[ValidateAntiForgeryToken]
        public StatusCodeResult DeleteImage(string Id, string Name)
        {
            string filepath = "";
            DirectoryInfo dir = new("path");

            dir = ImageModel.GetDir(int.Parse(Id));
            filepath = Path.Combine(dir.FullName, Name);

            FileInfo file = new FileInfo(filepath);

            if (file.Exists)
            {
                file.Delete();
            }
            else
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            return new StatusCodeResult((int)HttpStatusCode.OK);
        }

        [HttpDelete("DeleteImagePack")]
        public StatusCodeResult DeleteImage2(string Id, string Name , string FilePath = "")
        {
            string filepath = "";
            DirectoryInfo dir = new("path");
            if (FilePath == "") {
                dir = ImageModel.GetDir(int.Parse(Id));
                filepath = Path.Combine(dir.FullName, Name);
            }
            else
            {
                string projectPath = Directory.GetCurrentDirectory();
                filepath = Path.Combine(projectPath, "wwwroot" + FilePath);
            }

            FileInfo file = new FileInfo(filepath);

            if (file.Exists)
            {
                file.Delete();
            }
            else
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            return new StatusCodeResult((int)HttpStatusCode.OK);
        }
        #endregion

        #region Tags
        [HttpPost("AddTag")]
        //[ValidateAntiForgeryToken]
        public StatusCodeResult AddTag(string ItemModelId, string TagId)
        {

            if(db.DB.CheckRow("Tags", "Id", TagId))
            {
                Tag tag = db.DB.GetRow("Tags", new Tag(), TagId);
                db.DB.AddRowToTable($"ItemModel_{ItemModelId}_Tags", tag);
            }
            else
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            return new StatusCodeResult((int)HttpStatusCode.OK);
        }

        [HttpPost("RemoveTag")]
        //[ValidateAntiForgeryToken]
        public StatusCodeResult RemoveTag(string ItemModelId, string TagId)
        {
            string table = $"ItemModel_{ItemModelId}_Tags";

            if (db.DB.CheckRow("Tags", "Id", TagId))
            {
                Tag tag = db.DB.GetRow("Tags", new Tag(), TagId);
                string id = db.DB.GetField("Name",tag.Name,table, "Id");
                db.DB.RemoveRow(table, "Id", id);
            }
            else
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            return new StatusCodeResult((int)HttpStatusCode.OK);
        }

        [HttpPost("CreateTag")]
        //[ValidateAntiForgeryToken]
        public IActionResult CreateTag(string TagName)
        {
            Tag tag = new();
            tag.Name = TagName;
            tag.Id = -1;
            db.DB.AddRowToTable("Tags", tag);
            string id = db.DB.GetField("Name", TagName, "Tags", "Id");
            Response.Headers.Add("Id", id);
            return Ok();//new StatusCodeResult((int)HttpStatusCode.OK);
        }

        [HttpPost("DeleteTag")]
        //[ValidateAntiForgeryToken]
        public StatusCodeResult DeleteTag(string TagId)
        {
            db.DB.RemoveRow("Tags", "Id", TagId);
            return new StatusCodeResult((int)HttpStatusCode.OK);
        }
        #endregion
    }
}
