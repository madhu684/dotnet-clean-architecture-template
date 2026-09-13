using CleanTask.Application.Features.Tasks.Commands.CreateTask;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTask.Application.Tests.Tasks
{
    public class CreateTaskValidatorTests
    {
        private readonly CreateTaskValidator _validator;
        public CreateTaskValidatorTests()
        {
            _validator = new CreateTaskValidator();
        }
        [Fact]
        public async void Validate_EmptyTitle_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreateTaskCommand(
                Title: "",
                Description: null,
                Priority: Domain.Enums.TaskPriority.Medium,
                DueDate: null,
                AssignedToUserId: null,
                CreatedByUserId: Guid.NewGuid()
            );

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Title is required.");
        }

        [Fact]
        public async Task Validate_TitleExceeds200Characters_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreateTaskCommand(
                Title: new string('A', 201),
                Description: null,
                Priority: Domain.Enums.TaskPriority.Medium,
                DueDate: null,
                AssignedToUserId: null,
                CreatedByUserId: Guid.NewGuid()
            );

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public async Task Validate_DueDateInPast_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreateTaskCommand(
                Title: "Valid title",
                Description: null,
                Priority: Domain.Enums.TaskPriority.Medium,
                DueDate: DateTime.UtcNow.AddDays(-1),
                AssignedToUserId: null,
                CreatedByUserId: Guid.NewGuid()
            );

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.DueDate);
        }

        [Fact]
        public async Task Validate_ValidCommand_ShouldHaveNoValidationErrors()
        {
            // Arrange
            var command = new CreateTaskCommand(
                Title: "Write unit tests",
                Description: "A valid description",
                Priority: Domain.Enums.TaskPriority.High,
                DueDate: DateTime.UtcNow.AddDays(7),
                AssignedToUserId: null,
                CreatedByUserId: Guid.NewGuid()
            );

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
