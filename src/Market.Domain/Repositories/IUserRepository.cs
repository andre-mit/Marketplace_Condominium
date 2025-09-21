using Market.Domain.Entities;

namespace Market.Domain.Repositories;

public interface IUserRepository
{
    User? GetById(Guid id);
    bool UserAlreadyExists(string email, string cpf);
    User? GetByEmail(string email);
    User? GetByCPF(string cpf);
    
    void Add(User user);
    void Update(User user);
    void Delete(User user);
}