Для сервиса авторизации внутри keycloak создать realm bank включить в нём авторизацию, доваить Clients account-service
Для работы нужно создать миграцию и в ней прописать добавить внутрь  protected override void Up(MigrationBuilder migrationBuilder)
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

