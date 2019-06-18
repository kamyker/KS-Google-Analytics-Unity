using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace KS.GoogleAnalytics
{
	public class ParametersManager
	{
		private const string apiUrl = @"https://www.google-analytics.com/collect?";
		private const string apiUrlBatch = @"https://www.google-analytics.com/batch";

		private ParametersBuilder pernamentParameters = new ParametersBuilder();
		private ParametersBuilder parametersNextHit = new ParametersBuilder();

		private List<string> hitsBatched = new List<string>();

		public void SetUserId(string userId)
		{
			SetPernamentParameters((Parameters.User.USER_ID, userId));
		}

		public void ClearUserId()
		{
			RemovePermanent(Parameters.User.USER_ID);
		}

		public void RemovePermanent(string parameter)
		{
			pernamentParameters.Remove(parameter);
		}

		public void SetNextHitCustomMetric(params
			(int customDimensionIndex, long value)[] metrics)
		{
			foreach (var m in metrics)
				parametersNextHit.Set("cm" + m.customDimensionIndex, m.value.ToString());
		}

		public void SetPernamentCustomMetrics(params
			(int customDimensionIndex, long value)[] metrics)
		{
			foreach (var m in metrics)
				pernamentParameters.Set("cm" + m.customDimensionIndex, m.value.ToString());
		}

		public void SetNextHitCustomDimensions(params
			(int customDimensionIndex, string value)[] dimensions)
		{
			foreach (var d in dimensions)
				parametersNextHit.Set("cd" + d.customDimensionIndex, d.value);
		}

		public void SetPernamentCustomDimensions(params
			(int customDimensionIndex, string value)[] dimensions)
		{
			foreach (var d in dimensions)
				pernamentParameters.Set("cd" + d.customDimensionIndex, d.value);
		}

		public void SetNextHitParameters(params (string parameter, string value)[] _parameters)
		{
			foreach (var p in _parameters)
				parametersNextHit.Set(p.parameter, p.value);
		}

		public void SetPernamentParameters(params (string parameter, string value)[] _parameters)
		{
			foreach (var p in _parameters)
				pernamentParameters.Set(p.parameter, p.value);
		}

		private string CombinePermaAndNextHitParameters()
		{
			return pernamentParameters.GetUrl() + "&" + parametersNextHit.GetUrl() + CacheBuster();
		}

		private string CacheBuster()
		{
			return "&z=" + UnityEngine.Random.Range(0, 999999);
		}

		internal string GetNextHit()
		{
			string url = apiUrl + CombinePermaAndNextHitParameters();
			parametersNextHit.Clear();
			return url;
		}

		internal (string url, List<IMultipartFormSection> body) GetUrlAndBatchedBodyAndClear()
		{
			var formData = new List<IMultipartFormSection>();
			foreach (var cachedHit in hitsBatched)
				formData.Add(new MultipartFormDataSection(cachedHit));

			hitsBatched.Clear();

			return (apiUrlBatch, formData);
		}

		internal void BatchHit()
		{
			hitsBatched.Add(CombinePermaAndNextHitParameters());
			parametersNextHit.Clear();
		}
	}
}