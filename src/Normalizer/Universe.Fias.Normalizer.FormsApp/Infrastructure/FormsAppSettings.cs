using Universe.Fias.Core.Infrastructure;
using Universe.Windows.Forms.Controls.Settings;

namespace Universe.Fias.Normalizer.FormsApp.Infrastructure
{
    public class FormsAppSettings: FormAppSettings
    {
        public AppSettings Default { get; set; }

        public string AddressOne { get; set; }

        public string AddressTwo { get; set; }

        public FormsAppSettings()
        {
            Default = new AppSettings();
        }
    }
}