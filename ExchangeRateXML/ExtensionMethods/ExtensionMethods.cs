using System;
using System.Diagnostics;

namespace ExchangeRateXML.ExtensionMethods
{
  public static class ExtensionMethods
  {
    public static void TraceException(this Exception _this)
    {
      Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, _this);
    }
  }
}