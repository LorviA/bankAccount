using bankAccount.Commands;
using bankAccount.Dtos.AccountDto;
using bankAccount.Dtos.TransactionDto;
using bankAccount.Mappers;
using bankAccount.Models;
using bankAccount.Queries1;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;

namespace bankAccount.Controllers
{
    //сделать dto для вывода превращающую Enum в слова
    //[Route("bankAccount/account")]
    [ApiController]
    [Authorize]
    public class AccountController(IMediator mediator) : BaseController
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Get All Account
        /// </summary>
        /// <returns> List[account]</returns>
        /// <response code="200">Returns List[account]</response>
        [Authorize]
        [HttpGet("All")]
        [ProducesResponseType(typeof(MbResult<IEnumerable<Account>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllAccounts()
        {
            var result = await _mediator.Send(new GetAccountsQuery());

            return (ActionResult)HandleResult(result);
        }

        /// <summary>
        /// Get one account for id
        /// </summary>
        /// <param name="id"> Id return account</param>
        /// <returns></returns>
        ///  <response code="200">Returns account</response>
        ///  <response code="404">No accounts found for ID:</response>
        [Authorize]
        [HttpGet("{id}", Name = "GetAccountById")]
        [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MbError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAccountById(Guid id)
        {
            var result = await _mediator.Send(new GetAccountByIdQuery(id));
            Console.WriteLine("Controller");
            Console.WriteLine(id);

            return (ActionResult)HandleResult(result);
        }

        /// <summary>
        /// Get one account for owner id
        /// </summary>
        /// <param name="ownerId"> OwnerId return account</param>
        /// <returns></returns>
        ///  <response code="200">Returns account</response>
        ///  <response code="404">No accounts found for owner ID:</response>
        [Authorize]
        [HttpGet("ByOwner")]
        [ProducesResponseType(typeof(MbResult<IEnumerable<Account>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAccountsByOwner([FromQuery] Guid ownerId)
        {
            var result = await _mediator.Send(new GetAccountByOwnerQuery(ownerId));
            return (ActionResult)HandleResult(result);
        }

        /// <summary>
        /// Receives an account statement from startDate to endDate
        /// </summary>
        /// <param name="id">Id account for statement</param>
        /// <param name="startDate">The initial date of the account statement</param>
        /// <param name="endDate">The end date of the account statement</param>
        /// <returns>List[Transaction]</returns>
        ///   <response code="200">Returns List[Transaction]</response>
        ///  <response code="404">No accounts found for ID:</response>
        [Authorize]
        [HttpGet("accounts/{id}/statement")]
        [ProducesResponseType(typeof(MbResult<IEnumerable<Transaction>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAccountStatement(
            Guid id,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var accountResult = await _mediator.Send(new GetAccountByIdQuery(id));

            if (!accountResult.IsSuccess)
                return (ActionResult)HandleResult(accountResult);

            var transactions = accountResult.Value.Transactions;

            if (startDate.HasValue)
                transactions = transactions.Where(t => t.Time >= startDate).ToList();

            if (endDate.HasValue)
                transactions = transactions.Where(t => t.Time <= endDate.Value.AddDays(1)).ToList();

            return (ActionResult)HandleResult(MbResult<IEnumerable<Transaction>>.Success(transactions));
        }

        /// <summary>
        /// Creates a new bank account
        /// </summary>
        ///  <param name="accountDto">Account details</param>
        ///  <returns>Newly created account</returns>
        /// <response code="200">Returns the created account</response>
        /// <response code="421">If validation fails</response>
        /// 
        [Authorize]
        [HttpPost("accounts")]
        [ProducesResponseType(typeof(MbResult<Account>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateAccount([FromBody] CreateAccountDto accountDto)
        {
            var accountModel = accountDto.ToAccountFromCreate();
            accountModel.Id = Guid.NewGuid();
            accountModel.CloseDate = null;

            var result = await _mediator.Send(new AddAccountCommand(accountModel));

            if (result.IsSuccess)
            {
                return CreatedAtRoute("GetAccountById", new { id = result.Value.Id }, result);
            }

            return (ActionResult)HandleResult(result);
        }

        /// <summary>
        /// account modified
        /// </summary>
        /// <param name="id"> id account being modified</param>
        /// <param name="updateDto">Account details</param>
        /// <returns>Newly modified account</returns>
        /// <response code="200">Returns the created account</response>
        /// <response code="421">If validation fails</response>
        [Authorize]
        [HttpPut("update")]
        [ProducesResponseType(typeof(MbResult<Account>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountDto updateDto)
        {
            var accountModel = updateDto.ToAccountFromUpdate(id);
            var result = await _mediator.Send(new UpdateAccountCommand(id, accountModel));
            return (ActionResult)HandleResult(result);
        }

        /// <summary>
        /// Delete account with id
        /// </summary>
        /// <param name="id">Id delete account</param>
        /// <response code="204">Successful deletion</response>
        /// <response code="404">No accounts found for ID</response>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(MbResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            var result = await _mediator.Send(new DeleteAccountCommand(id));

            if (result.IsSuccess)
                return NoContent();

            return (ActionResult)HandleResult(result);
        }

        /// <summary>
        /// creating an account transaction
        /// </summary>
        /// <param name="id"> account id for adding a transaction</param>
        /// <param name="newTransactionDto">Transaction details</param>
        /// <returns>Newly modified account</returns>
        /// <response code="200">Returns the modified account</response>
        /// <response code="421">If validation fails</response>
        [Authorize]
        [HttpPost("accounts/{id}/transactions")]
        public async Task<ActionResult> CreateTransaction(Guid id, [FromBody] CreateTransactionDto newTransactionDto)
        {
            var trasnactionModel = newTransactionDto.ToTransactionFromCreate();
            trasnactionModel.Id = Guid.NewGuid();
            trasnactionModel.CounterpartyAccountId = newTransactionDto.AccountId;
            var account = await _mediator.Send(new  CreateTransactionCommand(id, trasnactionModel));
            if (account == null) return NotFound($"No accounts found for Id: {trasnactionModel.AccountId}");

            return Ok(account);
        }

        /// <summary>
        /// creating an account transfer
        /// </summary>
        /// <param name="transferDto">Transfer details</param>
        /// <returns>Newly modified account</returns>
        /// <response code="200">Returns the modified account</response>
        /// <response code="421">If validation fails</response>
        [Authorize]
        [HttpPost("transfers")]
        public async Task<ActionResult> CreateTransfer([FromBody] CreateTransferDto transferDto)
        {
            var transferModel = transferDto.ToTransferFromCreate();
            transferModel.Id = Guid.NewGuid();
            var account = await _mediator.Send(new AddTransferCommand(transferModel));

            if (account == null) return NotFound("One or both accounts not found");

            return Ok(account);
        }

        [AllowAnonymous]
        [HttpGet("userinfo")]
        public IActionResult GetUserInfo()
        {
            var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            return Ok(new
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Username = User.FindFirst(ClaimTypes.Name)?.Value,
                Roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value),
                Claims = userClaims
            });
        }
    }
}
