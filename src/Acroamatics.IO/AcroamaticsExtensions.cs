using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Acroamatics.IO
{
	public static class AcroamaticsExtensions
	{

		public static IBufferCollection AddBuffer(this IBufferCollection buffers, Action<BufferContext> configure)
		{
			var bc = new BufferContext();

			configure(bc);

			buffers.Add(bc);

			return buffers;
		}

		public static IServiceCollection AddAcroClient(this IServiceCollection services, Action<IAcroClientBuilder, IAcroInputBuilder, IAcroOutputBuilder> configureClient)
		{

			var clientbuilder = new AcroClientBuilder();
			var inputbuilder = new AcroInputBuilder();
			var outputbuilder = new AcroOutputBuilder();

			configureClient?.Invoke(clientbuilder, inputbuilder, outputbuilder);

			services.AddAcroServices(clientbuilder, inputbuilder, outputbuilder);

			return services;
		}

		
		private static IServiceCollection AddAcroServices(this IServiceCollection services, AcroClientBuilder clientbuilder, AcroInputBuilder inputbuilder, AcroOutputBuilder outputbuilder)
		{
			services.AddSingleton<IAcroInput>(inputbuilder.Build());
			services.AddHostedService<AcroInput>(sp => sp.GetRequiredService<IAcroInput>() as AcroInput);

			services.AddSingleton<IAcroOutput>(outputbuilder.Build());
			services.AddHostedService<AcroOutput>(sp => sp.GetRequiredService<IAcroOutput>() as AcroOutput);

			services.AddSingleton<IAcroClient>(sp =>
			{
				var input = sp.GetRequiredService<IAcroInput>();
				var output = sp.GetRequiredService<IAcroOutput>();

				clientbuilder.AddInput(input);
				clientbuilder.AddOutput(output);

				return clientbuilder.Build();

			});

			return services;
		}
	}
}
