using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Acroamatics.IO.Example
{
	class Program
	{
		static void Main(string[] args)
		{
			Host.CreateDefaultBuilder(args)
				.ConfigureServices((context, services) =>
				{
					services.AddAcroClient((client, input, output) =>
					{
						client.ConfigureOptions(options =>
						{
							options.Direction = Direction.InOut;
							options.Hosted = true;
						});

						input.ConfigureOptions(options =>
						{
							if (context.HostingEnvironment.IsDevelopment())
							{
								options.BufferCollection.AddBuffer(ctx =>
								{
									ctx.Start = 0x00B0001;
									ctx.End = 0x0000C0001;
									ctx.Counter = 0x020000000;
								});
							}
							options.BufferCollection.AddBuffer(ctx =>
							{
								ctx.Start = 0x0000001;
								ctx.End = 0x0000A0001;
								ctx.Counter = 0x010000000;
							});

						});

						output.ConfigureOptions(options =>
						{
							options.ChannelOptions.SingleReader = true;
						});
					});
					services.AddHostedService<Service>();
				})
				.Build()
				.Run();
		}

	}
}
