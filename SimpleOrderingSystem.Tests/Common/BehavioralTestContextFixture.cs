using Microsoft.AspNetCore.Mvc.Testing;
using System.Linq.Expressions;
using System.Net.Http.Headers;

namespace SimpleOrderingSystem.Tests.Common;

/// <summary>
/// A class level fixture that can be used to share a single instance of a <see cref="ConfigurableWebApplicationFactory{TEntryPoint}"/>
/// across all specs within a single specification test class.
/// </summary>
/// <typeparam name="TEntryPoint"></typeparam>
public class BehavioralTestContextFixture<TEntryPoint> : IDisposable
    where TEntryPoint : class
{
    private ConfigurableWebApplicationFactory<TEntryPoint>? _webApplicationFactory;
    private Dictionary<Type, Mock> _mockDictionary = new Dictionary<Type, Mock>();
    private HttpClient? _client;

    /// <summary>
    /// Optional: Setup for bindings for web application factory command line arguments.
    /// NOTE: these are created only ONCE for each instance of a <see cref="BehavioralTestContextFixture{TEntryPoint}"/>. If you need
    /// to have your command line parameters vary per test, you must use the <see cref="WebApplicationFactory"/> directly instead.
    /// </summary>
    /// <returns></returns>
    protected virtual Dictionary<string, object>? CommandLineArguments() => default;

    /// <summary>
    /// Create a new mock of the given type and adds it to the underlying <see cref="IConfigurableWebApplicationFactory"/>.
    /// NOTE: Calls made to this after the the first call to <see cref="CreateClient"/> will only return the same original mock but with <see cref="MockExtensions.Reset(Mock)"/> called on it first. 
    /// After <see cref="CreateClient()"/> has been called
    /// </summary>
    /// <typeparam name="T">Type of the mocked object.</typeparam>
    /// <param name="behavior"></param>
    /// <returns></returns>
    public Mock<T> Mock<T>(MockBehavior behavior = MockBehavior.Default)
        where T : class
    {
        if (_mockDictionary.TryGetValue(typeof(T), out var mock))
        {
            // clear out the mock
            mock.Reset();

            // the default value provider is NOT reset when the mock is reset so do this now if we need to
            if (mock.DefaultValueProvider is BehavioralDefaultValueProvider)
            {
                mock.DefaultValueProvider = new BehavioralDefaultValueProvider();
            }

            return (Mock<T>)mock;
        }

        if (HasCreatedClient)
        {
            throw new InvalidOperationException(
                $"Unable to find mock for type '{typeof(T).FullName}'. New mocks cannot be created once '{nameof(CreateClient)}' has been invoked. " +
                "Please make sure you mock out all items in the constructor of your specification file.");
        }

        mock = ApplicationFactory.Mock<T>(behavior);

        if (_mockDictionary.ContainsKey(typeof(T)))
        {
            _mockDictionary[typeof(T)] = mock;
        }
        else
        {
            _mockDictionary.Add(typeof(T), mock);
        }

        return (Mock<T>)mock;
    }

    /// <summary>
    /// Creates an instance of <see cref="HttpClient"/>
    /// </summary>
    /// <returns>The <see cref="HttpClient"/>.</returns>
    public HttpClient CreateClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions 
        { 
            HandleCookies = false,
            AllowAutoRedirect = false
        });
    }

    /// <summary>
    /// Creates an instance of <see cref="HttpClient"/>
    /// </summary>
    /// <returns>The <see cref="HttpClient"/>.</returns>
    public HttpClient CreateClient(WebApplicationFactoryClientOptions options)
    {
        if (_client is null)
        {
            _client = ApplicationFactory.CreateClient(options);
        }

        return _client;
    }

    #region Helpers
    private ConfigurableWebApplicationFactory<TEntryPoint> ApplicationFactory
    {
        get
        {
            if (!HasBeenSetup)
            {
                _webApplicationFactory = new ConfigurableWebApplicationFactory<TEntryPoint>(
                    ConfigurableWebApplicationFactoryOptions.CreateDefaultBehavioralOptions(CommandLineArguments()));
            }

            return _webApplicationFactory!;
        }
    }

    private bool HasBeenSetup => _webApplicationFactory is not null;

    private bool HasCreatedClient => _client is not null;

    #endregion

    public void Dispose()
    {
        _webApplicationFactory?.Dispose();
    }
}

