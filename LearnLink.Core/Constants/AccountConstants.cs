namespace LearnLink.Core.Constants
{
    public static class AccountConstants
    {
        public static readonly DateTimeOffset DeactivatedLockoutEnd = DateTimeOffset.MaxValue;

        public static readonly DateTimeOffset DeactivationThreshold = new(9000, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public const int MaxFailedAccessAttempts = 5;

        public static readonly TimeSpan FailedAccessLockoutDuration = TimeSpan.FromMinutes(15);
    }
}
