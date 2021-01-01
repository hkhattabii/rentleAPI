using MongoDB.Driver;
using RentleAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Fields;
namespace RentleAPI.Services
{
    public class LeaseJoined
    {
        public Lease Lease { get; set; }
        public Property Property { get; set; }
        public Occupant Occupant { get; set; }
    }
    public class LeaseService : Service
    {
        private readonly IMongoCollection<Lease> _lease;
        private readonly IMongoCollection<Property> _property;
        private readonly IMongoCollection<Occupant> _occupant;
        public LeaseService(IRentleDatabaseSettings settings) : base(settings)
        {
            _lease = _database.GetCollection<Lease>(settings.LeaseCollectionName);
            _property = _database.GetCollection<Property>(settings.PropertyCollectionName);
            _occupant = _database.GetCollection<Occupant>(settings.OccupantCollectionName);
        }


        public IEnumerable<LeaseJoined> Join()
        {
            IEnumerable<LeaseJoined> query = (
                from l in _lease.AsQueryable().AsEnumerable()
                join p in _property.AsQueryable() on l.PropertyID equals p.ID
                join o in _occupant.AsQueryable() on l.OccupantID equals o.ID
                select new LeaseJoined { Lease = l, Property = p, Occupant = o }
            );

            return query;
        }

        public IEnumerable<LeaseJoined> JoinOne(string id)
        {
            IEnumerable<LeaseJoined> query = (
                from l in _lease.AsQueryable().AsEnumerable()
                join p in _property.AsQueryable() on l.PropertyID equals p.ID
                join o in _occupant.AsQueryable() on l.OccupantID equals o.ID
                where l.ID == id
                select new LeaseJoined { Lease = l, Property = p, Occupant = o }
            );

            return query;
        }

        public List<Lease> Find(bool withouLease)
        {
            List<LeaseJoined> query = Join().ToList();



            List<Lease> leases = new List<Lease>();
            for (int i = 0; i < query.Count; i++)
            {
                Lease lease = query[i].Lease;
                lease.Property = query[i].Property;
                lease.Occupant = query[i].Occupant;
                lease.warranty = query[i].Property.Price * 2;
                lease.IsFirstMonthPaid = query[i].Lease.Deposit == 0 ? false : true;
                leases.Add(lease);
            }
            return leases;
        }

        public List<Lease> FindAlarms(DateTime now)
        {

            List<Lease> leases = new List<Lease>();


            var query = (
                from l in _lease.AsQueryable().AsEnumerable()
                join p in _property.AsQueryable() on l.PropertyID equals p.ID
                join o in _occupant.AsQueryable() on l.OccupantID equals o.ID
                where now.AddMonths(3) >= l.alarmDate
                select new { Lease = l, Property = p, Occupant = o }
                ).ToList();

            for (int i = 0; i < query.Count; i++)
            {
                Lease lease = query[i].Lease;
                lease.Property = query[i].Property;
                lease.Occupant = query[i].Occupant;
                lease.warranty = query[i].Property.Price * 2;
                lease.IsFirstMonthPaid = query[i].Lease.Deposit == 0 ? false : true;
                leases.Add(lease);
            }



            return leases;
        }

        public Lease FindOne(string id)
        {
            LeaseJoined query = Join().FirstOrDefault();

            Lease lease = query.Lease;
            lease.Property = query.Property;
            lease.Occupant = query.Occupant;
            lease.warranty = query.Property.Price * 2;
            lease.isDepositPaid = query.Lease.Deposit == 0 ? false : true;
            return lease;
        }




        public async Task<RentleResponse> Create(Lease lease)
        {
            try
            {
                await _lease.InsertOneAsync(lease);
                Lease leaseInserted = FindOne(lease.ID);
                return new RentleResponse<Lease>("Le bail a été inséré avec succès !", true, leaseInserted);
            }
            catch
            {
                return new RentleResponse("Une erreur interne est survenue", false);
            }

        }
        public async Task<RentleResponse> Put(Lease lease)
        {
            try
            {
                await _lease.ReplaceOneAsync(l => l.ID == lease.ID, lease);
                Lease leaseInserted = this.FindOne(lease.ID);
                return new RentleResponse<Lease>("Le bail a été mis à jour", true, leaseInserted);
            }
            catch
            {
                return new RentleResponse("Une erreur interne est survenue", false);
            }
        }

        public async Task<RentleResponse> Delete(IEnumerable<string> ids)
        {
            foreach (string id in ids)
            {
                Lease lease = await _lease.FindOneAndDeleteAsync(l => l.ID == id);
            }

            if (ids.Count() == 1) return new RentleResponse("Le bail a été supprimer avec succés", true);
            return new RentleResponse("Les baux ont bien été supprimé avec succés", true);

        }

        public void GenerateContract(string id)
        {
            string filename = "./PRINT/LEASE_CONTRACT/LEASE_CONTRACT-" + id + ".docx";
            file = new Document();
            fileBuilder = new DocumentBuilder(file);

            Lease document = FindOne(id);
            Dictionary<string, object> documentDIC = ToDictionnary(document);
            insertField(documentDIC, document.GetType().Name);


            foreach (string field in mergeFields)
            {
                fileBuilder.InsertTextInput("TextInput", TextFormFieldType.Regular, "", $"{field} : ", 0);
                fileBuilder.InsertField($"MERGEFIELD {field}");
                fileBuilder.InsertBreak(BreakType.LineBreak);
            }


            fileBuilder.Document.Save("merging-field-" + id + ".docx");
            file.MailMerge.Execute(mergeFields.ToArray(), mergeValues.ToArray());
            fileBuilder.Document.Save(filename);
            mergeFields.Clear();
            mergeValues.Clear();
        }

        public void GenerateGuarantorDeposit(string id)
        {
            string filename = "./PRINT/GUARANTOR_DEPOSIT/GUARANTOR_DEPOSIT-" + id + ".docx";
            file = new Document();
            fileBuilder = new DocumentBuilder(file);

            Lease document = FindOne(id);
            Dictionary<string, object> documentDIC = ToDictionnary(document);
            insertField(documentDIC, document.GetType().Name);


            foreach (string field in mergeFields)
            {
                if (field.Equals("Lease.Deposit"))
                {
                    fileBuilder.InsertTextInput("TextInput", TextFormFieldType.Regular, "", "Montant de la guarantie : ", 0);
                    fileBuilder.InsertField($"MERGEFIELD {field} €");
                    fileBuilder.InsertBreak(BreakType.LineBreak);
                }
            }


            fileBuilder.Document.Save("merging-field-" + id + ".docx");
            file.MailMerge.Execute(mergeFields.ToArray(), mergeValues.ToArray());
            fileBuilder.Document.Save(filename);
            mergeFields.Clear();
            mergeValues.Clear();
        }

    }
}

