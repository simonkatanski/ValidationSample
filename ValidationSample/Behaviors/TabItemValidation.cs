using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ValidationSample.Behaviors
{
    public static class TabItemValidation
    {
        // Dictionary of validated controls along with their parent TabItems
        private static List<KeyValuePair<DependencyObject, TabItem>>
          elements = new List<KeyValuePair<DependencyObject, TabItem>>();

        public static void SetActivateValidation(UIElement element, bool value)
        {
            element.SetValue(ActivateValidationProperty, value);
        }
        public static bool GetActivateValidation(UIElement element)
        {
            return (bool)element.GetValue(ActivateValidationProperty);
        }

        public static void SetIsTabValid(UIElement element, bool value)
        {
            element.SetValue(IsTabValidProperty, value);
        }
        public static bool GetIsTabValid(UIElement element)
        {
            return (bool)element.GetValue(IsTabValidProperty);
        }

        public static void SetDisplayMode(UIElement element, bool value)
        {
            element.SetValue(DisplayModeProperty, value);
        }
        public static bool GetDisplayMode(UIElement element)
        {
            return (bool)element.GetValue(DisplayModeProperty);
        }

        // Activate validation property
        public static readonly DependencyProperty ActivateValidationProperty =
                DependencyProperty.RegisterAttached("ActivateValidation", typeof(bool),
                typeof(TabItemValidation), new UIPropertyMetadata(ValueChanged));

        // Current state of the validation property
        public static readonly DependencyProperty IsTabValidProperty =
                DependencyProperty.RegisterAttached("IsTabValid", typeof(bool),
                typeof(TabItemValidation), new UIPropertyMetadata(true));

        public static readonly DependencyProperty DisplayModeProperty = 
                DependencyProperty.RegisterAttached("DisplayMode", typeof(DisplayMode),
                typeof(TabItemValidation), new UIPropertyMetadata(DisplayMode.ReadOnly, DisplayModeChanged));

        private static void ValueChanged(DependencyObject dp, DependencyPropertyChangedEventArgs args)
        {
            TabItem tabItem = dp as TabItem;
            if (tabItem != null)
            {
                Validation.AddErrorHandler(tabItem, ValidationOccurred);
            }
        }

        public static void ValidationOccurred(object sender, ValidationErrorEventArgs args)
        {
            TabItem item = sender as TabItem;
            // ValidationSource is the control that caused the validation to occur
            var validationSource = args.OriginalSource as DependencyObject;
            for (int i = elements.Count - 1; i >= 0; i--)
            {
                // Iterate through elements and remove the ones without errors
                if (!Validation.GetHasError(elements[i].Key))
                {
                    ((FrameworkElement)elements[i].Key).Unloaded -= UnloadedEvent;
                    elements.RemoveAt(i);
                }
                else if (!((FrameworkElement)elements[i].Key).IsEnabled)
                {
                    ((FrameworkElement)elements[i].Key).Unloaded -= UnloadedEvent;
                    elements.RemoveAt(i);
                }
            }

            // If the control is not in the list and has got validation error we
            // add it to the list
            if (!elements.Any(p => p.Key == validationSource) &&
                Validation.GetHasError(validationSource))
            {
                // We add Unloaded item to get the notification when the control
                // leaves the tree
                ((FrameworkElement)validationSource).Unloaded += UnloadedEvent;
                elements.Add(new KeyValuePair<DependencyObject, TabItem>
                    (validationSource, (TabItem)sender));
            }

            bool anyInvalid = false;
            // We check if any control in the list causes errors and if it is
            // inside our TabItem's content
            for (int i = 0; i < elements.Count; i++)
            {
                if (IsVisualChild(item.Content as DependencyObject,

                elements[i].Key) && Validation.GetHasError(elements[i].Key))
                {
                    anyInvalid = true;
                }
            }
            // We properly set the IsTabValid property on our TabItem
            if (anyInvalid)
                item.SetValue(IsTabValidProperty, false);
            else
                item.SetValue(IsTabValidProperty, true);
        }

        public static bool IsVisualChild(DependencyObject parent, DependencyObject child)
        {
            if (parent == null || child == null)
                return false;

            bool result = false;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var visualChild = VisualTreeHelper.GetChild(parent, i);
                if (child != visualChild)
                {
                    result = IsVisualChild(visualChild, child);
                    if (result)
                        break;
                }
                else
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public static void UnloadedEvent(object sender, RoutedEventArgs args)
        {
            if (elements.Count > 0)
            {
                var dpSender = elements.Find(p => p.Key == sender);
                if (dpSender.Value != null)
                {
                    var parent = dpSender.Value.Content as DependencyObject;
                    var child = sender as DependencyObject;
                    if (!IsVisualChild(parent, child))
                    {
                        if (elements.Count == 1)
                        {
                            elements[0].Value.SetValue(IsTabValidProperty, true);
                        }
                        ((FrameworkElement)sender).Unloaded -= UnloadedEvent;
                        elements.RemoveAll(p => p.Key == sender);
                    }
                }
            }
        }

        private static void DisplayModeChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs args)
        {
            if ((DisplayMode)args.NewValue == DisplayMode.ReadOnly &&
                ((DisplayMode)args.OldValue != DisplayMode.ReadOnly))
            {
                foreach (var tabItem in elements.Select(p => p.Value))
                {
                    tabItem.SetValue(IsTabValidProperty, true);
                }
                elements.Clear();
            }
        }
    }
}