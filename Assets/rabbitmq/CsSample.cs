using UnityEngine;

using System.Collections;
using System.Threading;
using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Util;


public class CsSample : MonoBehaviour {
    public string serverip = "192.168.99.66";
    public string exchange = "time";

    ConnectionFactory cf;
    IConnection conn;
    IModel ch = null;
    QueueingBasicConsumer consumer;

    System.Collections.Queue queue = null;
    string lastMessage;

    // Use this for initialization
    void Start()
    {

		Debug.Log("START");
        cf = new ConnectionFactory();
        cf.HostName = serverip;
        conn = cf.CreateConnection();

        conn.ConnectionShutdown += new ConnectionShutdownEventHandler(LogConnClose);

        ch = conn.CreateModel();
        ch.ExchangeDeclare(exchange, "fanout");
        string queueName = ch.QueueDeclare();

        ch.QueueBind(queueName, exchange, "");
		//ch.QueueBind(queueName,exchange,"",false,null);  //version shup

        consumer = new QueueingBasicConsumer(ch);	
        ch.BasicConsume(queueName, true, consumer);
		//ch.BasicConsume(queueName,null,consumer);  //version shup

        queue = new System.Collections.Queue();
        queue.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (ch == null) return;
        BasicDeliverEventArgs ea;
        while ((ea = (BasicDeliverEventArgs)consumer.Queue.DequeueNoWait(null)) != null)
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);
            queue.Enqueue(message);
        }
    }

    void OnGUI()
    {
        if (queue != null && queue.Count > 0)
        {
            lastMessage = queue.Dequeue().ToString();
        }
        GUILayout.Label((string)lastMessage);
    }

    public static void LogConnClose(IConnection conn, ShutdownEventArgs reason)
    {
        Debug.Log("Closing connection normally. " + conn + " with reason " + reason);
    }
}
