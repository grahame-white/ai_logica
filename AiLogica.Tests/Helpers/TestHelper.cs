using AiLogica.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;

namespace AiLogica.Tests.Helpers;

/// <summary>
/// Shared test utilities to reduce duplication across test files.
/// </summary>
public static class TestHelper
{
    /// <summary>
    /// Creates a HomeViewModel instance configured for testing.
    /// </summary>
    /// <returns>A HomeViewModel with NullLogger for test isolation.</returns>
    public static HomeViewModel CreateTestViewModel() => new(NullLogger<HomeViewModel>.Instance);
}
