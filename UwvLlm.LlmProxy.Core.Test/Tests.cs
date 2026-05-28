using UwvLlm.Infrastructure.Llm.Clients;
using UwvLlm.Infrastructure.Llm.Enums;
using UwvLlm.Infrastructure.Llm.Models;

namespace UwvLlm.LlmProxy.Core.Test;

public class Tests
{
    private OllamaClient _client = null!; 
    private Model _model = null!;
    private LlmRequest _request = null!;

    [SetUp]
    public void Setup()
    {
        _client = new OllamaClient();
        _model = new Model("test-model", 0, 4096, DateTime.UtcNow);
        var messages = new[] { new Message(Role.User, null, "Hello", null, null) };
        _request = new LlmRequest(messages, Array.Empty<Tool>());
    }

    [Test]
    public void TestCreateRequestJson_WithThinkTrue()
    {
        var json = _client.CreateRequestJson(_model, _request, new LlmOptions { Think = true });
        Assert.That(json, Does.Contain("\"think\": true"));
    }

    [Test]
    public void TestCreateRequestJson_WithThinkFalse()
    {
        var json = _client.CreateRequestJson(_model, _request, new LlmOptions { Think = false });
        Assert.That(json, Does.Contain("\"think\": false"));
    }

    [Test]
    public void TestCreateRequestJson_WithThinkNull()
    {
        var json = _client.CreateRequestJson(_model, _request, new LlmOptions { Think = null });
        Assert.That(json, Does.Not.Contain("\"think\":"));
    }

    [Test]
    public void TestCreateRequestJson_WithOptionsObject()
    {
        var options = new LlmOptions
        {
            Think = false,
            Temperature = 0.7,
            NumCtx = 2048,
            Seed = 42,
            Stop = new[] { "\n", "User:" }
        };

        var json = _client.CreateRequestJson(_model, _request, options);

        Assert.That(json, Does.Contain("\"think\": false"));
        Assert.That(json, Does.Contain("\"temperature\": 0.7"));
        Assert.That(json, Does.Contain("\"num_ctx\": 2048"));
        Assert.That(json, Does.Contain("\"seed\": 42"));
        Assert.That(json, Does.Contain("\"stop\": [\"\\n\", \"User:\"]"));
    }

    [TearDown]
    public void TearDown() => _client.Dispose();
}
