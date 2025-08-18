using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bankAccount.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");
            migrationBuilder.Sql(@"
        CREATE OR REPLACE PROCEDURE accrue_interest(account_id UUID)
LANGUAGE plpgsql
AS $$
DECLARE
    account_record RECORD;
    daily_interest NUMERIC;
BEGIN
    -- Блокировка счета для изменения
    SELECT * INTO account_record 
    FROM ""Accounts"" 
    WHERE ""Id"" = account_id
    FOR UPDATE;
    
    -- Проверка условий
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Account not found: %', account_id;
    END IF;
    
    IF account_record.""Type"" != 1 THEN -- 1 = Deposit
        RAISE EXCEPTION 'Account is not a deposit: %', account_id;
    END IF;
    
    IF account_record.""InterestRate"" IS NULL THEN
        RAISE EXCEPTION 'Interest rate not set: %', account_id;
    END IF;
    
    -- Расчет процентов за день
    daily_interest := (account_record.""Balance"" * account_record.""InterestRate"" / 100) / 365;
    daily_interest := ROUND(daily_interest, 2); -- Округление до копеек
    
    -- Обновление баланса
    UPDATE ""Accounts""
    SET ""Balance"" = ""Balance"" + daily_interest
    WHERE ""Id"" = account_id;
    
END;
$$;
    ");
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    Balance = table.Column<decimal>(type: "numeric", nullable: true),
                    InterestRate = table.Column<decimal>(type: "numeric", nullable: true),
                    OpeningDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CloseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CounterpartyAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_OwnerId",
                table: "Accounts",
                column: "OwnerId")
                .Annotation("Npgsql:IndexMethod", "HASH");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId_Time",
                table: "Transactions",
                columns: new[] { "AccountId", "Time" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Time",
                table: "Transactions",
                column: "Time")
                .Annotation("Npgsql:IndexMethod", "GIST");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
