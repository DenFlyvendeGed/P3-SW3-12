using Microsoft.AspNetCore.Mvc;
using P3_Project.Models.DB;
using P3_Project.Models;

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

        [Route("Test")]
        public IEnumerable<string> Test()
        {
            Console.WriteLine("Hello World");
            return new string[] { "test" };
        }

        // PromoCode
        [HttpGet]
        [Route("ValidatePromoCode")]
        public bool ValidatePromoCode([FromQuery]  string code){
            var db = new StorageDB();
            return db.ValidatePromoCode(code);
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

        // PromoCode
        [HttpPost]
        [Route("CreatePromoCode")]
        public void CreatePromoCode(
            [FromQuery] string code,
            [FromQuery] int promoCodeType,
            [FromQuery] string expirationDate
        ){
            Console.WriteLine("Hello " + code + " " + promoCodeType + " " + expirationDate);
            var db = new StorageDB();
            var date = expirationDate.Split('-');
            db.PushPromoCode(
                new PromoCode(
                    0, code, promoCodeType,
                    new DateTime(int.Parse(date[0]), int.Parse(date[1]), int.Parse(date[2]))
                )
            );
        }

        [HttpGet]
        [Route("GetSinglePromoCode")]
        public PromoCode? GetSinglePromoCode(
            [FromQuery] int id
        ){
            var db = new StorageDB();
            return db.GetSinglePromoCode(id); 
        }
    }
}
