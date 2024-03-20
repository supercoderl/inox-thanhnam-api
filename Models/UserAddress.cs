﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InoxThanhNamServer.Models;

[Table("UserAddress")]
public partial class UserAddress
{
    [Key]
    public int AddressID { get; set; }

    public Guid? UserID { get; set; }

    [Column(TypeName = "ntext")]
    public string Address { get; set; }

    public int? CityID { get; set; }

    public int? DistrictID { get; set; }

    public int? WardID { get; set; }

    [ForeignKey("CityID")]
    [InverseProperty("UserAddresses")]
    public virtual City City { get; set; }

    [ForeignKey("DistrictID")]
    [InverseProperty("UserAddresses")]
    public virtual District District { get; set; }

    [ForeignKey("UserID")]
    [InverseProperty("UserAddresses")]
    public virtual User User { get; set; }

    [ForeignKey("WardID")]
    [InverseProperty("UserAddresses")]
    public virtual Ward Ward { get; set; }
}