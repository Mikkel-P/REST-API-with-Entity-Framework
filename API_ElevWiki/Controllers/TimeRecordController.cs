using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using API_ElevWiki.DataTransferObjects;
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

    public class TimeRecordController : Controller
    {
        private readonly IDbInfoService iDbInfoService;
        private readonly IEmailService iEmailService;

        private readonly string commPort1 = "ACS ACR122U PICC Interface 00 00";
        private readonly string commPort2 = "ACS ACR122U PICC Interface 01 00";

        public TimeRecordController(IDbInfoService iDbInfoService, IEmailService iEmailService)
        {
            this.iDbInfoService = iDbInfoService;
            this.iEmailService = iEmailService;
        }

        #region Get requests
        [HttpGet("TimeRecord/{id}", Name = "GetTimeRecord"), EnableCors("AllowAll")]
        public async Task<ActionResult<IIncludableQueryable<TimeRecord, Student>>> GetTimeRecord(int id)
        {
            IIncludableQueryable<TimeRecord, Student> timeRecord = await iDbInfoService.GetTimeRecordById(id);

            if (timeRecord == null)
            {
                return NoContent();
            }
            return Ok(timeRecord);
        }

        [HttpGet, Route("Get/Students/TimeRecords/{id}"), EnableCors("AllowAll")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentsTimeRecords(int id)
        {
            IEnumerable<TimeRecord> result = await iDbInfoService.GetAllTimeRecordsById(id);

            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpGet, Route("Get/All/Students/Latest/TimeRecord"), EnableCors("AllowAll")]
        public async Task<ActionResult<IEnumerable<IIncludableQueryable<TimeRecord, Student>>>> GetAllStudentsLatestTimeRecord()
        {
            IEnumerable<TimeRecord> tRecHolder = await iDbInfoService.AllStudsLatestTRec();

            if (tRecHolder == null)
            {
                return NoContent();
            }
            return Ok(tRecHolder);
        }
        #endregion

        #region Post request
        [HttpPost, Route("Create"), EnableCors("AllowAll")]
        public async Task<ActionResult<NfcDTO>> CreateTimeRecord(NfcDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid client request");
            }

            #region NfcCheck
            Task<IIncludableQueryable<NfcCard, Student>> nfcStudHolder = iDbInfoService.GetStudentFromNfcTag(dto.nfcTag);

            if (nfcStudHolder == null)
            {
                // If it doesn't exist already, a no content 204 status code, is sent to the Kestrel CLI.
                Console.WriteLine($"\n NfcTag received: {dto.nfcTag} \n Check status: {dto.checkStatus} \n");
                return NoContent();
            }
            #endregion

            IEnumerable<NfcCard> nfcStudent = nfcStudHolder.Result.AsEnumerable();

            #region Console output
            Console.WriteLine("\n Student: {0} \n", nfcStudent.ToArray()[0].Student.studentId);
            #endregion

            #region TimeRecord
            TimeRecord timeRec = new()
            {
                timestamp = DateTime.Now,
                Student = nfcStudent.ToArray()[0].Student
            };
            #endregion

            #region Status + DayOfWeek check
            // Checks which comm port has been used, and reduces the string to a single 1 or 2, to easily differentiate.
            if (dto.checkStatus.Equals(commPort1))
            {
                timeRec.checkStatus = "1";
            }
            else if (dto.checkStatus.Equals(commPort2))
            {
                timeRec.checkStatus = "2";

                DayOfWeek dow = DateTime.Today.DayOfWeek;

                if (dow == DayOfWeek.Friday)
                {
                    // Send email to the person with an overview of the current week's timerecords.
                    LoginHolder user = new()
                    {
                        Student = nfcStudent.ToArray()[0].Student,
                    };

                    int studentId = user.Student.studentId;

                    string email = await iDbInfoService.GetEmailById(studentId);

                    TimeRecord[] timeRecords = await iDbInfoService.FetchTimeRecordsThisWeekById(studentId);

                    WeeklyOverviewEmailMessage message = new(new string[] { $"{email}" }, "ElevWiki");

                    await iEmailService.SendEmailWeeklyOverview(timeRecords, message);
                }
            }
            else
            {
                Console.WriteLine($"\n Invalid check status: {dto.checkStatus} \n");
                return NoContent();
            }
            #endregion

            #region Submission
            // Submits the timeTemp object to the current context.
            iDbInfoService.CreateTimeRecord(timeRec);

            await iDbInfoService.SaveChanges();

            Console.WriteLine($"\n NfcTag received: {dto.nfcTag} \n Check status: {dto.checkStatus} \n");

            // Returns a statuscode response.
            return CreatedAtAction(nameof(GetTimeRecord), new { id = timeRec.recId }, timeRec);
            #endregion
        }
        #endregion

        #region Delete requests
        [HttpDelete, Route("Delete/Specific/TimeRecord/{id}"), /*Authorize(Roles = "Admin"), */EnableCors("AllowAll")]
        public async Task<ActionResult<TimeRecord>> DeleteSpecificTimeRecord(int id)
        {
            TimeRecord tRec = new()
            {
                recId = id
            };

            await iDbInfoService.DeleteSpecificTimeRecord(tRec.recId);

            await iDbInfoService.SaveChanges();

            if (tRec == null)
            {
                return NoContent();
            }
            return Ok(tRec);
        }

        [HttpDelete, Route("Delete/Students/TimeRecords/{id}"), /*Authorize(Roles = "Admin"), */EnableCors("AllowAll")]
        public async Task<ActionResult<TimeRecord>> DeleteStudentsTimeRecords(int id)
        {
            Student student = await iDbInfoService.GetStudentById(id);

            TimeRecord tRec = new()
            {
                Student = student
            };

            await iDbInfoService.DeleteStudentsTimeRecords(tRec.Student.studentId);

            await iDbInfoService.SaveChanges();

            if (tRec == null)
            {
                return NoContent();
            }
            return Ok(tRec);
        }
        #endregion
    }
}
