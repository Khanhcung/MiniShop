using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging
{
	public class KafkaProducer : IKafkaProducer, IDisposable
	{

		private readonly IProducer<string, string> _producer;

		public KafkaProducer(IConfiguration cfg)
		{
			var conf = new ProducerConfig { BootstrapServers = cfg["Kafka:BootstrapServers"] };
			_producer = new ProducerBuilder<string, string>(conf).Build();
		}

		public async Task ProduceAsync(string topic, string key, string value)
		{
			await _producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = value });
		}

		public void Dispose()
		{
			_producer.Flush();
		}

		
	}
}
