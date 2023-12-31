﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KrMicro.Core.Models.Abstraction;

namespace KrMicro.Orders.Models;

[Table("Payments")]

public class Payment : BaseModelWithAuditAndTracking
{
    [Required] [Column("Name")] public string Name { get; set; } = string.Empty;

    public List<Transaction> Transactions;
}