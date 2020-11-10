using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RentleAPI.Models;
using RentleAPI.Services;
using Microsoft.AspNetCore.Http;

namespace RentleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly PropertyService _propertyService;

        public PropertiesController(PropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpGet]
        public List<Property> Get() {
            return _propertyService.Find();
        }

        [HttpGet("{type}")]
        public List<Property> Get(string type)
        {
            return _propertyService.FindByType(type);
        }
        [HttpGet("{type}/{id}")]
        public Property GetById(string id)
        {
            return _propertyService.FindOne(id);
        }


        [HttpDelete]
        public Task<RentleResponse> Delete(IEnumerable<string> ids) {
            Console.WriteLine("OUOH", ids);
            return _propertyService.Delete(ids);
        }
        [HttpPost]
        public async Task<RentleResponse> Post(Property property)
        {
            return await _propertyService.Create(property);
        }


        [HttpPut]
        public async Task<RentleResponse> Put(Property property) {
            return await _propertyService.Put(property);
        }
    }


}
