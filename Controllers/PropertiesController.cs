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

        [HttpGet]
        public List<Property> Get()
        {
            return _propertyService.Find();
        }
        [HttpGet("{id}")]
        public Property Get(string id)
        {
            return _propertyService.FindOne(id);
        }
        [HttpPost]
        public async Task<Property> Post(Property property)
        {
            await _propertyService.Create(property);
            return property;
        }
    }


}
