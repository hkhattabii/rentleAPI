namespace RentleAPI.Models
{
    public class RentleDatabaseSettings : IRentleDatabaseSettings
    {
        public string PropertyCollectionName { get; set; }
        public string OccupantCollectionName { get; set; }
        public string LeaseCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string ConnectionStringWLAN {get; set;}
        public string DatabaseName { get; set; }
    }

    public interface IRentleDatabaseSettings
    {
        string PropertyCollectionName { get; set; }
        string OccupantCollectionName { get; set; }
        string LeaseCollectionName { get; set; }
        string ConnectionString { get; set; }
        string ConnectionStringWLAN {get; set;}
        string DatabaseName { get; set; }
    }
}
