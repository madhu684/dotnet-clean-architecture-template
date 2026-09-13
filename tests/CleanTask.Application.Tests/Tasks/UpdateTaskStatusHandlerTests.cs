using CleanTask.Application.Common.Exceptions;
using CleanTask.Application.Features.Tasks.Commands.UpdateTaskStatus;
using CleanTask.Domain.Entities;
using CleanTask.Domain.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTask.Application.Tests.Tasks
{
    public class UpdateTaskStatusHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateTaskStatusHandler _handler;

        public UpdateTaskStatusHandlerTests() {
            //Create fake versions of dependencies
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            //Create the real handler with the mocked dependencies
            _handler = new UpdateTaskStatusHandler(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_UpdatesStatusAndReturnsDto()
        {
            // ── Arrange ──────────────────────────────────────────
            var taskId = Guid.NewGuid();

            var existingTask = new TaskItem
            {
                Id = taskId,
                Title = "Write unit tests",
                Status = Domain.Enums.TaskStatus.Todo,
                Priority = Domain.Enums.TaskPriority.High,
                CreatedByUserId = Guid.NewGuid()
            };

            var command = new UpdateTaskStatusCommand(
                TaskId: taskId,
                NewStatus: Domain.Enums.TaskStatus.InProgress
            );

            // Tell the fake: return the existing task when queried
            _unitOfWorkMock.Setup(u => u.Tasks.GetByIdWithDetailsAsync(taskId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(existingTask);

            _unitOfWorkMock.Setup(u => u.Tasks.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(1);

            // ── Act ──────────────────────────────────────────────
            var result = await _handler.Handle(command, CancellationToken.None);

            // ── Assert ───────────────────────────────────────────
            result.Should().NotBeNull();
            result.Status.Should().Be(Domain.Enums.TaskStatus.InProgress);
            result.Title.Should().Be("Write unit tests");

            // Verify UpdateAsync was actually called once
            _unitOfWorkMock.Verify(u => u.Tasks.UpdateAsync(
                It.IsAny<TaskItem>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_TaskNotFound_ThrowsNotFoundException()
        {
            // ── Arrange ──────────────────────────────────────────
            var nonExistentTaskId = Guid.NewGuid();

            var command = new UpdateTaskStatusCommand(
                TaskId: nonExistentTaskId,
                NewStatus: Domain.Enums.TaskStatus.InProgress
            );

            // Tell the fake: task does not exist — return null
            _unitOfWorkMock.Setup(u => u.Tasks.GetByIdWithDetailsAsync(
                nonExistentTaskId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem?)null);

            // ── Act ──────────────────────────────────────────────
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // ── Assert ───────────────────────────────────────────
            await act.Should().ThrowAsync<NotFoundException>();

            // Verify UpdateAsync was NEVER called — no point updating what doesn't exist
            _unitOfWorkMock.Verify(u => u.Tasks.UpdateAsync(
                It.IsAny<TaskItem>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
