using IntexLegoSecure.Models;
using Microsoft.AspNetCore.Mvc;

namespace IntexLegoSecure.Views.Shared.Components;

public class ProjectTypesViewComponent : ViewComponent
{
    private I_Repository _repo;
    //constructor (method ran once when building app)
    public ProjectTypesViewComponent(I_Repository temp)
    {
        _repo = temp;
    }
    public string Invoke()
    {
        var LegoTypes = _repo.Products
            .Select(x => x.Category)
            .Distinct()
            .OrderBy(x => x);
        return ("This worked");
    }
}