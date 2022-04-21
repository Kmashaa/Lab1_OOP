using Microsoft.Data.Sqlite;

namespace Lab1.Models;

public class DataBase : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    public async Task<List<List<string>>?> GetLogs()
    {
        var cmdText = "SELECT * FROM logs";
        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;
        if (!rdr.HasRows) return null;

        var tmp = new List<List<string>>();
        while (await rdr.ReadAsync()) tmp.Add(new List<string> {rdr.GetString(0), rdr.GetInt32(1).ToString()});

        return tmp;
    }

    #region DBFacilityMethods

    private DataBase(string password)
    {
        var dbSettings = new SqliteConnectionStringBuilder(@"Data Source=../../../bank_system.db")
        {
            Mode = SqliteOpenMode.ReadWriteCreate,
            Password = password
        }.ToString();

        _connection = new SqliteConnection(dbSettings);
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.CloseAsync();
        GC.SuppressFinalize(this);
    }

    public static async Task<DataBase> CreateAsync(string password)
    {
        var db = new DataBase(password);
        await db._connection.OpenAsync();

        return db;
    }

    private async Task ExecuteCmd(string queryString)
    {
        await using var command = new SqliteCommand(queryString, _connection);
        await command.ExecuteNonQueryAsync();
    }

    #endregion

    #region BankInfoMethods

    public async Task<List<string>> BankList()
    {
        var cmdText = "SELECT type, name FROM banks";

        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;
        if (!rdr.HasRows) return null!;

        List<string> tmp = new();
        while (await rdr.ReadAsync()) tmp.Add($"{rdr.GetString(0)} «{rdr.GetString(1)}»");
        tmp.Add("Exit");

        return tmp;
    }

    public async Task<List<Bank>?> BankInfo()
    {
        try
        {
            var cmdText = "SELECT * FROM banks";

            await using var command = new SqliteCommand(cmdText, _connection);
            await using var rdr = command.ExecuteReaderAsync().Result;
            if (!rdr.HasRows) return null!;

            List<List<string>> data = new();

            while (await rdr.ReadAsync())
                data.Add(new List<string>
                {
                    rdr.GetInt32(0).ToString(), rdr.GetString(1),
                    rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr.GetString(5)
                });

            await rdr.CloseAsync();
            List<List<int>?> users = new();
            var bankIds = new List<int>();

            foreach (var bank in data)
            {
                cmdText = $"SELECT id FROM clients WHERE bank_id = {Convert.ToInt32(bank[0])};";

                await using var command2 = new SqliteCommand(cmdText, _connection);
                await using var rdr2 = command2.ExecuteReaderAsync().Result;
                if (!rdr2.HasRows)
                {
                    users.Add(null);
                    continue;
                }

                List<int> ids = new();
                while (await rdr2.ReadAsync())
                    ids.Add(rdr2.GetInt32(0));

                await rdr2.CloseAsync();
                users.Add(ids);

                (bank[1], bank[2]) = (bank[2], bank[1]);
                bankIds.Add(Convert.ToInt32(bank[0]));
                bank.Remove(bank[0]);
            }

            return data.Select((t, i) => new Bank(bankIds[0], t, users[i])).ToList();
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region SignInMethods

    public async Task<User?> ClientSignIn(string? login, string? pass, int bank)
    {
        var cmdText =
            "SELECT * FROM clients_info JOIN clients c ON clients_info.client_id = c.id " +
            $"AND c.login = '{login}' AND c.pass = '{pass}' AND c.bank_id = {bank} " +
            "AND c.status = 'active' AND c.role = 'user';";

        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;
        await rdr.ReadAsync();

        if (!rdr.HasRows) return await ComSignIn(login, pass, bank);

        await UpdateLogs("User logged-in", rdr.GetInt32(0));
        try
        {
            var data = new List<string?>();
            for (var i = 1; i < 6; i++)
                data.Add(rdr.GetString(i));

            return new Client(rdr.GetInt32(0), data!);
        }
        catch
        {
            return null;
        }
    }

    private async Task<User?> ComSignIn(string? login, string? pass, int bank)
    {
        var cmdText =
            "SELECT * FROM  companies_info JOIN clients c ON companies_info.company_id = c.id " +
            $"AND c.login = '{login}' AND c.pass = '{pass}' AND c.bank_id = {bank} AND c.status = 'active';";

        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;
        await rdr.ReadAsync();

        if (!rdr.HasRows) return await StaffSignIn(login, pass, bank);

        await UpdateLogs("Company logged-in", rdr.GetInt32(0));
        try
        {
            var data = new List<string?>();
            for (var i = 1; i < 6; i++)
                data.Add(rdr.GetString(i));

            return new Company(rdr.GetInt32(0), data!);
        }
        catch
        {
            return null;
        }
    }

    private async Task<Staff?> StaffSignIn(string? login, string? pass, int bank)
    {
        var cmdText =
            $"SELECT id, role FROM clients WHERE bank_id = {bank} " +
            $"AND login = '{login}' AND pass = '{pass}' AND role != 'user' AND status = 'active';";

        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;
        await rdr.ReadAsync();

        if (!rdr.HasRows) return null;

        await UpdateLogs("Staff logged-in", rdr.GetInt32(0));
        try
        {
            var accessLvl = rdr.GetString(1) switch
            {
                "operator" => 2,
                "manager" => 3,
                "admin" => 4,
                _ => 1
            };

            var data = new List<string> {rdr.GetInt32(0).ToString(), accessLvl.ToString()};

            return new Staff(data);
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region SignUpMethods

    public async Task ClientSignUp(List<string> lData, List<string> uData)
    {
        var cmdText = "INSERT INTO clients(login, pass, role, bank_id, status) " +
                      $"VALUES('{lData[0]}','{lData[1]}', 'user', {Convert.ToInt32(lData[2])}, 'awaiting')";
        await ExecuteCmd(cmdText);

        var id = await GetLast("id", "clients");
        await UpdateLogs("Sign-up request (Client)", id);

        cmdText = "INSERT INTO clients_info(client_id, name, passNum, passID, phone, email) " +
                  $"VALUES({id}, '{uData[0]}', '{uData[1]}', '{uData[2]}', '{uData[3]}', '{uData[4]}')";
        await ExecuteCmd(cmdText);
    }

    public async Task ComSignUp(List<string> lData, List<string> bData)
    {
        var cmdText = "INSERT INTO clients(login, pass, role, bank_id, status) " +
                      $"VALUES('{lData[0]}','{lData[1]}', 'user', {Convert.ToInt32(lData[2])}, 'awaiting')";
        await ExecuteCmd(cmdText);

        var id = await GetLast("id", "clients");
        await UpdateLogs("Sign-up request (Company)", id);

        cmdText = "INSERT INTO companies_info(company_id, name, type, address, unp, bic) " +
                  $"VALUES({id}, '{bData[0]}', '{bData[1]}', '{bData[2]}', '{bData[3]}', '{bData[4]}')";
        await ExecuteCmd(cmdText);
    }

    public async Task<bool> CheckLogin(string? login, int bank)
    {
        var cmdText = $"SELECT * FROM clients WHERE login = '{login}' AND bank_id = {bank};";

        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;
        if (!rdr.HasRows) return false;

        await rdr.ReadAsync();
        try
        {
            return rdr.GetString(1) != "";
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region SalReqMethods

    public async Task<Dictionary<int, string>?> GetCompWorkers(int id)
    {
        var cmdText = $"SELECT workers FROM companies_info WHERE company_id = {id};";
        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;

        await rdr.ReadAsync();

        try
        {
            return rdr.GetString(0).Split(',')
                .Select(worker => worker.Split(':'))
                .ToDictionary(info => Convert.ToInt32(info[0]), info => info[1]);
        }
        catch
        {
            return null;
        }
    }

    public async Task GenSalaryRequest(List<int> ids, string sum)
    {
        var cmdText =
            "INSERT INTO salary_requests(comp_id, acc_id, bank_id, sum, status) " +
            $"VALUES({ids[0]}, {ids[1]}, {ids[2]}, '{sum}', 'awaiting');";
        await ExecuteCmd(cmdText);

        await UpdateLogs("Salary request", ids[0]);
    }

    public async Task SetCompWorkers(int id, Dictionary<int, string> workers)
    {
        var workersSer = workers.Aggregate(string.Empty,
            (current, worker) => current + $"{worker.Key.ToString()}:{worker.Value},");
        workersSer = workersSer.Remove(workersSer.Length - 1);

        var cmdText = $"UPDATE companies_info SET workers = '{workersSer}' WHERE company_id = {id}";
        await ExecuteCmd(cmdText);

        await UpdateLogs("Company workers updated", id);
    }

    #endregion

    #region AccMethods

    public async Task<List<Account>?> GetAccounts(int id, int bankId)
    {
        var cmdText = $"SELECT * FROM accounts WHERE client_id = {id} " +
                      $"AND bank_id = {bankId} AND status = 'active';";
        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;
        if (!rdr.HasRows) return null;

        List<Account> tmp = new();
        while (await rdr.ReadAsync())
            tmp.Add(new Account(
                new List<string>
                {
                    rdr.GetInt32(0).ToString(), rdr.GetInt32(1).ToString(),
                    rdr.GetString(2), rdr.GetString(3), rdr.GetString(4)
                }));

        return tmp;
    }

    public async Task AddAcc(Account acc)
    {
        var cmdText =
            $"INSERT INTO accounts(bank_id, client_id, balance, currency, status) VALUES({acc.BankId + 1}," +
            $"'{acc.ClientId}', '{Convert.ToDouble(acc.Balance):0.##}', '{acc.Currency}', 'active');";

        await ExecuteCmd(cmdText);

        var id = await GetLast("id", "accounts");
        await UpdateLogs("Account added", id);
    }

    public async Task RemoveAcc(Account acc)
    {
        var cmdText = $"DELETE FROM accounts WHERE id = '{acc.Id}';";

        await ExecuteCmd(cmdText);

        await UpdateLogs("Account removed", acc.Id);
    }

    public async Task ReplenishAcc(int accId, string sum)
    {
        var cmdText = $"SELECT balance FROM accounts WHERE id = {accId};";
        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;
        await rdr.ReadAsync();

        var newSum = (Convert.ToDouble(rdr.GetString(0)) + Convert.ToDouble(sum)).ToString("0.##");

        cmdText = $"UPDATE accounts SET balance = '{newSum}' WHERE id = {accId}";
        await ExecuteCmd(cmdText);

        var acc = await CheckAccId(accId);
        cmdText =
            "INSERT INTO transfers(sender, getter, sender_bank_id, getter_bank_id, sum, currency) " +
            $"VALUES(null, {acc!.Id}, null, {acc.BankId}, '{sum}', '{acc.Currency}');";

        await ExecuteCmd(cmdText);

        await UpdateLogs("Account credited", accId);
    }

    private async Task<bool> CheckAccSolvency(int accId, string sum)
    {
        var cmdText = $"SELECT balance FROM accounts WHERE id = {accId};";
        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;
        await rdr.ReadAsync();

        return !(Convert.ToDouble(rdr.GetString(0)) < Convert.ToDouble(sum));
    }

    public async Task<bool> CashOutAcc(int accId, string sum, string type = "cash out")
    {
        var cmdText = $"SELECT balance FROM accounts WHERE id = {accId};";
        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;

        await rdr.ReadAsync();
        if (Convert.ToDouble(rdr.GetString(0)) < Convert.ToDouble(sum))
            return false;

        var newSum = (Convert.ToDouble(rdr.GetString(0)) - Convert.ToDouble(sum)).ToString("0.##");

        cmdText = $"UPDATE accounts SET balance = '{newSum}' WHERE id = {accId}";
        await ExecuteCmd(cmdText);

        if (type != "cash out") return true;
        var acc = await CheckAccId(accId);
        cmdText =
            "INSERT INTO transfers(sender, getter, sender_bank_id, getter_bank_id, sum, currency) " +
            $"VALUES({acc!.Id}, null, {acc.BankId}, null,  '{sum}', '{acc.Currency}');";

        await ExecuteCmd(cmdText);
        await UpdateLogs("Account debited", accId);

        return true;
    }

    public async Task<bool> AccCurrencyCheck(AccountTransfer transfer)
    {
        var cmdText = $"SELECT currency FROM accounts WHERE id = {transfer.GetId} OR id = {transfer.SendId}";
        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;

        try
        {
            await rdr.ReadAsync();
            var cur1 = rdr.GetString(0);
            await rdr.ReadAsync();
            var cur2 = rdr.GetString(0);

            return cur1 == cur2;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AccTransfer(AccountTransfer transfer)
    {
        var cmdText = $"SELECT balance FROM accounts WHERE id = {transfer.GetId};";
        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;

        if (!rdr.HasRows) return false;
        if (!await CashOutAcc(transfer.SendId, transfer.Sum, "transfer")) return false;

        await rdr.ReadAsync();
        var newSum = (Convert.ToDouble(rdr.GetString(0)) + Convert.ToDouble(transfer.Sum)).ToString("0.##");

        cmdText = $"UPDATE accounts SET balance = '{newSum}' WHERE id = {transfer.GetId}";
        await ExecuteCmd(cmdText);

        cmdText =
            "INSERT INTO transfers(sender, getter, sender_bank_id, getter_bank_id, sum, currency) " +
            $"VALUES({transfer.SendId},{transfer.GetId}, {transfer.BankSendId}, " +
            $"{transfer.BankGetId}, '{transfer.Sum}', '{transfer.Currency}');";
        await ExecuteCmd(cmdText);

        var id = await GetLast("id", "transfers");
        await UpdateLogs("Money transfer", id);

        return true;
    }

    public async Task<Account?> CheckAccId(int id)
    {
        var cmdText = $"SELECT * FROM accounts WHERE id = {id};";
        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;
        if (!rdr.HasRows) return null;

        await rdr.ReadAsync();

        return new Account(new List<string>
        {
            rdr.GetInt32(0).ToString(), rdr.GetInt32(1).ToString(),
            rdr.GetString(2), rdr.GetString(3), rdr.GetString(4)
        });
    }

    #endregion

    #region LoanMethods

    public async Task<List<Loan>?> GetLoans(int userId)
    {
        var cmdText = $"SELECT * FROM loans_requests WHERE user_id = {userId} AND status = 'approved';";
        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;
        if (!rdr.HasRows) return null;

        List<Loan> tmp = new();
        while (await rdr.ReadAsync())
            tmp.Add(new Loan(new List<string>
            {
                rdr.GetInt32(0).ToString(), rdr.GetInt32(1).ToString(), rdr.GetInt32(2).ToString(),
                rdr.GetString(4), rdr.GetString(5), rdr.GetInt32(6).ToString(), rdr.GetString(7),
                rdr.GetInt32(8).ToString(), rdr.GetString(9), rdr.GetString(10)
            }));

        return tmp;
    }

    public async Task RequestLoan(Loan newLoan, int bankId)
    {
        var cmdText =
            "INSERT INTO loans_requests(user_id, acc_id, bank_id, sum, bank_sum, time, m_payment, payments, debt, date, status) " +
            $"VALUES({newLoan.UserId}, {newLoan.AccId}, {bankId}, '{newLoan.Sum}', '{newLoan.BankSum}', {newLoan.Time}, " +
            $"'{newLoan.MonPayment}',  {newLoan.Payments}, '{newLoan.Debt}', '{newLoan.Date}', 'awaiting')";
        await ExecuteCmd(cmdText);

        await UpdateLogs("Loan Request", newLoan.UserId);
    }

    public async Task UpdateDebt(int id, double debt)
    {
        var cmdText = $"UPDATE loans_requests SET debt = {debt.ToString("0.##")} WHERE id = {id}";
        await ExecuteCmd(cmdText);
    }

    public async Task<bool> PayOffLoan(Loan loan)
    {
        if (!await CheckAccSolvency(loan.AccId, loan.MonPayment))
            return false;

        var cmdText = $"UPDATE loans_requests SET debt = '0' WHERE id = {loan.Id}";
        await ExecuteCmd(cmdText);

        cmdText = $"SELECT time, payments FROM loans_requests WHERE id = {loan.Id} AND status = 'approved';";
        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;
        await rdr.ReadAsync();

        var newPay = rdr.GetInt32(1) + 1;
        cmdText = $"UPDATE loans_requests SET payments = {newPay} WHERE id = {loan.Id}";
        await ExecuteCmd(cmdText);

        if (rdr.GetInt32(0) != newPay) return true;

        cmdText = $"UPDATE loans_requests SET status = 'payed-off' WHERE id = {loan.Id}";
        await ExecuteCmd(cmdText);

        return true;
    }

    #endregion

    #region StaffMethods

    public async Task ChangeStatus(string where, int id, string status)
    {
        if (where is "clients")
            if (status == "approved")
                status = "active";

        var cmdText = $"UPDATE {where} SET status = '{status}' WHERE id = {id}";

        await ExecuteCmd(cmdText);

        await UpdateLogs($"New Status: {status}", id);
    }

    public async Task<List<AccountTransfer>> GetTransfers(int bankId, int accId = 0)
    {
        string cmdText;
        if (accId == 0)
            cmdText = $"SELECT * FROM transfers WHERE sender_bank_id = {bankId} OR getter_bank_id = {bankId}";
        else
            cmdText = $"SELECT * FROM transfers WHERE (sender_bank_id = {bankId} OR getter_bank_id = {bankId}) " +
                      $"AND (sender = {accId} OR getter = {accId});";
        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;

        var tmp = new List<AccountTransfer>();

        while (await rdr.ReadAsync())
        {
            var cond = true;
            var cond2 = true;

            try
            {
                var unused = rdr.GetString(1);
            }
            catch
            {
                cond = false;
            }

            try
            {
                var unused = rdr.GetString(2);
            }
            catch
            {
                cond2 = false;
            }

            switch (cond)
            {
                case true when cond2:
                    tmp.Add(new AccountTransfer(new List<Account>
                    {
                        (await CheckAccId(rdr.GetInt32(1)))!,
                        (await CheckAccId(rdr.GetInt32(2)))!
                    }, rdr.GetString(5), rdr.GetInt32(0)));
                    break;

                case true when !cond2:
                    tmp.Add(new AccountTransfer(rdr.GetInt32(0),
                        new List<Account?> {(await CheckAccId(rdr.GetInt32(1)))!, null},
                        rdr.GetString(5)));
                    break;

                case false when cond2:
                    tmp.Add(new AccountTransfer(rdr.GetInt32(0),
                        new List<Account?> {null, (await CheckAccId(rdr.GetInt32(2)))!},
                        rdr.GetString(5)));

                    break;
            }
        }

        return tmp;
    }

    public async Task<List<List<string>>?> GetReqs(string where, int bankId)
    {
        var cmdText = $"SELECT * FROM {where} WHERE status = 'awaiting' AND bank_id = {bankId}";
        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;
        if (!rdr.HasRows) return null;

        var tmp = new List<List<string>>();
        while (await rdr.ReadAsync())
            if (@where == "clients")
                tmp.Add(new List<string>
                {
                    rdr.GetInt32(0).ToString(), rdr.GetString(1),
                    rdr.GetString(2), rdr.GetString(3)
                });
            else
                tmp.Add(new List<string>
                {
                    rdr.GetInt32(0).ToString(), rdr.GetInt32(1).ToString(),
                    rdr.GetInt32(2).ToString(), rdr.GetString(4)
                });

        return tmp;
    }

    private async Task UpdateLogs(string type, int id)
    {
        var cmdText = "INSERT INTO logs(type, element_id, date) " +
                      $"VALUES('{type}', {id}, '{DateTime.Now.ToString("dd.MM.yyyy")}')";
        await ExecuteCmd(cmdText);
    }

    private async Task<int> GetLast(string what, string where)
    {
        var cmdText = $"SELECT {what} FROM '{where}' ORDER BY id DESC LIMIT 1";
        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;
        await rdr.ReadAsync();

        return rdr.GetInt32(0);
    }

    public async Task<bool> CheckClId(int id)
    {
        var cmdText = $"SELECT * FROM clients WHERE id = {id}";
        await using var command = new SqliteCommand(cmdText, _connection);
        await using var rdr = command.ExecuteReaderAsync().Result;

        return rdr.HasRows;
    }

    #endregion
}