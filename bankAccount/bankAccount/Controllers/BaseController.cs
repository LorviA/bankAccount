using Microsoft.AspNetCore.Mvc;

namespace bankAccount.Controllers
{
    /// <summary>
    /// Базовый контроллер с обработкой MbResult
    /// </summary>
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Обработка результата операции
        /// </summary>
        protected IActionResult HandleResult<T>(MbResult<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            if (result.Error != null)
            {
                return result.Error switch
                {
                    { StatusCode: 400 } => BadRequest(result.Error),
                    { StatusCode: 401 } => Unauthorized(result.Error),
                    { StatusCode: 404 } => NotFound(result.Error),
                    _ => StatusCode(result.Error.StatusCode, result.Error)
                };
            }
            return Ok(result.Value);
        }
    }
}