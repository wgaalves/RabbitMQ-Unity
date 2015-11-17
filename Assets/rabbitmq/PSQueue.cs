using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Util;
using System;


public class PSQueue : MonoBehaviour {
	private String uuid = "";

	// Use this for initialization
	void Start () {
		Debug.Log("Start");
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void changeQueue(){
		Text text;
		text =  GameObject.Find("queueText").GetComponent<Text>();
		text.text = "Publish / Subscribe Queue";
	}

	void setUuid(String id){
		uuid = id;
	}
	
	public void SendPSMessage(){
		
		string filepath = Utils.GetFullPathFileName("Chegou.png");
		byte[] messageBytes = Utils.GetFileAsBytesOrNull (filepath);
		
		var factory = new ConnectionFactory() { HostName = "diablo" , UserName = "guest" , Password = "guest"};
		using(var connection = factory.CreateConnection())
			using(var channel = connection.CreateModel())
		{
			channel.ExchangeDeclare(exchange: "publishSubEX", type: "fanout");
			
			var body = messageBytes;
			//var body = Encoding.UTF8.GetBytes(message);
			channel.BasicPublish(exchange: "publishSubEX",
			                     routingKey: "",
			                     basicProperties: null,
			                     body: body);
			
			Text text ,log;
			text =  GameObject.Find("TextPE").GetComponent<Text>();
			int count = int.Parse(text.text) + 1;
			text.text= count.ToString();
			log = GameObject.Find("console").GetComponent<Text>();
			var fileInfo = new System.IO.FileInfo("Chegou.png");
			var fileSize = (fileInfo.Length/1024f)/1024f;
			log.text = log.text + "[ "+ DateTime.Now.ToString("HH:mm:ss") +" ] Mensagem Enviada Publish / Subscribe : " + fileSize.ToString("0.00") + " MB" + "\n";
			
		}
		
	}
	public void receivePSMessage(){
		Debug.Log("Start");
		Text log = GameObject.Find("console").GetComponent<Text>();
		log.text = log.text + "[ "+ DateTime.Now.ToString("HH:mm:ss") +" ] Aguardando mensagens.\n";
		var factory = new ConnectionFactory() { HostName = "diablo", UserName = "guest", Password = "guest"  };
		using(var connection = factory.CreateConnection())
			using(var channel = connection.CreateModel())
		{
			if(uuid == ""){
				setUuid("amq-" + Guid.NewGuid().ToString());
			}
			channel.ExchangeDeclare(exchange: "publishSubEX", type: "fanout");
			var queueName = channel.QueueDeclare(uuid, false,false,false,null);
			channel.QueueBind(queueName,"publishSubEX","");
		
			
			var consumer = new EventingBasicConsumer(channel);
			consumer.Received += (model, ea) =>
			{
				var body = ea.Body;
				Utils.SaveFileToDisk("rabbit.png",body);
				var fileInfo = new System.IO.FileInfo("rabbit.png");
				var fileSize = (fileInfo.Length/1024f)/1024f;
				log.text = log.text = log.text + "[ "+ DateTime.Now.ToString("HH:mm:ss") +" ] Mensagem Enviada Publish / Subscribe : " + fileSize.ToString("0.00") + " MB" + "\n";
			};
			channel.BasicConsume(queue: queueName,
			                     noAck: true,
			                     consumer: consumer);
		}
	}
}
