﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InoxThanhNamServer.Models;

public partial class Order
{
    [Key]
    public int OrderID { get; set; }

    public Guid? UserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime OrderDate { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal TotalAmount { get; set; }

    public int Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Order")]
    public virtual ICollection<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();

    [ForeignKey("UserID")]
    [InverseProperty("Orders")]
    public virtual User User { get; set; }
}