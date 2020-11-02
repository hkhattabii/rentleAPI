using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RentleAPI.Models
{
    public class Property
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
         public string? occupantID { get; set; }

        public int FloorNumber { get; set; }

        public string Type { get; set; }

        public int BedroomCount { get; set; }

        public int Size { get; set; }

        public int SizeLivingRoom { get; set; }

        public int SizeKitchen { get; set; }

        public float Price { get; set; }

        public float Charges { get; set; }

        public string Image { get; set; }

        public Location Address { get; set; }

        [BsonIgnore]
        public Occupant leasedBy { get; set; }
        [BsonIgnore]
        public Lease lease {get;set;}
    }
}
