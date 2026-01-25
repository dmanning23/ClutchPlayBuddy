using Microsoft.Xna.Framework;
using MonogameScreenTools;

namespace ClutchPlayBuddy
{
	public interface IClutchPlayListener : IGameComponent
	{
		bool HasClutchPlay { get; }
		int CurrentHeuristic { get; }
		string CurrentMessage { get; }
		ImageList CurrentClutchPlay { get; }

		/// <summary>
		/// How long to keep recording after a play
		/// </summary>
		float PostTimeDelta { get; set; }

		void Reset();
		void AddPlay(int heuristic, string message);
	}
}
