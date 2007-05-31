#region License
/*
 *  Xna5D.Data.dll
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
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
#endregion

namespace XeFramework.Data.Collections
{
    public class Map<T> : IDisposable
    {
        #region Item Class
        private class MapItem
        {
            public T ObjectRef;
            public int Index;

            public MapItem()
            {
            }
        }
        #endregion

        #region Members
        private List<MapItem> m_map;
        private bool m_disposed = false;

        public event ItemAddedHandler ItemAdded;
        public event ItemRemovedHandler ItemRemoved;
        #endregion

        #region Constructors
        public Map()
        {
            m_map = null;

            m_map = new List<MapItem>();

            m_disposed = false;

            AddHandlers();
        }
        #endregion

        #region Event Handlers
        protected virtual void AddHandlers()
        {
            this.ItemAdded += new ItemAddedHandler(this.OnItemAdded);
            this.ItemRemoved += new ItemRemovedHandler(this.OnItemRemoved);
        }

        protected virtual void RemoveHandlers()
        {
            if (this.ItemAdded != null)
                this.ItemAdded -= this.OnItemAdded;

            if (this.ItemRemoved != null)
                this.ItemRemoved -= this.OnItemRemoved;
        }

        protected virtual void OnItemRemoved(object item)
        {
        }

        protected virtual void OnItemAdded(object item)
        {
        }
        #endregion

        #region Map Item Management (MIM)
        public void Add(int index, T objectRef)
        {
            try
            {
                foreach (MapItem _item in m_map)
                    if (_item.Index == index)
                        throw new ObjectFoundException(_item);

                MapItem _temp = new MapItem();
                _temp.ObjectRef = objectRef;
                _temp.Index = index;

                m_map.Add(_temp);
            }
            catch
            {
            }
        }

        public void Remove(int index)
        {
            try
            {
                object _temp = null;
                foreach (MapItem _item in m_map)
                {
                    if (_item.Index == index)
                    {
                        m_map.Remove(_item);

                        return;
                    }

                    _temp = _item;
                }

                throw new ObjectNotFoundException(_temp);
            }
            catch
            {
            }
        }

        public void Clear()
        {
            m_map.Clear();
        }
        #endregion

        #region XML
        public XmlNode ToXML()
        {
            return null;
        }
        #endregion

        #region Indexing
        public T this[int index]
        {
            get
            {
                foreach (MapItem _item in m_map)
                    if (_item.Index == index)
                        return (T)_item.ObjectRef;

                return default(T);
            }
            set
            {
                foreach (MapItem _item in m_map)
                {
                    if (_item.Index == index)
                    {
                        _item.ObjectRef = value;

                        return;
                    }
                }

                Add(index, value);
            }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            try
            {
                m_map.Clear();
                m_map = null;

                RemoveHandlers();

                m_disposed = true;
            }
            catch
            {
            }
        }
        #endregion

        #region Properties
        public bool Disposed
        {
            get
            {
                return m_disposed;
            }
        }
        #endregion
    }

    public class Map<I, T> : IDisposable
    {
        #region Item Class
        private class MapItem
        {
            public T ObjectRef;
            public I Index;

            public MapItem()
            {
            }
        }
        #endregion

        #region Members
        private List<MapItem> m_map;
        private bool m_disposed = false;

        public event ItemAddedHandler ItemAdded;
        public event ItemRemovedHandler ItemRemoved;
        #endregion

        #region Constructors
        public Map()
        {
            m_map = null;

            m_map = new List<MapItem>();

            m_disposed = false;

            AddHandlers();
        }
        #endregion

        #region Event Handlers
        protected virtual void AddHandlers()
        {
            this.ItemAdded += new ItemAddedHandler(this.OnItemAdded);
            this.ItemRemoved += new ItemRemovedHandler(this.OnItemRemoved);
        }

        protected virtual void RemoveHandlers()
        {
            if (this.ItemAdded != null)
                this.ItemAdded -= this.OnItemAdded;

            if (this.ItemRemoved != null)
                this.ItemRemoved -= this.OnItemRemoved;
        }

        protected virtual void OnItemRemoved(object item)
        {
        }

        protected virtual void OnItemAdded(object item)
        {
        }
        #endregion

        #region Map Item Management (MIM)
        public void Add(I index, T objectRef)
        {
            try
            {
                foreach (MapItem _item in m_map)
                    if (_item.Index.Equals(index))
                        throw new ObjectFoundException(_item);

                MapItem _temp = new MapItem();
                _temp.ObjectRef = objectRef;
                _temp.Index = index;

                m_map.Add(_temp);

                if (this.ItemAdded != null)
                    this.ItemAdded.Invoke(_temp);
            }
            catch
            {
            }
        }

        public void Remove(I index)
        {
            try
            {
                object _temp = null;
                foreach (MapItem _item in m_map)
                {
                    if (_item.Index.Equals(index))
                    {
                        m_map.Remove(_item);

                        if (this.ItemRemoved != null)
                            this.ItemRemoved.Invoke(_item);

                        return;
                    }

                    _temp = _item;
                }

                throw new ObjectNotFoundException(_temp);
            }
            catch
            {
            }
        }

        public void Clear()
        {
            m_map.Clear();
        }
        #endregion

        #region Indexing
        public T this[I index]
        {
            get
            {
                foreach (MapItem _item in m_map)
                    if (_item.Index.Equals(index))
                        return (T)_item.ObjectRef;

                return default(T);
            }
            set
            {
                foreach (MapItem _item in m_map)
                {
                    if (_item.Index.Equals(index))
                    {
                        _item.ObjectRef = value;

                        return;
                    }
                }

                Add(index, value);
            }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            try
            {
                m_map.Clear();
                m_map = null;

                RemoveHandlers();

                m_disposed = true;
            }
            catch
            {
            }
        }
        #endregion

        #region Properties
        public bool Disposed
        {
            get
            {
                return m_disposed;
            }
        }
        #endregion
    }

    #region Delegates
    public delegate void ItemAddedHandler(object item);
    public delegate void ItemRemovedHandler(object item);
    #endregion
}
