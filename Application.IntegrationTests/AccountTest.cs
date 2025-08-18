using bankAccount.Commands;
using bankAccount.Models;
using bankAccount.Queries1;
using FluentAssertions;

namespace Application.IntegrationTests
{
    public class AccountTest(IntegrationTestAppFactory factory) : BaseIntegrationTest(factory)
    {
    

        [Fact]
        public async Task CreateNewAccountToDataBase()
        {
            //Arrange
            var command = new AddAccountCommand(new Account(1, "USD", 324, 15, DateTime.UtcNow));

            //Act
            var result = await Sender.Send(command);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<Account>();
        }

        [Fact]
        public async Task GetAccountByIdFromDataBase()
        {
            //Arrange
            var command = new AddAccountCommand(new Account(1, "USD", 324, 15, DateTime.UtcNow));
            var result = await Sender.Send(command);
            var account = result.Value;
            if (account == null)
            {
                return;
            }

            var query = new GetAccountByIdQuery(account.Id);
            //Act
            var product = await Sender.Send(query);

            //Assert
            Assert.NotNull(product);
            product.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllAccountFromDataBase()
        {
            //Arrange
            var command1 = new AddAccountCommand(new Account(1, "USD", 324, 15, DateTime.UtcNow));
            await Sender.Send(command1);
            var command2 = new AddAccountCommand(new Account(1, "USD", 324, 15, DateTime.UtcNow));
            await Sender.Send(command2);
            var query = new GetAccountsQuery();

            //Act
            var product = await Sender.Send(query);

            //Assert
            Assert.NotNull(product);
            product.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task GetBalanceAccountFromDataBase()
        {
            //Arrange
            var command = new AddAccountCommand(new Account(1, "USD", 324, 15, DateTime.UtcNow));
            var result = await Sender.Send(command);
            var account = result.Value;
            if (account == null)
            {
                return;
            }
            var query = new GetBalanceQuery(account.Id);

            //Act
            var product = await Sender.Send(query);

            //Assert
            Assert.NotNull(product);
            product.IsSuccess.Should().BeTrue();
            product.Value.Should().Be(324);
        }
        
        [Fact]
        public async Task GetAccountByOwnerFromDataBase()
        {
            //Arrange
            var command = new AddAccountCommand(new Account(1, "USD", 324, 15, DateTime.UtcNow));
            var result = await Sender.Send(command);
            var account = result.Value;
            if (account == null)
            {
                return;
            }
            var query = new GetAccountByOwnerQuery(account.OwnerId);

            //Act
            var product = await Sender.Send(query);

            //Assert
            Assert.NotNull(product);
            product.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task CloseByIdFromDataBase()
        {
            //Arrange
            var command = new AddAccountCommand(new Account(1, "USD", 324, 15, DateTime.UtcNow));
            var result = await Sender.Send(command);
            var account = result.Value;
            if (account == null)
            {
                return;
            }
            var query = new CloseAccountCommand(account.Id);

            //Act
            var product = await Sender.Send(query);

            //Assert
            Assert.NotNull(product);
            product.IsSuccess.Should().BeTrue();
            if (product.Value == null)
            {
                return;
            }
            product.Value.CloseDate.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteAccountByIdFromDataBase()
        {
            //Arrange
            var command = new AddAccountCommand(new Account(1, "USD", 324, 15, DateTime.UtcNow));
            var result = await Sender.Send(command);
            var account = result.Value;
            if (account == null)
            {
                return;
            }
            var query = new DeleteAccountCommand(account.Id);

            //Act
            var product = await Sender.Send(query);

            //Assert
            product.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAccountByIdFromDataBase()
        {
            //Arrange
            var command = new AddAccountCommand(new Account(1, "USD", 324, 15, DateTime.UtcNow));
            var result = await Sender.Send(command);
            var account = result.Value;
            if (account == null)
            {
                return;
            }
            account.Balance += 200;
            var query = new UpdateAccountCommand(account.Id, account);

            //Act
            var product = await Sender.Send(query);

            //Assert
            account = result.Value;
            product.IsSuccess.Should().BeTrue();
            if (account == null)
            {
                return;
            }
            account.Balance.Should().Be(524);
        }

        [Fact]
        public async Task CreateTransactionFromDataBase()
        {
            //Arrange
            var command = new AddAccountCommand(new Account(1, "USD", 324, 15, DateTime.UtcNow));
            var result = await Sender.Send(command);
            var account = result.Value;
            if (account == null)
            {
                return;
            }
            var query = new CreateTransactionCommand(account.Id, new Transaction(0,100,"USD","Hi"));

            //Act
            var product = await Sender.Send(query);

            //Assert
            product.IsSuccess.Should().BeFalse();
            result.Value.Should().BeOfType<Account>();
        }
    }
}
