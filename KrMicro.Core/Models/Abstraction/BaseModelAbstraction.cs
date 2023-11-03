using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KrMicro.Core.Models.Abstraction;

public abstract class BaseModel
{
    [Key] [Column("Id")] public short? Id { get; set; }
}

public abstract class BaseModelWithAudit : BaseModel
{
    [Column("CreatedAt")] public DateTimeOffset? CreatedAt { get; set; }

    [Column("UpdatedAt")] public DateTimeOffset? UpdatedAt { get; set; }
}

public enum Status
{
    Available = 1,
    Disable = 0,
    Deleted = 2
}

public abstract class BaseModelWithTracking : BaseModel
{
    [Column("Status")] public Status Status { get; set; }
}

public abstract class BaseModelWithAuditAndTracking : BaseModel
{
    [Column("CreatedAt")] public DateTimeOffset? CreatedAt { get; set; }

    [Column("UpdatedAt")] public DateTimeOffset? UpdatedAt { get; set; }

    [Column("Status")] public Status? Status { get; set; }
}

public abstract class NoIdWithAuditAndTracking : BaseModel
{
    [Column("CreatedAt")] public DateTimeOffset? CreatedAt { get; set; }

    [Column("UpdatedAt")] public DateTimeOffset? UpdatedAt { get; set; }

    [Column("Status")] public Status? Status { get; set; }
}