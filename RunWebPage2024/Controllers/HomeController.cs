using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, NpgDbContext npgDbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _pgContext = npgDbContext;

            Initialize();
        }

        private void Initialize()
        {
            try
            {
                _rivalsFullCollection = _pgContext.Rivals
                    .Where(x => x.TotalResult > 0)
                    .OrderByDescending(x => x.TotalResult)
                    .ToList();
                
            }
            catch (Exception)
            {
                _rivalsFullCollection = new List<RivalModel>();
                _fullCompanyList = new List<FullCompanyList>();
            }
        }

        public IActionResult Index()
        {
            Tuple<List<RivalModel>, string, string> compositeModel;
            string gender = "male";
            string company = "all";

            _rivalsFilteredCollection = _rivalsFullCollection;
            compositeModel = new Tuple<List<RivalModel>, string, string>(_rivalsFullCollection, company, gender);
            return View(compositeModel);
        }

        [HttpPost]
        public IActionResult OnCompanySelect(string gender)
        {

            if (gender != "all")
            {
                _rivalsFilteredCollection = new List<RivalModel> ( _rivalsFullCollection.Where(x => x.Gender == gender[0]).ToList() );
            }
            else
            {
                _rivalsFilteredCollection = _rivalsFullCollection;
            }

            return PartialView("_PersonalChallengePartialTable", _rivalsFilteredCollection);
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
