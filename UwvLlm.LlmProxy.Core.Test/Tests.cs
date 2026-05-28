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

    [Test]
    public void TestGeminiRequestJson_WithSystemPromptAndOptions()
    {
        using var gemini = new GeminiClient("fake-key");
        var messages = new[]
        {
            new Message(Role.System, null, "You are a teacher", null, null),
            new Message(Role.User, null, "Hello", null, null)
        };
        var request = new LlmRequest(messages, Array.Empty<Tool>());
        var options = new LlmOptions
        {
            Temperature = 0.8,
            NumPredict = 150,
            Stop = new[] { "STOP" }
        };

        var json = gemini.CreateRequestJson(_model, request, options);

        // System prompt should go to system_instruction
        Assert.That(json, Does.Contain("\"system_instruction\""));
        Assert.That(json, Does.Contain("\"text\": \"You are a teacher\""));

        // Generation config assertions
        Assert.That(json, Does.Contain("\"temperature\": 0.8"));
        Assert.That(json, Does.Contain("\"maxOutputTokens\": 150"));
        Assert.That(json, Does.Contain("\"stopSequences\": [\"STOP\"]"));

        // contents should NOT contain system message
        Assert.That(json, Does.Not.Contain("\"role\": \"system\""));
    }

    [Test]
    public void TestGeminiRequestJson_WithTools()
    {
        using var gemini = new GeminiClient("fake-key");
        var tools = new[]
        {
            new Tool("my-func", "a test function", new[] { new ToolParameter("arg1", "string", "an argument") })
        };
        var request = new LlmRequest(_request.Messages, tools);

        var json = gemini.CreateRequestJson(_model, request);

        // Tools structure assertions
        Assert.That(json, Does.Contain("\"tools\""));
        Assert.That(json, Does.Contain("\"function_declarations\""));
        Assert.That(json, Does.Contain("\"name\": \"my-func\""));
        Assert.That(json, Does.Contain("\"description\": \"a test function\""));
    }

    [TearDown]
    public void TearDown() => _client.Dispose();
}
