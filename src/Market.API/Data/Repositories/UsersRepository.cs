using Market.Domain.Entities;
using Market.Domain.Repositories;

namespace Market.API.Data.Repositories;

public class UsersRepository(ApplicationDbContext context) : IUsersRepository
{
    public User? GetById(Guid id) => context.Users.Find(id);

    public bool UserAlreadyExists(string email, string cpf) => context.Users.Any(u => u.Email == email || u.CPF == cpf);

    public User? GetByEmail(string email) => context.Users.FirstOrDefault(u => u.Email == email);
    public User? GetByCPF(string cpf) => context.Users.FirstOrDefault(u => u.CPF == cpf);

    public async void Add(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public void Update(User user)
    {
        context.Users.Update(user);
        context.SaveChanges();
    }

    public void Delete(User user)
    {
        context.Users.Remove(user);
        context.SaveChanges();
    }
}