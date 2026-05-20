using System.Collections.Generic;

namespace MDD4All.DME.Configurations
{
    public class DmeConfiguration
    {
        public DataModelDescriptor? CurrentDataModel {  get; set; }

        public List<DataModelDescriptor> RecentDataModels { get; set; } = new List<DataModelDescriptor>();

        public List<DataFileDescriptor> RecentDataFiles { get; set; } = new List<DataFileDescriptor>();
    }
}
