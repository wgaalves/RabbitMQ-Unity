using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Threading;

public class Workqueue : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void changeQueue(){
		Text text;
		text =  GameObject.Find("queueText").GetComponent<Text>();
		text.text = "Work Queue";
	}

	public void NewTask(){
		string filepath = Utils.GetFullPathFileName("Chegou.png");
		byte[] body = Utils.GetFileAsBytesOrNull (filepath);
		var factory = new ConnectionFactory() { HostName = "diablo" };
		using(var connection = factory.CreateConnection())
			using(var channel = connection.CreateModel())
		{
			//channel.QueueDeclare("Work_Queue"); // version shup
			channel.QueueDeclare("Work_Queue",true,false,false,null);

			var properties = channel.CreateBasicProperties();
			properties.SetPersistent(true);
			
			channel.BasicPublish(exchange: "",
			                     routingKey: "Work_Queue",
			                     basicProperties: properties,
			                     body: body);
			Text text ,log;
			text =  GameObject.Find("TextPE").GetComponent<Text>();
			int count = int.Parse(text.text) + 1;
			text.text= count.ToString();
			log = GameObject.Find("console").GetComponent<Text>();
			var fileInfo = new System.IO.FileInfo("Chegou.png");
			var fileSize = (fileInfo.Length/1024f)/1024f;
			log.text = log.text + "[ "+ DateTime.Now.ToString("HH:mm:ss") +" ] Mensagem Enviada Work Queue : " + fileSize.ToString("0.00") + " MB" + "\n";
			
			connection.Close();
			//Console.WriteLine(" [x] Sent {0}", message);
		}

	}
	public void Worker(){

		Text log = GameObject.Find("console").GetComponent<Text>();
		   	  var factory = new ConnectionFactory() { HostName = "diablo", UserName = "guest" ,Password = "guest"};
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
			channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
		
			
			var consumer = new EventingBasicConsumer(channel);
			consumer.Received += (model, ea) =>
            {
                //var body = ea.Body;
                //var message = Encoding.UTF8.GetString(body);
                //Console.WriteLine(" [x] Received {0}", message);

               /// int dots = message.Split('.').Length - 1;
                //
                Thread.Sleep(1000);
				log.text = log.text + "[ "+ DateTime.Now.ToString("HH:mm:ss") +" ] recebendo mensagens. \n";
               

				channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

            };
			channel.BasicConsume(queue: "Work_Queue",
			                     noAck: false,
			                     consumer: consumer); 
	}

}
}
