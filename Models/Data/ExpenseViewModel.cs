using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models.Data
{
    public class ExpenseViewModel
    {
        public int Id { get; set; }

        public string name { get; set; }

        public double amount { get; set; }
        public DateTime DateTime { get; set; }
        public bool recurring { get; set; }

        public int CategoryID { get; set; }
        [ValidateNever]
        public Category? Category { get; set; } = null;
        public List<Expense> Expenses { get; set; } = new List<Expense>();
        [ValidateNever]
        public SelectList Categories { get; set; }


        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? minAmount {get; set;}
        public double? maxAmount { get; set;}

    }
}
