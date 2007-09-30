#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Xe.Input
{
    public class ControllerBase
    {
        #region Members
        private IEbiService m_ebiService;
        #endregion

        #region Constructor & Initialization
        public ControllerBase()
        {
        }
        #endregion

        #region Updating
        /// <summary>
        /// Handles Updating.
        /// </summary>
        /// <param name="gameTime">GameTime object that holds the elapsed time from the last update.</param>
        public virtual void Update(GameTime gameTime)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets called when the EbiService property
        /// is set.
        /// </summary>
        protected virtual void OnEbiServiceSet()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the EbiService
        /// </summary>
        public IEbiService EbiService
        {
            get { return m_ebiService; }
            set
            {
                m_ebiService = value;

                OnEbiServiceSet();
            }
        }
        #endregion
    }
}
