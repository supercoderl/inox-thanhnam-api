﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InoxThanhNamServer.Models;

[Table("ProductImage")]
public partial class ProductImage
{
    [Key]
    public int ImageID { get; set; }

    [StringLength(255)]
    public string ImageName { get; set; }

    [Required]
    [Column(TypeName = "text")]
    public string ImageURL { get; set; }

    public int? ProductID { get; set; }

    [Column(TypeName = "text")]
    public string Description { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("ProductID")]
    [InverseProperty("ProductImages")]
    public virtual Product Product { get; set; }
}