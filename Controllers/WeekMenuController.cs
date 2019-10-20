using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ipnbarbot.Controllers
{
    [Route("api/weekmenu")]
    [ApiController]
    public class WeekMenuController : ControllerBase
    {
        private const string TokenKeyname = "ApiToken";
        private readonly ILogger _logger;
        private IConfiguration _configuration { get; }

        private readonly Application.IMealSchedulesApp _mealSchedulesApp;
        
        public WeekMenuController(ILogger<WeekMenuController> logger, IConfiguration configuration,  
            Application.IMealSchedulesApp mealSchedulesApp)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this._mealSchedulesApp = mealSchedulesApp ?? throw new ArgumentNullException(nameof(mealSchedulesApp));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IDictionary<string, ViewModels.DailyMenuViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var weekMealSchedules = await this._mealSchedulesApp.GetForWeek(Application.Helpers.DateHelpers.Today);

                if (weekMealSchedules != null && weekMealSchedules.Count() > 0)
                {
                    IDictionary<string, ViewModels.DailyMenuViewModel> result = new Dictionary<string, ViewModels.DailyMenuViewModel>();

                    foreach (var dailyMeal in weekMealSchedules.OrderBy(wms => wms.Date))
                    {
                        ViewModels.DailyMenuViewModel dailyMenuEntry = new ViewModels.DailyMenuViewModel()
                        {
                            Soup = dailyMeal.SoupMeal.Name,
                            MainDish = dailyMeal.MainDishMeal.Name,
                            VeganDish = dailyMeal.VeganDishMeal.Name
                        };

                        result.Add(dailyMeal.Day, dailyMenuEntry);
                    }

                    if (result.Count() <= 0)
                        throw new Exceptions.APIHandledException("Ups! Didn't found any meals scheduled for this week!");
                    
                    return Ok(result);
                }
                else
                {
                    throw new Exceptions.APIHandledException("Ups! Didn't found any meals scheduled for this week!");
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
                this._mealSchedulesApp.Dispose();
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Post(string token, [FromBody]Dictionary<string, ViewModels.DailyMenuViewModel> request)
        {
            try
            {
                if (token != this._configuration.GetValue<string>(TokenKeyname))
                {
                    this._logger.LogWarning($"Tried to access the method {(nameof(WeekMenuController.Post))} with an invalid token [Token: '{token}'].");
                    return BadRequest();
                }
                if (request is null)
                    throw new ArgumentNullException(nameof(request));
                else if (request.Keys is null || request.Keys.Count == 0)
                    throw new Exceptions.APIHandledException("Provide the menu for at least one day of the week!");
                else if (request.Keys.All(d => Application.Constants.DaysOfWeek.ListOfDays.Contains(d)) == false)
                    throw new Exceptions.APIHandledException("Week menu contains an invalid day name");
                
                foreach (var dailyMenuEntry in request)
                {
                    var dailyMenu = dailyMenuEntry.Value;
                    if (ViewModels.DailyMenuViewModel.IsValid(dailyMenu) == false)
                        throw new Exceptions.APIHandledException($"Invalid menu for '{dailyMenu}'");

                    DateTime date = Application.Helpers.DateHelpers.GetDateForDailyMenuByDayOfWeek(dailyMenuEntry.Key);
                    
                    await this._mealSchedulesApp.AddAsync(date, dailyMenu.Soup, dailyMenu.MainDish, dailyMenu.VeganDish);
                }

                return CreatedAtAction(nameof(Post), "Menu sucessfully set for the current week!");
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
                this._mealSchedulesApp.Dispose();
            }
        }

        [Route("removeforweek")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]    
        public async Task<IActionResult> RemoveForWeek(string token, string weekdate)
        {
            try
            {
                if (token != this._configuration.GetValue<string>(TokenKeyname))
                {
                    this._logger.LogWarning($"Tried to access the method {(nameof(WeekMenuController.RemoveForWeek))} with an invalid token [Token: '{token}'].");
                    return BadRequest();
                }
                
                if (DateTime.TryParseExact(weekdate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate)
                    || parsedDate.DayOfWeek == DayOfWeek.Saturday || parsedDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    DateTime weekStartDate = parsedDate.AddDays(-(int)parsedDate.DayOfWeek).AddDays(1).Date;
                    DateTime weekEndDay = weekStartDate.AddDays(4).Date;

                    int nMenus = await this._mealSchedulesApp.RemoveForDateRange(weekStartDate, weekEndDay);

                    return Ok($"Removed {nMenus} menus from {weekStartDate.ToShortDateString()} to {weekEndDay.ToShortDateString()}");
                }
                else
                {
                    throw new ArgumentException(nameof(weekdate));
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
                this._mealSchedulesApp.Dispose();
            }
        }
    }
}