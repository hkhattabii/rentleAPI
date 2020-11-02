using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Text.Json.Serialization;

namespace RentleAPI.Models
{
    public class Person
    {
        public string Gender { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Gsm { get; set; }
        public string avatar {get; set;}
        public Location address {get; set;}
    }

    public class Occupant : Person
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PropertyID { get; set; }

        public string NationalRegistry { get; set; }

        public DateTime BirthDate { get; set; }

        public Person Guarantor { get; set; }
        
        [BsonIgnore]
        public Lease Lease {get; set;}

        [BsonIgnore]
        public Property PropertyLeased { get; set; }
        

    }
}
