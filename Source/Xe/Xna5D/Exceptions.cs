#region License
/*
 *  Xna5D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: December 04, 2006
 */
#endregion

#region Using Statements
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace XeFramework
{
    #region Exceptions
    public class ObjectFoundException : Exception
    {
        #region Members
        protected object m_object = null;
        #endregion

        #region Default Constructors
        public ObjectFoundException()
            : base("Object was found.")
        {
        }

        public ObjectFoundException(string message)
            : base(message)
        {
        }

        public ObjectFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion

        #region Extra Constructors
        public ObjectFoundException(object objectRef)
            : base("Object was found.")
        {
            m_object = objectRef;
        }

        public ObjectFoundException(object objectRef, string message)
            : base(message)
        {
            m_object = objectRef;
        }

        public ObjectFoundException(object objectRef, string message, Exception innerException)
            : base(message, innerException)
        {
            m_object = objectRef;
        }
        #endregion

        #region Properties
        public object ObjectRef
        {
            get
            {
                return m_object;
            }
        }
        #endregion
    }

    public class ObjectNotFoundException : Exception
    {
        #region Members
        protected object m_object = null;
        #endregion

        #region Default Constructors
        public ObjectNotFoundException()
            : base("Object could not be found.")
        {
        }

        public ObjectNotFoundException(string message)
            : base(message)
        {
        }

        public ObjectNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion

        #region Extra Constructors
        public ObjectNotFoundException(object objectRef)
            : base("Object could not be found.")
        {
            m_object = objectRef;
        }

        public ObjectNotFoundException(object objectRef, string message)
            : base(message)
        {
            m_object = objectRef;
        }

        public ObjectNotFoundException(object objectRef, string message, Exception innerException)
            : base(message, innerException)
        {
            m_object = objectRef;
        }
        #endregion

        #region Properties
        public object ObjectRef
        {
            get
            {
                return m_object;
            }
        }
        #endregion
    }
    #endregion
}
