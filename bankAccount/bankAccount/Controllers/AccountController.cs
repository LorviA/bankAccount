using bankAccount.Commands;
using bankAccount.Dtos.AccountDto;
using bankAccount.Dtos.TransactionDto;
using bankAccount.Mappers;
using bankAccount.Models;
using bankAccount.Queries1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace bankAccount.Controllers
{
    //сделать dto без id для создания, подумать нод получением времени при записи
    [Route("bankAccount/account")]
    [ApiController]
    public class AccountController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private static readonly List<Account> _accounts = [];

        [HttpGet("All")]
        public async Task<ActionResult> GetAllAccounts()
        {
            var accounts = await _mediator.Send(new GetAccountsQuery());

            return Ok(accounts);
        }

        [HttpGet("{id}", Name = "GetAccountById")]
        public async Task<ActionResult> GetAccountById(Guid id)
        {
            var account = await _mediator.Send(new GetAccountByIdQuery(id));

            return account != null ? Ok(account) : NotFound($"No accounts found for owner ID: {id}");
        }


        [HttpGet("ByOwner")]
        public async Task<ActionResult> GetAccountsByOwner([FromQuery] Guid ownerId)
        {
            var accounts = await _mediator.Send(new GetAccountByOwnerQuery(ownerId));

            if (!accounts.Any())
            {
                return NotFound($"No accounts found for owner ID: {ownerId}");
            }

            return Ok(accounts);
        }

        [HttpGet("statement/{Id}")]
        public async Task<ActionResult> GetAccountStatement(
           Guid id,
           [FromQuery] DateTime? startDate = null,
           [FromQuery] DateTime? endDate = null)
        {
            var account = await _mediator.Send(new GetAccountByIdQuery(id));

            if (account == null) return NotFound();

            var statment = account.Transactions;

            if (startDate.HasValue)
                statment = (List<Transaction>)statment.Where(t => t.Time >= startDate);

            if (endDate.HasValue)
                statment = (List<Transaction>)statment.Where(t => t.Time < endDate);

            return Ok(statment);

        }

        [HttpPost("create Account")]
        public async Task<ActionResult> CreateAccount([FromBody] CreateAccountDto accountDto)
        {
            var accountModel = accountDto.ToAccountFromCreate();
            accountModel.Id = Guid.NewGuid();
            accountModel.CloseDate = null;
            var accontToReturn =  await _mediator.Send(new AddAccountCommand(accountModel));

            return CreatedAtRoute("GetAccountById", new { id = accontToReturn.Id }, accontToReturn);
        }

        [HttpPut("update Account")]
        public async Task<ActionResult> UpdateAccont(Guid id, [FromBody] UpdateAccountDto updateDto)
        {
            var accountModel = updateDto.ToAccountFromUpdate(id);
            accountModel.Id = id;
            var account = await _mediator.Send(new  UpdateAccountCommand(id, accountModel));

            if (account == null) return NotFound($"No accounts found for owner ID: {id}");

            return Ok(account);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            var account = await _mediator.Send(new DeleteAccountCommand(id));

            return account != null ? NoContent() : NotFound($"No accounts found for owner ID: {id}");
        }

        [HttpPatch("create Transaction")]
        public async Task<ActionResult> CreateTransaction([FromBody] CreateTransactionDto newTransactionDto)
        {
            var trasnactionModel = newTransactionDto.ToTransactionFromCreate();
            trasnactionModel.Id = Guid.NewGuid();
            trasnactionModel.CounterpartyAccountId = newTransactionDto.AccountId;
            var account = await _mediator.Send(new  CreateTransactionCommand(trasnactionModel));
            if (account == null) return NotFound($"No accounts found for Id: {trasnactionModel.AccountId}");

            return Ok(account);
        }

        [HttpPatch("transfer")]
        public async Task<ActionResult> CreateTransfer([FromBody] CreateTransferDto transferDto)
        {
            var transferModel = transferDto.ToTransferFromCreate();
            transferModel.Id = Guid.NewGuid();
            var account = await _mediator.Send(new AddTransferCommand(transferModel));

            if (account == null) return NotFound("One or both accounts not found");

            return Ok(account);
        }
    }
}
