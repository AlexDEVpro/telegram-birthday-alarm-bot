namespace TelegramBirthdayAlarmBot.Features.Congratulation.Models;

/// <summary>
/// Represents the outcome of evaluating whether a birthday notification should be sent
/// including successful send cases and various skip reasons.
/// </summary>
internal enum SendDecision
{
    SendNormal,
    SendLate,

    SkipAlreadySent,

    SkipTooOld,

    SkipLateTooEarly,
    SkipTodayTooEarly,
    SkipTooEarly
}
