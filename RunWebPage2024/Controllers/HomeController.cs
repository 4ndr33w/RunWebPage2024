using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunBot2024.DbContexts;
using RunWebPage2024.Models;
using System.Diagnostics;

namespace RunWebPage2024.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly NpgDbContext _pgContext;

        private List<RivalModel> _rivalsFilteredCollection;
        private List<RivalModel> _rivalsFullCollection;

        private List<FullCompanyList> _fullCompanyList;

        private List<CompanyStatisticModel> _companyChampionshipCollection;
        

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, NpgDbContext npgDbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _pgContext = npgDbContext;

            Initialize();
        }

        private void Initialize()
        {
            _rivalsFullCollection = GetRivalCollection();
            _fullCompanyList = GetCompanyCollection();
            _companyChampionshipCollection = GetCompanyStatisticCollection();
        }
        private List<FullCompanyList> GetCompanyCollection()
        {
            var tempCollection = new List<FullCompanyList>();
            //пришлось ввести вторую коллекцию, в которую фильтруются результаты из первой
            //так как при фильтрации результатов в коллекцию напрямую из запроса - 
            //предприятия повторялись.
            var filteredCollection = new List<FullCompanyList>();
            try
            {
                var query = from Company in _pgContext.Companies.AsEnumerable()
                            from City in _pgContext.Cities.AsEnumerable()
                            from RegionCollection in _pgContext.Regions.AsEnumerable()

                            where Company.CityId.Equals(City.Id)
                            where RegionCollection.Id.Equals(City.RegionId)

                            select new FullCompanyList
                            {
                                CompanyName = Company.Name,
                                CityName = City.Name,
                                RegionName = RegionCollection.Name
                            };
                tempCollection = query.OrderBy(x => x.CityName).ToList();

                foreach (var rival in _rivalsFullCollection)
                {
                    foreach (var company in tempCollection)
                    {
                        if (company.CompanyName == rival.Company && !filteredCollection.Contains(company))
                        {
                            filteredCollection.Add(company);
                        }
                    }
                }
            }
            catch (Exception)
            { // Если соединение с БД не удалось, то просто возвращяем пустую коллекцию filteredCollection
            } // Сформированую над блоком try

            return filteredCollection;
        }
        private List<RivalModel> GetRivalCollection()
        {
            var tempCollection = new List<RivalModel>();
            try
            {
                tempCollection = _pgContext.Rivals
                    .Where(x => x.TotalResult > 0)
                    .OrderByDescending(x => x.TotalResult)
                    .ToList();
            }
            catch (Exception)
            { // Если соединение с БД не удалось, то просто возвращяем пустую коллекцию tempCollection
            } // Сформированую над блоком try
            return tempCollection;
        }
        private List<CompanyStatisticModel> GetCompanyStatisticCollection()
        {
            List<CompanyStatisticModel> tempCollection = new List<CompanyStatisticModel>();

            try
            {
                var query = from RivalCollection in _pgContext.Rivals.AsEnumerable()
                            group RivalCollection by RivalCollection.Company into test
                            
                            select new CompanyStatisticModel
                            {
                                CompanyName = test.Key,
                                Result = test.Sum(x => x.TotalResult),
                                RivalsCount = test.Count(),
                                AverageResult = test.Sum(x => x.TotalResult) / test.Count()
                            };
                tempCollection = query.OrderByDescending(x => x.Result).ToList();
            }
            catch (Exception)
            {
            }

            return tempCollection;
        }

        public IActionResult Index()
        {
            Tuple<List<FullCompanyList>, List<RivalModel>, string, string> compositeModel;
            string gender = "all";
            string company = "all";

            _rivalsFilteredCollection = _rivalsFullCollection;
            compositeModel = new Tuple<List<FullCompanyList>, List<RivalModel>, string, string>(_fullCompanyList, _rivalsFullCollection, company, gender);

            return View(compositeModel);
        }

        [HttpPost]
        public IActionResult OnCompanySelect(string gender, string company)
        {
            _rivalsFilteredCollection = CollectionFilteringByCompany(company);
            if (gender != "all")
            {
                _rivalsFilteredCollection = new List<RivalModel>(_rivalsFilteredCollection.Where(x => x.Gender == gender[0]).ToList());
            }

            Tuple<List<RivalModel>, string> compositeModel = new Tuple<List<RivalModel>, string>(_rivalsFilteredCollection, gender);

            return PartialView("_PersonalChallengePartialTable", compositeModel);
        }

        public IActionResult CompanyChampionship()
        {
            List<CompanyStatisticModel> companyStatisticsCollection = new List<CompanyStatisticModel>();

            companyStatisticsCollection = _companyChampionshipCollection;

            return View("CompanyChampionship", companyStatisticsCollection);
        }

        private List<RivalModel> CollectionFilteringByCompany(string company)
        {
            if (company != "all")
            {
                _rivalsFilteredCollection = _rivalsFullCollection.Where(x => x.Company == company).ToList();
            }
            else
            {
                _rivalsFilteredCollection = _rivalsFullCollection;
            }
            return _rivalsFilteredCollection;
        }

        public IActionResult CompanyStatistic()
        {
            List<CompanyStatisticModel> companyStatisticsCollection = new List<CompanyStatisticModel>();

            companyStatisticsCollection = _companyChampionshipCollection;

            return View(companyStatisticsCollection);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult OldResults()
        {
            ViewData["Title"] = _configuration["OldResultsTitle"];
            return View();
        }
    }
}
