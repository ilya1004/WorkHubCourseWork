namespace IdentityService.DAL.Abstractions.DbStartupService;

public interface IDbStartupService
{
    Task InitializeDb();
}