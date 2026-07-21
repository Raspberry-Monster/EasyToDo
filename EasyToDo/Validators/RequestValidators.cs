using EasyToDo.Models.DTO.Queries;
using EasyToDo.Models.DTO.Requests;
using FluentValidation;
using static EasyToDo.Validators.ValidationRules;

namespace EasyToDo.Validators
{
    public sealed class TaskItemCreateRequestValidator : AbstractValidator<TaskItemCreateRequest>
    {
        public TaskItemCreateRequestValidator()
        {
            RuleFor(request => request.Title).NotEmpty().MaximumLength(200);
            RuleFor(request => request.ListId).NotNull().Must(listId => listId.HasValue && listId.Value != Guid.Empty).WithMessage("ListId must be a valid GUID.");
            RuleFor(request => request.StartAt).Must(IsUtcOrNull).WithMessage("StartAt must be a UTC timestamp.");
            RuleFor(request => request.DueAt).Must(IsUtcOrNull).WithMessage("DueAt must be a UTC timestamp.");
            RuleFor(request => request.DueAt).Must((request, dueAt) => HasValidDateRange(request.StartAt, dueAt)).WithMessage("DueAt must be later than or equal to StartAt.");
        }
    }

    public sealed class SubTaskCreateRequestValidator : AbstractValidator<SubTaskCreateRequest>
    {
        public SubTaskCreateRequestValidator()
        {
            RuleFor(request => request.Title).NotEmpty().MaximumLength(200);
            RuleFor(request => request.StartAt).Must(IsUtcOrNull).WithMessage("StartAt must be a UTC timestamp.");
            RuleFor(request => request.DueAt).Must(IsUtcOrNull).WithMessage("DueAt must be a UTC timestamp.");
            RuleFor(request => request.DueAt).Must((request, dueAt) => HasValidDateRange(request.StartAt, dueAt)).WithMessage("DueAt must be later than or equal to StartAt.");
        }
    }

    public sealed class TaskItemUpdateRequestValidator : AbstractValidator<TaskItemUpdateRequest>
    {
        public TaskItemUpdateRequestValidator()
        {
            RuleFor(request => request.Title).NotEmpty().MaximumLength(200);
            RuleFor(request => request.ListId).NotNull().Must(listId => listId.HasValue && listId.Value != Guid.Empty).WithMessage("ListId must be a valid GUID.");
            RuleFor(request => request.Status).NotNull().Must(status => status.HasValue && Enum.IsDefined(status.Value)).WithMessage("Status must be a valid value.");
            RuleFor(request => request.Priority).NotNull().Must(priority => priority.HasValue && Enum.IsDefined(priority.Value)).WithMessage("Priority must be a valid value.");
            RuleFor(request => request.Progress).Must(progress => !progress.HasValue || progress is >= 0 and <= 100).WithMessage("Progress must be between 0 and 100.");
            RuleFor(request => request.StartAt).Must(IsUtcOrNull).WithMessage("StartAt must be a UTC timestamp.");
            RuleFor(request => request.DueAt).Must(IsUtcOrNull).WithMessage("DueAt must be a UTC timestamp.");
            RuleFor(request => request.DueAt).Must((request, dueAt) => HasValidDateRange(request.StartAt, dueAt)).WithMessage("DueAt must be later than or equal to StartAt.");
        }
    }

    public sealed class TaskItemStatusUpdateRequestValidator : AbstractValidator<TaskItemStatusUpdateRequest>
    {
        public TaskItemStatusUpdateRequestValidator()
        {
            RuleFor(request => request.Status).NotNull().Must(status => status.HasValue && Enum.IsDefined(status.Value)).WithMessage("Status must be a valid value.");
        }
    }

    public sealed class TaskItemProgressUpdateRequestValidator : AbstractValidator<TaskItemProgressUpdateRequest>
    {
        public TaskItemProgressUpdateRequestValidator()
        {
            RuleFor(request => request.Progress).NotNull().Must(progress => progress is >= 0 and <= 100).WithMessage("Progress must be between 0 and 100.");
        }
    }

    public sealed class TaskListCreateRequestValidator : AbstractValidator<TaskListCreateRequest>
    {
        public TaskListCreateRequestValidator()
        {
            RuleFor(request => request.Name).NotEmpty().MaximumLength(128);
            RuleFor(request => request.Color).NotEmpty().MaximumLength(20);
        }
    }

    public sealed class TaskListUpdateRequestValidator : AbstractValidator<TaskListUpdateRequest>
    {
        public TaskListUpdateRequestValidator()
        {
            RuleFor(request => request.Name).NotEmpty().MaximumLength(128);
            RuleFor(request => request.Color).NotEmpty().MaximumLength(20);
        }
    }

    public sealed class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
    {
        public UserRegisterRequestValidator()
        {
            RuleFor(request => request.Username).NotEmpty().MaximumLength(50);
            RuleFor(request => request.Nickname).NotEmpty().MaximumLength(50);
            RuleFor(request => request.Password).NotEmpty().MinimumLength(8).MaximumLength(128);
        }
    }

    public sealed class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
    {
        public UserLoginRequestValidator()
        {
            RuleFor(request => request.Username).NotEmpty().MaximumLength(50);
            RuleFor(request => request.Password).NotEmpty().MaximumLength(128);
        }
    }

    public sealed class UserProfileUpdateRequestValidator : AbstractValidator<UserProfileUpdateRequest>
    {
        public UserProfileUpdateRequestValidator()
        {
            RuleFor(request => request.Nickname).NotEmpty().MaximumLength(50);
        }
    }

    public sealed class UserChangePasswordRequestValidator : AbstractValidator<UserChangePasswordRequest>
    {
        public UserChangePasswordRequestValidator()
        {
            RuleFor(request => request.OldPassword).NotEmpty().MaximumLength(128);
            RuleFor(request => request.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(128).NotEqual(request => request.OldPassword);
        }
    }

    public sealed class CalendarTaskQueryValidator : AbstractValidator<CalendarTaskQuery>
    {
        public CalendarTaskQueryValidator()
        {
            RuleFor(query => query.StartAt).NotNull();
            RuleFor(query => query.EndAt).NotNull();
            RuleFor(query => query.StartAt).Must(IsUtcOrNull).WithMessage("StartAt must be a UTC timestamp.");
            RuleFor(query => query.EndAt).Must(IsUtcOrNull).WithMessage("EndAt must be a UTC timestamp.");
            RuleFor(query => query.EndAt).Must((query, endAt) => query.StartAt.HasValue && endAt.HasValue && query.StartAt <= endAt).WithMessage("EndAt must be later than or equal to StartAt.");
        }
    }

    internal static class ValidationRules
    {
        internal static bool HasValidDateRange(DateTime? startAt, DateTime? dueAt) => !startAt.HasValue || !dueAt.HasValue || startAt <= dueAt;

        internal static bool IsUtcOrNull(DateTime? value) => !value.HasValue || value.Value.Kind == DateTimeKind.Utc;
    }
}
