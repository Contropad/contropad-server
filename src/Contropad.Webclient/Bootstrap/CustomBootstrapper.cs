using Nancy;
using Nancy.Conventions;
using Nancy.TinyIoc;

namespace Contropad.Webclient.Bootstrap
{
    public class CustomBoostrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            conventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("assets", "Assets")
            );
        }
    }
}
