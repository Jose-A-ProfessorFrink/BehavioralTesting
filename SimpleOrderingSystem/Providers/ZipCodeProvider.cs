namespace SimpleOrderingSystem.Providers;

internal class ZipCodeProvider: IZipCodeProvider
{
    internal static readonly string HttpClientName = typeof(ZipCodeProvider).FullName!;
}