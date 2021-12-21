using MongoDB.Driver;

namespace Flocon.Models
{
    public class CustomersService
    {
        private readonly IMongoCollection<Company> _companies;

        public CustomersService(IFloconDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _companies = database.GetCollection<Company>(settings.CompaniesCollectionName);
        }

        public List<Company> GetCompaniesList()
        {
            return _companies.Find(company => true).ToList();
        }

        public Company GetCompany(string id)
        {
            return _companies.Find<Company>(comp => comp.Id == id).FirstOrDefault();
        }

        public async Task<Company> CreateCompany(Company cmpy)
        {
            cmpy.CreatedOn = DateTime.Now;
            await _companies.InsertOneAsync(cmpy);
            return cmpy;
        }

        public async Task UpdateAsset(string id, Company companyIn)
        {
            await _companies.ReplaceOneAsync(cmpy => cmpy.Id == id, companyIn);
        }

        public async Task DeleteCompany(string id)
        {
            await _companies.DeleteOneAsync(cmpy => cmpy.Id == id);
        }
    }
}
