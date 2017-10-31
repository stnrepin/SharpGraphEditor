using System;

using Caliburn.Micro;

namespace SharpGraphEditor.Models
{
    public class ZoomManager : PropertyChangedBase
    {
        public double MaxZoom { get; } = 2;
        public double MinZoom { get; } = 0.3;

        public double CurrentZoom { get; private set; }

        public int CurrentZoomInPercents => (int)(CurrentZoom * 100);

        public ZoomManager()
        {
            CurrentZoom = 1.0;
        }

        public void ChangeCurrentZoom(double value)
        {
            var newZoom = CurrentZoom + value;
            if (newZoom >= MinZoom && newZoom <= MaxZoom)
            {
                CurrentZoom = Math.Round(newZoom, 2);
            }
        }

        public void ChangeZoomByPercents(double percents)
        {
            ChangeCurrentZoom(percents / 100);
        }
    }
}
