using System.Threading;
using System.Threading.Tasks;
using Float.Core.Commands;
using Xamarin.Forms;
using Xunit;

namespace Float.Core.Tests
{
    public class DebounceCommandTests
    {
        [Fact]
        public async Task TestDoesDebounceTimer()
        {
            var count = 0;
            var incrementCommand = new Command(() => count += 1);

            // only allow an increment every 450ms
            var debouncedIncrementCommand = new DebounceCommand(incrementCommand, 450);

            // attempt to increment every 100ms
            new Timer(arg => debouncedIncrementCommand.Execute(arg), null, 100, 100);

            // wait one second
            await Task.Delay(1000);

            // in one second, we should have allowed at most two increments
            Assert.Equal(2, count);
        }

        [Fact(Skip = "Flaky")]
        public async Task TestDoesDebounceWhenAll()
        {
            var count = 0;
            var incrementCommand = new Command(() => count += 1);
            var debouncedIncrementCommand = new DebounceCommand(incrementCommand, 500);

            await Task.WhenAll(new[]
            {
                Task.Run(() => debouncedIncrementCommand.Execute(default)),
                Task.Run(() => debouncedIncrementCommand.Execute(default)),
                Task.Run(() => debouncedIncrementCommand.Execute(default)),
                Task.Run(() => debouncedIncrementCommand.Execute(default)),
            });

            await Task.Delay(500);

            Assert.Equal(1, count);
        }
    }
}
