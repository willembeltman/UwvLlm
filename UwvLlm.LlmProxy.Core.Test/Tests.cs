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
        var json = _client.CreateRequestJson(_model, _request, think: true);
        Assert.That(json, Does.Contain("\"think\": true"));
    }

    [Test]
    public void TestCreateRequestJson_WithThinkFalse()
    {
        var json = _client.CreateRequestJson(_model, _request, think: false);
        Assert.That(json, Does.Contain("\"think\": false"));
    }

    [Test]
    public void TestCreateRequestJson_WithThinkNull()
    {
        var json = _client.CreateRequestJson(_model, _request, think: null);
        Assert.That(json, Does.Not.Contain("\"think\":"));
    }

    [TearDown]
    public void TearDown() => _client.Dispose();
}
