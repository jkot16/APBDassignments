﻿using Tutorial7Task.Contracts.Responses;

namespace Tutorial7Task.Contracts.Responses
{
    public class ClientTripResponse
    {
        public int IdTrip { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo   { get; set; }
        public int MaxPeople     { get; set; }
        public int RegisteredAt  { get; set; }
        public int? PaymentDate  { get; set; }
        public List<CountryResponse> Countries { get; set; } = new();
    }
}