using Universe.CQRS.Infrastructure;
using Universe.Fias.Core.Infrastructure;
using Universe.Helpers.Extensions;
using Universe.Windows.Forms.Controls.Settings;

namespace Universe.Fias.Import.FormsApp.Infrastructure
{
    public class FormsAppSettings: FormAppSettings
    {
        public AppSettings Default { get; set; }

        public FormsAppSettings()
        {
            Default = new AppSettings();
        }
    }
}