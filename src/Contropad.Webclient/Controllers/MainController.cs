using Nancy;

namespace Contropad.Webclient.Controllers
{
    public class ControllerModule : NancyModule
    {
        public ControllerModule()
        {
            Get["/controller/{id}"] = parameters => View["controller.html", new
            {
                parameters.id
            }];
        }
    }
}
