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


    }
}
