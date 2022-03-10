using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using API_ElevWiki.DataTransfer;
using API_ElevWiki.Interfaces;
using API_ElevWiki.Repository;
using API_ElevWiki.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace API_ElevWiki.Controllers
{
    [Route("API/[controller]")]
    [ApiController]
    public class StudentController : Controller
    {
        private readonly IDbInfoService iDbInfoService;
        private readonly IEmailService iEmailService;

        public StudentController(IDbInfoService iDbInfoService, IEmailService iEmailService)
        {
            this.iDbInfoService = iDbInfoService;
            this.iEmailService = iEmailService;
        }

        #region Get request
        [HttpGet("StudentRecord/{id}", Name = "GetStudentRecord"), EnableCors("AllowAll")]
        public async Task<ActionResult<Student>> GetStudentRecord(int id)
        {
            // Retrieves 
            Student student = await iDbInfoService.GetStudentById(id);

            if (student == null)
            {
                return NoContent();
            }
            return Ok(student);
        }

        [HttpGet, Route("Get/All"), EnableCors("AllowAll")]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {
            IEnumerable<Student> result = await iDbInfoService.GetAllStudents();

            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
        #endregion

        #region Post request
        [HttpPost, Route("Create"), /*Authorize(Roles = "Admin"), */EnableCors("AllowAll")]
        public async Task<ActionResult<Student>> CreateStudentRecord(StudentRegistrationDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid client request");
            }

            bool check = await iDbInfoService.StudentCheck(dto.name.ToLower(), dto.phone);

            if (check == true)
            {
                return BadRequest("User already exists");
            }

            Student student = new()
            {
                name = dto.name.ToLower(),
                address = dto.address.ToLower(),
                phone = dto.phone
            };

            iDbInfoService.CreateStudentRecord(student);

            await iDbInfoService.SaveChanges();

            ActivityRecord aRec = new()
            {
                activeStatus = true,
                timestamp = DateTime.Now,
                Student = student
            };

            iDbInfoService.CreateActivityRecord(aRec);

            await iDbInfoService.SaveChanges();

            return CreatedAtAction(nameof(GetStudentRecord), new { id = student.studentId }, student);
        }
        #endregion

        #region Delete request
        [HttpDelete, Route("Delete/All/{id}"), /*Authorize(Roles = "Admin"), */EnableCors("AllowAll")]
        public async Task<ActionResult<Student>> DeleteStudentsRecordsFromDB(int id)
        {
            Student student = await iDbInfoService.GetStudentById(id);

            LoginHolder user = await iDbInfoService.GetLoginHolderFromStudentId(student.studentId);

            if (user != null)
            {
                await iDbInfoService.DeleteLoginRecord(user);
            }

            await iDbInfoService.DeleteStudentsActivityRecords(student.studentId);

            await iDbInfoService.DeleteStudentsAbscenceRecords(student.studentId);

            await iDbInfoService.DeleteStudentsTimeRecords(student.studentId);
            
            iDbInfoService.DeleteStudentRecord(student);
            
            await iDbInfoService.SaveChanges();

            if (student == null)
            {
                return NoContent();
            }
            return Ok(student);
        }
        #endregion
    }
}
