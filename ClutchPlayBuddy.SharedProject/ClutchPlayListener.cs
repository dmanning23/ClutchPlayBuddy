using GameTimer;
using Microsoft.Xna.Framework;
using MonogameScreenTools;

namespace ClutchPlayBuddy
{
	public class ClutchPlayListener : GameComponent, IClutchPlayListener
	{
		#region Properties

		public bool HasClutchPlay => CurrentHeuristic > 0;

		private IScreenGrabber Grabber { get; set; }
		private CountdownTimer PostPlayTimer { get; set; }

		public int CurrentHeuristic { get; private set; }

		public string CurrentMessage { get; private set; }

		public ImageList CurrentClutchPlay { get; private set; }

		public float PostTimeDelta { get; set; }

		#endregion //Properties

		#region Methods

		public ClutchPlayListener(Game game) : base(game)
		{
			PostTimeDelta = 0.6f;

			// Register ourselves to implement the IToastBuddy service.
			game.Components.Add(this);
			game.Services.AddService(typeof(IClutchPlayListener), this);

			Grabber = game.Services.GetService<IScreenGrabber>();
			Reset();
		}

		public void Reset()
		{
			CurrentHeuristic = 0;
			CurrentMessage = string.Empty;
			CurrentClutchPlay = new ImageList(Game.GraphicsDevice);
			PostPlayTimer = new CountdownTimer();
			PostPlayTimer.Stop();
		}

		public void AddPlay(int heuristic, string message)
		{
			if (heuristic > CurrentHeuristic)
			{
				CurrentHeuristic = heuristic;
				CurrentMessage = message;
				PostPlayTimer.Start(PostTimeDelta);
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			PostPlayTimer.Update(gameTime);
			if (!PostPlayTimer.Paused && !PostPlayTimer.HasTimeRemaining)
			{
				//grab the current image stack
				CurrentClutchPlay.CopyImageList(Grabber.CurrentImageList);
				PostPlayTimer.Stop();
			}
		}

		#endregion //Methods
	}
}
