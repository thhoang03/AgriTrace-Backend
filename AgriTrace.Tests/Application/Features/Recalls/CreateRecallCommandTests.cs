using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Features.Recalls.Commands;
using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.Recalls;
using AgriTrace.Domain.Entities.Users;
using AgriTrace.Domain.Interfaces.Inbound;
using FluentAssertions;
using Moq;

namespace AgriTrace.Tests.Application.Features.Recalls;

public class CreateRecallCommandTests
{
    private static Batch BuildBatch()
    {
        return new Batch(
            Guid.NewGuid(),
            "B001",
            10,
            Guid.NewGuid(),
            DateTime.UtcNow,
            null);
    }

    private static Mock<ICurrentUserService> BuildCurrentUser(string role = "Admin", string? orgType = "SYSTEM")
    {
        var mock = new Mock<ICurrentUserService>();
        mock.Setup(c => c.Role).Returns(role);
        mock.Setup(c => c.IsAuthenticated).Returns(true);
        mock.Setup(c => c.OrganizationId).Returns(Guid.NewGuid());
        mock.Setup(c => c.OrganizationType).Returns(orgType);
        return mock;
    }

    [Fact]
    public async Task Handle_Manager_ThrowsRbacForbiddenException()
    {
        var batch = BuildBatch();
        var batchMock = new Mock<IBatchReadService>();
        batchMock.Setup(s => s.GetByIdAsync(batch.Id, default))
            .ReturnsAsync(batch);

        var writeMock = new Mock<IBatchWriteService>();
        var recallMock = new Mock<IRecallService>();
        var userMock = new Mock<IUserService>();

        var sut = new CreateRecallCommandHandler(recallMock.Object, batchMock.Object, writeMock.Object, userMock.Object, BuildCurrentUser(role: "Manager").Object);
        var cmd = new CreateRecallCommand(batch.Id, "Defect", 2, Guid.NewGuid());

        var act = () => sut.Handle(cmd, default);

        await act.Should().ThrowAsync<RbacForbiddenException>()
            .WithMessage("*system administrator*");
    }

    [Fact]
    public async Task Handle_AdminNonSystem_ThrowsRbacForbiddenException()
    {
        var batch = BuildBatch();
        var batchMock = new Mock<IBatchReadService>();
        batchMock.Setup(s => s.GetByIdAsync(batch.Id, default))
            .ReturnsAsync(batch);

        var writeMock = new Mock<IBatchWriteService>();
        var recallMock = new Mock<IRecallService>();
        var userMock = new Mock<IUserService>();

        var sut = new CreateRecallCommandHandler(recallMock.Object, batchMock.Object, writeMock.Object, userMock.Object, BuildCurrentUser(role: "Admin", orgType: "FARM").Object);
        var cmd = new CreateRecallCommand(batch.Id, "Defect", 2, Guid.NewGuid());

        var act = () => sut.Handle(cmd, default);

        await act.Should().ThrowAsync<RbacForbiddenException>()
            .WithMessage("*system administrator*");
    }

    [Fact]
    public async Task Handle_AdminSystem_ValidCommand_CreatesRecall()
    {
        var batch = BuildBatch();
        var batchMock = new Mock<IBatchReadService>();
        batchMock.Setup(s => s.GetByIdAsync(batch.Id, default))
            .ReturnsAsync(batch);

        var writeMock = new Mock<IBatchWriteService>();
        var recallMock = new Mock<IRecallService>();
        recallMock.Setup(s => s.CreateAsync(It.IsAny<Recall>(), default))
            .ReturnsAsync((Recall r, CancellationToken _) => r);
        var userMock = new Mock<IUserService>();

        var sut = new CreateRecallCommandHandler(recallMock.Object, batchMock.Object, writeMock.Object, userMock.Object, BuildCurrentUser().Object);
        var cmd = new CreateRecallCommand(batch.Id, "Defect", 2, Guid.NewGuid());

        var result = await sut.Handle(cmd, default);

        result.Should().NotBeNull();
        recallMock.Verify(s => s.CreateAsync(It.IsAny<Recall>(), default), Times.Once);
        writeMock.Verify(s => s.UpdateAsync(It.IsAny<Batch>(), default), Times.Once);
    }
}
