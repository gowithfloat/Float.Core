using System.Threading;
using System.Threading.Tasks;
using Float.Core.Tasks;
using Xunit;

namespace Float.Core.Tests
{
    public class RepeatableTaskContainerTests
    {
        [Fact]
        public async Task TaskRunsOnce()
        {
            var counter = 0;
            var repeatableTaskContainer = new RepeatableTaskContainer(async () =>
            {
                counter += 1;
                await Task.Delay(10); // Simulate a "long" running task
            });

            await Task.WhenAll(
                Task.Run(repeatableTaskContainer.Run),
                Task.Run(repeatableTaskContainer.Run),
                Task.Run(repeatableTaskContainer.Run),
                Task.Run(repeatableTaskContainer.Run),
                Task.Run(repeatableTaskContainer.Run),
                Task.Run(repeatableTaskContainer.Run),
                Task.Run(repeatableTaskContainer.Run),
                Task.Run(repeatableTaskContainer.Run),
                Task.Run(repeatableTaskContainer.Run)
                );

            Assert.Equal(1, counter);
        }

        [Fact]
        public async Task GenericTaskRunsOnce()
        {
            var counter = 0;
            var repeatableTaskContainer = new RepeatableTaskContainer<string>(async () =>
            {
                counter += 1;
                await Task.Delay(10);
                return "test";
            });

            await Task.WhenAll(
                Task.Run(repeatableTaskContainer.Run),
                Task.Run(repeatableTaskContainer.Run),
                Task.Run(repeatableTaskContainer.Run),
                Task.Run(repeatableTaskContainer.Run),
                Task.Run(repeatableTaskContainer.Run),
                Task.Run(repeatableTaskContainer.Run),
                Task.Run(repeatableTaskContainer.Run),
                Task.Run(repeatableTaskContainer.Run),
                Task.Run(repeatableTaskContainer.Run)
                );

            Assert.Equal(1, counter);
        }

        [Fact]
        public async Task ReportsAsRunning()
        {
            var semaphore = new SemaphoreSlim(0,1);
            var repeatableTaskContainer = new RepeatableTaskContainer(async () =>
            {
                await semaphore.WaitAsync();
            });

            Assert.False(repeatableTaskContainer.IsRunning);

            var task = repeatableTaskContainer.Run();

            Assert.True(repeatableTaskContainer.IsRunning);
            semaphore.Release();
            await task;
            Assert.False(repeatableTaskContainer.IsRunning);
        }

        [Fact]
        public async Task GenericReportsAsRunning()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var repeatableTaskContainer = new RepeatableTaskContainer<string>(async () =>
            {
                await semaphore.WaitAsync();
                return "test";
            });

            Assert.False(repeatableTaskContainer.IsRunning);

            var task = repeatableTaskContainer.Run();

            Assert.True(repeatableTaskContainer.IsRunning);
            semaphore.Release();
            await task;
            Assert.False(repeatableTaskContainer.IsRunning);
        }
    }
}

