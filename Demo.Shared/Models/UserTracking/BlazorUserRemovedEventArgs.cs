
using System;

namespace Demo.Shared.Models.UserTracking
{
    public class BlazorUserRemovedEventArgs : EventArgs
    {
        public DateTime UserRemovedTime { get; set; }
    }
}
