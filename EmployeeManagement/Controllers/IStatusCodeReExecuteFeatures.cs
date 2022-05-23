namespace EmployeeManagement.Controllers
{
    internal interface IStatusCodeReExecuteFeatures
    {
        dynamic OriginalQueryString { get; }
        dynamic OriginalPath { get; set; }
    }
}