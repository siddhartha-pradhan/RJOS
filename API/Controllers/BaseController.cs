using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace RJOS.Controllers;

public class BaseController<T> : Controller where T : BaseController<T>
{
    [NonAction]
    public string ConvertViewToString<TModel>(string viewName, TModel model, bool partial = false)
    {
        if (string.IsNullOrEmpty(viewName))
        {
            viewName = ControllerContext.ActionDescriptor.ActionName;
        }

        ViewData.Model = model;

        using var writer = new StringWriter();
        
        var viewEngine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
        
        var viewResult = viewEngine?.FindView(this.ControllerContext, viewName, !partial);

        if (viewResult is { Success: false })
        {
            return $"A view with the name {viewName} could not be found";
        }

        if (viewResult?.View == null) return writer.ToString();
        
        var viewContext = new ViewContext(
            ControllerContext,
            viewResult.View,
            ViewData,
            TempData,
            writer,
            new HtmlHelperOptions()
        );

        viewResult.View.RenderAsync(viewContext);

        return writer.ToString();
    }

    [NonAction]
    protected ActionResult DownloadAnyFile(string sFileName, string sFileFullPath, string subDirectory = null)
    {
        if (System.IO.File.Exists(sFileFullPath))
        {
            var fileBytes = System.IO.File.ReadAllBytes(sFileFullPath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, sFileName);
        }
        else
        {
            var sPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "path");
            var fileBytes = System.IO.File.ReadAllBytes(sPath);
            return File(fileBytes, "image/jpg;base64");
        }
    }
}