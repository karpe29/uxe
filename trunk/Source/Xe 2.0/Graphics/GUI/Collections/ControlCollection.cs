#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.Gui
{
    public sealed class ControlCollection
    {
        #region Members
        private List<UIControl> m_masterList = new List<UIControl>();
        private List<UIControl> m_tabList = new List<UIControl>();
        private List<UIControl> m_drawList = new List<UIControl>();
        private List<UIControl> m_updateList = new List<UIControl>();

        private QuadRenderer m_renderer = null;
        private Font2D m_font2d = null;
        private int m_treeLevel;
        #endregion

        #region Events
        public event ControlCollectionHandler ControlAdded;
        public event ControlCollectionHandler ControlRemoved;
        #endregion

        #region Constructors
        public ControlCollection(QuadRenderer renderer, Font2D font2d, int treeLevel)
        {
            m_renderer = renderer;
            m_font2d = font2d;

            m_treeLevel = treeLevel;
        }
        #endregion

        #region Adding & Removing
        /// <summary>
        /// Adds a control to the lists.
        /// </summary>
        /// <param name="control"></param>
        public void Add(UIControl control)
        {
            control.TreeLevel = m_treeLevel + 1;
            control.DrawOrderChanged += new EventHandler(OnDrawOrderChanged);
            control.TabOrderChanged += new EventHandler(OnTabOrderChanged);

            m_masterList.Add(control);
            //m_masterList.Sort();

            m_tabList.Add(control);
            m_tabList.Sort(new TabComparer());

            m_drawList.Add(control);
            m_drawList.Sort(new DrawComparer());

            m_updateList.Add(control);
            m_updateList.Sort(new UpdateComparer());

            if (this.ControlAdded != null)
                this.ControlAdded.Invoke(control);
        }

        private void OnTabOrderChanged(object sender, EventArgs e)
        {
            m_tabList.Sort(new TabComparer());
        }

        private void OnDrawOrderChanged(object sender, EventArgs e)
        {
            m_drawList.Sort(new DrawComparer());
        }

        /// <summary>
        /// Removes a control from the lists.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public bool Remove(UIControl control)
        {
            try
            {
                // Init the done var
                bool _done = true;

                // If we couldn't remove, throw an error.
                if (!m_masterList.Remove(control))
                    _done = false;

                // If we couldn't remove, throw an error.
                if (!m_tabList.Remove(control))
                    _done = false;

                // If we couldn't remove, throw an error.
                if (!m_drawList.Remove(control))
                    _done = false;

                // If we couldn't remove, throw an error.
                if (!m_updateList.Remove(control))
                    _done = false;

                if (this.ControlRemoved != null)
                    this.ControlRemoved.Invoke(control);

                // Throw the error if necessary
                if (!_done)
                    throw new Exception("Could not remove from all lists.");

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Removes a control from the lists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Remove(string name)
        {
            // Search for and remove if possible
            for (int i = 0; i < m_masterList.Count; i++)
                if (m_masterList[i].Name == name)
                    return Remove(m_masterList[i]);

            return false;
        }
        #endregion

        #region Drawing
        public void Draw(GameTime gameTime)
        {
            if (m_renderer == null || m_font2d == null)
                return;

            if (m_renderer.UseBreadthFirst)
                DrawBreadthFirst(gameTime);
            else
                DrawDepthFirst(gameTime);
        }

        private void DrawBreadthFirst(GameTime gameTime)
        {
            // Maintain the current and last
            // tree level so we know when to flush
            // cached data.
            int last = 0;
            int cur = 0;

            // Initialize a queue for traversing the tree.
            Queue<UIControl> queue = new Queue<UIControl>();

            // Add the first set of controls, these should all have
            // level 0.
            for (int i = 0; i < m_drawList.Count; i++)
                if (m_drawList[i].Visible)
                    queue.Enqueue(m_drawList[i]);

            // While there are still controls to draw..
            while (queue.Count > 0)
            {
                // Get the next control
                UIControl control = queue.Dequeue();

                // Store the current tree level
                cur = control.TreeLevel;

                // If the current and previous level are not the same,
                // we need to flush out the drawing.
                if (cur != last)
                {
                    // Reset the tree level
                    last = cur;

                    // Flush out all current data.
                    Flush();
                }

                // Cache the data in the control
                control.Draw(gameTime);

                // Add the controls children onto the back
                // of the queue.
                for (int i = 0; i < control.Controls.DrawList.Count; i++)
                    if (control.Controls.DrawList[i].Visible)
                        queue.Enqueue(control.Controls.DrawList[i]);
            }
        }

        private void DrawDepthFirst(GameTime gameTime)
        {
            for (int i = 0; i < m_drawList.Count; i++)
            {
                // Draw the control.
                m_drawList[i].Draw(gameTime);

                // Draw children controls.
                m_drawList[i].Controls.Draw(gameTime);
            }
        }

        private void Flush()
        {
            // Flush out the background data.
            m_renderer.Flush();

            // Flush out font data.
            m_font2d.Flush();
        }
        #endregion

        #region Updating
        public void Update(GameTime gameTime)
        {
            // Update in order
            for (int i = 0; i < m_updateList.Count; i++)
                if (m_updateList[i].Enabled)
                    m_updateList[i].Update(gameTime);
        }
        #endregion

        #region Lists
        /// <summary>
        /// Gets the Master list with no order.
        /// </summary>
        public List<UIControl> MasterList
        {
            get { return m_masterList; }
            set { m_masterList = value; }
        }

        /// <summary>
        /// Gets the Tab list in order of lowest to
        /// highest tab order.
        /// </summary>
        public List<UIControl> TabList
        {
            get { return m_tabList; }
            set { m_tabList = value; }
        }

        /// <summary>
        /// Gets the Draw list in order of lowest
        /// to highest draw order.
        /// </summary>
        public List<UIControl> DrawList
        {
            get { return m_drawList; }
            set { m_drawList = value; }
        }

        /// <summary>
        /// Gets the Update list in order of lowest
        /// to highest update order.
        /// </summary>
        public List<UIControl> UpdateList
        {
            get { return m_updateList; }
            set { m_updateList = value; }
        }
        #endregion
    }

    public delegate void ControlCollectionHandler(UIControl control);
}
