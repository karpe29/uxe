#region License
/*
 *  Xna5D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: November 30, 2006
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

namespace Xe
{
    public class Manager : GameComponent, IManagerService, IService
    {
        #region Members
        // The collection of global services.
        private List<IService> m_services = new List<IService>();

        // Boolean that determines if multiples of the same type are allowed.
        private bool m_allowMultiples = false;

        public event ServiceAddedHandler ServiceAdded;
        public event ServiceRemovedHandler ServiceRemoved;

        // Our reporting service.
        protected IReporterService m_reporter;
        #endregion

        #region Constructor, Destructor
		public Manager(Game game)
            : base(game)
        {
            // Add the instance to the services collection.
            if (game != null)
                game.Services.AddService(typeof(IManagerService), this);
        }

        ~Manager()
        {
            this.Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                m_services.Clear();

                m_services = null;
            }
        }
        #endregion

        #region Initialization
        public override void Initialize()
        {
            base.Initialize();

            // Grab a reference to the reporter object.
            m_reporter = (IReporterService)this.Game.Services.GetService(typeof(IReporterService));
        }
        #endregion

        #region IManagerService Members
        public void LoadSettings(string xmlFile)
        {
            try
            {
                // Check if the file exists.
                if (!File.Exists(xmlFile))
                    throw new FileNotFoundException("Xml settings file could not be found.", xmlFile);

                // Create and load the Xml Document
                XmlDocument _doc = new XmlDocument();
                _doc.Load(xmlFile);

                // Get the first Xml Node
                XmlNode _node = _doc.FirstChild;

                // If it is the Xml declaration, move to the next one.
                if (_node.Name.Equals("xml"))
                    _node = _doc.NextSibling;

                // Loop through the nodes
                foreach (XmlNode _child in _node.ChildNodes)
                {
                    // Loop through the services
                    foreach (IService _service in m_services)
                    {
                        // If the service id matches the node name
                        if (_child.Name.Equals(_service.ID))
                        {
                            // Load the settings
                            _service.LoadSettings(_child);

                            // Break the loop.
                            break;
                        }
                    }
                }
            }
            catch (FileNotFoundException fnf)
            {
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, fnf.Message, fnf);
            }
            catch (NullReferenceException nre)
            {
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, nre.Message, nre);
            }
            catch (Exception e)
            {
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, e.Message, e);
            }
        }

        public void AddService(IService service)
        {
            try
            {
                // Loop through the services and try to find a duplicate.
                // If we find a duplicate and we are not allowing multiples throw an exception.
                foreach(IService _service in m_services)
                {
                    if (_service.GetType().Equals(service))
                        if (_service.ID.Equals(service.ID) || !m_allowMultiples)
                            throw new ServiceFoundException(String.Format("IService object with ID '{0}' was already found or multiples of the same type are not allowed.", service.ID));
                }

                // Add the service.
                m_services.Add(service);

                // Fire the ServiceAdded event.
                if (this.ServiceAdded != null)
                    this.ServiceAdded.Invoke(service);
            }
            catch(ServiceFoundException e)
            {
                // Handle the exception, and broadcast it if we can.
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, e.Message, e);
            }
        }

        public void RemoveService(IService service)
        {
            try
            {
                // Try to remove the service.
                if (m_services.Remove(service))
                {
                    // If successful, fire the ServiceRemoved event.
                    if (this.ServiceRemoved != null)
                        this.ServiceRemoved.Invoke(service);

                    return;
                }

                // If not removed, throw the NotFound exception.
                throw new ServiceNotFoundException(String.Format("IService object with ID '{0}' could not be found.", service.ID));
            }
            catch (ServiceNotFoundException e)
            {
                // Handle the exception, and broadcast it if we can.
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, e.Message, e);
            }
            catch (Exception ex)
            {
                // Handle the exception, and broadcast it if we can.
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, ex.Message, ex);
            }
        }

        public void RemoveService(string id)
        {
            // Loop through the services
            for (int i = 0; i < m_services.Count; i++)
            {
                // If the ID matches...
                if (m_services[i].ID.Equals(id))
                {
                    // Try to remove it.
                    RemoveService(m_services[i]);

                    return;
                }
            }
        }

        public object GetService(string id)
        {
            // Loop through the services, if the id matches...
            // return it.
            foreach (IService _service in m_services)
                if (_service.ID.Equals(id))
                    return _service;

            return null;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Allow multiples of the same type within the collection. Setting
        /// this to true allows for multiples of the same type but not the
        /// same ID.
        /// </summary>
        public bool AllowMultiples
        {
            get
            {
                return m_allowMultiples;
            }
            set
            {
                m_allowMultiples = value;
            }
        }
        #endregion

        #region IService Members
        /// <summary>
        /// Unique ID of the Manager class.
        /// </summary>
        public string ID
        {
            get
            {
                return "Xna5D_Manager";
            }
        }

        public void LoadSettings(XmlNode node)
        {
        }
        #endregion
    }

    #region IManagerService Interface
    public interface IManagerService
    {
        /// <summary>
        /// Load IService Settings from an Xml File.
        /// </summary>
        /// <param name="xmlFile">The Xml file to load.</param>
        void LoadSettings(string xmlFile);

        /// <summary>
        /// Add an IService object to the Manager's collection.
        /// </summary>
        /// <param name="service">The IService object to be added.</param>
        void AddService(IService service);

        /// <summary>
        /// Remove an IService object from the Manager's collection.
        /// </summary>
        /// <param name="service">The IService object to be removed.</param>
        void RemoveService(IService service);

        /// <summary>
        /// Remove an IService object from the Manager's collection.
        /// </summary>
        /// <param name="id">The ID of the IService object to be removed.</param>
        void RemoveService(string id);

        /// <summary>
        /// Get an IService object from the Manager's collection.
        /// </summary>
        /// <param name="id">The ID of the desired IService object.</param>
        /// <returns>An IService object.</returns>
        object GetService(string id);

        /// <summary>
        /// Allow multiples of the same type within the collection. Setting
        /// this to true allows for multiples of the same type but not the
        /// same ID.
        /// </summary>
        bool AllowMultiples { get; set; }

        /// <summary>
        /// ServiceAdded Event: Gets invoked whenever an IService object is
        /// added to the collection.
        /// </summary>
        event ServiceAddedHandler ServiceAdded;

        /// <summary>
        /// ServiceRemoved Event: Gets invoked whenever an IService object is
        /// removed from the collection.
        /// </summary>
        event ServiceRemovedHandler ServiceRemoved;
    }
    #endregion

    #region IService Interface
    public interface IService
    {
        /// <summary>
        /// A unique ID.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Load settings via XML.
        /// </summary>
        /// <param name="node">The appropriate XmlNode.</param>
        void LoadSettings(XmlNode node);
    }
    #endregion

    public delegate void ServiceAddedHandler(IService service);
    public delegate void ServiceRemovedHandler(IService service);

    #region Exceptions
    public class ServiceFoundException : Exception
    {
        public ServiceFoundException()
            : base()
        {
        }

        public ServiceFoundException(string msg)
            : base(msg)
        {
        }
    }

    public class ServiceNotFoundException : Exception
    {
        public ServiceNotFoundException()
            : base()
        {
        }

        public ServiceNotFoundException(string msg)
            : base(msg)
        {
        }
    }
    #endregion
}
