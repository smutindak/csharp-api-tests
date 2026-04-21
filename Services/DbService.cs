using Dapper;
using Npgsql;
using CSharpApiTests.Models;

namespace CSharpApiTests.Services;

public class DbService
{
    private readonly string _connectionString;

    static DbService()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public DbService()
    {
        _connectionString = "Host=localhost;Port=5432;Database=qadb;Username=qauser;Password=qapass";
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM users WHERE email = @Email",
            new { Email = email }
        );
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM users WHERE id = @Id",
            new { Id = id }
        );
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QueryAsync<User>("SELECT * FROM users ORDER BY id");
    }

    public async Task<int> InsertUserAsync(User user)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(
            @"INSERT INTO users (name, email, role) 
              VALUES (@Name, @Email, @Role) 
              RETURNING id",
            user
        );
    }

    public async Task DeleteUserAsync(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync("DELETE FROM users WHERE id = @Id", new { Id = id });
    }
}
