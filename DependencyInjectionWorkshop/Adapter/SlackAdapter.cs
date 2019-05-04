using DependencyInjectionWorkshop.Interface;
using SlackAPI;

namespace DependencyInjectionWorkshop.Adapter
{
	public class SlackAdapter : INotification
	{
		public void PostMessage(string message)
		{
			var slackClient = new SlackClient("my api token");
			slackClient.PostMessage(response1 => { }, "my channel", message, "my bot name");
		}
	}
}