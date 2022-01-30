namespace Flocon.Models
{
    /// <summary>
    /// Retrieves the database parameters and make is available
    /// </summary>
    public class FloconDbSettings : IFloconDbSettings
    {
        public string? UsersCollectionName { get; set; }
        public string? RolesCollectionName { get; set; }
        public string? CompaniesCollectionName { get; set; }
        public string? SignTrailsCollectionName { get; set; }
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
    }

    public interface IFloconDbSettings
    {
        string? UsersCollectionName { get; set; }
        string? RolesCollectionName { get; set; }
        string? CompaniesCollectionName { get; set; }
        string? SignTrailsCollectionName { get; set; }
        string? ConnectionString { get; set; }
        string? DatabaseName { get; set; }
    }
}