using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Microsoft.PowerFx;
using Microsoft.PowerFx.Core.Utils;
using Microsoft.PowerFx.LanguageServerProtocol;
using Microsoft.PowerFx.Syntax;
using Microsoft.PowerFx.Types;
using PowerFxWasm.Commons;
using PowerFxWasm.Model;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace PowerFxWasm
{
	public static class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			////builder.RootComponents.Add<App>("#app");
			////builder.RootComponents.Add<HeadOutlet>("head::after");

			//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

			await builder.Build().RunAsync();
		}

		private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
		{
			WriteIndented = true,
			Converters =
			{
				new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
				new NodeConverter<TexlNode>(),
				new NodeConverter<VariadicOpNode>(),
				new NodeConverter<ListNode>(),
				new NodeConverter<CallNode>(),
				new NodeConverter<Identifier>(),
				new NodeConverter<DName>()
			}
		};
		[JSInvokable]
		public static async Task<string> SayHello(string name)
		{
			return $"Hello {name}.";
		}
		[JSInvokable]
		public static async Task<string> EvaluateAsync(string context, string expression)
		{
			IReadOnlyList<Token> tokens = null;
			CheckResult check = null;
			var cts = new CancellationTokenSource();
			var _timeout = TimeSpan.FromSeconds(2);
			cts.CancelAfter(_timeout);
			try
			{
				var engine = new PowerFxScopeFactory().GetEngine();

				var parameters = (RecordValue)FormulaValue.FromJson(context);

				if (parameters == null)
				{
					parameters = RecordValue.Empty();
				}

				tokens = engine.Tokenize(expression);
				check = engine.Check(expression, parameters.Type, options: null);
				check.ThrowOnErrors();
				var eval = check.GetEvaluator();
				var result = await eval.EvalAsync(cts.Token, parameters);
				var resultString = PowerFxHelper.TestToString(result);

				return JsonSerializer.Serialize(new
				{
					result = resultString,
					tokens = tokens,
					parse = JsonSerializer.Serialize(check.Parse.Root, _jsonSerializerOptions)
				}, new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				});
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				cts.Dispose();
			}
		}
		[JSInvokable]
		public static async Task<string> LspAsync(string body)
		{
			var scopeFactory = new PowerFxScopeFactory();

			var sendToClientData = new List<string>();
			var languageServer = new LanguageServer((string data) => sendToClientData.Add(data), scopeFactory);

			try
			{
				languageServer.OnDataReceived(body.ToString());
				return JsonSerializer.Serialize(sendToClientData.ToArray());
			}
			catch (Exception ex)
			{
				throw;
			}
		}
	}

	public class VersionInfo
	{
		public string Key { get; set; }
		public string Value { get; set; }
		public string Url { get; set; }
	}
}