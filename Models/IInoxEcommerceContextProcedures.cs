﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using InoxThanhNamServer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace InoxThanhNamServer.Models
{
    public partial interface IInoxEcommerceContextProcedures
    {
        Task<int> sp_delete_productAsync(int? ProductID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<int> sp_delete_userAsync(Guid? UserID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    }
}
