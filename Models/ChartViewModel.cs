using ExpenseTracker.Controllers;
using ExpenseTracker.Models.Data;

namespace ExpenseTracker.Models
{
    public class ChartViewModel
    {
        public List<Expense> ChartExpenseList { get; set; }
        public List<int> AvailableYears { get; set; }
        public List<int> AvailableMonths {  get; set; }
        public decimal Budget {  get; set; }
    }
}
