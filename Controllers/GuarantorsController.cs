using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentleAPI.Models;
using RentleAPI.Services;

namespace RentleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuarantorsController : ControllerBase
    {
        private readonly GuarantorService _guarantor;

        public GuarantorsController(GuarantorService guarantorService)
        {
            _guarantor = guarantorService;
        }

        [HttpGet]
        public List<Guarantor> Get()
        {
            return  _guarantor.Find();
        }

        [HttpDelete]
        public async Task<RentleResponse> Delete(IEnumerable<string> ids)
        {
            return await _guarantor.Delete(ids);
        }

        [HttpPost]
        public async Task<RentleResponse> Post(Guarantor guarantor)
        {
            return await _guarantor.Create(guarantor);
        }
        [HttpPut]
        public async Task<RentleResponse> Put(Guarantor guarantor)
        {
            return await _guarantor.Put(guarantor);
        }

    }
}
