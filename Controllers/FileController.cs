using ExpenseTracker.Data;
using ExpenseTracker.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        private readonly ApplicationDbContext _context;
        public FileController(ApplicationDbContext context)
        {
            _context = context;
        }
       
        public IActionResult Import()
        {
            return View();
        }
        //Import Headers Current order Name, amount, Date, CategoryName, Recurring
        [HttpPost]
        public IActionResult Import(IFormFile file)
        {
            //Gets current user and only gives there expenses
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            using (var stream = new StreamReader(file.OpenReadStream()))
            {
                var importExpenseList = new List<Expense>();
                string headerLine = stream.ReadLine(); // Read the header line
               
                if (headerLine == null)
                {
                    throw new InvalidOperationException("CSV file is empty.");
                }

                while (!stream.EndOfStream)
                {
                    var line = stream.ReadLine();
                    var values = line.Split(',');

                    Category? category = _context.Category.FirstOrDefault(c => c.Name.ToLower() == values[3].ToLower());
                    var categoryId = 1; //default to one for now
                    
                    if(category == null)
                    {
                        Category newCategory = new Category { Name = values[3] };
                            _context.Category.Add(newCategory);
                        _context.SaveChanges();
                        categoryId = newCategory.CategoryId;
                    } else
                    {
                        categoryId = category.CategoryId;
                    }

                    var expense = new Expense
                    {
                        UserID = currentUserId,
                        name = values[0],
                        amount = double.Parse(values[1]),
                        DateTime = DateTime.Parse(values[2]),
                        CategoryID = categoryId,
                        recurring = bool.Parse(values[4]),
                        copied = false
                    };

                    importExpenseList.Add(expense);
                }
                _context.Expenses.AddRange(importExpenseList);
                int savedAmount = _context.SaveChanges();
                if(savedAmount > 0)
                {
                    return RedirectToAction("Index", "Expenses");
                }

            }
                return View();
        }
        public IActionResult Export()
        {
            //Get CurrentUserId
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            //Get all Expenses by UserID
            var expenses = _context.Expenses.Include(e => e.Category).Where(e => e.UserID == currentUserId).ToList();

            //Use stringbuilder to build each line
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Name,Amount,Date,Category,Recurring");

            foreach(var expense in expenses)
            {
                var csvLine = $"{expense.name},{expense.amount},{expense.DateTime:yyyy-MM-dd},{expense.Category.Name},{expense.recurring}";
                csvBuilder.AppendLine(csvLine);
            }
            //Build the Entire CSV
            var csvContent = csvBuilder.ToString();
            //Encode it for File
            var csvBytes = Encoding.UTF8.GetBytes(csvContent);

            //Sends the file back to browser to save it.
            return File(csvBytes, "text/csv", $"{DateTime.Now.ToShortDateString()} expenses.csv");
        }
    }
}
