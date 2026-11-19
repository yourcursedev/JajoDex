using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using JajoDex.Services;
using JajoDex.ViewModels;
using JajoDex.Views;

namespace JajoDex;

/// <summary>
/// Application bootstrapper that enforces single-instance execution via a named Global Mutex,
/// launches the animated dark glassmorphic SplashWindow, initializes the dual-window architecture
/// (OverlayWindow + MainWindow Control Panel) and low-overhead background hooks, and smoothly
/// transitions into MainWindow.
/// </summary>
public partial class App : Application
{
    private const string SingleInstanceMutexName = @"Global\JajoDex_SingleInstance_Mutex";
    private const string ActivateEventName = @"Global\JajoDex_Activate_Event";

    private Mutex? _singleInstanceMutex;
    private bool _ownsMutex;
    private EventWaitHandle? _activateEvent;
    private RegisteredWaitHandle? _registeredWaitHandle;

    private ConfigService? _configService;
    private ProcessMonitorService? _processMonitor;
    private MouseHookService? _mouseHook;
    private HotkeyService? _hotkeyService;
    private TrayIconService? _trayIconService;
    private OverlayWindow? _overlayWindow;
    private MainWindow? _mainWindow;
    private MainViewModel? _mainViewModel;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        bool createdNew;
        try
        {
            _singleInstanceMutex = new Mutex(true, SingleInstanceMutexName, out createdNew);
            _ownsMutex = createdNew;
        }
        catch (UnauthorizedAccessException)
        {
            // Another (e.g. elevated) instance already holds the Global mutex
            createdNew = false;
            _ownsMutex = false;
        }

        if (!createdNew)
        {
            try
            {
                using var existingEvent = EventWaitHandle.OpenExisting(ActivateEventName);
                existingEvent.Set();
            }
            catch
            {
                // Ignore if existing instance cannot be signaled
            }

            Shutdown(0);
            return;
        }

        try
        {
            _activateEvent = new EventWaitHandle(false, EventResetMode.AutoReset, ActivateEventName);
            _registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
                _activateEvent,
                (_, timedOut) =>
                {
                    if (!timedOut)
                    {
                        Dispatcher.InvokeAsync(() => _mainWindow?.ShowAndActivate());
                    }
                },
                null,
                Timeout.Infinite,
                false);
        }
        catch
        {
            // Non-fatal if cross-session event creation is restricted
        }

        SessionEnding += (_, _) => Shutdown();

        // 1. Launch Premium Dark Glassmorphic Splash Screen First
        var splash = new SplashWindow();
        splash.Show();

        // Allow layout pass so progress track ActualWidth is measured
        await Task.Delay(80);
        splash.SetProgress(18, "INITIALIZING OVERLAY ENGINE...");

        // 2. Load ConfigService & Persistent Profiles
        await Task.Delay(450);
        _configService = await Task.Run(() => new ConfigService());
        splash.SetProgress(46, "LOADING PROFILES & CONFIG...");

        // 3. Initialize Low-Level OS Hooks & Click-Through Overlay Window
        await Task.Delay(480);
        _processMonitor = new ProcessMonitorService();
        _mouseHook = new MouseHookService();
        _hotkeyService = new HotkeyService();
        _overlayWindow = new OverlayWindow();

        splash.SetProgress(76, "BINDING WIN32 HOOKS & DISPLAYS...");

        // 4. Construct MainViewModel, MainWindow & System Tray Icon
        await Task.Delay(480);
        _mainViewModel = new MainViewModel(
            _configService,
            _processMonitor,
            _mouseHook,
            _hotkeyService,
            _overlayWindow);

        _mainWindow = new MainWindow(_mainViewModel, _hotkeyService);

        _trayIconService = new TrayIconService(_mainViewModel);
        _trayIconService.OpenControlPanelRequested += (_, _) => _mainWindow.ShowAndActivate();
        _trayIconService.ToggleOverlayRequested += (_, _) => _mainViewModel.ToggleOverlayVisibility();
        _trayIconService.CycleProfileRequested += (_, _) => _mainViewModel.CycleNextProfile();
        _trayIconService.CenterCrosshairRequested += (_, _) => _mainViewModel.ResetOffsetToCenter();
        _trayIconService.ExitApplicationRequested += (_, _) => Shutdown();

        // Start event-driven Win32 hooks (0% polling overhead)
        _processMonitor.Start();
        _mouseHook.Start();

        splash.SetProgress(100, "READY");
        await Task.Delay(380);

        if (_configService.Config.IsOverlayVisible)
        {
            _overlayWindow.Show();
        }

        // 5. Smoothly Fade Out SplashWindow and Fade In MainWindow (or keep minimized to tray)
        await splash.FadeOutAndCloseAsync();

        if (!_configService.Config.StartMinimizedToTray)
        {
            _mainWindow.Opacity = 0.0;
            _mainWindow.Show();
            var fadeIn = new DoubleAnimation(0.0, 1.0, TimeSpan.FromMilliseconds(260));
            _mainWindow.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _configService?.Save();
        _trayIconService?.Dispose();
        _hotkeyService?.Dispose();
        _mouseHook?.Dispose();
        _processMonitor?.Dispose();
        _mainViewModel?.Dispose();

        _registeredWaitHandle?.Unregister(null);
        _activateEvent?.Dispose();

        if (_ownsMutex && _singleInstanceMutex != null)
        {
            try
            {
                _singleInstanceMutex.ReleaseMutex();
            }
            catch (ApplicationException)
            {
                // Already released or thread-affinity mismatch
            }
        }
        _singleInstanceMutex?.Dispose();

        base.OnExit(e);
    }
}
