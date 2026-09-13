using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanTask.Application.Common.Exceptions;
using CleanTask.Application.Features.Tasks.Commands.CreateTask;
using CleanTask.Domain.Entities;
using CleanTask.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace CleanTask.Application.Tests.Tasks
{
    public class CreateTaskHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateTaskHandler _handler;
        public CreateTaskHandlerTests()
        {
            //Create fake versions of dependencies
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            //Create the real handler with the mocked dependencies
            _handler = new CreateTaskHandler(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsTaskDto()
        {
            // ── Arrange ──────────────────────────────────────────
            var creatorId = Guid.NewGuid();

            var creator = new User
            {
                Id = creatorId,
                FirstName = "Anu",
                LastName = "Madhushani",
                Email = "anu@test.com",
                PasswordHash = "hash"
            };

            var command = new CreateTaskCommand(
                                Title: "Write unit tests",
                                Description: "Add tests to CleanTask API",
                                Priority: Domain.Enums.TaskPriority.High,
                                DueDate: DateTime.UtcNow.AddDays(7),
                                AssignedToUserId: null,
                                CreatedByUserId: creatorId
                            );

            //Tell the fake: when the handler calls GetByIdAsync for the creator, return the creator user
            _unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(creatorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(creator);

            //Tell the fake: when the handler calls AddAsync for a task, return the same task (simulating that it was saved)
            _unitOfWorkMock.Setup(uow => uow.Tasks.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem t, CancellationToken _) => t);

            //Tell the fake: when the handler calls SaveChangesAsync, return 1 (simulating that one record was saved)
            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // ── Act ──────────────────────────────────────────────
            var result = await _handler.Handle(command, CancellationToken.None);

            // ── Assert ───────────────────────────────────────────
            result.Should().NotBeNull();
            result.Title.Should().Be("Write unit tests");
            result.CreatedByUserId.Should().Be(creatorId);
            result.AssignedToUserId.Should().BeNull();
        }

        [Fact]
        public async Task Handle_CreatorNotFound_ThrowsNotFoundException()
        {
            // ── Arrange ──────────────────────────────────────────
            var nonExistentUserId = Guid.NewGuid();

            var command = new CreateTaskCommand(
                Title: "Write unit tests",
                Description: "Test description",
                Priority: Domain.Enums.TaskPriority.Medium,
                DueDate: DateTime.UtcNow.AddDays(3),
                AssignedToUserId: null,
                CreatedByUserId: nonExistentUserId
            );

            // Tell the fake: user does not exist — return null
            _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(nonExistentUserId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((User?)null);

            // ── Act ──────────────────────────────────────────────
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // ── Assert ───────────────────────────────────────────
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_AssigneeNotFound_ThrowsNotFoundException()
        {
            // ── Arrange ──────────────────────────────────────────
            var creatorId = Guid.NewGuid();
            var nonExistentAssigneeId = Guid.NewGuid();

            var creator = new User
            {
                Id = creatorId,
                FirstName = "Anu",
                LastName = "Madhushani",
                Email = "anu@test.com",
                PasswordHash = "hash"
            };

            var command = new CreateTaskCommand(
                Title: "Assigned task",
                Description: "This task has an assignee that does not exist",
                Priority: Domain.Enums.TaskPriority.Low,
                DueDate: DateTime.UtcNow.AddDays(5),
                AssignedToUserId: nonExistentAssigneeId,
                CreatedByUserId: creatorId
            );

            // Creator exists
            _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(creatorId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(creator);

            // Assignee does not exist — return null
            _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(nonExistentAssigneeId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((User?)null);

            // ── Act ──────────────────────────────────────────────
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // ── Assert ───────────────────────────────────────────
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
