using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging
{
	public interface IKafkaProducer
	{
		Task ProduceAsync(string topic, string key, string value);
	}
}
