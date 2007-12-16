using System;
using System.Collections.Generic;
using System.Text;
using Xe.Gui;
using Microsoft.Xna.Framework;

namespace Xe.Gui
{
	public class SliderUpButton : UIControl
	{
		public SliderUpButton(Game game, IGuiManager guiManager)
			: base(game, guiManager)
		{
			this.ControlTag = "sliderUpButton";
			this.IsTextVisible = false;
		}
	}

	public class SliderDownButton : UIControl
	{
		public SliderDownButton(Game game, IGuiManager guiManager)
			: base(game, guiManager)
		{
			this.ControlTag = "sliderDownButton";
			this.IsTextVisible = false;
		}
	}

	public class SliderRightButton : UIControl
	{
		public SliderRightButton(Game game, IGuiManager guiManager)
			: base(game, guiManager)
		{
			this.ControlTag = "sliderRightButton";
			this.IsTextVisible = false;
		}
	}

	public class SliderLeftButton : UIControl
	{
		public SliderLeftButton(Game game, IGuiManager guiManager)
			: base(game, guiManager)
		{
			this.ControlTag = "sliderLeftButton";
			this.IsTextVisible = false;
		}
	}

	public class SliderVerticalButton : UIControl
	{
		public SliderVerticalButton(Game game, IGuiManager guiManager)
			: base(game, guiManager)
		{
			this.ControlTag = "sliderVerticalButton";
			this.IsTextVisible = false;
		}
	}

	public class SliderHorizontalButton : UIControl
	{
		public SliderHorizontalButton(Game game, IGuiManager guiManager)
			: base(game, guiManager)
		{
			this.ControlTag = "sliderHorizontalButton";
			this.IsTextVisible = false;
		}
	}
}
