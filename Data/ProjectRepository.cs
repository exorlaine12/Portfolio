using portfolio.Models;
using Npgsql;

namespace portfolio.Data;

public class ProjectRepository
{
    private readonly NpgsqlDataSource _dataSource;
    public ProjectRepository(NpgsqlDataSource dataSource) => _dataSource = dataSource;

    private static Project ReadProject(NpgsqlDataReader r) => new()
    {
        ProjectId = r.GetInt64(0),
        CourseId = r.GetInt64(1),
        Title = r.GetString(2),
        Description = r.IsDBNull(3) ? null : r.GetString(3),
        CompletedDate = r.IsDBNull(4) ? null : r.GetDateTime(4),
        VideoUrl = r.IsDBNull(5) ? null : r.GetString(5),
        CourseCode = r.GetString(6),
        CourseName = r.GetString(7)
    };

    private static Image ReadImage(NpgsqlDataReader r) => new()
    {
        ImageId = r.GetInt64(0),
        ProjectId = r.GetInt64(1),
        PublicUrl = r.GetString(2),
        Caption = r.IsDBNull(3) ? null : r.GetString(3)
    };

    private const string SelectSql = @"SELECT p.project_id,p.course_id,p.title,p.description,
        p.completed_date,p.video_url,c.code,c.name
        FROM portfolio.project p JOIN portfolio.course c ON c.course_id=p.course_id";

    public async Task<List<Project>> GetAllAsync()
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(SelectSql + " ORDER BY c.code,p.title", conn);
        await using var r = await cmd.ExecuteReaderAsync();
        var items = new List<Project>();
        while (await r.ReadAsync()) items.Add(ReadProject(r));
        return items;
    }

    public async Task<Project?> GetByIdAsync(long id)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(SelectSql + " WHERE p.project_id=@id", conn);
        cmd.Parameters.AddWithValue("id", id);
        Project? project;
        await using (var r = await cmd.ExecuteReaderAsync())
            project = await r.ReadAsync() ? ReadProject(r) : null;
        if (project is not null) project.Images = await GetImagesAsync(id);
        return project;
    }

    private async Task<List<Image>> GetImagesAsync(long id)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT image_id,project_id,public_url,caption FROM portfolio.image WHERE project_id=@id ORDER BY image_id", conn);
        cmd.Parameters.AddWithValue("id", id);
        await using var r = await cmd.ExecuteReaderAsync();
        var images = new List<Image>();
        while (await r.ReadAsync()) images.Add(ReadImage(r));
        return images;
    }

    public Task AddWithImagesAsync(Project p, List<string> urls) =>
        SaveWithImagesAsync(p, urls, false);

    public Task UpdateWithImagesAsync(Project p, List<string> urls) =>
        SaveWithImagesAsync(p, urls, true);

    private async Task SaveWithImagesAsync(Project p, List<string> urls, bool update)
    {
        var cleanUrls = urls.Select(x => x.Trim()).Where(x => x.Length > 0).ToList();
        if (cleanUrls.Any(x => !Uri.TryCreate(x, UriKind.Absolute, out var u) ||
                               (u.Scheme != Uri.UriSchemeHttp && u.Scheme != Uri.UriSchemeHttps)))
            throw new ArgumentException("Every image URL must start with http:// or https://.");
        if (cleanUrls.Distinct(StringComparer.OrdinalIgnoreCase).Count() != cleanUrls.Count)
            throw new ArgumentException("Duplicate image URLs are not allowed.");

        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var tx = await conn.BeginTransactionAsync();
        try
        {
            long id = p.ProjectId;
            string sql = update
                ? @"UPDATE portfolio.project SET course_id=@cid,title=@title,description=@desc,
                    completed_date=@completed,video_url=@video WHERE project_id=@id"
                : @"INSERT INTO portfolio.project(course_id,title,description,completed_date,video_url)
                    VALUES(@cid,@title,@desc,@completed,@video) RETURNING project_id";

            await using (var cmd = new NpgsqlCommand(sql, conn, tx))
            {
                cmd.Parameters.AddWithValue("cid", p.CourseId);
                cmd.Parameters.AddWithValue("title", p.Title.Trim());
                cmd.Parameters.AddWithValue("desc", (object?)p.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("completed", (object?)p.CompletedDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("video", (object?)p.VideoUrl ?? DBNull.Value);
                if (update) cmd.Parameters.AddWithValue("id", p.ProjectId);
                if (update)
                {
                    if (await cmd.ExecuteNonQueryAsync() == 0) throw new InvalidOperationException("Project not found.");
                }
                else id = Convert.ToInt64(await cmd.ExecuteScalarAsync());
            }

            if (update)
            {
                await using var delete = new NpgsqlCommand("DELETE FROM portfolio.image WHERE project_id=@id", conn, tx);
                delete.Parameters.AddWithValue("id", id);
                await delete.ExecuteNonQueryAsync();
            }

            foreach (var url in cleanUrls)
            {
                await using var image = new NpgsqlCommand(
                    "INSERT INTO portfolio.image(project_id,public_url) VALUES(@id,@url)", conn, tx);
                image.Parameters.AddWithValue("id", id);
                image.Parameters.AddWithValue("url", url);
                await image.ExecuteNonQueryAsync();
            }
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteAsync(long id)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("DELETE FROM portfolio.project WHERE project_id=@id", conn);
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}
