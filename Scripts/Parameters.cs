public static class Parameters
{
	public static class User
	{
		public const string CLIENT_ID = "cid";
		public const string USER_ID = "uid";
	}

	public static class SystemInfo
	{
		public const string USER_LANGUAGE = "ul";
		public const string SCREEN_RESOLUTION = "sr";
	}

	public static class General
	{
		public const string PROTOCOL_VERSION = "v";
		public const string TRACKING_ID = "tid";
		public const string ANONYMIZE_IP = "aip";
		public const string CACHE_BUSTER = "z";
	}

	public static class AppTracking
	{
		public const string APPLICATION_NAME = "an";
		public const string APPLICATION_ID = "aid";
		public const string APPLICATION_VERSION = "av";
	}

	public static class Hit
	{
		public const string HIT_TYPE = "t";
	}

	public static class EventTracking
	{
		public const string EVENT_CATEGORY = "ec";
		public const string EVENT_ACTION = "ea";
		public const string EVENT_LABEL = "el";
		public const string EVENT_VALUE = "ev";
	}

	public static class Session
	{
		public const string SESSION_CONTROL = "sc";
	}

	public static class ContentInformation
	{
		public const string SCREEN_NAME = "cd";
		public const string DOCUMENT_LOCATION_URL = "dl";
		public const string DOCUMENT_TITLE = "dt";
	}

	public static class ECommerce
	{
		public const string TRANSACTION_ID = "ti";
		public const string TRANSACTION_AFFILIATION = "ta";
		public const string TRANSACTION_REVENUE = "tr";
		public const string TRANSACTION_SHIPPING = "ts";
		public const string TRANSACTION_TAX = "tt";
		public const string ITEM_NAME = "in";
		public const string ITEM_PRICE = "ip";
		public const string ITEM_QUANTITY = "iq";
		public const string ITEM_CODE = "ic";
		public const string ITEM_CATEGORY = "iv";
	}

	public static class ECommerceEnhanced
	{
		public const string CURRENCY_CODE = "cu";
	}

	public static class SocialInteractions
	{
		public const string NETWORK = "sn";
		public const string ACTION = "sa";
		public const string ACTION_TARGET = "sy";
	}

	public static class Timing
	{
		public const string USER_CATEGORY = "utc";
		public const string USER_VARIABLE = "utv";
		public const string USER_TIME = "utt";
		public const string USER_LABEL = "utl";
	}

	public static class Exceptions
	{
		public const string DESCRIPTION = "exd";
		public const string FATAL = "exf";
	}
}
