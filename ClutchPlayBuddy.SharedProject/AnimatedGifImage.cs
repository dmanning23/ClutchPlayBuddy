using GameTimer;
using MenuBuddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameScreenTools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClutchPlayBuddy
{
	/// <summary>
	/// This is a MenuBuddy image widget that displays a series of frames.
	/// </summary>
	public class AnimatedGifImage : Image, IDisposable
	{
		#region Properties

		List<ImageData> Images { get; set; }

		int CurrentIndex { get; set; }

		CountdownTimer Timer { get; set; }

		#endregion //Properties

		#region Methods

		public AnimatedGifImage(ImageList images, GraphicsDevice graphics)
		{
			Images = images.Images.ToList();
			CurrentIndex = 0;
			Timer = new CountdownTimer();
			Timer.Stop();

			Texture = new Texture2D(graphics, images.Width, images.Height, false, SurfaceFormat.Color);

			FillRect = true;
			UpdateTexture();
		}

		private void UpdateTexture()
		{
			CurrentIndex++;
			if (CurrentIndex >= Images.Count)
			{
				CurrentIndex = 0;
			}

			if (CurrentIndex < Images.Count)
			{
				Texture.SetData<Color>(Images[CurrentIndex].Data);
				var delay = (float)(Images[CurrentIndex].DelayMS) / 1000f;
				Timer.Start(delay);
			}
		}

		public override void Update(IScreen screen, GameClock gameTime)
		{
			base.Update(screen, gameTime);
			Timer.Update(gameTime);
			if (!Timer.HasTimeRemaining && !Timer.Paused)
			{
				UpdateTexture();
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			Texture?.Dispose();
			Texture = null;
		}

		#endregion //Methods
	}
}
