using System;
using System.ComponentModel.DataAnnotations;

namespace ExchangeRateXML.Models
{
  [Serializable]
  public class ExchangeRate
  {
    [Required]
    [StringLength(3)]
    public string baseCurrency { get; set; }
    [Required]
    [StringLength(3)]
    public string targetCurrency { get; set; }
    [Required]
    public decimal exchangeRate { get; set; }
    [Required]
    // C# 6: Auto-Property Initializer
    public DateTime timestamp { get; set; } = DateTime.Now;
  }
}
