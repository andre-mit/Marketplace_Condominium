using System.Text.RegularExpressions;

namespace UsersApi.Models;

public class User
{
    public string Name { get; set; }
    public string Cpf { get; set; }
    public DateOnly Birth { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public string Unit { get; set; }
    public string Tower { get; set; }

    public void Normalize()
    {
        Email = Email.ToLowerInvariant().Trim();
        Phone = Phone.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");

        Name = Name.ToLowerInvariant().Trim();
        Name = Regex.Replace(Name, @"[áàâã]", "a");
        Name = Regex.Replace(Name, @"[éèê]", "e");
        Name = Regex.Replace(Name, @"[íìî]", "i");
        Name = Regex.Replace(Name, @"[óòôõ]", "o");
        Name = Regex.Replace(Name, @"[úùû]", "u");
        Name = Regex.Replace(Name, @"[ç]", "c");
        
        Cpf = Cpf.ToLowerInvariant().Trim();
        Cpf = Regex.Replace(Cpf, @"[^0-9]", "");
    }
}