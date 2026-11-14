namespace Silo.Application.Features;

public enum GetTagsBySerialsStatusEnum
{
    /// <summary>
    /// فعال
    /// </summary>
    Active = 1,

    /// <summary>
    /// فریز شده
    /// </summary>
    Freezed,

    /// <summary>
    /// مصرف شده
    /// </summary>
    Used,

    /// <summary>
    /// باطل شده
    /// </summary>
    Revoked,

    /// <summary>
    /// خارج شده
    /// </summary>
    Exited
}
