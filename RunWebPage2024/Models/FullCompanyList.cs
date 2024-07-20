namespace RunWebPage2024.Models
{
    public class FullCompanyList
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CityName { get; set; }
        public string RegionName { get; set; }

        public FullCompanyList()
        { }

        public FullCompanyList(FullCompanyList companyListItem)
        {
            //Id = companyListItem.Id;
            CompanyName = companyListItem.CompanyName;
            CityName = companyListItem.CityName;
            RegionName = companyListItem.RegionName;
        }
    }
}
