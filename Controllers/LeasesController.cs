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

        [HttpGet("{id}")]
        public Lease Get(string id)
        {
            return _leaseService.FindOne(id);
        }
        [HttpPost]
        public async Task<Lease> Post(Lease lease)
        {
            return await _leaseService.Create(lease);
        }
    }
}
