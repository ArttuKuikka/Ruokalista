using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Util;
using Android.Widget;
using Android.OS;
using System.Net;

namespace Ruokalista.Droid.AppWidget
{

	[Activity(Label = "Ruokalista", Name = "com.arttu.ruokalista.AppWidget")]

	public class AppWidget : AppWidgetProvider
	{
		private static string AnnouncementClick = "AnnouncementClickTag";

		/// <summary>
		/// This method is called when the 'updatePeriodMillis' from the AppwidgetProvider passes,
		/// or the user manually refreshes/resizes.
		/// </summary>
		public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
		{
			var me = new ComponentName(context, Java.Lang.Class.FromType(typeof(AppWidget)).Name);
			appWidgetManager.UpdateAppWidget(me, BuildRemoteViews(context, appWidgetIds));
		}

		private RemoteViews BuildRemoteViews(Context context, int[] appWidgetIds)
		{
			

			// Retrieve the widget layout. This is a RemoteViews, so we can't use 'FindViewById'
			var widgetView = new RemoteViews(context.PackageName, Resource.Layout.Widget);


			SetTextViewText(widgetView);
			RegisterClicks(context, appWidgetIds, widgetView);

			return widgetView;
		}

		private void SetTextViewText(RemoteViews widgetView)
		{
			var url = "https://peda.net/isokyro/ylakoulu/hyv%C3%A4-tiet%C3%A4%C3%A4/kouluruokailu:atom";

			char quote = '\u0022';

			WebClient client = new WebClient();

			string http = client.DownloadString(url);

			var basehtml = GetBetween(http, "<entry>", "</entry>");

			

			var content = GetBetween(basehtml, $"<content type={quote}html{quote}>", "</content>");


			var Title = GetBetween(content, "&lt;p&gt;&lt;b&gt;", "&lt;br/&gt;&#10;");

			

			var ruoka = content.Replace("&lt;br/&gt;&#10;", "\n");
			ruoka = ruoka.Replace("&lt;p&gt;&lt;b&gt;" + Title, string.Empty);
			ruoka = ruoka.Replace("&lt;br/&gt;&#10;", string.Empty);
			ruoka = ruoka.Replace("&lt;/b&gt;", string.Empty);
			ruoka = ruoka.Replace("&lt;/p&gt;&#10;", string.Empty);
			ruoka = ruoka.Replace("\n\n\n", string.Empty);
			//ruoka = ruoka.Replace("\n", "\n\n");

			
		


			widgetView.SetTextViewText(Resource.Id.textView1, "Widget päivitetty: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
			widgetView.SetTextViewText(Resource.Id.RuokaTitle, Title);
			widgetView.SetTextViewText(Resource.Id.RuokaListaText, ruoka);
			
			
			
			
		}

		private void RegisterClicks(Context context, int[] appWidgetIds, RemoteViews widgetView)
		{
			var intent = new Intent(context, typeof(AppWidget));
			intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
			intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);

			// Register click event for the Background
			var piBackground = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.Immutable);
			widgetView.SetOnClickPendingIntent(Resource.Id.widgetBackground, piBackground);

			// Register click event for the Announcement-icon
			
		}

		private PendingIntent GetPendingSelfIntent(Context context, string action)
		{
			var intent = new Intent(context, typeof(AppWidget));
			intent.SetAction(action);
			return PendingIntent.GetBroadcast(context, 0, intent, 0);
		}

		/// <summary>
		/// This method is called when clicks are registered.
		/// </summary>
		public override void OnReceive(Context context, Intent intent)
		{
			base.OnReceive(context, intent);

			// Check if the click is from the "Announcement" button
			if (AnnouncementClick.Equals(intent.Action))
			{
				var pm = context.PackageManager;
				try
				{
					var packageName = "com.android.settings";
					var launchIntent = pm.GetLaunchIntentForPackage(packageName);
					context.StartActivity(launchIntent);
				}
				catch
				{
					// Something went wrong :)
				}
			}
		}

		public static string GetBetween(string strSource, string strStart, string strEnd)
		{
			if (strSource.Contains(strStart) && strSource.Contains(strEnd))
			{
				int Start, End;
				Start = strSource.IndexOf(strStart, 0) + strStart.Length;
				End = strSource.IndexOf(strEnd, Start);
				return strSource.Substring(Start, End - Start);
			}

			return "";
		}
	}
}
