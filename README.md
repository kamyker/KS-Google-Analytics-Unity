# KS Google Analytics for Unity (Standalone)

## Installation:
1. Open "unity_project_name/Packages/manifest.json"
2. Under "dependencies" add:
  ```  
  "com.ks.googleanalytics": "https://github.com/kamyker/google-analytics.git", 
  ```
3. Drop GA prefab to your starting scene.
4. Set tracking id in prefab Google Analytics component

## How to use
  - Add `using KS.GoogleAnalytics`to your script
  - [API Reference](https://developers.google.com/analytics/devguides/collection/unity/reference) - All basic methods are the same
  - Ex. logging event use:
  ```
  GoogleAnalytics.Instance.LogEvent(eventCategory,eventAction,eventLabel,0);
  ```
  - Ex. setting pernament dimension parameters
  ```
  GoogleAnalytics.Instance.ParametersManager.SetPernamentCustomDimensions((1, "abTest1"), (2, "premiumUser"));
  ```
  
## Differences to official GA Plugin:
 - Probably works only in standalone builds (tested in Windows)
 - **Possibilty to batch events**, for ex. if u want to send 5 hits together run `StartBatchingInsteadOfSendingHits()`, log you events as usual and run `StopBatchingAndSendHits()`
 - **Possibilty to send events in OnApplicationQuit()**, simply set `GoogleAnalytics.Instance.RunSynchronously = true;` before sending hits, it will prevent application from quiting
 - **Added LogPageView(string location, (optional)string title)**, use it instead of LogScreen as it is much better integrated in GA, works in realtime and in user explorer. 
 
Ex: `GoogleAnalyticsV4.instance.LogPageView("MainMenu/Settings")`
![GitHub Logo](/imgs~/pageview_example.png)
 - Simpler GA prefab settings (rest ino pulled from player settings)
 - Added EndSession event when GA prefab is Destoyed (player quited game)
 - Added StartSession event when GA prefab is Awaken
 - Updated WWW to UnityWebRequest