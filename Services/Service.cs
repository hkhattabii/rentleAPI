using System;
using System.Collections.Generic;
using Aspose.Words;
using MongoDB.Bson;
using MongoDB.Driver;
using RentleAPI.Models;

namespace RentleAPI.Services
{
    public enum DOC_TYPE
    {
        LEASE_CONTRACT,
        GUARANTOR_DEPOSIT,
        STATE_ENTRY,
        STATE_EXIT,
        EARLY_TERMINATION_LEASE,
        LEASEHOLD_LEAVE
    }
    public class Service
    {
        private List<string> mergeFields;
        private List<object> mergeValues;
        private Document file;
        private DocumentBuilder fileBuilder;
        protected readonly IMongoDatabase _database;


        public Service(IRentleDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
            mergeFields = new List<string>();
            mergeValues = new List<object>();
        }

        public void generateDocument<T>(DOC_TYPE docType, T document, string id)
        {
            string filename = docType.ToString() + id + ".docx";
            file = new Document();
            fileBuilder = new DocumentBuilder(file);

            Dictionary<string, object> documentDIC = document.ToBsonDocument().ToDictionary();

            insertField(documentDIC, document.GetType().Name);


            foreach (string field in mergeFields)
            {
                GenerationServices.GenLeaseContract(fileBuilder, field);
            }


            fileBuilder.Document.Save("Ouoh.docx");

            file.MailMerge.Execute(mergeFields.ToArray(), mergeValues.ToArray());
            fileBuilder.Document.Save(filename);
        }


        private void insertField(Dictionary<string, object> dictionnary, string keyParent)
        {

            foreach (string key in dictionnary.Keys)
            {

                object value = dictionnary.GetValueOrDefault(key);
                if (value.GetType() == typeof(Dictionary<string, object>))
                {
                    insertField((Dictionary<string, object>)value, keyParent + "." + key);
                }
                else
                { //On ne prend pas en compte les valeur dictionnary ex : occupant.Guarantor.Address est fauxx car Address est un sous document et pas une valeur
                    string field = keyParent + "." + key;
                    mergeFields.Add(field);
                    mergeValues.Add(value);
                }
            }
        }
    }
}
