using API.DTOs;
using Core.Entities;

namespace API.Extentions
{
    public static class AddressMappingExtentions
    {
        public static AddressDto? toDto(this Address? address)
        {
           if(address == null) return null;

            return new AddressDto
            {
                Line1 = address.Line1,
                Line2 = address.Line2,
                City = address.City,
                State = address.State,
                Postal_code = address.Postal_code,
                Country = address.Country
            };
        }

          public static Address toEntity(this AddressDto addressDto)
        {
            if(addressDto == null) throw new ArgumentNullException(nameof(addressDto));

            return new Address
            {
                Line1 = addressDto.Line1,
                Line2 = addressDto.Line2,
                City = addressDto.City,
                State = addressDto.State,
                Postal_code = addressDto.Postal_code,
                Country = addressDto.Country
            };
        }

         public static void UpdateFromDto(this Address address,AddressDto addressDto)
        {
            if(addressDto == null) throw new ArgumentNullException(nameof(addressDto));
            if(address == null) throw new ArgumentNullException(nameof(address));
                address.Line1 = addressDto.Line1;
                address.Line2 = addressDto.Line2;
                address.City = addressDto.City;
                address.State = addressDto.State;
                address.Postal_code = addressDto.Postal_code;
                address.Country = addressDto.Country;


         
                
           
        }
    }
}