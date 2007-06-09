#region License
/*
 *  Xna5D.Net.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 */
#endregion

#region Using Statements
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
#endregion

namespace Xe.Net
{
    public class HostNotFoundException : Exception
    {
        public HostNotFoundException(string host)
            :base(String.Format("Host ({0}) could not be found.", host))
        {
        }
    }
}
