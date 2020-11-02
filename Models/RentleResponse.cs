using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentleAPI.Models
{
    public class RentleResponse
    {
        public string message {get; set;}
        public bool success {get; set;}

        public RentleResponse(string message,bool success) {
            this.message = message;
            this.success = success;
        }
    }

}
