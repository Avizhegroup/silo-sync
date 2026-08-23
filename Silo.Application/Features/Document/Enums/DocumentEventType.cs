namespace Silo.Application.Features;
public enum DocumentEventType
{
    NotSpecified = -1,
    InsertDocument,
    ChangeStatusForward,
    ChangeStatusMasterForward,
    ChangeStatusBackward,
    ChangeStatusMasterBackward,
    Aggregate,
    InsertAggregate,
    RevokeAggregate,
    RemoveAggregate,
    Divide,
    InsertDivide,
    RevokeDivide,
    RemoveDivide,
    RemoveDocument
}
