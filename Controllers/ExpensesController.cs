using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ExpenseTracker.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Components.Routing;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Applicationuser> _userManager;
        public ExpensesController(ApplicationDbContext context, UserManager<Applicationuser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Expenses
        public async Task<IActionResult> Index(int? categoryId, DateTime? startDate, DateTime? endDate, double? minAmount, double? maxAmount)
        {
            
            //Gets current user and only gives there expenses
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            DateTime month = DateTime.Now.AddDays(-30);

            var recurring = _context.Expenses.Where(e => e.UserID == currentUserId && e.recurring && !e.copied && e.DateTime <= month).ToList();

           

            foreach (var oldExpense in recurring)
            {
                Expense newExpense = new Expense
                {
                    DateTime = DateTime.Now,
                    UserID = currentUserId,
                    name = oldExpense.name,
                    amount = oldExpense.amount,
                    recurring = oldExpense.recurring,
                    CategoryID = oldExpense.CategoryID

                };
                _context.Add(newExpense);

                oldExpense.copied = true;
                _context.Update(oldExpense);
            }

            await _context.SaveChangesAsync();

            IQueryable<Expense> applicationDbContext = _context.Expenses.Where(e => e.UserID == currentUserId).Include(e => e.Category);

            if (categoryId.HasValue) // filter category
            {
                applicationDbContext = applicationDbContext.Where(e => e.CategoryID == categoryId.Value);
            }

            if (startDate.HasValue) // filter start date
            {
                applicationDbContext = applicationDbContext.Where(e => e.DateTime >= startDate.Value);
            }
            if (endDate.HasValue) // filter end date
            {
                applicationDbContext = applicationDbContext.Where(e => e.DateTime <= endDate.Value);
            }

            if (minAmount.HasValue) // filter minimum amount
            {
                applicationDbContext = applicationDbContext.Where(e => e.amount >= minAmount.Value);
            }


            if (maxAmount.HasValue) // filter max amount
            {
                applicationDbContext = applicationDbContext.Where(e => e.amount <= maxAmount.Value);
            }

            var viewModel = new ExpenseViewModel
            {
                Expenses = await applicationDbContext.ToListAsync(),
                Categories = new SelectList(await _context.Category.ToListAsync(), "CategoryId", "Name"),
                CategoryID = categoryId ?? 0,
                StartDate = startDate,
                EndDate = endDate,
                minAmount = minAmount,
                maxAmount = maxAmount,

            };
            //var applicationDbContext = _context.Expenses.Where(e => e.UserID == currentUserId).Include(e => e.Category);
            return View(viewModel);
        }

        // GET: Expenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Expenses == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // GET: Expenses/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryId", "Name");
            return View();
        }

        // POST: Expenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseViewModel model)
        {
            

            if (ModelState.IsValid)
            {
                ClaimsPrincipal currentUser = this.User;
                var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                
                Expense newExpense = new Expense
                {
                    Id = model.Id,
                    DateTime = model.DateTime,
                    UserID = currentUserId,
                    name = model.name,
                    amount = model.amount,
                    recurring = model.recurring,
                    CategoryID = model.CategoryID
                };
                
                _context.Add(newExpense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryId", "Name", model.CategoryID);
            return View(model);
        }

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Expenses == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryId", "Name", expense.CategoryID);
            return View(expense);
        }

        // POST: Expenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserID,Datetime,name,amount,recurring,CategoryID")] Expense expense)
        {
            if (id != expense.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    expense.DateTime = DateTime.Now;
                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryId", "Name", expense.CategoryID);
            return View(expense);
        }

        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Expenses == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Expenses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Expenses'  is null.");
            }
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult NewCat()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewCat([Bind("CategoryID, Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                Category newCategory = new Category
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name
                };

                _context.Add(newCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            
            return View(category);
        }

        public async Task<IActionResult> Budget()
        {
            //Gets current user and only gives there expenses
            
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                return NotFound();
            }


            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Budget(Applicationuser User)
        {

            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = await _userManager.FindByIdAsync(currentUserId);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                user.Budget = User.Budget.Value;
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Index");
            }


            return View(user);
        }

        public async Task<IActionResult> Charts()
        {
            //Current user only
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            var oUser = await _userManager.FindByIdAsync(currentUserId);

            //Current Expenses
            var currentUserExpenses = _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserID == currentUserId);
            
            //Summarize by categoryName for chart generation
            var sumExpensesbyCategoryName = currentUserExpenses.GroupBy(s => s.Category.Name).Select(e => new Expense { name = e.Key, amount = e.Sum(a => a.amount) }).ToList();

            //Get the years availble
            var years = currentUserExpenses.Select(e => e.DateTime.Year).Distinct().OrderBy(y => y).ToList();
            //get months
            var months = currentUserExpenses.Select(e => e.DateTime.Month).Distinct().OrderBy(m => m).ToList();
            ChartViewModel model = new ChartViewModel();
            
            model.ChartExpenseList = sumExpensesbyCategoryName;
            model.AvailableYears = years;
            model.AvailableMonths = months;
            model.Budget = oUser.Budget ?? 0;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetDateRangeCharts([FromBody] DateRangeRequest request)
        {
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            var oUser = await _userManager.FindByIdAsync(currentUserId);

            var currentUserExpenses = _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserID == currentUserId);

            IQueryable<Expense> queryList = null;
            if (request.RangeType == "custom")
            {
                queryList = currentUserExpenses.Where(e => e.DateTime >= request.StartDate && e.DateTime <= request.EndDate);
            }
            else if(request.RangeType == "year")
            {
                queryList = currentUserExpenses.Where(e => e.DateTime.Year == request.Year);
            }
            else if(request.RangeType == "month")
            {
                queryList = currentUserExpenses.Where(e => e.DateTime.Year == request.Year && e.DateTime.Month == request.Month);
            } else //All route
            {
                queryList = currentUserExpenses;
            }
            
            var expenseList = queryList.GroupBy(s => s.Category.Name).Select(e => new Expense { name = e.Key, amount = e.Sum(a => a.amount) }).ToList();
            ChartViewModel model = new ChartViewModel();

            model.ChartExpenseList = expenseList;
            model.Budget = oUser.Budget ?? 0;

            return PartialView("_GeneratedCharts", model);
        }
        private bool ExpenseExists(int id)
        {
          return (_context.Expenses?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        public async Task<IActionResult> Recurring()
        {

            //Gets current user and only gives there expenses
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

          

            await _context.SaveChangesAsync();
            var applicationDbContext = _context.Expenses.Where(e => e.UserID == currentUserId && e.recurring == true).Include(e => e.Category);
            return View(await applicationDbContext.ToListAsync());
        }
    }
}
