namespace Market.SharedApplication.ViewModels.UserViewModels;

public class UserVerificationViewModel
{
    public string Name { get; set; }
    public string Cpf { get; set; }
    public DateOnly Birth { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Unit { get; set; }
    public string Tower { get; set; }

    public UserVerificationViewModel()
    {
    }

    public UserVerificationViewModel(string name,
        string cpf,
        DateOnly birth,
        string email,
        string phone,
        string unit,
        string tower)
    {
        Name = name;
        Cpf = cpf;
        Birth = birth;
        Email = email;
        Phone = phone;
        Unit = unit;
        Tower = tower;
    }
}