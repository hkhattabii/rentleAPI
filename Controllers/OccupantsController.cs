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
    public class OccupantsController : ControllerBase
    {
        private readonly OccupantService _occupantService;

        public OccupantsController(OccupantService occupantService)
        {
            _occupantService = occupantService;
        }

        [HttpGet]
        public List<Occupant> Get()
        {
            return _occupantService.Find();
        }

        [HttpPost]
        public async Task<RentleResponse> Post(Occupant occupant)
        {
            return await _occupantService.Create(occupant);
        }

        [HttpPut]
        public async Task<RentleResponse> Put(Occupant occupant) {
            return await _occupantService.Put(occupant);
        }

        [HttpDelete()]
        public async Task<RentleResponse> Delete(IEnumerable<string> ids) {
            return await _occupantService.Delete(ids);
        }

        [HttpGet("{id}/generateDoc/{docType}")]
        public object generateDocument(string id, string docType)
        {
            Occupant occupant = _occupantService.Find().FirstOrDefault();
            _occupantService.generateDocument<Occupant>(DOC_TYPE.LEASE_CONTRACT, occupant, "1");
            return new { success = true, message = "Coucou"};
        }



    }
}
