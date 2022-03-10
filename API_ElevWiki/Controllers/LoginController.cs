using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using API_ElevWiki.Models.Configuration;
using API_ElevWiki.DataTransferObjects;
using API_ElevWiki.DataTransfer;
using API_ElevWiki.Repository;
using API_ElevWiki.Interfaces;
using API_ElevWiki.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Query;
using System.Diagnostics;

namespace API_ElevWiki.Controllers
{
    [Route("API/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IDbInfoService iDbInfoService;
        private readonly IEmailService iEmailService;
        private readonly ITokenService iTokenService;

        public LoginController(IDbInfoService iDbInfoService,
            ITokenService iTokenService, IEmailService iEmailService)
        {
            this.iDbInfoService = iDbInfoService;
            this.iEmailService = iEmailService;
            this.iTokenService = iTokenService;
        }

        #region Get request
        [HttpGet("LoginRecord/{id}", Name = "GetLoginRecord"), EnableCors("AllowAll")]
        public async Task<ActionResult<IIncludableQueryable<LoginHolder, Student>>> GetLoginRecord(int id)
        {
            IIncludableQueryable<LoginHolder, Student> loginRecord = await iDbInfoService.GetLoginRecordById(id);

            if (loginRecord == null)
            {
                return NoContent();
            }
            return Ok(loginRecord);
        }

        [HttpGet, Route("Verify/Token={token}"), EnableCors("AllowAll")]
        public async Task<ActionResult> Verify(string token)
        {
            LoginHolder user = await iDbInfoService.GetLoginHolderFromToken(token);

            if (user.confirmationToken == token)
            {
                return Ok(user.confirmationToken);
            }
            return NoContent();
        }
        #endregion

        #region Post requests
        [HttpPost, Route("Register"), EnableCors("AllowAll")]
        public async Task<IActionResult> Register(LoginRegistrationDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid client request");
            }

            Student student = await iDbInfoService.FetchStudentFromNameAndPhone(dto.name.ToLower(), dto.phone);
            string emailCheck = await iDbInfoService.GetEmailById(student.studentId);

            if (emailCheck != null)
            {
                return BadRequest("User already exists");
            }

            LoginHolder loginHolder = new()
            {
                admin = false,
                Student = student,
                email = dto.email.ToLower(),
                passwordHash = dto.passwordHash,
                confirmationToken = await iTokenService.GenerateRandomToken()
            };

            ConfirmationEmailMessage emailMessage = new(new string[] 
            {
                $"{loginHolder.email}" 
            }, "ElevWiki - Verificering af email");

            // Manipulates the clientURI string to create the a part of the email confirmation link.
            string[] clientURI = dto.clientURI.Split(@"/");
            string[] hostAndPort = clientURI[2].Split(":");

            // Adds a path to the specified request in the URL.
            string actionPath = Url.Action("Verify");

            UriBuilder builder = new()
            {
                Scheme = clientURI[0],
                Host = hostAndPort[0],
                Port = Convert.ToInt32(hostAndPort[1]),
                Path = clientURI[3] + actionPath + $"/Token={loginHolder.confirmationToken}"
            };

            Uri uri = builder.Uri;

            string confirmationLink = uri.ToString();

            await iEmailService.SendEmailConfirmationLink(emailMessage, confirmationLink);

            await iDbInfoService.CreateLoginRecord(loginHolder);

            await iDbInfoService.SaveChanges();

            return CreatedAtAction(nameof(GetLoginRecord), new
            {
                id = loginHolder.Student.studentId
            }, loginHolder);
        }

        [HttpPost, Route("Login"), EnableCors("AllowAll"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid client request");
            }

            bool check = await iDbInfoService.SignInCheck(dto.email, dto.passwordHash);

            if (await iDbInfoService.IsEmailConfirmed(dto.email, dto.passwordHash) == false)
            {
                return BadRequest("You must confirm your email before you can login.");
            }

            if (check == true)
            {
                List<Claim> allClaims = new();
                List<Claim> roleClaim = await iTokenService.AddRoleClaim(dto.email);
                List<Claim> emailClaim = await iTokenService.AddEmailClaim(dto.email);

                allClaims.AddRange(emailClaim);
                allClaims.AddRange(roleClaim);

                string token = await iTokenService.GenerateAccessToken(allClaims);
                return Ok(new { Token = token });
            }
            else
            {
                return Unauthorized("No credentials match the attempted login.");
            }
        }

        [HttpPost, Route("Refresh"), EnableCors("AllowAll")]
        public async Task<IActionResult> Refresh(TokenDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid client request");
            }

            string accessToken = dto.accessToken;
            string refreshToken = dto.refreshToken;

            ClaimsPrincipal principal = await iTokenService.GetPrincipalFromExpiredToken(accessToken);

            string userEmail = principal.FindFirst("Email").Value;

            LoginHolder user = await iDbInfoService.GetLoginHolderFromEmail(userEmail);

            if (user == null || user.refreshToken != refreshToken || user.refreshTokenExpiryTime <= DateTime.Now)
            {
                return NoContent();
            }

            string newAccessToken = await iTokenService.GenerateAccessToken(principal.Claims);
            string newRefreshToken = await iTokenService.GenerateRandomToken();

            user.refreshToken = newRefreshToken;
            user.refreshTokenExpiryTime = DateTime.Now.AddHours(2);

            await iDbInfoService.SaveChanges();

            return new ObjectResult(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        [HttpPost, Route("Revoke"), EnableCors("AllowAll")]
        public async Task<IActionResult> Revoke(TokenDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid client request");
            }

            string token = dto.accessToken;

            ClaimsPrincipal principal = await iTokenService.GetPrincipalFromExpiredToken(token);

            string userEmail = principal.FindFirst("Email").Value;

            LoginHolder user = await iDbInfoService.GetLoginHolderFromEmail(userEmail);

            if (user == null)
            {
                return NoContent();
            }

            user.refreshToken = null;

            await iDbInfoService.SaveChanges();

            return Ok();
        }
        #endregion

        #region Put requests
        [HttpPut, Route("Confirm/Email/{token}"), EnableCors("AllowAll"), AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            LoginHolder user = await iDbInfoService.GetLoginHolderFromToken(token);

            if (user == null)
            {
                return NoContent();
            }

            if (user.confirmationToken == token)
            {
                user.confirmedEmail = true;
            }
            else
            {
                return BadRequest();
            }

            await iDbInfoService.SaveChanges();

            return Ok(user);
        }

        [HttpPut, Route("Alter/Password"), EnableCors("AllowAll")]
        public async Task<ActionResult> AlterPassword(AlterPasswordDTO dto)
        {
            LoginHolder user = await iDbInfoService.AlterPassword(dto.email.ToLower(), dto.oldPasswordHash);

            if (dto == null)
            {
                return BadRequest("Invalid client request");
            }

            if (user == null)
            {
                return NoContent();
            }

            user.passwordHash = await iDbInfoService.ComputePasswordHash(dto.newPasswordHash, new SHA256CryptoServiceProvider());

            try
            {
                await iDbInfoService.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (user.passwordHash == dto.oldPasswordHash)
                {
                    return BadRequest("The new password, cant be the same as the old one.");
                }
                else
                {
                    Console.WriteLine(ex);
                    Debug.WriteLine(ex);
                }
            }
            return Ok(user);
        }

        [HttpPut, Route("Alter/Role"), /*Authorize(Roles = "Admin"), */EnableCors("AllowAll")]
        public async Task<ActionResult> AlterAdminRole(AlterRoleDTO dto)
        {
            LoginHolder user = await iDbInfoService.GetLoginHolderFromStudentId(dto.studentId);

            if (dto == null)
            {
                return BadRequest("Invalid client request");
            }

            if (user == null)
            {
                return NoContent();
            }

            if (dto.admin.Contains("true") || dto.admin.Contains("True"))
            {
                user.admin = true;
            }

            try
            {
                await iDbInfoService.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (user.Student.studentId != dto.studentId)
                {
                    return BadRequest();
                }
                else
                {
                    Console.WriteLine(ex);
                    Debug.WriteLine(ex);
                }
            }
            return Ok(user);
        }
        #endregion

        #region Delete request
        [HttpDelete, Route("Delete/{email}"), /*Authorize(Roles = "Admin"), */EnableCors("AllowAll")]
        public async Task<ActionResult<LoginHolder>> DeleteLoginRecord(string email)
        {
            LoginHolder loginRecord = await iDbInfoService.GetLoginHolderFromEmail(email);

            if (loginRecord == null)
            {
                return NoContent();
            }

            await iDbInfoService.DeleteLoginRecord(loginRecord);

            await iDbInfoService.SaveChanges();

            return Ok(loginRecord);
        }
        #endregion
    }
}
