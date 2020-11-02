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
        public List<Occupant> GetAll()
        {
            return _occupantService.Find();
        }

        [HttpGet("{id}")]
        public Occupant GetById(string id) {
            return _occupantService.FindOne(id);
        }

        [HttpDelete("{id}/delete")]
        public async Task<RentleResponse> Delete(string id) {
            return await _occupantService.Delete(id);
        }


        [HttpGet("{id}/generateDoc/{docType}")]
        public object generateDocument(string id, string docType)
        {
            Occupant occupant = _occupantService.Find().FirstOrDefault();
            _occupantService.generateDocument<Occupant>(DOC_TYPE.LEASE_CONTRACT, occupant, "1");
            return new { success = true, message = "Coucou"};
        }



        [HttpPost]
        public async Task<RentleResponse> Post(Occupant occupant)
        {
            return await _occupantService.Create(occupant);
        }
    }
}
