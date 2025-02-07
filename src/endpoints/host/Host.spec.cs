using NUnit.Framework;
using tcg;
using System;
using System.Collections.Generic;

namespace tcgTests
{
  [TestFixture]
  public class HostTestsFixture
  {
    private (MiddlewareLocal, MiddlewareLocal, MiddlewareLocal, Host) InitializePreset()
    {
      MiddlewareLocal mid1, mid2, midHost;
      (mid1, mid2, midHost) = MiddlewareLocal.GenerateLinkedMiddlewareLocalTriple();

      Host host = new Host(midHost, new List<MiddlewareLocal> { mid1, mid2 });

      return (mid1, mid2, midHost, host);
    }

    [Test]
    public void Initialize()
    {
      Middleware mid1, mid2, midHost;
      Host host;

      (mid1, mid2, midHost, host) = InitializePreset();
    }

    [Test]
    public void PlayerToHostSendDataTest()
    {
      Middleware mid1, mid2, midHost;
      Host host;

      (mid1, mid2, midHost, host) = InitializePreset();


      int executions = 0;
      Action<int, string> inputHandler = (playerIndex, input) =>
      {
        if (playerIndex == 0)
          Assert.AreEqual("player1 message", input);
        else if (playerIndex == 1)
          Assert.AreEqual("player2 message", input);

        executions++;
      };

      host.AddInputHandler(inputHandler);
      mid1.AddInputHandler((a, b) => { });
      mid2.AddInputHandler((a, b) => { });

      mid1.SendData("player1 message");
      mid2.SendData("player2 message");

      Assert.AreEqual(executions, 2);
    }

    [Test]
    public void HostToPlayerSendDataTest()
    {
      Middleware mid1, mid2, midHost;
      Host host;

      (mid1, mid2, midHost, host) = InitializePreset();


      int executions = 0;
      Action<int, string> inputHandler = (_, input) =>
      {
        Assert.AreEqual("host's message", input);
        executions++;
      };

      mid1.AddInputHandler(inputHandler);
      mid2.AddInputHandler(inputHandler);

      midHost.SendData("host's message");

      Assert.AreEqual(executions, 2);
    }

    [Test]
    public void HostInputValidationTest()
    {
      Middleware mid1, mid2, midHost;
      Host host;

      (mid1, mid2, midHost, host) = InitializePreset();

      bool responseGot = false;
      mid1.AddInputHandler((sender, input) =>
      {
        Assert.AreEqual(input, "Command name is not listed in commandsMap");
        responseGot = true;
      });

      Client client = new Client(mid1);
      client.SendCommand("that's a wrong command");


      Assert.IsTrue(responseGot);
    }
  }
}