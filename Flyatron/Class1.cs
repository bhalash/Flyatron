using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Flyatron
{
	class Drawable
	{
		int x;
		int y;
		Texture2D;
		Rectangle rectanglePosition;

		public void Update(KeyboardState keyboardState)
		{
		}

		public void Draw(Rectangle cameraRect)
		{
			if (rectanglePosition.Intersects(cameraRect))
			{
# Draw me
			}
		}
	}

	class Mine : Drawable
	{

	}

	class Player : Drawable
	{
		public void Update(keyboardState keyboardState
	}
}
