using Universe.Fias.Core.Infrastructure;
using Universe.Windows.Forms.Controls.Settings;

namespace Universe.Fias.XML.GAR.Converter.x64.FormsApp.Infastructure
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