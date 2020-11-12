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
    public class Lease
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string PropertyID { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string OccupantID {get; set;}

        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime VisitBeginDate { get; set; }
        public DateTime VisitEndDate { get; set; }

        public DateTime SignatureDate { get; set; }



        [BsonIgnore]
        public float warranty {get; set;}
        [BsonIgnore]
        public bool isDepositPaid {get; set;} 
        public DateTime? DepositDate { get; set; }

        public float Index { get; set; }
        public bool IsFirstMonthPaid { get; set; }

        public Meter gasMeter {get; set;}
        public Meter waterMeter {get; set;}
        public Meter electricityMeter {get; set;}
        public DateTime? alarmDate { get; set; }

        [BsonIgnore]
        public Property Property { get; set; }
        [BsonIgnore]
        public Occupant Occupant {get; set;}
    }
}
