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
        private readonly IMongoCollection<Guarantor> _guarantor;
        public OccupantService(IRentleDatabaseSettings settings) : base(settings)
        {
            _occupant = _database.GetCollection<Occupant>(settings.OccupantCollectionName);
            _property = _database.GetCollection<Property>(settings.PropertyCollectionName);
            _lease = _database.GetCollection<Lease>(settings.LeaseCollectionName);
            _guarantor = _database.GetCollection<Guarantor>(settings.GuarantorCollectionName);
        }

        public List<Occupant> Find()
        {
            List<Occupant> occupants = new List<Occupant>();
            var query = (
                from o in _occupant.AsQueryable().AsEnumerable()
                join g in _guarantor.AsQueryable() on o.GuarantorID equals g.ID
                into guarantorJoin from guarantor in guarantorJoin.DefaultIfEmpty()
                join p in _property.AsQueryable() on o.ID equals p.occupantID
                into propertyJoin from propertyLeased in propertyJoin.DefaultIfEmpty()
                join l in _lease.AsQueryable() on o.ID equals l.OccupantID
                into leaseJoin from lease in leaseJoin.DefaultIfEmpty()
                select new { Occupant = o, Guarantor = guarantor, Property = propertyLeased, Lease = lease }
            ).ToList();




            for (int i = 0; i < query.Count; i++)
            {
                Occupant occupant = query[i].Occupant;
                occupant.Guarantor = query[i].Guarantor;
                occupant.PropertyLeased = query[i].Property;
                occupant.Lease = query[i].Lease;
                occupants.Add(occupant);
            }

            return occupants;
        }

        public Occupant FindOne(string id)
        {
            var query = (
                from o in _occupant.AsQueryable().AsEnumerable()
                join g in _guarantor.AsQueryable() on o.GuarantorID equals g.ID
                into guarantorJoin from guarantor in guarantorJoin.DefaultIfEmpty()
                join p in _property.AsQueryable() on o.ID equals p.occupantID
                into propertyJoin from propertyLeased in propertyJoin.DefaultIfEmpty()
                join l in _lease.AsQueryable() on o.ID equals l.OccupantID
                into leaseJoin from lease in leaseJoin.DefaultIfEmpty()
                where o.ID == id
                select new { Occupant = o, Property = propertyLeased, Lease = lease }
            ).Single();

            Occupant occupant = query.Occupant;
            occupant.PropertyLeased = query.Property;
            occupant.Lease = query.Lease;
            return occupant;
        }
        public async Task<RentleResponse> Create(Occupant occupant)
        {
            await _occupant.InsertOneAsync(occupant); //Insertion d'un locataire
            Occupant occupantInserted = FindOne(occupant.ID);

            return new RentleResponse<Occupant>("Le locataire a bien été ajouté", true, occupantInserted);
        }

        public async Task<RentleResponse> Delete(IEnumerable<string> ids)
        {
            foreach (string id in ids)
            {
                Occupant occupant = await _occupant.FindOneAndDeleteAsync(o => o.ID == id);
                await _guarantor.DeleteOneAsync(g => g.ID == occupant.GuarantorID);
            }

            if (ids.Count() == 1) return new RentleResponse("Le locataire a été supprimer avec succés", true);
            return new RentleResponse("Les locataires ont bien été supprimé avec succés", true);

        }

        public async Task<RentleResponse> Put(Occupant occupant)
        {
            await _occupant.ReplaceOneAsync(o => o.ID == occupant.ID, occupant);
            Occupant occupantInserted = FindOne(occupant.ID);
            return new RentleResponse<Occupant>("Le bien a été mis à jour", true, occupantInserted);
        }
    }
}
