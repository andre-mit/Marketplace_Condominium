using Market.Domain.Entities;
using Market.Domain.Enums;

namespace Market.Domain.Repositories;

public interface IUsersRepository
{
    Task<PaginatedList<User>> GetAllUsersAsync(int page, int pageSize,
        CancellationToken cancellationToken = default);

    User? GetById(Guid id);
    bool UserAlreadyExists(string email, string cpf);
    User? GetByEmail(string email);
    User? GetByCPF(string cpf);
    User? GetByEmailOrCPF(string identification);
    
    Task<List<User>> GetUsersByStatusAsync(UserVerificationStatus status, CancellationToken cancellationToken = default);

    Task UpdateStatusAsync(Guid userId, UserVerificationStatus status, CancellationToken cancellationToken = default);

    void Add(User user);
    void Update(User user);
    void Delete(User user);

    bool AddResetPasswordCode(string email, string code);
    bool ResetPassword(string email, string code, string newPasswordHash);
}