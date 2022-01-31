using MongoDB.Driver;

namespace Flocon.Models
{
    public class CustomersService
    {
        private readonly IMongoCollection<Company> _companies;
        private readonly IMongoCollection<SignTrail> _signTrails;

        public CustomersService(IFloconDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _companies = database.GetCollection<Company>(settings.CompaniesCollectionName);
            _signTrails = database.GetCollection<SignTrail>(settings.SignTrailsCollectionName);
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

        // ------------------------ Sign Trails ------------------------ //

        public List<SignTrail> GetSignTrailsList()
        {
            return _signTrails.Find(st => true).ToList();
        }

        public SignTrail GetSignTrail(string stid)
        {
            return _signTrails.Find(st => st.Id == stid).FirstOrDefault();
        }

        public List<SignTrail> GetUserSignTrails(string userId)
        {
            return _signTrails.Find(st => st.CreatorId == userId).ToList();
        }

        public async Task<SignTrail> CreateSignTrail(SignTrail st)
        {
            st.CreatedOn = DateTime.Now;
            await _signTrails.InsertOneAsync(st);
            return st;
        }

        public List<SignTrail> GetUsrDocumentsOpen(string usrAddr)
        {
            return _signTrails.Find(st => st.CreatorId == usrAddr &&
                                          (string.IsNullOrEmpty(st.SignWriterTx) || string.IsNullOrEmpty(st.SignApproverTx))).ToList();
        }

        public List<SignTrail> GetUsrPendingDocs(string usrAddr)
        {
            var s1 = _signTrails.Find(st => st.SignWriterId == usrAddr && string.IsNullOrEmpty(st.SignWriterTx)).ToList();
            var s2 = _signTrails.Find(st => st.SignApproverId == usrAddr && string.IsNullOrEmpty(st.SignApproverTx)).ToList();
            return s1.Concat(s2).ToList();
        }
    }
}