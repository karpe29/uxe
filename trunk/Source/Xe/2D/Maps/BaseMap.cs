//#region License
///*
// *  Xna5D.Graphics2D.dll
// *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
// *  
// *  This software is distributed in the hope that it will be useful,
// *  but WITHOUT ANY WARRANTY; without even the implied warranty of
// *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// * 
// *  Date Created: December 04, 2006
// */
//#endregion License

//#region Using Statements
//using System;
//using System.IO;
//using System.Xml;
//using System.Xml.Schema;
//using System.Collections.Generic;

//using Xna5D;
//using Xna5D.Data;

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
//#endregion

//namespace Xna5D.Graphics2D.Maps
//{
//    public partial class BaseMap : Microsoft.Xna.Framework.DrawableGameComponent
//    {
//        #region Members
//        protected string m_mapName = "MAP";

//        protected int m_tileSize;
//        protected int m_mapWidth, m_mapHeight;

//        protected int m_absX = 0, m_absY = 0;
//        protected int m_dX = 0, m_dY = 0;

//        protected bool m_showWalkable = false;
//        protected bool m_showGrid = false;
//        protected Color m_gridColor = Color.Black;

//        protected Texture2D m_texture;
//        protected string m_texSource;

//        protected ContentManager m_conManager;
//        protected SpriteBatch m_sBatch;

//        protected BaseTile[,] m_map;
//        protected List<Tile> m_tiles = new List<Tile>();
//        #endregion

//        #region Constructors and Destructors
//        public BaseMap(Game game)
//            : base(game)
//        {
//        }

//        ~BaseMap()
//        {
//        }
//        #endregion

//        #region Loading Maps
//        public bool LoadMap(string xmlFile)
//        {
//            try
//            {
//                XmlDocument _doc = new XmlDocument();
//                _doc.Load(xmlFile);

//                XmlNode _node = _doc.FirstChild;
//                while (!_node.Name.Equals("Xna5D_Map"))
//                {
//                    _node = _doc.NextSibling;
//                }

//                if (_node.Name.Equals("Xna5D_Map"))
//                {
//                    int.TryParse(_node.Attributes["width"].Value.ToString(), out m_mapWidth);
//                    int.TryParse(_node.Attributes["height"].Value.ToString(), out m_mapHeight);

//                    int.TryParse(_node.Attributes["tile_size"].Value.ToString(), out m_tileSize);

//                    m_mapName = (_node.Attributes["name"] != null) ? _node.Attributes["name"].Value : "BASE_MAP";

//                    m_map = new BaseTile[m_mapWidth, m_mapHeight];
//                }

//                foreach(XmlNode _curNode in _node.ChildNodes)
//                    LoadXml(_curNode);

//                return true;
//            }
//            catch (NullReferenceException nre)
//            {
//                return false;
//            }
//            catch (Exception e)
//            {
//                return false;
//            }
//        }

//        protected virtual void LoadXml(XmlNode node)
//        {
//            if (node.Name.Equals("Tile"))
//            {
//                LoadTile(node);
//            }
//            else if (node.Name.Equals("Content"))
//            {
//                m_texSource = node.FirstChild.Value.ToString();
//            }
//            else if (node.Name.Equals("Data"))
//            {

//            }

//            foreach (XmlNode _child in node.ChildNodes)
//                LoadXml(_child);
//        }

//        protected virtual void LoadTile(XmlNode node)
//        {
//        }
//        #endregion

//        #region Initializing
//        public override void Initialize()
//        {
//            base.Initialize();
//        }
//        #endregion

//        #region Updating
//        public override void Update(GameTime gameTime)
//        {
//            base.Update(gameTime);
//        }
//        #endregion

//        #region Drawing
//        public override void Draw(GameTime gameTime)
//        {
//            base.Draw(gameTime);
//        }
//        #endregion

//        #region Load / Unload Graphics
//        protected override void LoadGraphicsContent(bool loadAllContent)
//        {
//            base.LoadGraphicsContent(loadAllContent);

//            if (loadAllContent)
//            {
//            }
//        }

//        protected override void UnloadGraphicsContent(bool unloadAllContent)
//        {
//            base.UnloadGraphicsContent(unloadAllContent);

//            if (unloadAllContent)
//            {
//            }
//        }
//        #endregion
//    }

//    public class BaseTile
//    {
//        public bool Walkable = true;
//        public int TileIndex = 0;
//    }

//    public class Tile : BaseTile
//    {
//        public Rectangle Source;
//        public string Name;
//        public string Tag;
//    }
//}


