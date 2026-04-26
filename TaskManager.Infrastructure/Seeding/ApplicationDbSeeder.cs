using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskManager.Application.Interfaces.Security;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Seeding;

public class ApplicationDbSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOptions<AdminUserSeedOptions> _adminOptions;
    private readonly ILogger<ApplicationDbSeeder> _logger;

    public ApplicationDbSeeder(
        ApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        IOptions<AdminUserSeedOptions> adminOptions,
        ILogger<ApplicationDbSeeder> logger)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _adminOptions = adminOptions;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var options = _adminOptions.Value;

        if (!options.Enabled)
        {
            _logger.LogInformation("Admin seed desabilitado por configuracao.");
            return;
        }

        if (!IsValidSeedConfiguration(options))
        {
            _logger.LogWarning("Admin seed ignorado porque as credenciais configuradas nao atendem aos requisitos minimos.");
            return;
        }

        var normalizedEmail = options.Email.Trim().ToLowerInvariant();

        var userExists = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.Email == normalizedEmail, cancellationToken);

        if (userExists)
        {
            _logger.LogInformation("Usuario admin de seed ja existe. Nenhuma acao foi necessaria.");
            return;
        }

        var adminUser = new User(
            options.Name,
            normalizedEmail,
            _passwordHasher.Hash(options.Password));

        await _dbContext.Users.AddAsync(adminUser, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Usuario admin de seed criado com sucesso para o email {Email}.", normalizedEmail);
    }

    private static bool IsValidSeedConfiguration(AdminUserSeedOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Name) ||
            string.IsNullOrWhiteSpace(options.Email) ||
            string.IsNullOrWhiteSpace(options.Password))
        {
            return false;
        }

        return options.Password.Length >= 12 &&
               options.Password.Any(char.IsUpper) &&
               options.Password.Any(char.IsLower) &&
               options.Password.Any(char.IsDigit) &&
               options.Password.Any(ch => !char.IsLetterOrDigit(ch));
    }
}
