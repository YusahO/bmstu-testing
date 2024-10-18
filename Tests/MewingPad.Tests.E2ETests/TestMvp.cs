using System.Net;
using System.Text;
using System.Text.Json;
using MewingPad.Common.Entities;
using MewingPad.DTOs.Auth;
using Newtonsoft.Json;

namespace MewingPad.Tests.E2ETests;

public class TestMvp : IClassFixture<PgWebApplicationFactory<Program>>
{
    private readonly PgWebApplicationFactory<Program> _factory;

    public TestMvp(PgWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RegisterUser_Ok()
    {
        var client = _factory.CreateClient();

        var request = new RegisterDto()
        {
            Username = "User",
            Email = "user@example.com",
            Password = "passwd",
        };
        var httpContent = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json"
        );
        var response = await client.PostAsync(
            "/api/auth/registration",
            httpContent
        );

        var str = await response.Content.ReadAsStringAsync();
        string? id,
            token;
        using (JsonDocument doc = JsonDocument.Parse(str))
        {
            JsonElement root = doc.RootElement;
            id = root.GetProperty("userDto").GetProperty("id").GetString()!;
            token = root.GetProperty("token").GetString()!;
        }
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        var playlists = await client.GetAsync($"/api/users/{id}/playlists");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (
            JsonDocument doc = JsonDocument.Parse(
                await playlists.Content.ReadAsStringAsync()
            )
        )
        {
            JsonElement root = doc.RootElement;

            Assert.Equal(1, root.GetArrayLength());
            foreach (var p in root.EnumerateArray())
            {
                string title = p.GetProperty("title").GetString()!;
                string userId = p.GetProperty("userId").GetString()!;

                Assert.Equal("Favourites", title);
                Assert.Equal(id, userId);
            }
        }
    }
}
