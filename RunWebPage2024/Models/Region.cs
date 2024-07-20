using System.ComponentModel.DataAnnotations;

namespace RunWebPage2024.Models
{
    public class Region
    {
        private int _Id;
        private string _name;

        [Key]
        public int Id { get { return _Id; } set => _Id = value; }
        public string Name { get { return _name; } set => _name = value; }
    }
}
