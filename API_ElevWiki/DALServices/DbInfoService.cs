using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using API_ElevWiki.DataContext;
using API_ElevWiki.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace API_ElevWiki.Repository
{
    public class DbInfoService : IDbInfoService
    {
        private readonly EFContext context;

        public DbInfoService(EFContext context)
        {
            this.context = context;
        }

        #region Create/Delete/Alter methods
        #region Student methods
        public void CreateStudentRecord(Student student)
        {
            if (student.Equals(null))
            {
                throw new ArgumentNullException("Student object: " + nameof(student) + " cannot be null!");
            }
            context.Students.Add(student);
        }

        public void DeleteStudentRecord(Student student)
        {
            if (student.Equals(null))
            {
                throw new ArgumentNullException("Student object: " + nameof(student) + " cannot be null.");
            }
            context.Students.Remove(student);
        }
        #endregion

        #region Login methods
        public async Task CreateLoginRecord(LoginHolder loginHolder)
        {
            if (loginHolder.Equals(null))
            {
                throw new ArgumentNullException("Login object: " + nameof(loginHolder) + " cannot be null!");
            }

            loginHolder.passwordHash = await ComputePasswordHash(loginHolder.passwordHash, new SHA256CryptoServiceProvider());

            context.LoginHolders.Add(loginHolder);
        }

        public async Task DeleteLoginRecord(LoginHolder loginHolder)
        {
            if (loginHolder.Equals(null))
            {
                throw new ArgumentNullException("Login object: " + nameof(loginHolder) + " cannot be null!");
            }

            loginHolder.passwordHash = await ComputePasswordHash(loginHolder.passwordHash, new SHA256CryptoServiceProvider());

            context.LoginHolders.Remove(loginHolder);
        }
        #endregion

        #region Activity methods
        public void CreateActivityRecord(ActivityRecord activeStatus)
        {
            if (activeStatus.Equals(null))
            {
                throw new ArgumentNullException("Activity object: " + nameof(activeStatus) + " cannot be null.");
            }
            context.Actives.Add(activeStatus);
        }

        public async Task DeleteSpecificActivityRecord(int id)
        {
            if (id.Equals(null))
            {
                throw new ArgumentNullException("TimeRecord object: " + nameof(id) + " cannot be null.");
            }

            ActivityRecord[] activityHolder = await context.Actives.Where<ActivityRecord>
                (t => t.activeId == id).ToArrayAsync();

            context.Actives.Remove(activityHolder[0]);
        }

        public async Task DeleteStudentsActivityRecords(int id)
        {
            if (id.Equals(null))
            {
                throw new ArgumentNullException("TimeRecord object: " + nameof(id) + " cannot be null.");
            }

            ActivityRecord[] activityRecHolder = await context.Actives.Where<ActivityRecord>
                (t => t.Student.studentId == id).ToArrayAsync();

            for (int i = 0; i < activityRecHolder.Length; i++)
            {
                context.Actives.Remove(activityRecHolder[i]);
            }
        }
        #endregion

        #region Abscence methods
        public void CreateAbscenceRecord(AbscenceRecord abscenceRec)
        {
            if (abscenceRec.Equals(null))
            {
                throw new ArgumentNullException("Abscence object: " + nameof(abscenceRec) + " cannot be null.");
            }
            context.Abscences.Add(abscenceRec);
        }

        public async Task DeleteSpecificAbscenceRecord(int id)
        {
            if (id.Equals(null))
            {
                throw new ArgumentNullException("TimeRecord object: " + nameof(id) + " cannot be null.");
            }

            AbscenceRecord[] abscenceHolder = await context.Abscences.Where<AbscenceRecord>
                (a => a.abscenceId == id).ToArrayAsync();

            context.Abscences.Remove(abscenceHolder[0]);
        }

        public async Task DeleteStudentsAbscenceRecords(int id)
        {
            if (id.Equals(null))
            {
                throw new ArgumentNullException("TimeRecord object: " + nameof(id) + " cannot be null.");
            }

            AbscenceRecord[] abscenceRecHolder = await context.Abscences.Where<AbscenceRecord>
                (t => t.Student.studentId == id).ToArrayAsync();

            for (int i = 0; i < abscenceRecHolder.Length; i++)
            {
                context.Abscences.Remove(abscenceRecHolder[i]);
            }
        }
        #endregion

        #region TimeRecord methods
        public void CreateTimeRecord(TimeRecord tRec)
        {
            if (tRec.Equals(null))
            {
                throw new ArgumentNullException("TimeRecord object: " + nameof(tRec) + " cannot be null.");
            }
            context.TimeRecords.Add(tRec);
        }

        public async Task DeleteSpecificTimeRecord(int id)
        {
            if (id.Equals(null))
            {
                throw new ArgumentNullException("TimeRecord object: " + nameof(id) + " cannot be null.");
            }

            TimeRecord[] tRecHolder = await context.TimeRecords.Where<TimeRecord>
                (t => t.recId == id).ToArrayAsync();

            context.TimeRecords.Remove(tRecHolder[0]);
        }

        public async Task DeleteStudentsTimeRecords(int id)
        {
            if (id.Equals(null))
            {
                throw new ArgumentNullException("TimeRecord object: " + nameof(id) + " cannot be null.");
            }

            TimeRecord[] tRecHolder = await context.TimeRecords.Where<TimeRecord>
                (t => t.Student.studentId == id).ToArrayAsync();

            for (int i = 0; i < tRecHolder.Length; i++)
            {
                context.TimeRecords.Remove(tRecHolder[i]);
            }
        }
        #endregion

        #region Alter methods
        public async Task<LoginHolder> AlterLoginHolderById(int id)
        {
            return await context.LoginHolders.FirstOrDefaultAsync(l => l.Student.studentId == id);
        }

        public async Task<LoginHolder> AlterPassword(string email, string password)
        {
            password = await ComputePasswordHash(password, new SHA256CryptoServiceProvider());
            return await context.LoginHolders.FirstOrDefaultAsync
                (l => l.passwordHash == password && l.email == email);
        }
        #endregion
        #endregion

        #region IEnumerable methods
        public async Task<IEnumerable<Student>> GetAllStudents()
        {
            return await context.Students.AsNoTracking().ToArrayAsync();
        }

        public async Task<IEnumerable<ActivityRecord>> GetAllActiveStudents()
        {
            return await context.Actives.AsNoTracking().Where
                (a => a.activeStatus == true).Include((s) => s.Student).ToArrayAsync();
        }

        public async Task<IEnumerable<ActivityRecord>> GetAllInactiveStudents()
        {
            return await context.Actives.AsNoTracking().Where
                (a => a.activeStatus == false).Include((s) => s.Student).ToArrayAsync();
        }

        public async Task<IEnumerable<AbscenceRecord>> GetAllAbscenceRecordsById(int id)
        {
            return await context.Abscences.AsNoTracking().Where
                (t => t.Student.studentId == id).Include((s) => s.Student).ToArrayAsync();
        }

        public async Task<IEnumerable<ActivityRecord>> GetAllActivityRecordsById(int id)
        {
            return await context.Actives.AsNoTracking().Where
                (t => t.Student.studentId == id).Include((s) => s.Student).ToArrayAsync();
        }

        public async Task<IEnumerable<TimeRecord>> GetAllTimeRecordsById(int id)
        {
            return await context.TimeRecords.AsNoTracking().Where
                (t => t.Student.studentId == id).Include((s) => s.Student).ToArrayAsync();
        }

        public async Task<IEnumerable<TimeRecord>> AllStudsLatestTRec()
        {
            TimeRecord[] tRec = await context.TimeRecords.AsNoTracking().Where
                (t => t.timestamp > DateTime.Now.AddMinutes(-720)).Include
                ((s) => s.Student).ToArrayAsync();

            return tRec;
        }
        #endregion

        #region IIncludeQueryable methods
        public async Task<IIncludableQueryable<NfcCard, Student>> GetStudentFromNfcTag(string nfcTag)
        {
            return await Task.FromResult(context.NfcCards.Where<NfcCard>
                (p => p.nfcTag == nfcTag).Include((p) => p.Student));
        }

        public async Task<IIncludableQueryable<AbscenceRecord, Student>> GetAbscenceRecordsById(int id)
        {
            return await Task.FromResult(context.Abscences.Where<AbscenceRecord>
                (a => a.Student.studentId == id).Include((s) => s.Student));
        }

        public async Task<IIncludableQueryable<ActivityRecord, Student>> GetActivityRecordsById(int id)
        {
            return await Task.FromResult(context.Actives.Where<ActivityRecord>
                (a => a.Student.studentId == id).Include((s) => s.Student));
        }

        public async Task<IIncludableQueryable<LoginHolder, Student>> GetLoginRecordById(int id)
        {
            return await Task.FromResult(context.LoginHolders.Where<LoginHolder>
                (l => l.Student.studentId == id).Include((s) => s.Student));
        }

        public async Task<IIncludableQueryable<TimeRecord, Student>> GetTimeRecordById(int id)
        {
            return await Task.FromResult(context.TimeRecords.Where<TimeRecord>
                (t => t.Student.studentId == id).Include((s) => s.Student));
        }
        #endregion

        #region Fetch/Get methods
        public async Task<string> GetEmailById(int id)
        {
            LoginHolder login = await context.LoginHolders.FirstOrDefaultAsync
                (l => l.Student.studentId == id);

            if (login == null)
            {
                return null;
            }

            return login.email;
        }

        public async Task<Student> GetStudentById(int id)
        {
            return await context.Students.FirstOrDefaultAsync
                (s => s.studentId == id);
        }

        public async Task<Student> FetchStudentFromNameAndPhone(string name, string phone)
        {
            return await context.Students.FirstOrDefaultAsync
                (s => s.name == name && s.phone == phone);
        }

        public async Task<LoginHolder> GetLoginHolderFromStudentId(int id)
        {
            return await context.LoginHolders.FirstOrDefaultAsync
                (l => l.Student.studentId == id);
        }

        public async Task<LoginHolder> GetLoginHolderFromEmail(string email)
        {
            return await context.LoginHolders.FirstOrDefaultAsync(e => e.email == email);
        }

        public async Task<LoginHolder> GetLoginHolderFromToken(string token)
        {
            return await context.LoginHolders.FirstOrDefaultAsync(t => t.confirmationToken == token);
        }

        public async Task<TimeRecord[]> FetchTimeRecordsThisWeekById(int id)
        {
            TimeRecord[] timeRecords = await context.TimeRecords.Where<TimeRecord>
                (t => t.Student.studentId == id && t.timestamp > DateTime.Now.AddDays(-6)).ToArrayAsync();

            return timeRecords;
        }
        #endregion

        #region Check methods
        public async Task<bool> SignInCheck(string email, string password)
        {
            password = await ComputePasswordHash(password, new SHA256CryptoServiceProvider());

            LoginHolder user = await context.LoginHolders.FirstOrDefaultAsync<LoginHolder>
                (l => l.email == email && l.passwordHash == password);

            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> IsEmailConfirmed(string email, string password)
        {
            password = await ComputePasswordHash(password, new SHA256CryptoServiceProvider());

            LoginHolder user = await context.LoginHolders.FirstOrDefaultAsync<LoginHolder>
                (l => l.email == email && l.passwordHash == password);

            if (user.confirmedEmail == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> StudentCheck(string name, string phone)
        {
            Student student = await context.Students.FirstOrDefaultAsync(s => s.name == name && s.phone == phone);

            if (student == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region Password encryption method
        public async Task<string> ComputePasswordHash(string input, HashAlgorithm algorithm)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            using (MemoryStream stream = new(inputBytes))
            {                
                byte[] hashedBytes = await algorithm.ComputeHashAsync(stream);
                
                await stream.DisposeAsync();

                return BitConverter.ToString(hashedBytes);
            }
        }
        #endregion

        #region SaveChanges method
        public async Task<bool> SaveChanges()
        {
            return await context.SaveChangesAsync() >= 0;
        }
        #endregion
    }
}
