﻿namespace Wirin.Api.Dtos.Requests;

public class StudentDeliveryRequest
{
    public string StudentId { get; set; }
    public int OrderDeliveryId { get; set; }
    public DateTime? CreateDate { get; set; } = DateTime.UtcNow;
}
