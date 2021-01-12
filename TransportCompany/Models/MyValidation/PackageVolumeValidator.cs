using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransportCompany.Models
{
    public class PackageVolumeValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var package = (Package)validationContext.ObjectInstance;
            int volume = package.Volume;
            //Volumul trebuie sa fie >= 1
            if(volume < 1 || volume > 100)
            {
                return new ValidationResult("The volume must be greater than 0 and lesser than 100");
            }

            //Intre 1 si 5 se accepta orice
            if(volume >=1 && volume <=5)
            {
                return ValidationResult.Success;
            }

            //Daca este pana in 20 trebuie sa fie numar par
            if(volume <=20)
            {
                if(volume%2 == 0)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("The volume must be even if between 5 and 20");
                }
            }

            //Altfel trebuie sa fie divizibil cu 5
            if(volume%5 == 0)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("The volume must be divisible with 5 if between 21 and 100");
            }
        }
    }
}