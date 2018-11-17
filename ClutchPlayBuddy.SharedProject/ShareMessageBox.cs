using ExternalStorageBuddy;
using InputHelper;
using MenuBuddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonogameScreenTools;

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

		#endregion //Properties

		#region Methods

		public ShareMessageBox(string message, string filename, string shareText, string menuTitle = "", ContentManager content = null) : base(message, menuTitle, content)
		{
			Filename = filename;
			ShareText = shareText;

			OkText = "Yes";
			CancelText = "No";
			OnSelect += OnShareYes;
			OnCancel += OnShareNo;
		}

		protected override void AddAddtionalControls()
		{
			base.AddAddtionalControls();

			StorageHelper = ScreenManager.Game.Services.GetService<IExternalStorageHelper>();
			Listener = ScreenManager.Game.Services.GetService<IClutchPlayListener>();

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
			if (!StorageHelper.HasPermission)
			{
				var externalStorageMessage = new OkScreen("The image will be saved to the Pictures folder in external storage before it is shared.", Content);
				externalStorageMessage.OnSelect += ExternalStorageMessage_OnSelect;
				ScreenManager.AddScreen(externalStorageMessage);
			}
			else
			{
				BeginGifProcess();
			}
		}

		private void ExternalStorageMessage_OnSelect(object sender, ClickEventArgs e)
		{
			StorageHelper.StoragePermissionGranted -= Helper_StoragePermissionGranted;
			StorageHelper.StoragePermissionGranted += Helper_StoragePermissionGranted;
			StorageHelper.AskPermission();
		}

		private void Helper_StoragePermissionGranted(object sender, ExternalStoragePermissionEventArgs e)
		{
			if (e.PermissionGranted)
			{
				BeginGifProcess();
			}
		}

		private void BeginGifProcess()
		{
			gif = new GifHelper();

			gif.OnGifCreated -= Gif_OnGifCreated;
			gif.OnGifCreated += Gif_OnGifCreated;
			ScreenManager.AddScreen(new GifProgressScreen(gif, Content));
			gif.Export(Listener.CurrentClutchPlay, Filename, true, 2);
		}

		private void Gif_OnGifCreated(object sender, GifCreatedEventArgs e)
		{
			gif.OnGifCreated -= Gif_OnGifCreated;

			var sharer = new ShareBuddy(ScreenManager.Game);

			sharer.ShareImage(e.Filename, ShareText);
		}

		private void OnShareNo(object sender, ClickEventArgs e)
		{
			ExitScreen();
		}

		#endregion //Methods
	}
}
