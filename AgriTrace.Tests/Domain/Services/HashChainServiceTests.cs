using AgriTrace.Domain.Services;
using FluentAssertions;

namespace AgriTrace.Tests.Domain.Services;

public class HashChainServiceTests
{
    private readonly HashChainService _sut = new();

    // ── Determinism ──────────────────────────────────────────────────────────
    [Fact]
    public void ComputeHash_SameInputs_ReturnsSameHash()
    {
        var hash1 = _sut.ComputeHash("GENESIS", "event data");
        var hash2 = _sut.ComputeHash("GENESIS", "event data");

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void ComputeHash_DifferentEventData_ReturnsDifferentHash()
    {
        var hash1 = _sut.ComputeHash("GENESIS", "data A");
        var hash2 = _sut.ComputeHash("GENESIS", "data B");

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void ComputeHash_DifferentPreviousHash_ReturnsDifferentHash()
    {
        var hash1 = _sut.ComputeHash("HASH_A", "same data");
        var hash2 = _sut.ComputeHash("HASH_B", "same data");

        hash1.Should().NotBe(hash2);
    }

    // ── Output format ────────────────────────────────────────────────────────
    [Fact]
    public void ComputeHash_ReturnsUppercaseHexString()
    {
        var hash = _sut.ComputeHash("GENESIS", "test");

        // SHA-256 hex = 64 chars
        hash.Should().HaveLength(64);
        hash.Should().MatchRegex("^[0-9A-F]+$");
    }

    [Fact]
    public void ComputeHash_EmptyEventData_ReturnsValidHash()
    {
        var act = () => _sut.ComputeHash("GENESIS", "");
        act.Should().NotThrow();

        var hash = _sut.ComputeHash("GENESIS", "");
        hash.Should().HaveLength(64);
    }

    [Fact]
    public void ComputeHash_NullEventData_ReturnsValidHash()
    {
        // ComputeHash concatenates previousHash + eventData;
        // null eventData becomes the literal "null" in $"{prev}{data}"
        var act = () => _sut.ComputeHash("GENESIS", null!);
        act.Should().NotThrow();
    }

    // ── Chaining ─────────────────────────────────────────────────────────────
    [Fact]
    public void ComputeHash_ChainedHashes_AreConsistent()
    {
        // Simulate a 3-event hash chain
        const string genesis = "GENESIS";
        const string data1 = "event 1";
        const string data2 = "event 2";
        const string data3 = "event 3";

        var hash1 = _sut.ComputeHash(genesis, data1);
        var hash2 = _sut.ComputeHash(hash1, data2);
        var hash3 = _sut.ComputeHash(hash2, data3);

        // Re-verify the chain
        _sut.ComputeHash(genesis, data1).Should().Be(hash1);
        _sut.ComputeHash(hash1, data2).Should().Be(hash2);
        _sut.ComputeHash(hash2, data3).Should().Be(hash3);
    }

    [Fact]
    public void ComputeHash_GenesisKnownValue_IsConsistent()
    {
        // Ensures the hash doesn't change between runs (no random salt)
        var first = _sut.ComputeHash("GENESIS", "harvest");
        var second = _sut.ComputeHash("GENESIS", "harvest");
        first.Should().Be(second);
    }
}
