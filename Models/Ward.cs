﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InoxThanhNamServer.Models;

[Table("Ward")]
public partial class Ward
{
    [Key]
    public int WardID { get; set; }

    [StringLength(255)]
    public string Name { get; set; }

    public int? DistrictID { get; set; }

    [ForeignKey("DistrictID")]
    [InverseProperty("Wards")]
    public virtual District District { get; set; }

    [InverseProperty("Ward")]
    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
}