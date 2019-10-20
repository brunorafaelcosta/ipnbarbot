using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ipnbarbot.Controllers
{
    [Route("admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private const string TokenKeyname = "AdminToken";

        private readonly ILogger _logger;
        private IConfiguration _configuration { get; }

        private readonly Application.IUsersApp _usersApp;

        public AdminController(ILogger<AdminController> logger, IConfiguration configuration,
            Application.IUsersApp usersApp)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this._usersApp = usersApp ?? throw new ArgumentNullException(nameof(usersApp));
        }

        [Route("getusers")]
        [HttpGet]
        [ProducesResponseType(typeof(IDictionary<string, string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUsers(string token)
        {
            try
            {
                if (token != this._configuration.GetValue<string>(TokenKeyname))
                {
                    this._logger.LogWarning($"Tried to access the method {(nameof(AdminController.GetUsers))} with an invalid token [Token: '{token}'].");
                    return BadRequest();
                }

                IEnumerable<Application.Models.User> users = await this._usersApp.GetAll();

                if (users != null && users.Count() > 0)
                {
                    IDictionary<string, string> result = new Dictionary<string, string>();
                    result = users.ToDictionary(u => u.ChannelAccountId, u => u.Name);

                    return Ok(result);
                }
                else
                {
                    throw new Exceptions.APIHandledException("Any user registered!");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);

                string msg = null;
                if (ex is Exceptions.APIHandledException || ex is Exceptions.ApplicationHandledException)
                    msg = ex.Message;

                return BadRequest(msg);
            }
            finally
            {
                this._usersApp.Dispose();
            }
        }

        [Route("getdb")]
        [HttpGet]
        [ProducesResponseType(typeof(FileContentResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetDB(string token)
        {
            try
            {
                if (token != this._configuration.GetValue<string>(TokenKeyname))
                {
                    this._logger.LogWarning($"Tried to access the method {(nameof(AdminController.GetDB))} with an invalid token [Token: '{token}'].");
                    return BadRequest();
                }

                string dbFilePath = this._configuration.GetValue<string>("DatabasePath");
                string downloadFileName = $"database-{DateTime.Now.ToString("yyyyMMddHHmmss")}.db";

                byte[] dbFileBytes = await System.IO.File.ReadAllBytesAsync(dbFilePath);

                this._logger.LogInformation($"Database downloaded on {DateTime.Now.ToString()} [Filename: {downloadFileName}]");

                return File(dbFileBytes, "application/force-download", downloadFileName);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

        [Route("getlog")]
        [HttpGet]
        [ProducesResponseType(typeof(FileContentResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLog(string token, string date)
        {
            try
            {
                if (token != this._configuration.GetValue<string>(TokenKeyname))
                {
                    this._logger.LogWarning($"Tried to access the method {(nameof(AdminController.GetLog))} with an invalid token [Token: '{token}'].");
                    return BadRequest();
                }

                if (DateTime.TryParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    string logFilePath = this._configuration.GetValue<string>("LogPathFormat");
                    logFilePath = logFilePath.Replace("{Date}", date);

                    string downloadFileName = $"log-{parsedDate.ToString("yyyyMMdd")}.txt";

                    if (System.IO.File.Exists(logFilePath))
                    {
                        using (var fs = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var sr = new StreamReader(fs, Encoding.Default))
                        {
                            var binaryReader = new BinaryReader(fs);

                            byte[] logFileBytes = await Task.Run<byte[]>(() =>
                            {
                                return binaryReader.ReadBytes((int)fs.Length);
                            });

                            this._logger.LogInformation($"Log downloaded on {DateTime.Now.ToString()} [Filename: {downloadFileName}]");

                            return File(logFileBytes, "application/force-download", downloadFileName);
                        }
                    }
                    else
                    {
                        this._logger.LogError($"Log file not found [Path: {logFilePath}]");
                        return NotFound();
                    }
                }
                else
                {
                    throw new ArgumentException(nameof(date));
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }
    }
}