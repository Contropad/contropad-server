using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace Contropad.Webclient.Controllers
{
    public class ControllerModule : NancyModule
    {
        public ControllerModule()
        {
            Get["/controller/{id}"] = parameters => View["controller.html", new
            {
                id = parameters.id,
                hostname = "192.168.2.3"
            }];
        }
    }
}
