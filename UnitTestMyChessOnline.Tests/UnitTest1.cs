using Microsoft.AspNetCore.SignalR.Client;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace UnitTestMyChessOnline.Tests
{
    public class Tests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task ChatTestAsync()
        {
            HubConnection connection;
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:44374/Home/Index")
                .Build();
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            for(int i = 0; i < 1000; i++)
            {
                try
                {
                    await connection.InvokeAsync("SendMessage", $"Сообщение {i}", $"User{i}");
                }
                catch
                {
                    Assert.IsFalse(true);
                }
            }


            Assert.Pass();
        }
    }
}