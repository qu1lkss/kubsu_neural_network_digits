using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;

namespace MO31_Leonov_Digits;

// Окно предупреждения для Avalonia — аналог MessageBox из WinForms.
internal static class WarningDialog
{
    public static Task ShowAsync(string message) => ShowDialogAsync(message, true);

    public static Task ShowInformationAsync(string message) => ShowDialogAsync(message, false);

    private static async Task ShowDialogAsync(string message, bool isWarning)
    {
        // Создание и показ окна выполняются в потоке интерфейса.
        if (!Dispatcher.UIThread.CheckAccess())
        {
            await Dispatcher.UIThread.InvokeAsync(() => ShowDialogAsync(message, isWarning));
            return;
        }

        var owner = (Application.Current?.ApplicationLifetime
            as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (owner is null)
        {
            throw new InvalidOperationException("Для показа предупреждения необходимо главное окно приложения.");
        }

        var okButton = new Button
        {
            Content = "ОК",
            MinWidth = 90,
            IsDefault = true,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        var dialog = new Window
        {
            Title = isWarning ? "Внимание" : "Информация",
            Width = 460,
            SizeToContent = SizeToContent.Height,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Thickness(24),
                Spacing = 20,
                Children =
                {
                    new TextBlock
                    {
                        Text = (isWarning ? "⚠ " : "ⓘ ") + message,
                        TextWrapping = TextWrapping.Wrap
                    },
                    okButton
                }
            }
        };

        // Закрытие окна крестиком отменяет сохранение: подтверждение — только кнопкой ОК.
        okButton.Click += (_, _) => dialog.Close(true);
        bool confirmed = await dialog.ShowDialog<bool>(owner);
        if (isWarning && !confirmed)
        {
            throw new OperationCanceledException("Сохранение весов отменено.");
        }
    }
}
