﻿using KrMicro.Core.Models.Abstraction;
using KrMicro.Transaction.Constants;
using KrMicro.Transaction.CQS.Commands.Payment;
using KrMicro.Transaction.CQS.Commands.Transaction;
using KrMicro.Transaction.CQS.Queries.Payment;
using KrMicro.Transaction.CQS.Queries.Transaction;
using KrMicro.Transaction.Models;
using KrMicro.Transaction.Services;
using Microsoft.AspNetCore.Mvc;

namespace KrMicro.Transaction.Controllers;

[Route("api/[controller]")]
[ApiController]

public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }


    // GET: api/transaction
    [HttpGet]
    public async Task<ActionResult<GetAllTransactionQueryResult>> GetTransaction()
    {
        return new GetAllTransactionQueryResult(new List<Models.Transaction>(await _transactionService.GetAllAsync()));
    }
    
    // GET: api/Transaction/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetTransactionByIdQueryResult>> GetPayment(short id)
    {
        var item = await _transactionService.GetDetailAsync(item => item.Id == id);

        if (item.Id == null) return BadRequest();

        return new GetTransactionByIdQueryResult(item);
    }
    
    // PATCH: api/Transaction/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPatch("{id}")]
    public async Task<ActionResult<UpdateTransactionCommandResult>> PatchBrand(short id, UpdateTransactionCommandRequest request)
    {
        var item = await _transactionService.GetDetailAsync(x => x.Id == id);
        if (item.Id == null) return BadRequest();
       
        item.CustomerId = request.CustomerId ?? item.CustomerId; 
        item.PhoneNumber = request.PhoneNumber ?? item.CustomerId; 
        item.OrderId = request.OrderId ?? item.CustomerId; 
        item.PaymentId = request.PaymentId ?? item.CustomerId; 
        item.UpdatedAt = DateTimeOffset.UtcNow;
        var result = await _transactionService.UpdateAsync(item);
        return new UpdateTransactionCommandResult(result);
    }
    
    // POST: api/Transaction
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<CreateTransactionCommandResult>> CreateTransaction(CreateTransactionCommandRequest request)
    {
        var newItem = new Models.Transaction
        {
            CustomerId = request.CustomerId,
            PhoneNumber = request.PhoneNumber,
            OrderId = request.OrderId,
            PaymentId = request.PaymentId,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = Status.Disable
        };
        var result = await _transactionService.InsertAsync(newItem);
        return new CreateTransactionCommandResult(result);
    }
    
    // POST: api/Transaction/id
    [HttpPost("{id}/UpdateStatus")]
    public async Task<ActionResult<UpdateTransactionStatusCommandResult>> UpdateStatus(short id,
        UpdateTransactionStatusRequest request)
    {
        var item = await _transactionService.GetDetailAsync(x => x.Id == id);
        if (item.Id == null) return BadRequest();
        item.Status = request.Status;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await _transactionService.UpdateAsync(item);

        return new UpdateTransactionStatusCommandResult(NetworkSuccessResponse.UpdateStatusSuccess);
    }
    
    private async Task<bool> TransactionExists(short id)
    {
        return await _transactionService.CheckExistsAsync(e => e.Id == id);
    }
    
}