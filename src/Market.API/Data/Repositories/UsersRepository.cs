using Market.Domain;
using Market.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Data.Repositories;

public class UsersRepository(ApplicationDbContext context) : IUsersRepository
{
    public async Task<PaginatedList<User>> GetAllUsersAsync(int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.Users.Include(u => u.Products).AsQueryable().AsNoTracking();
        var total = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<User>(users, total);
    }
    
    public User? GetById(Guid id) => context.Users.Include(u => u.Products).FirstOrDefault(x => x.Id == id);

    public bool UserAlreadyExists(string email, string cpf) => context.Users.Any(u => u.Email == email || u.Cpf == cpf);

    public User? GetByEmail(string email) => context.Users.Include(u => u.Roles).Include(u => u.Products).FirstOrDefault(u => u.Email == email);
    public User? GetByCPF(string cpf) => context.Users.Include(u => u.Products).FirstOrDefault(u => u.Cpf == cpf);
    public User? GetByEmailOrCPF(string identification)
    {
        return context.Users.Include(u => u.Products).FirstOrDefault(u => u.Email == identification || u.Cpf == identification);
    }

    public async Task<List<User>> GetUsersByStatusAsync(UserVerificationStatus status, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Where(u => u.VerificationStatus == status)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    
    public async Task UpdateStatusAsync(Guid userId, UserVerificationStatus status, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user != null)
        {
            user.VerificationStatus = status;
            context.Users.Update(user);
        }
    }

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