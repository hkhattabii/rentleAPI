using System;
using MongoDB.Driver;
using RentleAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentleAPI.Services
{
    public class OccupantService : Service
    {
        public readonly IMongoCollection<Occupant> _occupant;
        private readonly IMongoCollection<Property> _property;
        private readonly IMongoCollection<Lease> _lease;
        public OccupantService(IRentleDatabaseSettings settings) : base(settings)
        {
            _occupant = _database.GetCollection<Occupant>(settings.OccupantCollectionName);
            _property = _database.GetCollection<Property>(settings.PropertyCollectionName);
            _lease = _database.GetCollection<Lease>(settings.LeaseCollectionName);
        }

        public List<Occupant> Find()
        {
            List<Occupant> occupants = new List<Occupant>();
            var query = (
                from o in _occupant.AsQueryable().AsEnumerable()
                join p in _property.AsQueryable() on o.ID equals p.occupantID
                select new { Occupant = o, Property = p }
            ).ToList();

            for (int i = 0; i < query.Count; i++)
            {
                Occupant occupant = query[i].Occupant;
                occupant.PropertyLeased = query[i].Property;
                occupants.Add(occupant);
            }

            return occupants;
        }

        public Occupant FindOne(string id) {
            var query =  (
                from o in _occupant.AsQueryable().AsEnumerable()
                join p in _property.AsQueryable() on o.ID equals p.occupantID
                join l in _lease.AsQueryable() on o.ID equals l.OccupantID
                where o.ID == id
                select new { Occupant = o, Property = p, Lease = l}
            ).Single();

            Occupant occupant = query.Occupant;
            occupant.PropertyLeased = query.Property;
            occupant.Lease = query.Lease;
            return occupant;
        }
        public async Task<RentleResponse> Create(Occupant occupant)
        {
            await _occupant.InsertOneAsync(occupant); //Insertion d'un locataire
            await _property.UpdateOneAsync(p => p.ID == occupant.PropertyID, Builders<Property>.Update.Set(p => p.occupantID, occupant.ID)); //Met à jour la table du bien

            occupant.Lease.PropertyID = occupant.PropertyID;
            occupant.Lease.OccupantID = occupant.ID;
            
            await _lease.InsertOneAsync(occupant.Lease); //Insertion d'un bail
            

            return new RentleResponse("Le locataire a bien été ajouté", true);
        }

        public async Task<RentleResponse> Delete(string id) {
            Occupant occupant = await _occupant.FindOneAndDeleteAsync(o => o.ID == id);
            
            if (occupant == null) {
                return new RentleResponse("Le locataire n'éxiste pas", false);
            }

            await _property.UpdateOneAsync(p => p.ID == occupant.PropertyID, Builders<Property>.Update.Set(p => p.occupantID, null));
            await _lease.DeleteManyAsync(l => l.OccupantID == occupant.ID);

            return new RentleResponse("Le locataire a bien été supprimé", true);
        }
    }
}
