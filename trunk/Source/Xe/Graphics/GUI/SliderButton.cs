using System;
using System.Collections.Generic;
using System.Text;
using Xe.GUI;
using Microsoft.Xna.Framework;

namespace Xe.GUI
{
	public class SliderUpButton : UIControl
	{
		SliderUpButton(Game game, IGUIManager guiManager)
			: base(game, guiManager)
		{
			this.ControlTag = "sliderUpButton";
			this.IsTextVisible = false;
		}
	}

	public class SliderDownButton : UIControl
	{
		SliderDownButton(Game game, IGUIManager guiManager)
			: base(game, guiManager)
		{
			this.ControlTag = "sliderDownButton";
			this.IsTextVisible = false;
		}
	}

	public class SliderRightButton : UIControl
	{
		SliderRightButton(Game game, IGUIManager guiManager)
			: base(game, guiManager)
		{
			this.ControlTag = "sliderRightButton";
			this.IsTextVisible = false;
		}
	}

	public class SliderLeftButton : UIControl
	{
		SliderLeftButton(Game game, IGUIManager guiManager)
			: base(game, guiManager)
		{
			this.ControlTag = "sliderLeftButton";
			this.IsTextVisible = false;
		}
	}

	public class SliderVerticalButton : UIControl
	{
		SliderVerticalButton(Game game, IGUIManager guiManager)
			: base(game, guiManager)
		{
			this.ControlTag = "sliderVerticalButton";
			this.IsTextVisible = false;
		}
	}

	public class SliderHorizontalButton : UIControl
	{
		SliderHorizontalButton(Game game, IGUIManager guiManager)
			: base(game, guiManager)
		{
			this.ControlTag = "sliderHorizontalButton";
			this.IsTextVisible = false;
		}
	}
}
