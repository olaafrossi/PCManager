// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 02 28
// by Olaaf Rossi

using System;

namespace PCManager.WPFUI
{
    public class ControlInfoDataItem
    {
        public ControlInfoDataItem(Type pageType, string title = null)
        {
            this.PageType = pageType;
            this.Title = title ?? pageType.Name.Replace("Page", null);
        }

        public Type PageType { get; }

        public string Title { get; }

        public override string ToString()
        {
            return this.Title;
        }
    }
}