using Market.Domain.Entities;
using Market.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Data.Repositories;

public class UsersRepository(ApplicationDbContext context) : IUsersRepository
{
    public User? GetById(Guid id) => context.Users.AsNoTracking().FirstOrDefault(x => x.Id == id);

    public bool UserAlreadyExists(string email, string cpf) => context.Users.Any(u => u.Email == email || u.Cpf == cpf);

    public User? GetByEmail(string email) => context.Users.Include(u => u.Roles).FirstOrDefault(u => u.Email == email);
    public User? GetByCPF(string cpf) => context.Users.FirstOrDefault(u => u.Cpf == cpf);

    public async void Add(User user)
    {
        await context.Users.AddAsync(user);
    }

    public void Update(User user)
    {
        context.Users.Update(user);
    }

    public void Delete(User user)
    {
        context.Users.Remove(user);
    }
}