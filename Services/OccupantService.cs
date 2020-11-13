using System;
using MongoDB.Driver;
using RentleAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentleAPI.Services
{
    public class OccupantJoined
    {
        public Occupant Occupant { get; set; }
        public Guarantor Guarantor { get; set; }
        public Property Property { get; set; }
        public Lease Lease { get; set; }
    }
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

        private IEnumerable<OccupantJoined> Join()
        {
            IEnumerable<OccupantJoined> query = (
                from o in _occupant.AsQueryable().AsEnumerable()
                join g in _guarantor.AsQueryable() on o.GuarantorID equals g.ID
                into guarantorJoin from guarantor in guarantorJoin.DefaultIfEmpty()
                join l in _lease.AsQueryable() on o.ID equals l.OccupantID
                into leaseJoin from lease in leaseJoin.DefaultIfEmpty()
                join p in _property.AsQueryable() on lease?.PropertyID equals p.ID
                into propertyJoin from propertyLeased in propertyJoin.DefaultIfEmpty()
                select new OccupantJoined { Occupant = o, Guarantor = guarantor, Property = propertyLeased, Lease = lease }
            );

            return query;
        }

        public List<Occupant> Find(bool withoutLease)
        {
            List<Occupant> occupants = new List<Occupant>();
            IEnumerable<OccupantJoined> query = Join();

            if (withoutLease)
            {
                query = query.Where(q => {
                    return q.Lease == null;
                });
            }
            List<OccupantJoined> queryList = query.ToList();

            for (int i = 0; i < queryList.Count; i++)
            {
                Occupant occupant = queryList[i].Occupant;
                occupant.Guarantor = queryList[i].Guarantor;
                occupant.PropertyLeased = queryList[i].Property;
                occupant.Lease = queryList[i].Lease;
                occupants.Add(occupant);
            }

            return occupants;
        }

        public Occupant FindOne(string id)
        {
            var query = Join().Where(q =>  q.Occupant.ID == id).FirstOrDefault();

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
