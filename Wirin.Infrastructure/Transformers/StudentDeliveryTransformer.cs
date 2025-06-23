using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wirin.Domain.Models;
using Wirin.Infrastructure.Entities;

namespace Wirin.Infrastructure.Transformers;

public class StudentDeliveryTransformer
{
    public static StudentDelivery ToDomain(StudentDeliveryEntity studentDelivery)
    {
        return new StudentDelivery
        {
            Id = studentDelivery.Id,
            StudentId = studentDelivery.StudentId,
            OrderDeliveryId = studentDelivery.OrderDeliveryId,
            CreateDate = studentDelivery.CreateDate
        };
    }

    public static StudentDeliveryEntity ToEntity(StudentDelivery studentDelivery)
    {
        return new StudentDeliveryEntity
        {
            Id = studentDelivery.Id,
            StudentId = studentDelivery.StudentId,
            OrderDeliveryId = studentDelivery.OrderDeliveryId,
            CreateDate = studentDelivery.CreateDate
        };
    }
}
