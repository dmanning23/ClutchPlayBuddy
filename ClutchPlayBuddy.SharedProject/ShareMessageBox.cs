using ExternalStorageBuddy;
using InputHelper;
using MenuBuddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonogameScreenTools;
using System;
using ToastBuddyLib;
#if ANDROID
using Plugin.CurrentActivity;
#endif

namespace ClutchPlayBuddy
{
	public class ShareMessageBox : MessageBoxScreen
	{
		#region Properties

		IClutchPlayListener Listener { get; set; }
		IExternalStorageHelper StorageHelper { get; set; }
		IGifHelper gif;
		string Filename { get; set; }
		string ShareText { get; set; }

		bool shareYes = false;

		#endregion //Properties

		#region Methods

		public ShareMessageBox(string message, string filename, string shareText, string menuTitle = "", ContentManager content = null) : base(message, menuTitle, content)
		{
			Filename = filename;
			ShareText = shareText;
			ExitOnOk = false;

			OkText = "Share";
			CancelText = "Skip";
			OnSelect += OnShareYes;
			OnCancel += OnShareNo;
		}

		protected override void AddAddtionalControls()
		{
			base.AddAddtionalControls();

			StorageHelper = ScreenManager.Game.Services.GetService<IExternalStorageHelper>();

			if (null == StorageHelper)
			{
				throw new Exception("You need to add the IExternalStorageHelper to Game.Services");
			}

			Listener = ScreenManager.Game.Services.GetService<IClutchPlayListener>();

			if (null == Listener)
			{
				throw new Exception("You need to add the IClutchPlayListener to Game.Services");
			}

			//add a shim between the text and the buttons
			ControlStack.AddItem(new Shim() { Size = new Vector2(0, 56f) });

			//add the gif image
			var pp = ScreenManager.Game.GraphicsDevice.PresentationParameters;
			ControlStack.AddItem(new AnimatedGifImage(Listener.CurrentClutchPlay, ScreenManager.Game.GraphicsDevice)
			{
				Size = new Vector2(pp.BackBufferWidth / 3, pp.BackBufferHeight / 3),
				Horizontal = HorizontalAlignment.Center,
				Vertical = VerticalAlignment.Center,
			});
		}

		private void OnShareYes(object sender, ClickEventArgs e)
		{
			shareYes = true;

			if (!StorageHelper.HasPermission)
			{
				var externalStorageMessage = new OkScreen("The image will be saved to the Pictures folder in external storage before it is shared.", Content);
				externalStorageMessage.OnSelect += ExternalStorageMessage_OnSelect;
				ScreenManager.AddScreen(externalStorageMessage);
			}
			else
			{
				BeginGifProcess();
				ExitScreen();
			}
		}

		private void ExternalStorageMessage_OnSelect(object sender, ClickEventArgs e)
		{
			StorageHelper.AskPermission();
		}

		private void BeginGifProcess()
		{
			gif = new GifHelper();

			ScreenManager.AddScreen(new GifProgressScreen(gif, ShareText, Content));
			
			gif.Export(Listener.CurrentClutchPlay, Filename, true, 2);
		}

		private void OnShareNo(object sender, ClickEventArgs e)
		{
			ExitScreen();
		}

		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

			if (shareYes && StorageHelper.HasResult && !IsExiting)
			{
				ExitScreen();

				if (StorageHelper.HasPermission)
				{
					BeginGifProcess();
				}
				else
				{
					//set this flag to false so it asks again next time
					StorageHelper.HasResult = false;
				}
			}
		}

		#endregion //Methods
	}
}
