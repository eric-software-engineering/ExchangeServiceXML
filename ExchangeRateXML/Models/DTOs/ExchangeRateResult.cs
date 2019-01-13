using System;

namespace ExchangeRateXML.Models.DTOs
{
  [Serializable]
  public class ExchangeRateResult
  {
    public Status Status { get; set; }
    public decimal Value { get; set; }
  }
}