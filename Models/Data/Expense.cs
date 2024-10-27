using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models.Data
{
    public class Expense
    {
        public int Id { get; set; }

        public string UserID { get; set; }

        public string name { get; set; }

        public double amount { get; set; }

        public bool recurring { get; set; }
        public bool copied { get; set; }

        public DateTime DateTime { get; set; }

         public int CategoryID { get; set; }
        [ValidateNever]
        public Category? Category { get; set; } = null;

    }
}
