using Automate.Application.InfrastructureValueObjects;
using Automate.Infrastructure.DataRetrievalFormats;
using Automate.Infrastructure.LeafClientService;
using Automate.Infrastructure.Test.TestConfigurations;
using CSharpFunctionalExtensions;

namespace Automate.Infrastructure.Test.LeafApiServiceTests;

public class LeafApiServiceTest
{
    [Fact]
    public void EnsureThatIdempotencyIsMaintainedFromOldRepoToNewRepo()
    {
        // Assemble
        // Create a list of threads
        int threadslen = 10;
        int msgLen = 5;
        List<LeafThread> oldThreads = CreateThreads(threadslen, msgLen, true);

        // Additional Threads
        int newThreadsLen = threadslen + 5;
        int messagelen = 3;
        List<LeafThread> newThreads = CreateThreads(newThreadsLen, messagelen, false);
        PreemptiveAssert(newThreads);

        // Generate the service
        LeafApiService leafApiService = new(new InfrastructureTestSettings());

        // Act
        Result<List<LeafThread>> action = leafApiService.MaintainLocalRepoIdempotency(newThreads, oldThreads);

        // Assert
        Assert.True(action.IsSuccess);
        foreach (LeafThread item in action.Value)
        {
            foreach (var thread in oldThreads)
            {
                if (string.Equals(thread.Uuid, item.Uuid))
                {
                    Assert.NotNull(thread.Messages);
                    Assert.NotNull(item.Messages);

                    foreach (var msg in thread.Messages)
                        Assert.NotNull(msg.Source);
                    foreach (var msg in item.Messages)
                        Assert.NotNull(msg.Source);
                }
            }
        }

        #region Local
        static void PreemptiveAssert(List<LeafThread> newThreads)
        {
            foreach (var thread in newThreads)
            {
                Assert.NotNull(thread.Messages);
                foreach (var msg in thread.Messages)
                    Assert.Null(msg.Source);
            }
        }
        static List<LeafThread> CreateThreads(int threadslen, int msgLen, bool sourceNotNull)
        {
            List<LeafThread> oldThreads = new(threadslen);
            for (var i = 0; i < threadslen; i++)
            {
                Msg[] msgs = new Msg[msgLen];
                for (var j = 0; j < msgLen; j++)
                {
                    Msg msg = new();
                    if (sourceNotNull) msg.Source = $"{j}";
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    else msg.Source = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    msgs[j] = msg;
                }
                LeafThread thread = new()
                {
                    Uuid = $"{i}",
                    Messages = msgs
                };
                oldThreads.Add(thread);
            }

            return oldThreads;
        }
        #endregion
    }

}
