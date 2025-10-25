using BookLibraryApp.Models.ViewModels;

namespace BookLibraryApp.Services
{
    // The 'I' stands for Interface
    public interface IPatronService
    {
        Task<IEnumerable<PatronViewModel>> GetAllPatrons();
        Task<PatronViewModel?> GetPatronById(int id);
        Task AddPatron(PatronViewModel model);
        Task<bool> UpdatePatron(PatronViewModel model);
        Task<bool> DeletePatron(int id);
    }
}