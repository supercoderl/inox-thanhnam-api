﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InoxThanhNamServer.Models;

public partial class ProductReview
{
    [Key]
    public int ReviewID { get; set; }

    public int ProductID { get; set; }

    [StringLength(100)]
    public string ReviewerName { get; set; }

    [Column(TypeName = "ntext")]
    public string ReviewContent { get; set; }

    public int? Rating { get; set; }

    public int? Likes { get; set; }

    public int? Unlikes { get; set; }

    public Guid? UserID { get; set; }

    public Guid? SessionID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReviewDate { get; set; }

    [ForeignKey("ProductID")]
    [InverseProperty("ProductReviews")]
    public virtual Product Product { get; set; }

    [ForeignKey("UserID")]
    [InverseProperty("ProductReviews")]
    public virtual User User { get; set; }
}