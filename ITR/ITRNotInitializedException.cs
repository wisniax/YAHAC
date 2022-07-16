using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITR
{
	/// <summary>
	/// Exception thrown when ItemTextureResolver is being used without initialization
	/// </summary>
	public class ITRNotInitializedException : Exception
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public ITRNotInitializedException() : base() { }
	}
}
