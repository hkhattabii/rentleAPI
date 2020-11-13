using MongoDB.Driver;
using RentleAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentleAPI.Services
{
    public class PropertyJoined
    {
        public Property Property { get; set; }
        public Occupant Occupant { get; set; }
    }
    public class PropertyService : Service
    {
        private readonly IMongoCollection<Property> _property;
        private readonly IMongoCollection<Occupant> _occupant;
        private readonly IMongoCollection<Lease> _lease;
        public PropertyService(IRentleDatabaseSettings settings) : base(settings)
        {
            _property = _database.GetCollection<Property>(settings.PropertyCollectionName);
            _occupant = _database.GetCollection<Occupant>(settings.OccupantCollectionName);
            _lease = _database.GetCollection<Lease>(settings.LeaseCollectionName);

        }

        public IEnumerable<PropertyJoined> Join()
        {
            IEnumerable<PropertyJoined> query = (
                from p in _property.AsQueryable().AsEnumerable()
                join l in _lease.AsQueryable() on p.ID equals l.PropertyID
                into leaseJoin from lease in leaseJoin.DefaultIfEmpty()
                join o in _occupant.AsQueryable() on lease?.OccupantID equals o.ID
                into leasedByJoin from leasedBy in leasedByJoin.DefaultIfEmpty()
                select new PropertyJoined { Property = p, Occupant = leasedBy });
            return query;
        }

        public List<Property> Find(bool withoutLease)
        {
            List<Property> properties = new List<Property>();
            IEnumerable<PropertyJoined> query = Join();

            if (withoutLease) {
                query = query.Where(q => q.Occupant == null);
            }
            
            List<PropertyJoined> queryList = query.ToList();

            for (int i = 0; i < queryList.Count; i++)
            {
                Property property = computeFields(queryList[i]);
                properties.Add(property);
            }
            
            return properties;
        }

        public Property FindOne(string id)
        {
            PropertyJoined query = Join().Where(q => q.Property.ID == id).FirstOrDefault();

            Property property = computeFields(query);
            return property;
        }
        
        public async Task<RentleResponse> Create(Property property)
        {
            try {
                await _property.InsertOneAsync(property);
                Property propertyInserted = FindOne(property.ID);
                return new RentleResponse<Property>("le bien a été inséré avec succès  !", true, propertyInserted );
            } catch {
                return new RentleResponse("Une erreur interne est survenue", false);
            }
 
        }

        public async Task<RentleResponse> Delete(IEnumerable<string> ids) {
            foreach (string id in ids) {
                Property property = await _property.FindOneAndDeleteAsync(p => p.ID == id);
                Lease leaseDeleted = await _lease.FindOneAndDeleteAsync(l => l.PropertyID == property.ID);
                await _occupant.DeleteOneAsync(o => o.ID == leaseDeleted.OccupantID);
                
            }

            if (ids.Count() == 1) return new RentleResponse("le bien a été supprimer avec succés", true);

            return new RentleResponse("les biens ont été supprimer avec succés", true);
        }

        public async Task<RentleResponse> Put(Property property) {
            await _property.ReplaceOneAsync(p => p.ID == property.ID, property);
            Property propertyInserted = this.FindOne(property.ID);
            return new RentleResponse<Property>("Le bien a été mis à jour", true, propertyInserted);
        }

        public Property computeFields(PropertyJoined query)
        {
            Property property = query.Property;
            property.leasedBy = query.Occupant;
            property.bedroomCount = query.Property.bedrooms.Count();
            property.sizeBedrooms = query.Property.bedrooms.Sum();
            return property;
        }
    }
}
