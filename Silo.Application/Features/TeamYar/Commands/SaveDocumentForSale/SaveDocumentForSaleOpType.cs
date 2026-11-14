namespace Silo.Application.Features;

public enum SaveDocumentForSaleOpType
{
    /// <summary>
    /// عملیات ذخیره
    /// </summary>
    Save = 1,

    /// <summary>
    /// عملیات بروزرسانی
    /// </summary>
    Update,

    /// <summary>
    /// ابطال
    /// </summary>
    Revoke
}
