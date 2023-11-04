using System.ComponentModel.Composition;
using System.Windows;

namespace AlexHelms.NINA.PrometheusExporter {

    [Export(typeof(ResourceDictionary))]
    partial class Options : ResourceDictionary {

        public Options() {
            InitializeComponent();
        }
    }
}