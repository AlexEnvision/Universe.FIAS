using Universe.Fias.Core.Infrastructure;
using Universe.Windows.Forms.Controls.Settings;

namespace Universe.Fias.DBF.Converter.x64.FormsApp.Infrastructure
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