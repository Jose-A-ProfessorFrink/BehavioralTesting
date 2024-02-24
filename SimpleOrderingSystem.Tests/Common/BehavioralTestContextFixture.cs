using Microsoft.AspNetCore.Mvc.Testing;

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
    /// Setup for the fixture which creates the initial web application factory and bindings for its test configuration and command line arguments.
    /// NOTEL: Only the FIRST call to this will honored. Any subsequent calls will be ignored.
    /// </summary>
    /// <param name="testConfiguration"></param>
    /// <returns></returns>
    public void Setup(
        Dictionary<string, string?>? testConfiguration = null,
        Dictionary<string, object>? commandLineArguments = null)
    {
        if (HasBeenSetup)
        {
            return;
        }

        _webApplicationFactory = new ConfigurableWebApplicationFactory<TEntryPoint>(
            ConfigurableWebApplicationFactoryOptions.CreateDefaultBehavioralOptions(testConfiguration, commandLineArguments));
    }

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
                throw new InvalidOperationException($"You must call '{nameof(Setup)}' first.");
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

