using System.ComponentModel.DataAnnotations;

namespace BookLibraryApp.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Core Counts
        public int TotalBooks { get; set; }
        public int TotalActiveLoans { get; set; }
        public int TotalPatrons { get; set; }

        // Loan/Activity Insights
        public int AvailableBooks { get; set; }
        public int ExpiringLoansNext3Days { get; set; } // Loans due soon

        // Popularity Insight
        public string MostPopularBookTitle { get; set; } = "N/A";
        public int MostPopularBookLoanCount { get; set; } = 0;
    }
}
