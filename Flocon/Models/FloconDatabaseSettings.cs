namespace Flocon.Models
{
    /// <summary>
    /// Retrieves the database parameters and make is available
    /// </summary>
    public class FloconDatabaseSettings : IFloconDatabaseSettings
    {
        public string UsersCollectionName { get; set; }
        public string RolesCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IFloconDatabaseSettings
    {
        string UsersCollectionName { get; set; }
        string RolesCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
