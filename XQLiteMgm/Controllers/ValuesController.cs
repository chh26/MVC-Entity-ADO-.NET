using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XQLiteMgm.Models.UnitOfWork;
using XQLiteMgm.Models.Repository;

namespace XQLiteMgm.Controllers
{
    public class ValuesController : ApiController
    {
        // GET: api/Values
        public IEnumerable<string> Get()
        {
            using (var uow =  UnitOfWorkFactory.Create())
            {
                var CustomerMasterRepository = new CustomerMasterRepository(uow);
                //CustomerMasterRepository.Get(123456);
            }
            return new string[] { "value1", "value2" };
        }

        // GET: api/Values/5
        public string Get(int id)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var CustomerMasterRepository = new CustomerMasterRepository(uow);
                //CustomerMasterRepository.Get(id);
            }
            return "value";
        }

        // POST: api/Values
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Values/5
        public void Delete(int id)
        {
        }
    }
}
