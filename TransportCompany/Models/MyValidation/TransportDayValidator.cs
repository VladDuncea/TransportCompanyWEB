using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransportCompany.Models
{
    public class TransportDayValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var transport = (Transport)validationContext.ObjectInstance;
            int day = transport.TransportDay;
            int? month = transport.TransportMonth;
            int?[] luni_30 = { 4, 6, 9, 11};
            
            if(month == null || month == 0)
            {
                return ValidationResult.Success;
            }

            //Verificare februarie
            if(month == 2 && day > 29)
            {
                return new ValidationResult("February has at most 29 days!");
            }

            if(luni_30.Contains(month) && day >30)
            {
                return new ValidationResult("Given month has only 30 days!");
            }

            return ValidationResult.Success;
        }
    }
}