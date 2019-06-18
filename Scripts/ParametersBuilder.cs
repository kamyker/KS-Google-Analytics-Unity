using System.Collections.Generic;
using System.Text;

public class ParametersBuilder
{
	private Dictionary<string, string> parameters = new Dictionary<string, string>();

	public void Clear()
	{
		parameters.Clear();
	}

	public bool Remove(string parameter)
	{
		return parameters.Remove(parameter);
	}

	public ParametersBuilder Set(string parameter, string value)
	{
		if (string.IsNullOrEmpty(value))
			Remove(parameter);
		else
		{
			if (parameters.ContainsKey(parameter))
				parameters[parameter] = value;
			else
				parameters.Add(parameter, value);
		}

		return this;
	}

	public string GetUrl()
	{
		StringBuilder sb = new StringBuilder();
		foreach (var par in parameters)
		{
			sb.Append(par.Key);
			sb.Append("=");
			sb.Append(par.Value);
			sb.Append("&");
		}
		sb.Length--; //removing last &
		return sb.ToString();
	}
}
