using IntexLegoSecure.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IntexLegoSecure.Views.Shared.Components
{
    public class ProjectTypesViewComponent : ViewComponent
    {
        private readonly I_Repository _repo;

        // Constructor
        public ProjectTypesViewComponent(I_Repository temp)
        {
            _repo = temp;
        }

        public string Invoke()
        {
            var legoTypes = _repo.Products
                .Select(x => x.Category)
                .Distinct()
                .OrderBy(x => x);

            // Join the distinct category strings into a single string
            var joinedCategories = string.Join(", ", legoTypes);

            // Pass the joined string as the model to the view
            return(joinedCategories);
        }
    }
}