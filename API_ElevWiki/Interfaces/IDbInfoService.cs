using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Query;
using API_ElevWiki.Models;

namespace API_ElevWiki.Repository
{
    // Interface needed to interact with the DbInfoService methods
    public interface IDbInfoService
    {
        #region Create/Delete/Alter/Entity State methods
        #region Student methods
        void CreateStudentRecord(Student student);
        void DeleteStudentRecord(Student student);
        #endregion

        #region LoginHolder methods
        Task CreateLoginRecord(LoginHolder loginHolder);
        Task DeleteLoginRecord(LoginHolder loginHolder);
        #endregion

        #region ActivityRecord methods
        void CreateActivityRecord(ActivityRecord activeStatus);
        Task DeleteSpecificActivityRecord(int id);
        Task DeleteStudentsActivityRecords(int id);
        #endregion

        #region AbscenceRecord methods
        void CreateAbscenceRecord(AbscenceRecord abscenceRec);
        Task DeleteSpecificAbscenceRecord(int id);
        Task DeleteStudentsAbscenceRecords(int id);
        #endregion

        #region TimeRecord methods
        void CreateTimeRecord(TimeRecord tRec);
        Task DeleteSpecificTimeRecord(int id);
        Task DeleteStudentsTimeRecords(int id);
        #endregion

        #region Alter methods
        Task<LoginHolder> AlterLoginHolderById(int id);
        Task<LoginHolder> AlterPassword(string email, string password);
        #endregion
        #endregion

        #region Fetch/Get methods
        Task<string> GetEmailById(int id);
        Task<Student> GetStudentById(int id);
        Task<Student> FetchStudentFromNameAndPhone(string name, string phone);
        Task<LoginHolder> GetLoginHolderFromStudentId(int id);
        Task<LoginHolder> GetLoginHolderFromEmail(string email);
        Task<LoginHolder> GetLoginHolderFromToken(string token);
        Task<TimeRecord[]> FetchTimeRecordsThisWeekById(int id);
        #endregion

        #region Check methods
        Task<bool> StudentCheck(string name, string phone);
        Task<bool> SignInCheck(string email, string password);
        Task<bool> IsEmailConfirmed(string email, string password);
        #endregion

        #region IIncludeQueryable methods
        Task<IIncludableQueryable<NfcCard, Student>> GetStudentFromNfcTag(string nfcTag);
        Task<IIncludableQueryable<AbscenceRecord, Student>> GetAbscenceRecordsById(int id);
        Task<IIncludableQueryable<ActivityRecord, Student>> GetActivityRecordsById(int id);
        Task<IIncludableQueryable<LoginHolder, Student>> GetLoginRecordById(int id);
        Task<IIncludableQueryable<TimeRecord, Student>> GetTimeRecordById(int id);
        #endregion

        #region IEnumerable methods
        Task<IEnumerable<Student>> GetAllStudents();
        Task<IEnumerable<ActivityRecord>> GetAllActiveStudents();
        Task<IEnumerable<ActivityRecord>> GetAllInactiveStudents();
        Task<IEnumerable<ActivityRecord>> GetAllActivityRecordsById(int id);
        Task<IEnumerable<AbscenceRecord>> GetAllAbscenceRecordsById(int id);
        Task<IEnumerable<TimeRecord>> GetAllTimeRecordsById(int id);
        Task<IEnumerable<TimeRecord>> AllStudsLatestTRec();
        #endregion

        Task<string> ComputePasswordHash(string input, HashAlgorithm algorithm);

        Task<bool> SaveChanges();
    }
}
