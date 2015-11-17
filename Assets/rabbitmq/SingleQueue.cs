using System.IO;
using UnityEngine;
using System.Collections;
using RabbitMQ.Client;
using System.Text;
using System;
using UnityEngine.UI;


public class SingleQueue : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void ChangeQueue(){
		Text text;
		text =  GameObject.Find("queueText").GetComponent<Text>();
		text.text = "Single Queue";
	}

	public void SendSimpleQueue(){
		string filepath = Utils.GetFullPathFileName("rabbit.ogg");
		byte[] body = Utils.GetFileAsBytesOrNull (filepath);


		var factory = new ConnectionFactory() { HostName = "diablo" ,UserName = "guest" ,Password = "guest" };
		using(var connection = factory.CreateConnection())
			using(var channel = connection.CreateModel())
		{
			//channel.QueueDeclare("SimpleQueue"); //version shup
			channel.QueueDeclare("SimpleQueue",true,false,false,null);
			
//			string message = "Hello World!";
			//var body = Encoding.UTF8.GetBytes(message);
			
			channel.BasicPublish(exchange: "",
			                     routingKey: "SimpleQueue",
			                     basicProperties: null,
			                     body: body);

			Text text ,log;
			text =  GameObject.Find("TextPE").GetComponent<Text>();
			int count = int.Parse(text.text) + 1;
			text.text= count.ToString();
			log = GameObject.Find("console").GetComponent<Text>();
			var fileInfo = new System.IO.FileInfo("rabbit.ogg");
			var fileSize = (fileInfo.Length/1024f)/1024f;
			log.text = log.text + "[ "+ DateTime.Now.ToString("HH:mm:ss") +" ] Mensagem Enviada SingleQueue : " + fileSize.ToString("0.00") + " MB" + "\n";

			connection.Close();
		}


	}

	public void ConsumeSimpleQueue(){
	
		var factory = new ConnectionFactory() { HostName = "diablo" ,UserName = "guest" ,Password = "guest" };
		using(var connection = factory.CreateConnection())
			using(var channel = connection.CreateModel())
		{
			//channel.QueueDeclare("SimpleQueue"); //version shup
			channel.QueueDeclare("SimpleQueue",true,false,false,null);

			BasicGetResult result = channel.BasicGet("SimpleQueue", true);
			while (result != null)
			{
				//string message = result.Body;

				Utils.SaveFileToDisk("rabbitVideo.ogg",result.Body);
				result = channel.BasicGet("SimpleQueue", true);
				var fileInfo = new System.IO.FileInfo("rabbitVideo.ogg");
				var fileSize = (fileInfo.Length/1024f)/1024f;
				Atualiza(fileSize.ToString("0.00") + " MB");


			}
			if(result == null){
				Text log;
				log = GameObject.Find("console").GetComponent<Text>();
				log.text = log.text + "[ "+ DateTime.Now.ToString("HH:mm:ss") +" ] Não Há mensagens para consumir \n";
				connection.Close();
			
			}


		}
	}
	public void Atualiza(String message){
		Text text ,log;
		text =  GameObject.Find("TextPR").GetComponent<Text>();
		int count = int.Parse(text.text) + 1;
		text.text= count.ToString();
		log = GameObject.Find("console").GetComponent<Text>();
		log.text = log.text + "[ "+ DateTime.Now.ToString("HH:mm:ss") +" ] Mensagem Recebida SingleQueue : " + message + "\n";
		
	}

}


