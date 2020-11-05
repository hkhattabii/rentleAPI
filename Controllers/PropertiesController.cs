using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RentleAPI.Models;
using RentleAPI.Services;

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


        [HttpGet("{type}")]
        public List<Property> Get(string type)
        {
            return _propertyService.Find(type);
        }
        [HttpGet("{type}/{id}")]
        public Property GetById(string id)
        {
            return _propertyService.FindOne(id);
        }


        [HttpDelete("{id}")]
        public Task<RentleResponse> Delete(string id) {
            Console.WriteLine("Coucou");
            return _propertyService.Delete(id);
        }
        [HttpPost]
        public async Task<Property> Post(Property property)
        {
            await _propertyService.Create(property);
            return property;
        }

        [HttpPut]
        public async Task<RentleResponse> Put(Property property) {
            return await _propertyService.Put(property);
        }
    }


}
