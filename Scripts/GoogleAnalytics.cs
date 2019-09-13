using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace KS.GoogleAnalytics
{
    public class GoogleAnalytics : MonoBehaviour
    {
        [Tooltip("Web tracking code in format: UA-XXXXXX-Y."), SerializeField]
        string trackingId;

        [SerializeField] bool anonymizeIP = false;
        [SerializeField] bool sendStartSessionEvent = true;
        [SerializeField] bool sendEndSessionEvent = true;
        [SerializeField] bool testMode = false;
        [SerializeField] bool enableSuccessLogs = true;
        [SerializeField] bool enableErrorLogs = true;

        public static GoogleAnalytics Instance = null;

        [NonSerialized]
        public bool RunSynchronously = false;

        private bool batchingMode = false;

        public ParametersManager ParametersManager = new ParametersManager();

        void Awake()
        {
            Init();

            if (sendStartSessionEvent)
                StartSession();
        }

        private void OnDestroy()
        {
            if (sendEndSessionEvent)
            {
                RunSynchronously = true;
                StopSession();
            }
        }

        private void Init()
        {
            if (Instance == null)
            {
                Instance = this;

                DontDestroyOnLoad(gameObject);
                ParametersManager.SetPernamentParameters(
                    (Parameters.General.PROTOCOL_VERSION, "1"),
                    (Parameters.General.TRACKING_ID, trackingId),
                    (Parameters.SystemInfo.USER_LANGUAGE, Application.systemLanguage.ToString()),
                    (Parameters.SystemInfo.SCREEN_RESOLUTION, Screen.width + "x" + Screen.height),
                    (Parameters.AppTracking.APPLICATION_ID, Application.identifier),
                    (Parameters.AppTracking.APPLICATION_NAME, Application.productName),
                    (Parameters.AppTracking.APPLICATION_VERSION, Application.version),
                    (Parameters.User.CLIENT_ID, SystemInfo.deviceUniqueIdentifier)
                    );

                if (anonymizeIP)
                    ParametersManager.SetPernamentParameters((Parameters.General.ANONYMIZE_IP, "1"));
            }
        }

        public void StartSession()
        {
            ParametersManager.SetNextHitParameters((Parameters.Session.SESSION_CONTROL, "start"));
            LogEvent("Google Analytics", "Auto Instrumentation", "Session started");
        }

        public void StopSession()
        {
            ParametersManager.SetNextHitParameters((Parameters.Session.SESSION_CONTROL, "end"));
            LogEvent("Google Analytics", "Auto Instrumentation", "Session ended");
        }

        public void LogScreen(string screenName)
        {
            ParametersManager.SetNextHitParameters(
                (Parameters.Hit.HIT_TYPE, "screenview"),
                (Parameters.ContentInformation.SCREEN_NAME, screenName));

            SendOrBatchHit();
        }

        public void LogPageView(string pageLocation, string pageTitle = "")
        {
            ParametersManager.SetNextHitParameters(
                (Parameters.Hit.HIT_TYPE, "pageview"),
                (Parameters.ContentInformation.DOCUMENT_LOCATION_URL, pageLocation),
                (Parameters.ContentInformation.DOCUMENT_TITLE, pageTitle));

            SendOrBatchHit();
        }

        public void LogEvent(string eventCategory, string eventAction,
             string eventLabel, long value = 0)
        {
            ParametersManager.SetNextHitParameters(
                (Parameters.Hit.HIT_TYPE, "event"),
                (Parameters.EventTracking.EVENT_CATEGORY, eventCategory),
                (Parameters.EventTracking.EVENT_ACTION, eventAction),
                (Parameters.EventTracking.EVENT_LABEL, eventLabel),
                (Parameters.EventTracking.EVENT_VALUE, value.ToString()));

            SendOrBatchHit();
        }

        public void LogTransaction(string transID, string affiliation,
             double revenue, double tax, double shipping, string currencyCode = "")
        {
            ParametersManager.SetNextHitParameters(
                (Parameters.Hit.HIT_TYPE, "transaction"),
                (Parameters.ECommerce.TRANSACTION_ID, transID),
                (Parameters.ECommerce.TRANSACTION_AFFILIATION, affiliation),
                (Parameters.ECommerce.TRANSACTION_REVENUE, revenue.ToString()),
                (Parameters.ECommerce.TRANSACTION_SHIPPING, shipping.ToString()),
                (Parameters.ECommerce.TRANSACTION_TAX, tax.ToString()),
                (Parameters.ECommerceEnhanced.CURRENCY_CODE, currencyCode));

            SendOrBatchHit();
        }

        public void LogItem(string transID, string name, string codeOrSku,
             string category, double price, long quantity, string currencyCode)
        {
            ParametersManager.SetNextHitParameters(
                (Parameters.Hit.HIT_TYPE, "item"),
                (Parameters.ECommerce.TRANSACTION_ID, transID),
                (Parameters.ECommerce.ITEM_NAME, name),
                (Parameters.ECommerce.ITEM_CODE, codeOrSku),
                (Parameters.ECommerce.ITEM_CATEGORY, category),
                (Parameters.ECommerce.ITEM_PRICE, price.ToString()),
                (Parameters.ECommerce.ITEM_QUANTITY, quantity.ToString()),
                (Parameters.ECommerceEnhanced.CURRENCY_CODE, currencyCode));

            SendOrBatchHit();
        }

        public void LogSocial(string socialNetwork, string socialAction,
             string socialTarget)
        {
            ParametersManager.SetNextHitParameters(
                (Parameters.Hit.HIT_TYPE, "social"),
                (Parameters.SocialInteractions.NETWORK, socialNetwork),
                (Parameters.SocialInteractions.ACTION, socialAction),
                (Parameters.SocialInteractions.ACTION_TARGET, socialTarget));

            SendOrBatchHit();
        }

        public void LogTiming(string timingCategory, long timingInterval,
             string timingVariableName, string timingLabel)
        {
            ParametersManager.SetNextHitParameters(
                (Parameters.Hit.HIT_TYPE, "social"),
                (Parameters.Timing.USER_CATEGORY, timingCategory),
                (Parameters.Timing.USER_TIME, timingInterval.ToString()),
                (Parameters.Timing.USER_LABEL, timingLabel),
                (Parameters.Timing.USER_VARIABLE, timingVariableName));

            SendOrBatchHit();
        }

        public void LogException(string description, bool isFatal)
        {
            ParametersManager.SetNextHitParameters(
                (Parameters.Hit.HIT_TYPE, "exception"),
                (Parameters.Exceptions.DESCRIPTION, description),
                (Parameters.Exceptions.FATAL, isFatal ? "1" : "0"));

            SendOrBatchHit();
        }

        public void StartBatchingInsteadOfSendingHits()
        {
            batchingMode = true;
        }

        public void StopBatchingAndSendHits()
        {
            batchingMode = false;
            var urlAndBody = ParametersManager.GetUrlAndBatchedBodyAndClear();

            if (urlAndBody.body.Count > 0)
                Send(urlAndBody.url, urlAndBody.body);
        }

        private void SendOrBatchHit()
        {
            if (batchingMode)
                ParametersManager.BatchHit();
            else
                Send(ParametersManager.GetNextHit());
        }

        private void Send(string url, List<IMultipartFormSection> body = null)
        {
            if (testMode)
            {
                Debug.Log("TestMode enabled, Google Analytics won't work.");
                return;
            }

            if (RunSynchronously)
                HandleWWWSynchronously(UnityWebRequest.Post(url, body));
            else
                StartCoroutine(HandleWWW(UnityWebRequest.Post(url, body)));
        }

        private void HandleWWWSynchronously(UnityWebRequest request)
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            { }
            LogHandleWWWResult(request);
        }

        private IEnumerator HandleWWW(UnityWebRequest request)
        {
            yield return request.SendWebRequest();
            LogHandleWWWResult(request);
        }

        private void LogHandleWWWResult(UnityWebRequest request)
        {
            if ((request.isNetworkError || request.isHttpError) && enableErrorLogs)
            {
                Debug.LogWarning("Google Analytics request failed with error "
                     + request.error);
            }
            else
            {
                if (request.responseCode == 200 && enableSuccessLogs)
                {
                    Debug.Log("Successfully sent Google Analytics hit: " + request.url);
                }
                else if (enableErrorLogs)
                    Debug.LogWarning("Google Analytics hit request rejected with " +
                         "status code " + request.responseCode);
            }
        }
    }
}