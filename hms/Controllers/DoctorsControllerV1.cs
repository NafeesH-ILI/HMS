using hms.Models;
using hms.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Threading.Tasks;

namespace hms.Controllers
{
    [ApiController]
    [Route("/api/v1/doctors")]
    public class DoctorsControllerV1(ILogger<DoctorsControllerV1> logger) : ControllerBase
    {
        private readonly ILogger<DoctorsControllerV1> logger = logger;
        private readonly NpgsqlDataSource db = NpgsqlDataSource.Create(DbCtx.ConnStr)!;

        [HttpGet]
        public async Task<ActionResult<IAsyncEnumerable<Doctor>>> GetAll(int page = 1, int page_size = 10, string? fmt=null)
        {
            if (page <= 0)
                page = 1;
            if (page_size <= 0 || page_size > 50)
                page_size = 10;
            IList<Doctor> doctors = [];
            try
            {
                string q = "SELECT * FROM doctors ";
                if (fmt != null)
                    q += " WHERE name LIKE @fmt";
                q += " ORDER BY uname OFFSET @offset LIMIT @limit;";
                await using var cmd = db.CreateCommand(q);
                cmd.Parameters.AddWithValue("offset", (page - 1) * page_size);
                cmd.Parameters.AddWithValue("limit", page_size);
                if (fmt != null)
                    cmd.Parameters.AddWithValue("fmt", fmt);
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    Doctor d = new()
                    {
                        UName = reader.GetString(0),
                        Name = reader.GetString(1),
                        MaxQualification = reader.GetString(2),
                        Specialization = reader.GetString(3),
                        DeptKey = reader.GetString(4),
                    };
                    doctors.Add(d);
                }
                if (doctors.Count > 0)
                    return Ok(new PaginatedResponse<IList<Doctor>> { Count = await Count(fmt), Value = doctors });
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Get Doctors v1");
                return BadRequest();
            }
        }

        [HttpGet("{uname}", Name = "GetDoctorV1ByUName")]
        public async Task<ActionResult<Doctor>> Get(string uname)
        {
            try
            {
                await using var cmd = db.CreateCommand("SELECT * FROM doctors WHERE uname=@uname;");
                cmd.Parameters.AddWithValue("uname", uname);
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    Doctor d = new()
                    {
                        UName = reader.GetString(0),
                        Name = reader.GetString(1),
                        MaxQualification = reader.GetString(2),
                        Specialization = reader.GetString(3),
                        DeptKey = reader.GetString(4),
                    };
                    return Ok(d);
                }
                return NotFound();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(DoctorDtoNew doctor)
        {
            Doctor d = new() {
                UName = UNamer.Generate("doctors", doctor.Name),
                Name = doctor.Name,
                MaxQualification = doctor.MaxQualification,
                Specialization = doctor.Specialization,
                DeptKey = doctor.DeptKey,
            };
            try
            {
                await using var cmd = db.CreateCommand("INSERT INTO doctors (uname, name, max_qual, specialization, dept) " +
                    "VALUES (@uname, @name, @max_qual, @spec, @dept);");
                cmd.Parameters.AddWithValue("uname", d.UName);
                cmd.Parameters.AddWithValue("name", d.Name);
                cmd.Parameters.AddWithValue("max_qual", d.MaxQualification);
                cmd.Parameters.AddWithValue("spec", d.Specialization);
                cmd.Parameters.AddWithValue("dept", d.DeptKey);
                if (cmd.ExecuteNonQuery() == 1)
                    return CreatedAtRoute("GetDoctorV1ByUName", new { uname = d.UName }, d);
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        
        private async Task<int> CountByUname(string uname)
        {
            await using var cmd = db.CreateCommand("SELECT Count(*) FROM doctors WHERE uname=@uname;");
            cmd.Parameters.AddWithValue("uname", uname);
            await using var reader = cmd.ExecuteReader();
            await reader.ReadAsync();
            return reader.GetInt32(0);
        }

        private async Task<int> Count(string? fmt = null)
        {
            await using var cmd = db.CreateCommand("SELECT Count(*) FROM doctors " +
                (fmt == null ? ";" : "WHERE name LIKE @fmt;"));
            if (fmt != null)
                cmd.Parameters.AddWithValue("fmt", fmt);
            await using var reader = cmd.ExecuteReader();
            await reader.ReadAsync();
            return reader.GetInt32(0);
        }

        [HttpPut("{uname}")]
        public async Task<ActionResult> Put(string uname, DoctorDtoNew doctor)
        {
            try
            {
                if (await CountByUname(uname) == 0)
                    return NotFound();
                Doctor d = new()
                {
                    UName = uname,
                    Name = doctor.Name,
                    MaxQualification = doctor.MaxQualification,
                    Specialization = doctor.Specialization,
                    DeptKey = doctor.DeptKey
                };
                await using var cmd = db.CreateCommand("UPDATE doctors SET name=@name, max_qual=@max_qual, " +
                    "specialization=@spec, dept=@dept WHERE uname=@uname;");
                cmd.Parameters.AddWithValue("uname", d.UName);
                cmd.Parameters.AddWithValue("name", d.Name);
                cmd.Parameters.AddWithValue("max_qual", d.MaxQualification);
                cmd.Parameters.AddWithValue("spec", d.Specialization);
                cmd.Parameters.AddWithValue("dept", d.DeptKey);
                if (cmd.ExecuteNonQuery() != 0)
                    return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Put Doctor v1");
                return BadRequest();
            }
        }

        [HttpPatch("{uname}")]
        public async Task<ActionResult> Patch(string uname, DoctorDtoPatch doctor)
        {
            Doctor? curr = (await Get(uname)).Value;
            try
            {
                await using var cmd = db.CreateCommand("SELECT * FROM doctors WHERE uname=@uname;");
                cmd.Parameters.AddWithValue("uname", uname);
                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    return NotFound();
                curr = new()
                {
                    UName = reader.GetString(0),
                    Name = reader.GetString(1),
                    MaxQualification = reader.GetString(2),
                    Specialization = reader.GetString(3),
                    DeptKey = reader.GetString(4),
                };
            }
            catch (Exception)
            {
                return NotFound();
            }
            if (curr == null)
                return NotFound();
            DoctorDtoNew d = new()
            {
                Name = curr.Name!,
                MaxQualification = curr.MaxQualification!,
                Specialization = curr.Specialization!,
                DeptKey = curr.DeptKey!
            };
            if (doctor.Name != null)
                d.Name = doctor.Name;
            if (doctor.Specialization != null)
                d.Specialization = doctor.Specialization;
            if (doctor.MaxQualification != null)
                d.MaxQualification = doctor.MaxQualification;
            if (doctor.DeptKey != null)
                d.DeptKey = doctor.DeptKey;
            return await Put(uname, d);
        }

        [HttpDelete("{uname}")]
        public async Task<ActionResult> Delete(string uname)
        {
            await using var cmd = db.CreateCommand("DELETE FROM doctors WHERE uname=@uname");
            cmd.Parameters.AddWithValue("uname", uname);
            try
            {
                if (cmd.ExecuteNonQuery() == 0)
                    return NotFound();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to Delete Doctor");
                return BadRequest();
            }
            return Ok();
        }
    }
}
