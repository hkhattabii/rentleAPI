﻿using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RentleAPI.Models
{
    public class Property
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        public int FloorNumber { get; set; }

        public string Type { get; set; }

        public int Size { get; set; }

        public int SizeLivingRoom { get; set; }

        public int SizeKitchen { get; set; }

        public float Price { get; set; }

        public float Charges { get; set; }

        public string Image { get; set; }

        public IEnumerable<int> bedrooms {get; set;}
        public Location Address { get; set; }

        [BsonIgnore]
        public int bedroomCount {get;set;}
        [BsonIgnore]
        public int sizeBedrooms {get;set;}
        [BsonIgnore]
        public Occupant leasedBy { get; set; }
    }
}
