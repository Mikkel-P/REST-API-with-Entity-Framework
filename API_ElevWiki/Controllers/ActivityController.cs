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

    public class ActivityController : Controller
    {
        private readonly IDbInfoService iDbInfoService;
        private readonly IEmailService iEmailService;

        public ActivityController(IDbInfoService iDbInfoService, IEmailService iEmailService)
        {
            this.iDbInfoService = iDbInfoService;
            this.iEmailService = iEmailService;
        }

        #region Get requests
        [HttpGet("ActivityRecord/{id}", Name = "GetActivityRecord"), EnableCors("AllowAll")]
        public async Task<ActionResult<IIncludableQueryable<ActivityRecord, Student>>> GetActivityRecord(int id)
        {
            IEnumerable<ActivityRecord> activityRecord = await iDbInfoService.GetAllActivityRecordsById(id);

            if (activityRecord == null)
            {
                return NoContent();
            }
            return Ok(activityRecord);
        }

        [HttpGet, Route("Active/Students"), EnableCors("AllowAll")]
        public async Task<ActionResult<IEnumerable<Student>>> GetActiveStudents()
        {
            IEnumerable<ActivityRecord> result = await iDbInfoService.GetAllActiveStudents();

            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpGet, Route("Inactive/Students"), EnableCors("AllowAll")]
        public async Task<ActionResult<IEnumerable<Student>>> GetInactiveStudents()
        {
            IEnumerable<ActivityRecord> result = await iDbInfoService.GetAllInactiveStudents();

            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        #endregion

        #region Post request
        [HttpPost, Route("Create"), /*Authorize(Roles = "Admin"), */EnableCors("AllowAll")]
        public async Task<ActionResult<ActivityRecord>> CreateActivityRecord(ActivityDTO dto)
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

            ActivityRecord actRec = new()
            {
                timestamp = DateTime.Now,
                Student = student
            };

            if (dto.activeStatus.ToLower().Equals("true") || dto.activeStatus.Equals("1"))
            {
                actRec.activeStatus = true;
            }
            else
            {
                actRec.activeStatus = false;
            }

            iDbInfoService.CreateActivityRecord(actRec);

            await iDbInfoService.SaveChanges();

            return CreatedAtAction(nameof(GetActivityRecord), new { id = actRec.Student.studentId }, actRec);
        }
        #endregion

        #region Delete requests
        [HttpDelete, Route("Delete/Specific/ActivityRecord/{id}"), /*Authorize(Roles = "Admin"), */EnableCors("AllowAll")]
        public async Task<ActionResult<ActivityRecord>> DeleteSpecificActivityRecord(int id)
        {
            ActivityRecord aRec = new()
            {
                activeId = id
            };

            await iDbInfoService.DeleteSpecificActivityRecord(aRec.activeId);

            await iDbInfoService.SaveChanges();

            if (aRec == null)
            {
                return NoContent();
            }
            return Ok(aRec);
        }

        [HttpDelete, Route("Delete/Students/ActivityRecords/{id}"), /*Authorize(Roles = "Admin"), */EnableCors("AllowAll")]
        public async Task<ActionResult<ActivityRecord>> DeleteStudentsActivityRecords(int id)
        {
            Student student = await iDbInfoService.GetStudentById(id);

            ActivityRecord aRec = new()
            {
                Student = student
            };

            await iDbInfoService.DeleteStudentsActivityRecords(aRec.Student.studentId);

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
