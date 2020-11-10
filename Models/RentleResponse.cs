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

    public class RentleResponse<T> : RentleResponse {
        public T data {get; set;}
        public RentleResponse(string message, bool success, T data) : base(message, success)  {
            this.data = data;
        } 
    }



}
