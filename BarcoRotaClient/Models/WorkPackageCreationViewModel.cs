using BarcoRota.Models;
using System;

namespace BarcoRota.Client.Models
{
    public class WorkPackageCreationViewModel
    {
        public string Name { get; set; }
        public DateTime FirstJob { get; set; }
        public DateTime LastJob { get; set; }
        public int JobCapacity { get; set; }
        public JobType JobType { get; set; }
    }
}
