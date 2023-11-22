using Sentry;

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

        Task.Run(() =>
        {
            var transaction = SentrySdk.StartTransaction("Foo", "Bar");
            FindPrimeNumber(100000);
            transaction.Finish();
            //SentrySdk.CaptureException(new NotSupportedException());
            SentrySdk.Flush(TimeSpan.FromMinutes(5));
            //throw new NotImplementedException();
        });
    }

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

