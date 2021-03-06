﻿using MenuBuddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonogameScreenTools;
using ResolutionBuddy;
using ShareBuddy;
using System;
using System.Threading.Tasks;
using ToastBuddyLib;

namespace ClutchPlayBuddy
{
	public class GifProgressScreen : WidgetScreen
	{
		IGifHelper GifHelper { get; set; }
		string ShareText { get; set; }

		public GifProgressScreen(IGifHelper gifHelper, string shareText, ContentManager content = null) : base("GifProgressScreen", content)
		{
			CoverOtherScreens = true;
			CoveredByOtherScreens = false;
			ShareText = shareText;

			GifHelper = gifHelper;
			GifHelper.OnGifCreated += GifHelper_OnGifCreated;
		}

		private async void GifHelper_OnGifCreated(object sender, GifCreatedEventArgs e)
		{
			GifHelper.OnGifCreated -= GifHelper_OnGifCreated;

			ExitScreen();

			if (e.Success)
			{
				var sharer = new ShareHelper(ScreenManager.Game);
				await sharer.ShareImage(e.Filename, ShareText);
			}
			else
			{
				var messageDisplay = ScreenManager.Game.Services.GetService<IToastBuddy>();
				messageDisplay.ShowMessage($"Error creating gif", Color.Yellow);

				await ScreenManager.AddScreen(new OkScreen(e.ErrorMessage));
			}
		}

		public override async Task LoadContent()
		{
			await base.LoadContent();

			var layout = new RelativeLayout
			{
				Horizontal = HorizontalAlignment.Center,
				Vertical = VerticalAlignment.Center,
				Position = Resolution.TitleSafeArea.Center,
			};

			//create the message widget
			var width = 0f;
			var msg = new PaddedLayout(0, 0, 24, 0, new Label("Creating Gif...", Content)
			{
				Highlightable = false
			})
			{
				Horizontal = HorizontalAlignment.Right,
				Vertical = VerticalAlignment.Center,
			};
			width += msg.Rect.Width;

			Texture2D hourglassTex = null;
			try
			{
				hourglassTex = Content.Load<Texture2D>(StyleSheet.LoadingScreenHourglassImageResource);
			}
			catch (Exception)
			{
				//No hourglass texture :P
			}

			if (null != hourglassTex)
			{
				//create the hourglass widget
				var hourglass = new Image(hourglassTex)
				{
					Horizontal = HorizontalAlignment.Left,
					Vertical = VerticalAlignment.Center,
					Scale = 1.5f,
					Highlightable = false,
				};
				layout.AddItem(hourglass);
				width += hourglass.Rect.Width;

				//add a little shim in between the widgets
				width += 32f;
			}

			layout.AddItem(msg);
			layout.Size = new Vector2(width, 64f);
			AddItem(layout);

			AddCancelButton();
		}

		/// <summary>
		/// Draws the "game over" menu screen. 
		/// This darkens down the gameplay screen that is underneath us, and then chains to the base MenuScreen.Draw.
		/// </summary>
		public override void Draw(GameTime gameTime)
		{
			ScreenManager.SpriteBatchBegin();
			FadeBackground();
			ScreenManager.SpriteBatchEnd();
			base.Draw(gameTime);
		}
	}
}
