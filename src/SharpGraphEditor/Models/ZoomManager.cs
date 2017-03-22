using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Caliburn.Micro;

namespace SharpGraphEditor.Models
{
    public class ZoomManager : PropertyChangedBase
    {

        public int MaxZoom { get; } = 2;


        public double CurrentZoom { get; private set; }

        public int CurrentZoomInPercents => (int)(CurrentZoom * 100);

        public ZoomManager()
        {
            CurrentZoom = 1;
        }

        public void ChangeCurrentZoom(double value)
        {
            if (value >= (1 / MaxZoom) && value <= MaxZoom)
            {
                CurrentZoom = Math.Round(value, 2);
            }
        }

        public void ChangeZoomByPercents(double percents)
        {
            CurrentZoom += percents / 100;
        }
    }
}
