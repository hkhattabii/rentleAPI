using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace RentleAPI.Models
{
    public class Person
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        public string Gender { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Gsm { get; set; }
        public string image {get; set;}
        public Location address {get; set;}
    }

    public class Guarantor : Person {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? OccupantID { get; set; }

        [BsonIgnore]
        public Occupant Occupant { get; set; }
    }

    public class Occupant : Person
    {

        [BsonRepresentation(BsonType.ObjectId)]
        public string PropertyID { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string GuarantorID { get; set; }

        public string NationalRegistry { get; set; }

        public DateTime BirthDate { get; set; }

        
        [BsonIgnore]
        public Lease Lease {get; set;}

        [BsonIgnore]
        public Property PropertyLeased { get; set; }
        

    }
}
