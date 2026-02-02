using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PettyCashManager
{
    enum Role { Requester, Approver, Accountant, Auditor, Admin }
    enum TransactionStatus { Pending, Approved, Rejected }

    interface IEntity { Guid Id { get; } }

    class Result<T>
    {
        public bool Success { get; }
        public string Message { get; }
        public T Data { get; }
        public Result(bool success, string message = null, T data = default) { Success = success; Message = message; Data = data; }
        public static Result<T> Ok(T data = default, string message = null) => new Result<T>(true, message, data);
        public static Result<T> Fail(string message) => new Result<T>(false, message, default);
    }

    interface IRepository<T> where T : IEntity
    {
        Result<T> Add(T item);
        Result<T> Update(T item);
        Result<T> Remove(Guid id);
        T GetById(Guid id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Func<T, bool> predicate);
    }

    class InMemoryRepository<T> : IRepository<T> where T : IEntity
    {
        readonly Dictionary<Guid, T> store = new Dictionary<Guid, T>();
        public Result<T> Add(T item)
        {
            if (item == null) return Result<T>.Fail("Item is null");
            if (store.ContainsKey(item.Id)) return Result<T>.Fail("Duplicate id");
            store[item.Id] = item;
            return Result<T>.Ok(item);
        }
        public Result<T> Update(T item)
        {
            if (item == null) return Result<T>.Fail("Item is null");
            if (!store.ContainsKey(item.Id)) return Result<T>.Fail("Not found");
            store[item.Id] = item;
            return Result<T>.Ok(item);
        }
        public Result<T> Remove(Guid id)
        {
            if (!store.ContainsKey(id)) return Result<T>.Fail("Not found");
            var item = store[id];
            store.Remove(id);
            return Result<T>.Ok(item);
        }
        public T GetById(Guid id) => store.ContainsKey(id) ? store[id] : default;
        public IEnumerable<T> GetAll() => store.Values.ToList();
        public IEnumerable<T> Find(Func<T, bool> predicate) => store.Values.Where(predicate).ToList();
    }

    class AuditLogEntry : IEntity
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
    }

    class AuditService
    {
        readonly IRepository<AuditLogEntry> repo;
        public AuditService(IRepository<AuditLogEntry> repo) { this.repo = repo; }
        public void Log(Guid userId, string action, string details)
        {
            var e = new AuditLogEntry { UserId = userId, Action = action, Details = details, Timestamp = DateTime.UtcNow };
            repo.Add(e);
        }
        public IEnumerable<AuditLogEntry> Query(Func<AuditLogEntry, bool> predicate) => repo.Find(predicate);
    }

    class User : IEntity
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public Role Role { get; set; }
    }

    abstract class Transaction : IEntity
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Guid FundId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Narration { get; set; }
        public TransactionStatus Status { get; set; }
        public string VoucherNumber { get; set; }
        public Guid RequesterId { get; set; }
        public Guid? ApproverId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        public string Category { get; set; }
    }

    class ExpenseTransaction : Transaction { }
    class ReimbursementTransaction : Transaction { public string ReferenceNumber { get; set; } }

    class PettyCashFund : IEntity
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal CurrentBalance { get; set; }
    }

    class FundService
    {
        readonly IRepository<PettyCashFund> fundRepo;
        readonly IRepository<Transaction> txRepo;
        readonly AuditService audit;
        public FundService(IRepository<PettyCashFund> fr, IRepository<Transaction> tr, AuditService audit) { fundRepo = fr; txRepo = tr; this.audit = audit; }
        public Result<PettyCashFund> CreateFund(Guid userId, string name, decimal openingBalance)
        {
            if (string.IsNullOrWhiteSpace(name)) return Result<PettyCashFund>.Fail("Invalid name");
            if (openingBalance < 0) return Result<PettyCashFund>.Fail("Opening balance cannot be negative");
            var f = new PettyCashFund { Name = name.Trim(), OpeningBalance = openingBalance, CurrentBalance = openingBalance };
            var r = fundRepo.Add(f);
            if (!r.Success) return Result<PettyCashFund>.Fail(r.Message);
            audit.Log(userId, "CreateFund", $"Fund:{f.Name}, Opening:{f.OpeningBalance}");
            return Result<PettyCashFund>.Ok(f);
        }
        public PettyCashFund GetFund(Guid id) => fundRepo.GetById(id);
        public IEnumerable<PettyCashFund> GetAllFunds() => fundRepo.GetAll();
        public void RecalculateBalance(Guid fundId)
        {
            var fund = fundRepo.GetById(fundId);
            if (fund == null) return;
            var approvedReimb = txRepo.Find(t => t.FundId == fundId && t.Status == TransactionStatus.Approved && t is ReimbursementTransaction).Sum(t => t.Amount);
            var approvedExpenses = txRepo.Find(t => t.FundId == fundId && t.Status == TransactionStatus.Approved && t is ExpenseTransaction).Sum(t => t.Amount);
            fund.CurrentBalance = fund.OpeningBalance + approvedReimb - approvedExpenses;
            fundRepo.Update(fund);
        }
    }

    class TransactionService
    {
        readonly IRepository<Transaction> txRepo;
        readonly IRepository<PettyCashFund> fundRepo;
        readonly AuditService audit;
        readonly FundService fundService;
        public TransactionService(IRepository<Transaction> txRepo, IRepository<PettyCashFund> fundRepo, AuditService audit, FundService fs)
        {
            this.txRepo = txRepo; this.fundRepo = fundRepo; this.audit = audit; fundService = fs;
        }
        public Result<Transaction> AddExpense(Guid requesterId, Guid fundId, string category, decimal amount, DateTime date, string narration, string voucherNo)
        {
            if (amount <= 0) return Result<Transaction>.Fail("Amount must be greater than 0");
            if (string.IsNullOrWhiteSpace(voucherNo)) return Result<Transaction>.Fail("Voucher number required");
            var fund = fundRepo.GetById(fundId);
            if (fund == null) return Result<Transaction>.Fail("Fund not found");
            if (date == default) return Result<Transaction>.Fail("Invalid date");
            var tx = new ExpenseTransaction { FundId = fundId, Category = category ?? "Unspecified", Amount = amount, Date = date, Narration = narration, VoucherNumber = voucherNo.Trim(), Status = TransactionStatus.Pending, RequesterId = requesterId };
            var r = txRepo.Add(tx);
            if (!r.Success) return Result<Transaction>.Fail(r.Message);
            audit.Log(requesterId, "AddExpense", $"Fund:{fund.Name}, Voucher:{tx.VoucherNumber}, Amount:{tx.Amount}");
            return Result<Transaction>.Ok(tx);
        }
        public Result<Transaction> AddReimbursement(Guid userId, Guid fundId, decimal amount, DateTime date, string narration, string referenceNo)
        {
            if (amount <= 0) return Result<Transaction>.Fail("Amount must be greater than 0");
            var fund = fundRepo.GetById(fundId);
            if (fund == null) return Result<Transaction>.Fail("Fund not found");
            if (date == default) return Result<Transaction>.Fail("Invalid date");
            var tx = new ReimbursementTransaction { FundId = fundId, Amount = amount, Date = date, Narration = narration, VoucherNumber = referenceNo ?? string.Empty, ReferenceNumber = referenceNo, Status = TransactionStatus.Approved, RequesterId = userId, ApproverId = userId, ProcessedAt = DateTime.UtcNow };
            var r = txRepo.Add(tx);
            if (!r.Success) return Result<Transaction>.Fail(r.Message);
            fundService.RecalculateBalance(fundId);
            audit.Log(userId, "AddReimbursement", $"Fund:{fund.Name}, Ref:{referenceNo}, Amount:{amount}");
            return Result<Transaction>.Ok(tx);
        }
        public IEnumerable<Transaction> QueryTransactions(Guid fundId, DateTime? from = null, DateTime? to = null, string category = null, Guid? requesterId = null, TransactionStatus? status = null)
        {
            var q = txRepo.Find(t => t.FundId == fundId);
            if (from.HasValue) q = q.Where(t => t.Date >= from.Value).ToList();
            if (to.HasValue) q = q.Where(t => t.Date <= to.Value).ToList();
            if (!string.IsNullOrWhiteSpace(category)) q = q.Where(t => string.Equals(t.Category, category, StringComparison.OrdinalIgnoreCase)).ToList();
            if (requesterId.HasValue) q = q.Where(t => t.RequesterId == requesterId.Value).ToList();
            if (status.HasValue) q = q.Where(t => t.Status == status.Value).ToList();
            return q.OrderByDescending(t => t.Date).ToList();
        }
    }

    class ApprovalWorkflowService
    {
        readonly IRepository<Transaction> txRepo;
        readonly IRepository<PettyCashFund> fundRepo;
        readonly AuditService audit;
        readonly FundService fundService;
        public ApprovalWorkflowService(IRepository<Transaction> txRepo, IRepository<PettyCashFund> fundRepo, AuditService audit, FundService fs)
        {
            this.txRepo = txRepo; this.fundRepo = fundRepo; this.audit = audit; fundService = fs;
        }
        public Result<Transaction> ApproveExpense(Guid approverId, Guid expenseId, User approver)
        {
            var tx = txRepo.GetById(expenseId);
            if (tx == null) return Result<Transaction>.Fail("Expense not found");
            if (!(tx is ExpenseTransaction)) return Result<Transaction>.Fail("Transaction is not an expense");
            if (tx.Status != TransactionStatus.Pending) return Result<Transaction>.Fail("Already processed");
            if (tx.RequesterId == approverId) return Result<Transaction>.Fail("Approver cannot be the requester");
            var fund = fundRepo.GetById(tx.FundId);
            if (fund == null) return Result<Transaction>.Fail("Fund not found");
            var projBalance = fund.OpeningBalance + txRepo.Find(t => t.FundId == fund.Id && t.Status == TransactionStatus.Approved && t is ReimbursementTransaction).Sum(t => t.Amount) - txRepo.Find(t => t.FundId == fund.Id && t.Status == TransactionStatus.Approved && t is ExpenseTransaction).Sum(t => t.Amount);
            if (tx.Amount > projBalance) return Result<Transaction>.Fail("Insufficient balance");
            tx.Status = TransactionStatus.Approved;
            tx.ApproverId = approverId;
            tx.ProcessedAt = DateTime.UtcNow;
            var r = txRepo.Update(tx);
            if (!r.Success) return Result<Transaction>.Fail(r.Message);
            fundService.RecalculateBalance(fund.Id);
            audit.Log(approverId, "ApproveExpense", $"Voucher:{tx.VoucherNumber}, Amount:{tx.Amount}, Fund:{fund.Name}");
            return Result<Transaction>.Ok(tx);
        }
        public Result<Transaction> RejectExpense(Guid approverId, Guid expenseId, string reason)
        {
            var tx = txRepo.GetById(expenseId);
            if (tx == null) return Result<Transaction>.Fail("Expense not found");
            if (!(tx is ExpenseTransaction)) return Result<Transaction>.Fail("Transaction is not an expense");
            if (tx.Status != TransactionStatus.Pending) return Result<Transaction>.Fail("Already processed");
            tx.Status = TransactionStatus.Rejected;
            tx.ApproverId = approverId;
            tx.ProcessedAt = DateTime.UtcNow;
            var r = txRepo.Update(tx);
            if (!r.Success) return Result<Transaction>.Fail(r.Message);
            audit.Log(approverId, "RejectExpense", $"Voucher:{tx.VoucherNumber}, Reason:{reason}");
            return Result<Transaction>.Ok(tx);
        }
    }

    class ReportBuilder<T>
    {
        public IEnumerable<string> BuildDailySummary(IEnumerable<T> items, Func<T, DateTime> dateSelector, Func<T, decimal> amountSelector)
        {
            return items.GroupBy(dateSelector)
                        .OrderBy(g => g.Key)
                        .Select(g => $"{g.Key:yyyy-MM-dd} | Count: {g.Count()} | Total: {g.Sum(amountSelector):0.00}");
        }
        public IEnumerable<string> BuildCategoryMonthly(IEnumerable<T> items, Func<T, string> categorySelector, Func<T, DateTime> dateSelector, Func<T, decimal> amountSelector)
        {
            return items.GroupBy(i => (categorySelector(i), dateSelector(i).Year, dateSelector(i).Month))
                        .OrderBy(g => g.Key.Item2).ThenBy(g => g.Key.Item3)
                        .Select(g => $"{g.Key.Item1} | {g.Key.Item2}-{g.Key.Item3:D2} | Count: {g.Count()} | Total: {g.Sum(amountSelector):0.00}");
        }
        public IEnumerable<string> BuildPending(IEnumerable<T> items, Func<T, string> idSelector)
        {
            return items.Select(i => idSelector(i));
        }
    }

    class ConsoleApp
    {
        readonly IRepository<User> userRepo = new InMemoryRepository<User>();
        readonly IRepository<PettyCashFund> fundRepo = new InMemoryRepository<PettyCashFund>();
        readonly IRepository<Transaction> txRepo = new InMemoryRepository<Transaction>();
        readonly IRepository<AuditLogEntry> auditRepo = new InMemoryRepository<AuditLogEntry>();
        readonly AuditService auditService;
        readonly FundService fundService;
        readonly TransactionService txService;
        readonly ApprovalWorkflowService approvalService;
        readonly ReportBuilder<Transaction> reportBuilder = new ReportBuilder<Transaction>();

        User currentUser;

        public ConsoleApp()
        {
            auditService = new AuditService(auditRepo);
            fundService = new FundService(fundRepo, txRepo, auditService);
            txService = new TransactionService(txRepo, fundRepo, auditService, fundService);
            approvalService = new ApprovalWorkflowService(txRepo, fundRepo, auditService, fundService);
            SeedUsers();
        }

        void SeedUsers()
        {
            userRepo.Add(new User { Name = "Alice Requester", Role = Role.Requester });
            userRepo.Add(new User { Name = "Bob Approver", Role = Role.Approver });
            userRepo.Add(new User { Name = "Cara Accountant", Role = Role.Accountant });
            userRepo.Add(new User { Name = "Dan Auditor", Role = Role.Auditor });
            userRepo.Add(new User { Name = "Eve Admin", Role = Role.Admin });
        }

        public void Run()
        {
            while (true)
            {
                if (currentUser == null) LoginMenu();
                else MainMenu();
            }
        }

        void LoginMenu()
        {
            Console.WriteLine("Select user to login:");
            var users = userRepo.GetAll().ToList();
            for (int i = 0; i < users.Count; i++) Console.WriteLine($"{i + 1}. {users[i].Name} ({users[i].Role})");
            Console.WriteLine("0. Exit");
            var key = Console.ReadLine();
            if (key == "0") Environment.Exit(0);
            if (int.TryParse(key, out int idx) && idx >= 1 && idx <= users.Count)
            {
                currentUser = users[idx - 1];
                Console.WriteLine($"Logged in as: {currentUser.Name} ({currentUser.Role})\n");
                auditService.Log(currentUser.Id, "Login", $"User {currentUser.Name} logged in");
            }
            else Console.WriteLine("Invalid selection\n");
        }

        void MainMenu()
        {
            Console.WriteLine("Main Menu:");
            Console.WriteLine("1. Create Petty Cash Fund");
            Console.WriteLine("2. Add Expense Voucher");
            Console.WriteLine("3. Approve/Reject Expense");
            Console.WriteLine("4. Add Reimbursement (Top-up)");
            Console.WriteLine("5. View Fund Balance & Ledger");
            Console.WriteLine("6. Generate Reports");
            Console.WriteLine("7. Audit Trail Review");
            Console.WriteLine("8. Switch User");
            Console.WriteLine("0. Exit");
            var choice = Console.ReadLine();
            Console.WriteLine();
            switch (choice)
            {
                case "1": CmdCreateFund(); break;
                case "2": CmdAddExpense(); break;
                case "3": CmdApproveReject(); break;
                case "4": CmdAddReimbursement(); break;
                case "5": CmdViewLedger(); break;
                case "6": CmdGenerateReports(); break;
                case "7": CmdAuditReview(); break;
                case "8": currentUser = null; break;
                case "0": Environment.Exit(0); break;
                default: Console.WriteLine("Invalid option\n"); break;
            }
        }

        void CmdCreateFund()
        {
            if (currentUser.Role != Role.Admin && currentUser.Role != Role.Accountant) { Console.WriteLine("Access denied\n"); return; }
            Console.Write("Fund name: "); var name = Console.ReadLine();
            Console.Write("Opening balance: "); var ob = Console.ReadLine();
            if (!decimal.TryParse(ob, out decimal opening)) { Console.WriteLine("Invalid amount\n"); return; }
            var r = fundService.CreateFund(currentUser.Id, name, opening);
            if (!r.Success) Console.WriteLine($"Error: {r.Message}\n"); else Console.WriteLine($"Fund created. Current balance: {r.Data.CurrentBalance:0.00}\n");
        }

        void CmdAddExpense()
        {
            if (currentUser.Role != Role.Requester && currentUser.Role != Role.Admin) { Console.WriteLine("Access denied\n"); return; }
            var funds = fundRepo.GetAll().ToList(); if (!funds.Any()) { Console.WriteLine("No funds available\n"); return; }
            for (int i = 0; i < funds.Count; i++) Console.WriteLine($"{i + 1}. {funds[i].Name} | Balance: {funds[i].CurrentBalance:0.00}");
            Console.Write("Select fund: "); if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > funds.Count) { Console.WriteLine("Invalid selection\n"); return; }
            var fund = funds[idx - 1];
            Console.Write("Category: "); var category = Console.ReadLine();
            Console.Write("Amount: "); if (!decimal.TryParse(Console.ReadLine(), out decimal amount)) { Console.WriteLine("Invalid amount\n"); return; }
            Console.Write("Voucher number: "); var voucher = Console.ReadLine();
            Console.Write("Narration: "); var narration = Console.ReadLine();
            Console.Write("Date (yyyy-MM-dd) or leave blank for today: "); var ds = Console.ReadLine(); DateTime date = DateTime.UtcNow; if (!string.IsNullOrWhiteSpace(ds) && !DateTime.TryParse(ds, out date)) { Console.WriteLine("Invalid date\n"); return; }
            var r = txService.AddExpense(currentUser.Id, fund.Id, category, amount, date, narration, voucher);
            if (!r.Success) Console.WriteLine($"Error: {r.Message}\n"); else Console.WriteLine($"Expense added with status {r.Data.Status}.\n");
        }

        void CmdApproveReject()
        {
            if (currentUser.Role != Role.Approver && currentUser.Role != Role.Admin) { Console.WriteLine("Access denied\n"); return; }
            var pending = txRepo.Find(t => t.Status == TransactionStatus.Pending && t is ExpenseTransaction).ToList();
            if (!pending.Any()) { Console.WriteLine("No pending expenses\n"); return; }
            for (int i = 0; i < pending.Count; i++) Console.WriteLine($"{i + 1}. Voucher:{pending[i].VoucherNumber} | Amount:{pending[i].Amount:0.00} | Fund:{fundRepo.GetById(pending[i].FundId)?.Name} | Requester:{GetUserName(pending[i].RequesterId)}");
            Console.Write("Select expense: "); if (!int.TryParse(Console.ReadLine(), out int sel) || sel < 1 || sel > pending.Count) { Console.WriteLine("Invalid selection\n"); return; }
            var tx = pending[sel - 1];
            Console.WriteLine("1. Approve\n2. Reject"); var a = Console.ReadLine();
            if (a == "1")
            {
                var user = currentUser;
                var r = approvalService.ApproveExpense(currentUser.Id, tx.Id, user);
                Console.WriteLine(r.Success ? $"Approved. New status: {r.Data.Status}\n" : $"Error: {r.Message}\n");
            }
            else if (a == "2")
            {
                Console.Write("Reason: "); var reason = Console.ReadLine();
                var r = approvalService.RejectExpense(currentUser.Id, tx.Id, reason);
                Console.WriteLine(r.Success ? $"Rejected. New status: {r.Data.Status}\n" : $"Error: {r.Message}\n");
            }
            else Console.WriteLine("Invalid option\n");
        }

        void CmdAddReimbursement()
        {
            if (currentUser.Role != Role.Accountant && currentUser.Role != Role.Admin) { Console.WriteLine("Access denied\n"); return; }
            var funds = fundRepo.GetAll().ToList(); if (!funds.Any()) { Console.WriteLine("No funds available\n"); return; }
            for (int i = 0; i < funds.Count; i++) Console.WriteLine($"{i + 1}. {funds[i].Name} | Balance: {funds[i].CurrentBalance:0.00}");
            Console.Write("Select fund: "); if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > funds.Count) { Console.WriteLine("Invalid selection\n"); return; }
            var fund = funds[idx - 1];
            Console.Write("Top-up amount: "); if (!decimal.TryParse(Console.ReadLine(), out decimal amount)) { Console.WriteLine("Invalid amount\n"); return; }
            Console.Write("Reference number: "); var refNo = Console.ReadLine();
            Console.Write("Narration: "); var narration = Console.ReadLine();
            Console.Write("Date (yyyy-MM-dd) or leave blank for today: "); var ds = Console.ReadLine(); DateTime date = DateTime.UtcNow; if (!string.IsNullOrWhiteSpace(ds) && !DateTime.TryParse(ds, out date)) { Console.WriteLine("Invalid date\n"); return; }
            var r = txService.AddReimbursement(currentUser.Id, fund.Id, amount, date, narration, refNo);
            Console.WriteLine(r.Success ? $"Reimbursement added and approved. New balance: {fundRepo.GetById(fund.Id).CurrentBalance:0.00}\n" : $"Error: {r.Message}\n");
        }

        void CmdViewLedger()
        {
            var funds = fundRepo.GetAll().ToList(); if (!funds.Any()) { Console.WriteLine("No funds available\n"); return; }
            for (int i = 0; i < funds.Count; i++) Console.WriteLine($"{i + 1}. {funds[i].Name} | Balance: {funds[i].CurrentBalance:0.00}");
            Console.Write("Select fund: "); if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > funds.Count) { Console.WriteLine("Invalid selection\n"); return; }
            var fund = funds[idx - 1];
            Console.Write("From date (yyyy-MM-dd) or blank: "); var fs = Console.ReadLine(); DateTime? from = null; if (!string.IsNullOrWhiteSpace(fs) && DateTime.TryParse(fs, out DateTime fdt)) from = fdt;
            Console.Write("To date (yyyy-MM-dd) or blank: "); var ts = Console.ReadLine(); DateTime? to = null; if (!string.IsNullOrWhiteSpace(ts) && DateTime.TryParse(ts, out DateTime tdt)) to = tdt;
            Console.Write("Category or blank: "); var category = Console.ReadLine();
            Console.Write("Status (Pending/Approved/Rejected) or blank: "); var ss = Console.ReadLine(); TransactionStatus? status = null; if (!string.IsNullOrWhiteSpace(ss) && Enum.TryParse<TransactionStatus>(ss, true, out var sst)) status = sst;
            var list = txService.QueryTransactions(fund.Id, from, to, category, null, status).ToList();
            if (!list.Any()) { Console.WriteLine("No transactions found\n"); return; }
            Console.WriteLine($"Ledger for {fund.Name} | Balance: {fund.CurrentBalance:0.00}");
            foreach (var t in list)
            {
                Console.WriteLine($"{t.Date:yyyy-MM-dd} | {t.GetType().Name.Replace("Transaction","")} | {t.Status} | {t.Amount:0.00} | Voucher:{t.VoucherNumber} | Category:{t.Category} | Requester:{GetUserName(t.RequesterId)} | Approver:{(t.ApproverId.HasValue?GetUserName(t.ApproverId.Value):"-")}");
            }
            Console.WriteLine();
        }

        void CmdGenerateReports()
        {
            if (currentUser.Role != Role.Accountant && currentUser.Role != Role.Auditor && currentUser.Role != Role.Admin) { Console.WriteLine("Access denied\n"); return; }
            var funds = fundRepo.GetAll().ToList(); if (!funds.Any()) { Console.WriteLine("No funds available\n"); return; }
            for (int i = 0; i < funds.Count; i++) Console.WriteLine($"{i + 1}. {funds[i].Name}");
            Console.Write("Select fund: "); if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > funds.Count) { Console.WriteLine("Invalid selection\n"); return; }
            var fund = funds[idx - 1];
            Console.WriteLine("Report types:\n1. Daily Summary\n2. Category-wise Monthly\n3. Pending Approvals\n4. Fund Balance Statement"); var rt = Console.ReadLine();
            Console.Write("From date (yyyy-MM-dd) or blank: "); var fs = Console.ReadLine(); DateTime? from = null; if (!string.IsNullOrWhiteSpace(fs) && DateTime.TryParse(fs, out DateTime fdt)) from = fdt;
            Console.Write("To date (yyyy-MM-dd) or blank: "); var ts = Console.ReadLine(); DateTime? to = null; if (!string.IsNullOrWhiteSpace(ts) && DateTime.TryParse(ts, out DateTime tdt)) to = tdt;
            var items = txService.QueryTransactions(fund.Id, from, to).ToList();
            IEnumerable<string> lines = Enumerable.Empty<string>();
            if (rt == "1") lines = reportBuilder.BuildDailySummary(items, t => t.Date.Date, t => t.Amount);
            else if (rt == "2") lines = reportBuilder.BuildCategoryMonthly(items, t => t.Category ?? "Unspecified", t => t.Date, t => t.Amount);
            else if (rt == "3") lines = reportBuilder.BuildPending(items.Where(t => t.Status == TransactionStatus.Pending), t => $"{t.VoucherNumber} | Amount:{t.Amount:0.00} | Requester:{GetUserName(t.RequesterId)}");
            else if (rt == "4") lines = new List<string> { $"Fund:{fund.Name}", $"Opening:{fund.OpeningBalance:0.00}", $"Current:{fund.CurrentBalance:0.00}", $"As of:{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}" };
            else { Console.WriteLine("Invalid report type\n"); return; }
            if (!lines.Any()) Console.WriteLine("No data for report\n"); else foreach (var l in lines) Console.WriteLine(l);
            Console.WriteLine("Save to file? (y/n): "); if (Console.ReadLine()?.Trim().ToLower() == "y")
            {
                var path = $"report_{fund.Name}_{DateTime.UtcNow:yyyyMMddHHmmss}.txt".Replace(' ', '_');
                File.WriteAllLines(path, lines);
                Console.WriteLine($"Saved to {path}\n");
            }
            else Console.WriteLine();
        }

        void CmdAuditReview()
        {
            if (currentUser.Role != Role.Auditor && currentUser.Role != Role.Admin) { Console.WriteLine("Access denied\n"); return; }
            Console.Write("User name filter or blank: "); var uname = Console.ReadLine();
            Console.Write("From date (yyyy-MM-dd) or blank: "); var fs = Console.ReadLine(); DateTime? from = null; if (!string.IsNullOrWhiteSpace(fs) && DateTime.TryParse(fs, out DateTime fdt)) from = fdt;
            Console.Write("To date (yyyy-MM-dd) or blank: "); var ts = Console.ReadLine(); DateTime? to = null; if (!string.IsNullOrWhiteSpace(ts) && DateTime.TryParse(ts, out DateTime tdt)) to = tdt;
            var logs = auditService.Query(a => true);
            if (!string.IsNullOrWhiteSpace(uname))
            {
                var users = userRepo.GetAll().Where(u => u.Name.IndexOf(uname, StringComparison.OrdinalIgnoreCase) >= 0).Select(u => u.Id).ToHashSet();
                logs = logs.Where(l => users.Contains(l.UserId));
            }
            if (from.HasValue) logs = logs.Where(l => l.Timestamp >= from.Value);
            if (to.HasValue) logs = logs.Where(l => l.Timestamp <= to.Value);
            var list = logs.OrderByDescending(l => l.Timestamp).ToList();
            if (!list.Any()) { Console.WriteLine("No audit entries found\n"); return; }
            foreach (var a in list) Console.WriteLine($"{a.Timestamp:yyyy-MM-dd HH:mm:ss} | User:{GetUserName(a.UserId)} | Action:{a.Action} | {a.Details}");
            Console.WriteLine();
        }

        string GetUserName(Guid id) { var u = userRepo.GetById(id); return u != null ? u.Name : id.ToString(); }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var app = new ConsoleApp();
            app.Run();
        }
    }
}
