using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace AiLogica.Tests.EndToEnd
{
    /// <summary>
    /// Base class for end-to-end tests using Playwright for browser automation
    /// </summary>
    public abstract class EndToEndTestBase : IAsyncLifetime
    {
        protected WebApplicationFactory<Program> Factory { get; private set; } = null!;
        protected IPlaywright Playwright { get; private set; } = null!;
        protected IBrowser Browser { get; private set; } = null!;
        protected IBrowserContext BrowserContext { get; private set; } = null!;
        protected IPage Page { get; private set; } = null!;
        protected string BaseUrl { get; private set; } = string.Empty;

        public virtual async Task InitializeAsync()
        {
            // Create the web application factory
            Factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                    builder.ConfigureLogging(logging =>
                    {
                        // Reduce logging noise in tests
                        logging.SetMinimumLevel(LogLevel.Warning);
                    });
                });

            // Start the test server and get the base URL
            var client = Factory.CreateClient();
            BaseUrl = client.BaseAddress?.ToString() ?? throw new InvalidOperationException("Could not determine base URL");

            // Initialize Playwright
            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

            // Launch browser - try system Firefox first, fallback to Playwright's version
            var firefoxPath = FindSystemFirefox();
            
            try
            {
                // Try system Firefox first
                Browser = await Playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    ExecutablePath = firefoxPath,
                    Args = new[] { 
                        "--no-sandbox", 
                        "--disable-dev-shm-usage",
                        "--disable-gpu",
                        "--disable-extensions",
                        "--disable-plugins",
                        "--no-first-run",
                        "--disable-default-apps"
                    }
                });
            }
            catch (Exception ex) when (firefoxPath != null)
            {
                // System Firefox failed, fall back to Playwright's Firefox
                Console.WriteLine($"System Firefox failed: {ex.Message}. Falling back to Playwright's Firefox.");
                Browser = await Playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    Args = new[] { "--no-sandbox", "--disable-dev-shm-usage" }
                });
            }

            // Create browser context with reasonable viewport
            BrowserContext = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
            });

            // Create a new page
            Page = await BrowserContext.NewPageAsync();
        }

        public virtual async Task DisposeAsync()
        {
            if (Page != null)
                await Page.CloseAsync();

            if (BrowserContext != null)
                await BrowserContext.CloseAsync();

            if (Browser != null)
                await Browser.CloseAsync();

            Playwright?.Dispose();
            Factory?.Dispose();
        }

        /// <summary>
        /// Find system Firefox installation path
        /// </summary>
        private static string? FindSystemFirefox()
        {
            var commonPaths = new[]
            {
                "/usr/bin/firefox",           // Ubuntu/Debian
                "/usr/lib/firefox/firefox",   // Some Ubuntu installations
                "/snap/bin/firefox",          // Snap package
                "/usr/bin/firefox-esr",       // Extended Support Release
                "/opt/firefox/firefox",       // Manual installation
                "/Applications/Firefox.app/Contents/MacOS/firefox", // macOS
                @"C:\Program Files\Mozilla Firefox\firefox.exe",    // Windows
                @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe" // Windows 32-bit
            };

            foreach (var path in commonPaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return null; // Let Playwright use its own Firefox
        }

        /// <summary>
        /// Navigate to the application home page
        /// </summary>
        protected async Task NavigateToHomePageAsync()
        {
            await Page.GotoAsync(BaseUrl);
            // Wait for the page to be ready
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Take a screenshot for debugging purposes
        /// </summary>
        protected async Task<byte[]> TakeScreenshotAsync(string? name = null)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss");
            var filename = name != null ? $"{name}_{timestamp}.png" : $"screenshot_{timestamp}.png";

            return await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = $"/tmp/{filename}",
                FullPage = true
            });
        }

        /// <summary>
        /// Assert that an element contains specific text with a custom timeout
        /// </summary>
        protected async Task AssertElementContainsTextAsync(string selector, string expectedText, int timeoutMs = 5000)
        {
            var element = Page.Locator(selector);
            await element.WaitForAsync(new LocatorWaitForOptions { Timeout = timeoutMs });
            var text = await element.TextContentAsync();

            if (text == null || !text.Contains(expectedText))
            {
                // Take screenshot for debugging
                await TakeScreenshotAsync($"assertion_failed_{expectedText.Replace(" ", "_")}");
                throw new Xunit.Sdk.XunitException($"Expected element '{selector}' to contain text '{expectedText}', but got: '{text}'");
            }
        }

        /// <summary>
        /// Wait for element to be visible and return it
        /// </summary>
        protected async Task<ILocator> WaitForElementAsync(string selector, int timeoutMs = 5000)
        {
            var element = Page.Locator(selector);
            await element.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeoutMs
            });
            return element;
        }
    }
}
