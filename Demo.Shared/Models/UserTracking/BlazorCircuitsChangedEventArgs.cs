using System;

namespace Demo.Shared.Models.UserTracking
{
    public class BlazorCircuitsChangedEventArgs : EventArgs
    {
        public DateTime CircuitsChangedTime { get; set; }
    }
}
