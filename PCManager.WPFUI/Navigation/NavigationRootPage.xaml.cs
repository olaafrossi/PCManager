// Created by Three Byte Intemedia, Inc. | project: PCManager |
// Created: 2021 02 27
// by Olaaf Rossi

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using PCManager.WPFUI.Presets;

using ModernWpf;
using ModernWpf.Controls;

using PCManager.WPFUI.Common;
using PCManager.WPFUI.ControlPages;

using Frame = ModernWpf.Controls.Frame;

namespace PCManager.WPFUI
{
    /// <summary>
    ///     Interaction logic for NavigationRootPage.xaml
    /// </summary>
    public partial class NavigationRootPage
    {
        private const string AutoHideScrollBarsKey = "AutoHideScrollBars";

        private static readonly ThreadLocal<NavigationRootPage> _current = new();

        private static readonly ThreadLocal<Frame> _rootFrame = new();

        private readonly ControlPagesData _controlPagesData = new();

        private bool _ignoreSelectionChange;

        private Type _startPage;

        public NavigationRootPage()
        {
            this.InitializeComponent();

            if (App.IsMultiThreaded)
            {
                //PresetsMenu.Visibility = Visibility.Collapsed;
                this.NewWindowMenuItem.Visibility = Visibility.Visible;
            }

            Loaded += delegate
                {
                    PCManager.WPFUI.Presets.PresetManager.Current.ColorPresetChanged += OnColorPresetChanged;

                    //controlsSearchBox.Focus();
                };

            Unloaded += delegate
                {
                    PresetManager.Current.ColorPresetChanged -= OnColorPresetChanged;
                };

            OnColorPresetChanged(null, null);

            Current = this;
            RootFrame = this.rootFrame;

            this.SetStartPage();
            if (this._startPage != null)
            {
                this.PagesList.SelectedItem = this.PagesList.Items.OfType<ControlInfoDataItem>()
                    .FirstOrDefault(x => x.PageType == this._startPage);
            }

            this.NavigateToSelectedPage();

            if (Debugger.IsAttached)
            {
                this.DebugMenuItem.Visibility = Visibility.Visible;
            }
        }

        public static NavigationRootPage Current
        {
            get => _current.Value;
            private set => _current.Value = value;
        }

        public static Frame RootFrame
        {
            get => _rootFrame.Value;
            private set => _rootFrame.Value = value;
        }

        private void AutoHideScrollBarsAuto_Checked(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources.Remove(AutoHideScrollBarsKey);
        }

        private void AutoHideScrollBarsOff_Checked(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources[AutoHideScrollBarsKey] = false;
        }

        private void AutoHideScrollBarsOn_Checked(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources[AutoHideScrollBarsKey] = true;
        }

        private void ContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine(nameof(ContextMenu_Loaded));
            var menu = (ContextMenu)sender;
            var tabItem = (TabItem)menu.PlacementTarget;
            var content = (FrameworkElement)tabItem.Content;
            this.FindMenuItem(menu, ThemeManager.GetRequestedTheme(content)).IsChecked = true;
        }

        private RadioMenuItem FindMenuItem(ContextMenu menu, ElementTheme theme)
        {
            return menu.Items.OfType<RadioMenuItem>().First(x => (ElementTheme)x.Tag == theme);
        }

        private void ForceGC(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private FrameworkElement GetTabItemContent(MenuItem menuItem)
        {
            return ((menuItem?.Parent as ContextMenu)?.PlacementTarget as TabItem)?.Content as FrameworkElement;
        }

        private void NavigateToSelectedPage()
        {
            if (this.PagesList.SelectedValue is Type type)
            {
                RootFrame?.Navigate(type);
            }
        }

        private void NewWindow(object sender, RoutedEventArgs e)
        {
            var thread = new Thread(
                () =>
                    {
                        var window = new MainWindow();
                        window.Closed += delegate
                            {
                                Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
                            };
                        window.Show();
                        Dispatcher.Run();
                    });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }

        private void OnControlsSearchBoxQuerySubmitted(
            AutoSuggestBox sender,
            AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null && args.ChosenSuggestion is ControlInfoDataItem)
            {
                var pageType = (args.ChosenSuggestion as ControlInfoDataItem).PageType;
                RootFrame.Navigate(pageType);
            }
            else if (!string.IsNullOrEmpty(args.QueryText))
            {
                var item = this._controlPagesData.FirstOrDefault(
                    i => i.Title.Equals(args.QueryText, StringComparison.OrdinalIgnoreCase));
                if (item != null)
                {
                    RootFrame.Navigate(item.PageType);
                }
            }
        }

        private void OnControlsSearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var suggestions = new List<ControlInfoDataItem>();

            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var querySplit = sender.Text.Split(' ');
                var matchingItems = this._controlPagesData.Where(
                    item =>
                        {
                            // Idea: check for every word entered (separated by space) if it is in the name,  
                            // e.g. for query "split button" the only result should "SplitButton" since its the only query to contain "split" and "button" 
                            // If any of the sub tokens is not in the string, we ignore the item. So the search gets more precise with more words 
                            bool flag = true;
                            foreach (string queryToken in querySplit)
                            {
                                // Check if token is not in string 
                                if (item.Title.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                                {
                                    // Token is not in string, so we ignore this item. 
                                    flag = false;
                                }
                            }

                            return flag;
                        });
                foreach (var item in matchingItems)
                {
                    suggestions.Add(item);
                }

                if (suggestions.Count > 0)
                {
                    this.ControlsSearchBox.ItemsSource = suggestions.OrderByDescending(
                            i => i.Title.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase))
                        .ThenBy(i => i.Title);
                }
                else
                {
                    this.ControlsSearchBox.ItemsSource = new[] { "No results found" };
                }
            }
        }

        private void OnThemeButtonClick(object sender, RoutedEventArgs e)
        {
            if (ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark)
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            }
            else
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            }
        }

        private void PagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this._ignoreSelectionChange)
            {
                this.NavigateToSelectedPage();
            }
        }

        private void Default_Checked(object sender, RoutedEventArgs e)
        {
            SetApplicationTheme(null);
        }

        private void Light_Checked(object sender, RoutedEventArgs e)
        {
            SetApplicationTheme(ApplicationTheme.Light);
        }

        private void Dark_Checked(object sender, RoutedEventArgs e)
        {
            SetApplicationTheme(ApplicationTheme.Dark);
        }

        private void PresetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem menuItem)
            {
                PresetManager.Current.ColorPreset = (string)menuItem.Header;
            }
        }

        private void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            Debug.Assert(!RootFrame.CanGoForward);

            this._ignoreSelectionChange = true;
            this.PagesList.SelectedValue = RootFrame.CurrentSourcePageType;
            this._ignoreSelectionChange = false;
        }

        private void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                RootFrame.RemoveBackEntry();
            }
        }

        private void SetApplicationTheme(ApplicationTheme? theme)
        {
            {
                ThemeManager.Current.ApplicationTheme = theme;
            }
        }

        partial void SetStartPage();

        private void ShadowsAuto_Checked(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources.Remove(SystemParameters.DropShadowKey);
        }

        private void ShadowsDisabled_Checked(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources[SystemParameters.DropShadowKey] = false;
        }

        private void ShadowsEnabled_Checked(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources[SystemParameters.DropShadowKey] = true;
        }

        private void SizingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem menuItem)
            {
                bool compact = menuItem.Tag as string == "Compact";

                var xcr = Application.Current.Resources.MergedDictionaries.OfType<XamlControlsResources>()
                    .FirstOrDefault();
                if (xcr != null)
                {
                    xcr.UseCompactResources = compact;
                }
            }
        }

        private void ThemeMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine($"{((RadioMenuItem)e.Source).Header} checked");
            var menuItem = (RadioMenuItem)e.Source;
            var tabItemContent = this.GetTabItemContent(menuItem);
            if (tabItemContent != null)
            {
                ThemeManager.SetRequestedTheme(tabItemContent, (ElementTheme)menuItem.Tag);
            }
        }

        private void ThemeMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine($"{((RadioMenuItem)e.Source).Header} unchecked");
        }

        private void ToggleTheme(object sender, RoutedEventArgs e)
        {
            this.GetTabItemContent(sender as MenuItem)?.ToggleTheme();
        }

        private void OnColorPresetChanged(object sender, EventArgs e)
        {
            //{
            //    PresetsMenu.Items
            //    .OfType<RadioMenuItem>()
            //    .Single(mi => mi.Header.ToString() == PresetManager.Current.ColorPreset)
            //    .IsChecked = true;
            //}
        }
    }

    public class ControlPagesData : List<ControlInfoDataItem>
    {
        public ControlPagesData()
        {
            this.AddPage(typeof(PCManagerInfoView), "PC Manager Info");
            this.AddPage(typeof(SliderPage), "Slider Page for testing");
            this.AddPage(typeof(ProcessMonitorView), "Monitored Application");
            this.AddPage(typeof(PCNetworkListenerView), "PC Network Monitor");
        }

        private void AddPage(Type pageType, string displayName = null)
        {
            this.Add(new ControlInfoDataItem(pageType, displayName));
        }
    }

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