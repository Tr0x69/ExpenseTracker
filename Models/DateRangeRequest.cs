namespace ExpenseTracker.Models
{
    public class DateRangeRequest
    {
        public string RangeType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
    
     }
}
