using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class Share : MonoBehaviour 
{
	public void share()
	{
		StartCoroutine( TakeScreenshotAndShare() );
	}
	
	private IEnumerator TakeScreenshotAndShare()
	{
		yield return new WaitForEndOfFrame();

		Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height ), 0, 0);
		ss.Apply();

		string filePath = Path.Combine(Application.persistentDataPath, System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
		File.WriteAllBytes(filePath, ss.EncodeToPNG());

		// To avoid memory leaks
		Destroy(ss);

		var score = UserManager.Instance.Kills;
		new NativeShare().AddFile(filePath)
			.SetSubject("Another Shooting Game")
			.SetText("I killed " + score + " monsters in ASG! Think you can beat me??")
			//.SetUrl( "https://github.com/yasirkula/UnityNativeShare" )
			//.SetCallback( ( result, shareTarget ) => Debug.Log( "Share result: " + result + ", selected app: " + shareTarget ) )
			.Share();

		// Share on WhatsApp only, if installed (Android only)
		//if( NativeShare.TargetExists( "com.whatsapp" ) )
		//	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
	}
}
