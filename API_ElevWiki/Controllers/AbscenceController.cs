using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using API_ElevWiki.DataTransfer;
using API_ElevWiki.Interfaces;
using API_ElevWiki.Repository;
using API_ElevWiki.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore.Query;

namespace API_ElevWiki.Controllers
{
    [Route("API/[controller]")]
    [ApiController]

    public class AbscenceController : Controller
    {
        private readonly IDbInfoService iDbInfoService;
        private readonly IEmailService iEmailService;

        public AbscenceController(IDbInfoService iDbInfoService, IEmailService iEmailService)
        {
            this.iDbInfoService = iDbInfoService;
            this.iEmailService = iEmailService;
        }

        #region Get request
        [HttpGet("AbscenceRecord/{id}", Name = "GetAbscenceRecord"), EnableCors("AllowAll")]
        public async Task<ActionResult<IIncludableQueryable<AbscenceRecord, Student>>> GetAbscenceRecord(int id)
        {
            IEnumerable<AbscenceRecord> abscenceRecord = await iDbInfoService.GetAllAbscenceRecordsById(id);

            if (abscenceRecord == null)
            {
                return NoContent();
            }
            return Ok(abscenceRecord);
        }
        #endregion

        #region Post request
        [HttpPost, Route("Create"), /*Authorize(Roles = "Admin"), */EnableCors("AllowAll")]
        public async Task<ActionResult<AbscenceRecord>> CreateAbscenceRecord(AbscenceDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid client request");
            }

            Student student = await iDbInfoService.GetStudentById(dto.studentId);

            if (student == null)
            {
                return NoContent();
            }

            AbscenceRecord abscStatus = new()
            {
                timestamp = DateTime.Now,
                Student = student
            };

            if (dto.legalAbscence.ToLower().Equals("true") || dto.legalAbscence.Equals("1"))
            {
                abscStatus.legalAbscence = true;
            }
            else
            {
                abscStatus.legalAbscence = false;
            }

            iDbInfoService.CreateAbscenceRecord(abscStatus);

            await iDbInfoService.SaveChanges();

            return CreatedAtAction(nameof(GetAbscenceRecord), new { id = abscStatus.Student.studentId }, abscStatus);
        }
        #endregion

        #region Delete requests
        [HttpDelete, Route("Delete/Specific/AbscenceRecord/{id}"), /*Authorize(Roles = "Admin"), */EnableCors("AllowAll")]
        public async Task<ActionResult<AbscenceRecord>> DeleteSpecificAbscenceRecord(int id)
        {
            AbscenceRecord aRec = new()
            {
                abscenceId = id
            };

            await iDbInfoService.DeleteSpecificAbscenceRecord(aRec.abscenceId);

            await iDbInfoService.SaveChanges();

            if (aRec == null)
            {
                return NoContent();
            }
            return Ok(aRec);
        }

        [HttpDelete, Route("Delete/Students/AbscenceRecords/{id}"), /*Authorize(Roles = "Admin"), */EnableCors("AllowAll")]
        public async Task<ActionResult<AbscenceRecord>> DeleteStudentsAbscenceRecords(int id)
        {
            Student student = await iDbInfoService.GetStudentById(id);

            AbscenceRecord aRec = new()
            {
                Student = student
            };

            await iDbInfoService.DeleteStudentsAbscenceRecords(aRec.Student.studentId);

            await iDbInfoService.SaveChanges();

            if (aRec == null)
            {
                return NoContent();
            }
            return Ok(aRec);
        }
        #endregion
    }
}
