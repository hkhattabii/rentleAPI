using MongoDB.Driver;
using RentleAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentleAPI.Services
{
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

        public List<Property> Find()
        {
            List<Property> properties = new List<Property>();
            var query = (
                from p in _property.AsQueryable().AsEnumerable()
                join o in _occupant.AsQueryable() on p.occupantID equals o.ID 
                into leasedByJoin from leasedBy in leasedByJoin.DefaultIfEmpty()
                select new { Property = p, Occupant = leasedBy}).ToList();
        
            
            for (int i = 0; i < query.Count; i++)
            {
                Property property = query[i].Property;
                property.leasedBy = query[i].Occupant;
                property.bedroomCount = query[i].Property.bedrooms.Count();
                property.sizeBedrooms = query[i].Property.bedrooms.Sum();
                properties.Add(property);
            }
            
            return properties;
        }
        public List<Property> FindByType(string type)
        {
            List<Property> properties = new List<Property>();
            var query = (
                from p in _property.AsQueryable().AsEnumerable()
                join o in _occupant.AsQueryable() on p.occupantID equals o.ID 
                into leasedByJoin from leasedBy in leasedByJoin.DefaultIfEmpty()
                where p.Type == type
                select new { Property = p, Occupant = leasedBy}).ToList();
        
            
            for (int i = 0; i < query.Count; i++)
            {
                Property property = query[i].Property;
                property.leasedBy = query[i].Occupant;
                properties.Add(property);
            }
            
            return properties;
        }

        public Property FindOne(string id)
        {
            var query = (
                from p in _property.AsQueryable().AsEnumerable() 
                join o in _occupant.AsQueryable() on p.ID equals o.PropertyID
                into leasedByJoin from leasedBy in leasedByJoin.DefaultIfEmpty()
                join l in _lease.AsQueryable() on p.ID equals l.PropertyID
                into leaseJoin from lease in leaseJoin.DefaultIfEmpty()
                where p.ID == id 
                select new { Property = p, Occupant = leasedBy, Lease = lease  }).FirstOrDefault();
            Property property = query.Property;
            property.leasedBy = query.Occupant;
            property.lease = query.Lease;
            return property;
        }
        
        public async Task<RentleResponse> Create(Property property)
        {
            try {
                await _property.InsertOneAsync(property);
                Property propertyInserted =FindOne(property.ID);
                return new RentleResponse<Property>("le bien a été inséré avec succès  !", true, propertyInserted );
            } catch {
                return new RentleResponse("Une erreur interne est survenue", false);
            }
 
        }

        public async Task<RentleResponse> Delete(IEnumerable<string> ids) {
            foreach (string id in ids) {
                Property property = await _property.FindOneAndDeleteAsync(p => p.ID == id);
                await _occupant.DeleteOneAsync(o => o.ID == property.occupantID);
                await _lease.DeleteOneAsync(l => l.PropertyID == property.ID);
            }

            if (ids.Count() == 1) return new RentleResponse("le bien a été supprimer avec succés", true);

            return new RentleResponse("les biens ont été supprimer avec succés", true);
        }

        public async Task<RentleResponse> Put(Property property) {
            await _property.ReplaceOneAsync(p => p.ID == property.ID, property);
            Property propertyInserted = this.FindOne(property.ID);
            return new RentleResponse<Property>("Le bien a été mis à jour", true, propertyInserted);
        }
    }
}
