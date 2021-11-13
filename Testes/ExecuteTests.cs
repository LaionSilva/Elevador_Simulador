using System;
using System.Windows.Threading;

namespace Elevador_Simulador
{
	public class ExecuteTests
	{
		public ExecuteTests()
		{
		}
		private static DispatcherTimer timer0;
		public static void Run()
		{
			timer0 = new DispatcherTimer();
			timer0.Interval = TimeSpan.FromMilliseconds(1000);
			timer0.Tick += ExecuteTests.Teste1;
			timer0.Start();


		}
		public static void Teste1(object sender, EventArgs e)
		{

		}
	}
}
