namespace CloudCityCenter.Models;

public enum ProductType
{
    DedicatedServer = 0,
    Hosting = 1,
    Website = 2,
    // Newly added product categories
    VPS = 3,
    VPN = 4,
    Storage = 5,
    WebsiteBuilder = 6,  // Сайты на конструкторе
    WebsiteCode = 7,     // Сайты на коде
    VDI = 8              // Virtual Desktop Infrastructure
}
