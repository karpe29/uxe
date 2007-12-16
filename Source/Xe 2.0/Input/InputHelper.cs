using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Xe.Input
{
	public class InputHelper : GameComponent
	{
		public InputHelper(Game game)
			: base(game)
		{
			if (game != null)
				game.Services.AddService(typeof(InputHelper), this);
		}

		#region Variables
#if !XBOX360
		/// <summary>
		/// Mouse state, set every frame in the Update method.
		/// </summary>
		private static MouseState mouseState, mouseStateLastFrame;
#endif

		/// <summary>
		/// Was a mouse detected? Returns true if the user moves the mouse.
		/// On the Xbox 360 there will be no mouse movement and theirfore we
		/// know that we don't have to display the mouse.
		/// </summary>
		private static bool mouseDetected = false;

		/// <summary>
		/// Keyboard state, set every frame in the Update method.
		/// Note: KeyboardState is a class and not a struct,
		/// we have to initialize it here, else we might run into trouble when
		/// accessing any keyboardState data before BaseGame.Update() is called.
		/// We can also NOT use the last state because everytime we call
		/// Keyboard.GetState() the old state is useless (see XNA help for more
		/// information, section Input). We store our own array of keys from
		/// the last frame for comparing stuff.
		/// </summary>
		private static KeyboardState keyboardState =
			Microsoft.Xna.Framework.Input.Keyboard.GetState();

		/// <summary>
		/// Keys pressed last frame, for comparison if a key was just pressed.
		/// </summary>
		private static List<Keys> keysPressedLastFrame = new List<Keys>();

		/// <summary>
		/// GamePads states, set every frame in the Update method.
		/// </summary>
		private static GamePadState[] gamePadState = new GamePadState[4];
		private static GamePadState[] gamePadStateLastFrame = new GamePadState[4];

		/// <summary>
		/// Mouse wheel delta this frame. XNA does report only the total
		/// scroll value, but we usually need the current delta!
		/// </summary>
		/// <returns>0</returns>
		private static int mouseWheelDelta = 0;
#if !XBOX360
		private static int mouseWheelValue = 0;
#endif

		/// <summary>
		/// Start dragging pos, will be set when we just pressed the left
		/// mouse button. Used for the MouseDraggingAmount property.
		/// </summary>
		private static Point startDraggingPos;
		#endregion

		#region Mouse Properties
		/// <summary>
		/// Was a mouse detected? Returns true if the user moves the mouse.
		/// On the Xbox 360 there will be no mouse movement and theirfore we
		/// know that we don't have to display the mouse.
		/// </summary>
		/// <returns>Bool</returns>
		public static bool MouseDetected
		{
			get
			{
				return mouseDetected;
			}
		}

		/// <summary>
		/// Mouse position
		/// </summary>
		/// <returns>Point</returns>
		public static Point MousePos
		{
			get
			{
#if !XBOX360
				return new Point(mouseState.X, mouseState.Y);
#else
                return Point.Zero;
#endif
			}
		}

		/// <summary>
		/// X and y movements of the mouse this frame
		/// </summary>
#if !XBOX360
		private static float mouseXMovement, mouseYMovement;
		private static float lastMouseXMovement, lastMouseYMovement;
#endif

		/// <summary>
		/// Mouse x movement
		/// </summary>
		/// <returns>Float</returns>
		public static float MouseXMovement
		{
			get
			{
#if !XBOX360
				return mouseXMovement;
#else
                return 0;
#endif
			}
		}

		/// <summary>
		/// Mouse y movement
		/// </summary>
		/// <returns>Float</returns>
		public static float MouseYMovement
		{
			get
			{
#if !XBOX360
				return mouseYMovement;
#else
                    return 0;
#endif
			}
		}

		/// <summary>
		/// Mouse has moved in either the X or Y direction
		/// </summary>
		/// <returns>Boolean</returns>
		public static bool HasMouseMoved
		{
			get
			{
#if !XBOX360
				//TODO: Introduce a mouse movement threshold constant
				if (MouseXMovement > 1 || MouseYMovement > 1)
					return true;
#endif
				return false;
			}
		}

		/// <summary>
		/// Mouse left button pressed
		/// </summary>
		/// <returns>Bool</returns>
		public static bool MouseLeftButtonPressed
		{
			get
			{
#if !XBOX360
				return mouseState.LeftButton == ButtonState.Pressed;
#else
                return false;
#endif
			}
		}

		/// <summary>
		/// Mouse right button pressed
		/// </summary>
		/// <returns>Bool</returns>
		public static bool MouseRightButtonPressed
		{
			get
			{
#if !XBOX360
				return mouseState.RightButton == ButtonState.Pressed;
#else
                return false;
#endif
			}
		}

		/// <summary>
		/// Mouse middle button pressed
		/// </summary>
		/// <returns>Bool</returns>
		public static bool MouseMiddleButtonPressed
		{
			get
			{
#if !XBOX360
				return mouseState.MiddleButton == ButtonState.Pressed;
#else
                return false;
#endif
			}
		}

		/// <summary>
		/// Mouse left button just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public static bool MouseLeftButtonJustPressed
		{
			get
			{
#if !XBOX360
				return mouseState.LeftButton == ButtonState.Pressed &&
					   mouseStateLastFrame.LeftButton == ButtonState.Released;
#else
                return false;
#endif
			}
		}

		/// <summary>
		/// Mouse right button just pressed
		/// </summary>
		/// <returns>Bool</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
			Justification = "Makes this class reuseable.")]
		public static bool MouseRightButtonJustPressed
		{
			get
			{
#if !XBOX360
				return mouseState.RightButton == ButtonState.Pressed &&
					   mouseStateLastFrame.RightButton == ButtonState.Released;
#else
                return false;
#endif
			}
		}

		/// <summary>
		/// Mouse dragging amount
		/// </summary>
		/// <returns>Point</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
			Justification = "Makes this class reuseable.")]
		public static Point MouseDraggingAmount
		{
			get
			{
				return new Point(
					startDraggingPos.X - MousePos.X,
					startDraggingPos.Y - MousePos.Y);
			}
		}

		/// <summary>
		/// Reset mouse dragging amount
		/// </summary>
		public static void ResetMouseDraggingAmount()
		{
			startDraggingPos = MousePos;
		}

		/// <summary>
		/// Mouse wheel delta
		/// </summary>
		/// <returns>Int</returns>
		public static int MouseWheelDelta
		{
			get
			{
				return mouseWheelDelta;
			}
		}
		#endregion

		#region Keyboard Properties
		/// <summary>
		/// Keyboard
		/// </summary>
		/// <returns>Keyboard state</returns>
		public static KeyboardState Keyboard
		{
			get
			{
				return keyboardState;
			}
		}

		public static bool IsSpecialKey(Keys key)
		{
			// All keys except A-Z, 0-9 and `-\[];',./= (and space) are special keys.
			// With shift pressed this also results in this keys:
			// ~_|{}:"<>? !@#$%^&*().
			int keyNum = (int)key;
			if ((keyNum >= (int)Keys.A && keyNum <= (int)Keys.Z) ||
				(keyNum >= (int)Keys.D0 && keyNum <= (int)Keys.D9) ||
				key == Keys.Space || // well, space ^^
				key == Keys.OemTilde || // `~
				key == Keys.OemMinus || // -_
				key == Keys.OemPipe || // \|
				key == Keys.OemOpenBrackets || // [{
				key == Keys.OemCloseBrackets || // ]}
				key == Keys.OemQuotes || // '"
				key == Keys.OemQuestion || // /?
				key == Keys.OemPlus) // =+
			{
				return false;
			}

			// Else is is a special key
			return true;
		}

		/// <summary>
		/// Key to char helper conversion method.
		/// Note: If the keys are mapped other than on a default QWERTY
		/// keyboard, this method will not work properly. Most keyboards
		/// will return the same for A-Z and 0-9, but the special keys
		/// might be different.
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>Char</returns>
		public static char KeyToChar(Keys key, bool shiftPressed)
		{
			// If key will not be found, just return space
			char ret = ' ';
			int keyNum = (int)key;
			if (keyNum >= (int)Keys.A && keyNum <= (int)Keys.Z)
			{
				if (shiftPressed)
					ret = key.ToString()[0];
				else
					ret = key.ToString().ToLower()[0];
			}
			else if (keyNum >= (int)Keys.D0 && keyNum <= (int)Keys.D9 &&
				shiftPressed == false)
			{
				ret = (char)((int)'0' + (keyNum - Keys.D0));
			}
			else if (key == Keys.D1 && shiftPressed)
				ret = '!';
			else if (key == Keys.D2 && shiftPressed)
				ret = '@';
			else if (key == Keys.D3 && shiftPressed)
				ret = '#';
			else if (key == Keys.D4 && shiftPressed)
				ret = '$';
			else if (key == Keys.D5 && shiftPressed)
				ret = '%';
			else if (key == Keys.D6 && shiftPressed)
				ret = '^';
			else if (key == Keys.D7 && shiftPressed)
				ret = '&';
			else if (key == Keys.D8 && shiftPressed)
				ret = '*';
			else if (key == Keys.D9 && shiftPressed)
				ret = '(';
			else if (key == Keys.D0 && shiftPressed)
				ret = ')';
			else if (key == Keys.OemTilde)
				ret = shiftPressed ? '~' : '`';
			else if (key == Keys.OemMinus)
				ret = shiftPressed ? '_' : '-';
			else if (key == Keys.OemPipe)
				ret = shiftPressed ? '|' : '\\';
			else if (key == Keys.OemOpenBrackets)
				ret = shiftPressed ? '{' : '[';
			else if (key == Keys.OemCloseBrackets)
				ret = shiftPressed ? '}' : ']';
			else if (key == Keys.OemSemicolon)
				ret = shiftPressed ? ':' : ';';
			else if (key == Keys.OemQuotes)
				ret = shiftPressed ? '"' : '\'';
			else if (key == Keys.OemComma)
				ret = shiftPressed ? '<' : '.';
			else if (key == Keys.OemPeriod)
				ret = shiftPressed ? '>' : ',';
			else if (key == Keys.OemQuestion)
				ret = shiftPressed ? '?' : '/';
			else if (key == Keys.OemPlus)
				ret = shiftPressed ? '+' : '=';

			// Return result
			return ret;
		}

		/// <summary>
		/// Handle keyboard input helper method to catch keyboard input
		/// for an input text. Only used to enter the player name in the game.
		/// </summary>
		/// <param name="inputText">Input text</param>
		public static string HandleKeyboardInput()
		{
			string outputText = "";

			// Is a shift key pressed (we have to check both, left and right)
			bool isShiftPressed =
				keyboardState.IsKeyDown(Keys.LeftShift) ||
				keyboardState.IsKeyDown(Keys.RightShift);

			// Go through all pressed keys
			foreach (Keys pressedKey in keyboardState.GetPressedKeys())
				// Only process if it was not pressed last frame
				if (keysPressedLastFrame.Contains(pressedKey) == false)
				{
					// No special key?
					if (IsSpecialKey(pressedKey) == false &&
						// Max. allow 32 chars
						outputText.Length < 32)
					{
						// Then add the letter to our inputText.
						// Check also the shift state!
						outputText += KeyToChar(pressedKey, isShiftPressed);
					}
					else if (pressedKey == Keys.Back &&
						outputText.Length > 0)
					{
						// Remove 1 character at end
						outputText = outputText.Substring(0, outputText.Length - 1);
					}
				}

			return outputText;
		}

		/// <summary>
		/// Keyboard key just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public static bool KeyboardKeyJustPressed(Keys key)
		{
			return keyboardState.IsKeyDown(key) &&
				keysPressedLastFrame.Contains(key) == false;
		}

		/// <summary>
		/// Keyboard space just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public static bool KeyboardSpaceJustPressed
		{
			get
			{
				return keyboardState.IsKeyDown(Keys.Space) &&
					keysPressedLastFrame.Contains(Keys.Space) == false;
			}
		}

		/// <summary>
		/// Keyboard F1 just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public static bool KeyboardF1JustPressed
		{
			get
			{
				return keyboardState.IsKeyDown(Keys.F1) &&
					keysPressedLastFrame.Contains(Keys.F1) == false;
			}
		}

		/// <summary>
		/// Keyboard escape just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public static bool KeyboardEscapeJustPressed
		{
			get
			{
				return keyboardState.IsKeyDown(Keys.Escape) &&
					keysPressedLastFrame.Contains(Keys.Escape) == false;
			}
		}

		/// <summary>
		/// Keyboard left just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public static bool KeyboardLeftJustPressed
		{
			get
			{
				return keyboardState.IsKeyDown(Keys.Left) &&
					keysPressedLastFrame.Contains(Keys.Left) == false;
			}
		}

		/// <summary>
		/// Keyboard right just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public static bool KeyboardRightJustPressed
		{
			get
			{
				return keyboardState.IsKeyDown(Keys.Right) &&
					keysPressedLastFrame.Contains(Keys.Right) == false;
			}
		}

		/// <summary>
		/// Keyboard up just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public static bool KeyboardUpJustPressed
		{
			get
			{
				return keyboardState.IsKeyDown(Keys.Up) &&
					keysPressedLastFrame.Contains(Keys.Up) == false;
			}
		}

		/// <summary>
		/// Keyboard down just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public static bool KeyboardDownJustPressed
		{
			get
			{
				return keyboardState.IsKeyDown(Keys.Down) &&
					keysPressedLastFrame.Contains(Keys.Down) == false;
			}
		}

		/// <summary>
		/// Keyboard left pressed
		/// </summary>
		/// <returns>Bool</returns>
		public static bool KeyboardLeftPressed
		{
			get
			{
				return keyboardState.IsKeyDown(Keys.Left);
			}
		}

		/// <summary>
		/// Keyboard right pressed
		/// </summary>
		/// <returns>Bool</returns>
		public static bool KeyboardRightPressed
		{
			get
			{
				return keyboardState.IsKeyDown(Keys.Right);
			}
		}

		/// <summary>
		/// Keyboard up pressed
		/// </summary>
		/// <returns>Bool</returns>
		public static bool KeyboardUpPressed
		{
			get
			{
				return keyboardState.IsKeyDown(Keys.Up);
			}
		}

		/// <summary>
		/// Keyboard down pressed
		/// </summary>
		/// <returns>Bool</returns>
		public static bool KeyboardDownPressed
		{
			get
			{
				return keyboardState.IsKeyDown(Keys.Down);
			}
		}
		#endregion

		#region GamePad Properties
		/// <summary>
		/// Game pad
		/// </summary>
		/// <returns>Game pad state</returns>
		public static GamePadState[] GamePad
		{
			get
			{
				return gamePadState;
			}
		}

		static bool[] m_isGamePadConnected = new bool[4];
		/// <summary>
		/// Is game pad connected
		/// </summary>
		/// <returns>Bool</returns>
		public static bool[] IsGamePadConnected
		{
			get
			{
				m_isGamePadConnected[0] = gamePadState[(int)PlayerIndex.One].IsConnected;
				m_isGamePadConnected[1] = gamePadState[(int)PlayerIndex.Two].IsConnected;
				m_isGamePadConnected[2] = gamePadState[(int)PlayerIndex.Three].IsConnected;
				m_isGamePadConnected[3] = gamePadState[(int)PlayerIndex.Four].IsConnected; ;

				return m_isGamePadConnected;
			}
		}

		#endregion

		#region Update
		/// <summary>
		/// Update, called from BaseGame.Update().
		/// Will catch all new states for keyboard, mouse and the gamepad.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
#if XBOX360
            // No mouse support on the XBox360 yet :(
            mouseDetected = false;
#else
			// Handle mouse input variables
			mouseStateLastFrame = mouseState;
			mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();

			// Update mouseXMovement and mouseYMovement
			lastMouseXMovement += mouseState.X - mouseStateLastFrame.X;
			lastMouseYMovement += mouseState.Y - mouseStateLastFrame.Y;
			mouseXMovement = lastMouseXMovement / 2.0f;
			mouseYMovement = lastMouseYMovement / 2.0f;
			lastMouseXMovement -= lastMouseXMovement / 2.0f;
			lastMouseYMovement -= lastMouseYMovement / 2.0f;

			if (MouseLeftButtonPressed == false)
				startDraggingPos = MousePos;
			mouseWheelDelta = mouseState.ScrollWheelValue - mouseWheelValue;
			mouseWheelValue = mouseState.ScrollWheelValue;

			// Check if mouse was moved this frame if it is not detected yet.
			// This allows us to ignore the mouse even when it is captured
			// on a windows machine if just the gamepad or keyboard is used.
			if (mouseDetected == false)// &&
				//always returns false: Microsoft.Xna.Framework.Input.Mouse.IsCaptured)
				mouseDetected = mouseState.X != mouseStateLastFrame.X ||
					mouseState.Y != mouseStateLastFrame.Y ||
					mouseState.LeftButton != mouseStateLastFrame.LeftButton;
#endif

			// Handle keyboard input
			keysPressedLastFrame = new List<Keys>(keyboardState.GetPressedKeys());
			keyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();

			// And finally catch the XBox Controller input
			gamePadStateLastFrame = gamePadState;

			gamePadState[(int)PlayerIndex.One] = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One);
			gamePadState[(int)PlayerIndex.Two] = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.Two);
			gamePadState[(int)PlayerIndex.Three] = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.Three);
			gamePadState[(int)PlayerIndex.Four] = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.Four);
		}
		#endregion
	}
}
