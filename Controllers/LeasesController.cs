using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RentleAPI.Models;
using RentleAPI.Services;

namespace RentleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeasesController : ControllerBase
    {
        private readonly LeaseService _leaseService;

        public LeasesController(LeaseService leaseService)
        {
            _leaseService = leaseService;
        }

        [HttpGet]
        public List<Lease> Get()
        {
            return _leaseService.Find();
        }

        [HttpPost]
        public async Task<RentleResponse> Post(Lease lease)
        {
            return await _leaseService.Create(lease);
        }

        [HttpPut]
        public async Task<RentleResponse> Put(Lease lease) {
            return await _leaseService.Put(lease);
        }

        [HttpDelete]
        public async Task<RentleResponse> Delete(IEnumerable<string> ids) {
            return await _leaseService.Delete(ids);
        }
    }
}
