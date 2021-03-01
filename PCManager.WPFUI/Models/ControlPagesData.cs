// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 02 28
// by Olaaf Rossi

using System;
using System.Collections.Generic;

using PCManager.WPFUI.ControlPages;

namespace PCManager.WPFUI
{
    public class ControlPagesData : List<ControlInfoDataItem>
    {
        public ControlPagesData()
        {
            this.AddPage(typeof(PCManagerInfoView), "PC Manager Info");
            this.AddPage(typeof(SliderPage), "Control Palette");
        }

        private void AddPage(Type pageType, string displayName = null)
        {
            this.Add(new ControlInfoDataItem(pageType, displayName));
        }
    }
}