using System;

namespace ch.appl.psoft.Interface
{
    /// <summary>
    /// Summary description for ExchangeException.
    /// </summary>
    public class ExchangeException : Exception
	{
		public ExchangeException(string message) : base (message) 
		{
		}
	}
}
