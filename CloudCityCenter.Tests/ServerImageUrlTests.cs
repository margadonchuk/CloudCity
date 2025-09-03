using CloudCityCenter.Models;

namespace CloudCityCenter.Tests;

public class ServerImageUrlTests
{
    [Fact]
    public void ImageUrl_Rewrites_Relative_FileName()
    {
        var server = new Server { Name = "S", Location = "US" };
        server.ImageUrl = "pic.png";
        Assert.Equal("/images/servers/pic.png", server.ImageUrl);
    }

    [Fact]
    public void ImageUrl_Preserves_Absolute_Url()
    {
        var server = new Server { Name = "S", Location = "US" };
        var url = "https://example.com/pic.png";
        server.ImageUrl = url;
        Assert.Equal(url, server.ImageUrl);
    }
}
