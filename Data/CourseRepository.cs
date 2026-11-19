using portfolio.Models;
using Npgsql;

namespace portfolio.Data;

public class CourseRepository
{
    private readonly NpgsqlDataSource _dataSource;
    public CourseRepository(NpgsqlDataSource dataSource) => _dataSource = dataSource;

    private static Course Read(NpgsqlDataReader r) => new()
    {
        CourseId = r.GetInt64(0), Code = r.GetString(1), Name = r.GetString(2),
        Semester = r.IsDBNull(3) ? null : r.GetString(3),
        AcademicYear = r.IsDBNull(4) ? null : r.GetString(4),
        Description = r.IsDBNull(5) ? null : r.GetString(5)
    };

    public async Task<List<Course>> GetAllAsync()
    {
        const string sql = "SELECT course_id, code, name, semester, academic_year, description FROM portfolio.course ORDER BY code";
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        await using var r = await cmd.ExecuteReaderAsync();
        var rows = new List<Course>();
        while (await r.ReadAsync()) rows.Add(Read(r));
        return rows;
    }

    public async Task<Course?> GetByIdAsync(long id)
    {
        const string sql = "SELECT course_id, code, name, semester, academic_year, description FROM portfolio.course WHERE course_id=@id";
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", id);
        await using var r = await cmd.ExecuteReaderAsync();
        return await r.ReadAsync() ? Read(r) : null;
    }

    public async Task AddAsync(Course c)
    {
        const string sql = "INSERT INTO portfolio.course(code,name,semester,academic_year,description) VALUES(@code,@name,@sem,@ay,@desc)";
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        AddParameters(cmd, c);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(Course c)
    {
        const string sql = "UPDATE portfolio.course SET code=@code,name=@name,semester=@sem,academic_year=@ay,description=@desc WHERE course_id=@id";
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        AddParameters(cmd, c);
        cmd.Parameters.AddWithValue("id", c.CourseId);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(long id)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("DELETE FROM portfolio.course WHERE course_id=@id", conn);
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    private static void AddParameters(NpgsqlCommand cmd, Course c)
    {
        cmd.Parameters.AddWithValue("code", c.Code.Trim());
        cmd.Parameters.AddWithValue("name", c.Name.Trim());
        cmd.Parameters.AddWithValue("sem", (object?)c.Semester ?? DBNull.Value);
        cmd.Parameters.AddWithValue("ay", (object?)c.AcademicYear ?? DBNull.Value);
        cmd.Parameters.AddWithValue("desc", (object?)c.Description ?? DBNull.Value);
    }
}
