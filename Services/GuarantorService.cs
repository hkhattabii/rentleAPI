using System;
using MongoDB.Driver;
using RentleAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;

namespace RentleAPI.Services
{ 
    public class GuarantorJoined {
        public Guarantor Guarantor { get; set; }
        public Occupant Occupant { get; set; }
    }
    public class GuarantorService : Service
    {
        public readonly IMongoCollection<Occupant> _occupant;
        private readonly IMongoCollection<Guarantor> _guarantor;
        private readonly IMongoCollection<Property> _property;

        public GuarantorService(IRentleDatabaseSettings settings) : base(settings)
        {
            _occupant = _database.GetCollection<Occupant>(settings.OccupantCollectionName);
            _guarantor = _database.GetCollection<Guarantor>(settings.GuarantorCollectionName);
            _property = _database.GetCollection<Property>(settings.PropertyCollectionName);
        }

        public IEnumerable<GuarantorJoined> Join()
        {
            IEnumerable<GuarantorJoined> query = (
                from g in _guarantor.AsQueryable().AsEnumerable()
                join o in _occupant.AsQueryable() on g.ID equals o.GuarantorID
                into occupantJoin
                from occupant in occupantJoin.DefaultIfEmpty()
                select new GuarantorJoined { Guarantor = g, Occupant = occupant });

            return query;
        }

        public List<Guarantor> Find()
        {

            List<Guarantor> guarantors = new List<Guarantor>();
            var query = Join().ToList();

            for (int i = 0; i < query.Count; i++)
            {
                Guarantor guarantor = query[i].Guarantor;
                guarantor.Occupant = query[i].Occupant;
                guarantors.Add(guarantor);
            }

            return guarantors;
        }

        public Guarantor FindOne(string id) {
            var query = Join().Where(q => q.Guarantor.ID == id).FirstOrDefault();
                
            Guarantor guarantor = query.Guarantor;
            guarantor.Occupant = query.Occupant;

            return guarantor;
        }
        public async Task<RentleResponse> Create(Guarantor guarantor)
        {
            try {
                await _guarantor.InsertOneAsync(guarantor);
                Guarantor guarantorInserted = FindOne(guarantor.ID);
                return new RentleResponse<Guarantor>("Le guarant a bien �t� ajout�", true, guarantorInserted);
            } catch
            {
                return new RentleResponse("Une erreur interne est survenue", false);
            }
 
            
        }

        public async Task<RentleResponse> Delete(IEnumerable<string> ids) {
            foreach (string id in ids)
            {
                Guarantor guarantor = await _guarantor.FindOneAndDeleteAsync(g => g.ID == id);
                await _occupant.DeleteOneAsync(o => o.ID == guarantor.OccupantID);
                await _property.DeleteOneAsync(p => p.occupantID == guarantor.OccupantID);
            }

            if (ids.Count() == 1) return new RentleResponse("le guarant a �t� supprimer avec succ�s", true);

            return new RentleResponse("les guarants a �t� supprimer avec succ�s", true);
        }

        public async Task<RentleResponse> Put(Guarantor guarantor)
        {
            await _guarantor.ReplaceOneAsync(g => g.ID == guarantor.ID, guarantor);
            Guarantor guarantorInserted = FindOne(guarantor.ID);
            return new RentleResponse<Guarantor>("Le bien a �t� mis � jour", true, guarantorInserted);
        }
    }
}
