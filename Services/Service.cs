using System;
using System.Collections.Generic;
using System.Linq;
using Aspose.Words;
using MongoDB.Driver;
using RentleAPI.Models;

namespace RentleAPI.Services
{
    public class Service
    {
        protected List<string> mergeFields;
        protected List<object> mergeValues;
        protected Document file;
        protected DocumentBuilder fileBuilder;
        protected readonly IMongoDatabase _database;


        public Service(IRentleDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
            mergeFields = new List<string>();
            mergeValues = new List<object>();
        }

        public IMongoDatabase getDatabase()
        {
            return _database;
        }

        public void generateDocument(DOC_TYPE docType, string id)
        {

            /*
                        string filename = docType.ToString() + "-" + id + ".docx";
                        file = new Document();
                        fileBuilder = new DocumentBuilder(file);




                        Dictionary<string, object> documentDIC = document.ToBsonDocument().ToDictionary();
                        insertField(documentDIC, document.GetType().Name);

                        fileBuilder.Document.Save("merging-field-" + id + ".docx");
                        file.MailMerge.Execute(mergeFields.ToArray(), mergeValues.ToArray());
                        fileBuilder.Document.Save(filename);
            */
        }


        protected void insertField(Dictionary<string, object> dictionnary, string keyParent)
        {

            foreach (string key in dictionnary.Keys)
            {

                object value = dictionnary.GetValueOrDefault(key);
                if (value != null && value.GetType() == typeof(Dictionary<string, object>))
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

        public static Dictionary<string, object> ToDictionnary(object entity)
        {
            return entity.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(entity, null));
        }
    }
}
