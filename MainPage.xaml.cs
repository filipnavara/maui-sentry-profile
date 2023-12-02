using Sentry;
using System.Runtime.CompilerServices;

namespace maui_sentry;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);

        _ = Task.Run(() =>
        {
            /*var sentryIdType = Type.GetType("Sentry.CocoaSdk.SentryId, Sentry.Bindings.Cocoa")!;
            var privateSentrySDKOnlyType = Type.GetType("Sentry.CocoaSdk.PrivateSentrySDKOnly, Sentry.Bindings.Cocoa")!;
            // public static ulong StartProfilerForTrace(SentryId traceId)
            var startProfiler = privateSentrySDKOnlyType!.GetMethod("StartProfilerForTrace", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            // public static NSDictionary<NSString, NSObject>? CollectProfileBetween(ulong startSystemTime, ulong endSystemTime, SentryId traceId)
            var collectProfileBetween = privateSentrySDKOnlyType!.GetMethod("CollectProfileBetween", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);*/


            var transaction = SentrySdk.StartTransaction("Foo", "Bar");
            //var cocoaSentryId = sentryIdType.GetConstructor(new[] { typeof(string) })!.Invoke(new[] { transaction.TraceId.ToString() });
            //ulong startTime = (ulong)startProfiler.Invoke(null, new[] { cocoaSentryId });
            // transaction.TraceId
            FindPrimeNumber(100000);
            //ulong endTime = startTime + (ulong)(TimeSpan.NanosecondsPerTick * TimeSpan.FromSeconds(30).Ticks);
            //var collectedProfile = collectProfileBetween.Invoke(null, new[] { startTime, endTime, cocoaSentryId });
            //var cpText = collectedProfile.ToString();
            transaction.Finish();

            //Sentry.Protocol.Envelopes.Envelope.FromTransaction(transaction);
            //SentrySdk.CaptureException(new NotSupportedException());
            SentrySdk.Flush(TimeSpan.FromMinutes(5));
            //throw new NotImplementedException();

            //SentrySdk.
        });
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long FindPrimeNumber(int n)
    {
        int count = 0;
        long a = 2;
        while (count < n)
        {
            long b = 2;
            int prime = 1;// to check if found a prime
            while (b * b <= a)
            {
                if (a % b == 0)
                {
                    prime = 0;
                    break;
                }
                b++;
            }
            if (prime > 0)
            {
                count++;
            }
            a++;
        }
        return (--a);
    }
}

