﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InoxThanhNamServer.Models;

public partial class User
{
    [Key]
    public Guid UserID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string Username { get; set; }

    [Required]
    [Column(TypeName = "text")]
    public string Password { get; set; }

    [StringLength(255)]
    public string Firstname { get; set; }

    [StringLength(255)]
    public string Lastname { get; set; }

    public int Phone { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("User")]
    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();

    [InverseProperty("User")]
    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();

    [InverseProperty("User")]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}