using MongoDB.Driver;
using RentleAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentleAPI.Services
{
    public class LeaseService : Service
    {
        private readonly IMongoCollection<Lease> _lease;
        private readonly IMongoCollection<Property> _property;
        private readonly IMongoCollection<Occupant> _occupant;
        private readonly PropertyService _propertyService;
        public LeaseService(IRentleDatabaseSettings settings) : base(settings)
        {
            _lease = _database.GetCollection<Lease>(settings.LeaseCollectionName);
            _property = _database.GetCollection<Property>(settings.PropertyCollectionName);
            _occupant = _database.GetCollection<Occupant>(settings.OccupantCollectionName);
            _propertyService = new PropertyService(settings);
        }


        public List<Lease> Find()
        {
            var query = (
                from l in _lease.AsQueryable().AsEnumerable()
                join p in _property.AsQueryable() on l.PropertyID equals p.ID
                join o in _occupant.AsQueryable() on l.OccupantID equals o.ID
                select new { Lease = l, Property = p, Occupant = o }
            ).ToList();

            List<Lease> leases = new List<Lease>();
            for (int i = 0; i < query.Count; i++)
            {
                Lease lease = query[i].Lease;
                lease.Property = query[i].Property;
                lease.Occupant = query[i].Occupant;
                lease.warranty = query[i].Property.Price * 2;
                lease.IsFirthMonthPaid = query[i].Lease.DepositDate == null ? false : true;
                leases.Add(lease);
            }
            return leases;
        }

        public Lease FindOne(string id)
        {
            var query = (
                from l in _lease.AsQueryable().AsEnumerable()
                join p in _property.AsQueryable() on l.PropertyID equals p.ID
                join o in _occupant.AsQueryable() on l.OccupantID equals o.ID
                where l.ID == id
                select new { Lease = l, Property = p, Occupant = o }
            ).Single();

            Lease lease = query.Lease;
            lease.Property = query.Property;
            lease.Occupant = query.Occupant;
            lease.warranty = query.Property.Price * 2;
            lease.IsFirthMonthPaid = query.Lease.DepositDate == null ? false : true;
            return lease;
        }

        public async Task<RentleResponse> Create(Lease lease)
        {
            try {
                await _lease.InsertOneAsync(lease);
                Lease leaseInserted = FindOne(lease.ID);
                return new RentleResponse<Lease>("Le bail a été inséré avec succès !", true, leaseInserted);
            }
            catch {
                return new RentleResponse("Une erreur interne est survenue", false);
            }

        }

        public async Task<RentleResponse> Put(Lease lease) {
            try {
                await _lease.ReplaceOneAsync(l => l.ID == lease.ID, lease);
                Lease leaseInserted = this.FindOne(lease.ID);
                return new RentleResponse<Lease>("Le bail a été mis à jour", true, leaseInserted);
            } catch {
                return new RentleResponse("Une erreur interne est survenue", false);
            }
        }
    }
}

